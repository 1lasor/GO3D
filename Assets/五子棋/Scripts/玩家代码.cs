using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class 玩家代码 : MonoBehaviour
{
    //建立一个二维数组防止重复寻找
    private int[,] search_matrix = new int[15, 15];

    public Vector3 zeroPointPos;
    public float cellWidth;
    public PieceColor color = PieceColor.Black;

    private int row;
    private int column;

    public GameObject BlackPiece;
    public GameObject WhitePiece;

    public List<棋子代码> PreBlackPieceList = new List<棋子代码>();
    public List<棋子代码> PreWhitePieceList = new List<棋子代码>();
    public List<棋子代码> CurrentPieceList = new List<棋子代码>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            Vector3 offsetPos = worldPos - zeroPointPos;
            column = (int)Mathf.Round(offsetPos.y / cellWidth);
            row = (int)Mathf.Round(offsetPos.x / cellWidth);

            int[] RowColumnValue = { row, column };

            棋子代码 currentPiece = new 棋子代码();
            currentPiece.row = row;
            currentPiece.column = column;
            currentPiece.color = color;

            //判断是否越界
            if (row < 0 || row > 14 || column < 0 || column > 14) return;
            //判断是否已经存在棋子
            CurrentPieceList = GameObject.FindObjectsOfType<棋子代码>().ToList();
            foreach (var piece in CurrentPieceList)
            {
                if (piece.row == row && piece.column == column)
                {
                    return;
                }
            }

            //将search_matrix清零
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    search_matrix[i, j] = 0;
                }
            }

            CurrentPieceList.Add(currentPiece);
            var currentColorlist= CurrentPieceList.Where(en => en.color == currentPiece.color).ToList();
            var samePieceBy = GetSamePieceBy(currentColorlist, currentPiece);

            //判断落子后是否会产生提子
            var DeadPieces = TiZi(currentPiece, CurrentPieceList);

            //判断落子后是否有气
            //如果没气判断是否能够提子
            if (!ExistQi(currentPiece, CurrentPieceList, samePieceBy))
            {
                if (DeadPieces.Count != 0)
                {
                    //打劫判断能否落子
                    if (DeadPieces.Count == 1)
                    {
                        foreach (var piece in DeadPieces)
                        {
                            CurrentPieceList.Remove(piece);
                        }
                        if (IsKo(CurrentPieceList,PreBlackPieceList,PreWhitePieceList)) 
                        {
                            return;
                        }
                        else
                        {
                            foreach (var piece in DeadPieces)
                            {
                                Destroy(piece.gameObject);
                            }
                        }
                    }
                    else
                    {
                        foreach (var piece in DeadPieces)
                        {
                            Destroy(piece.gameObject);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                foreach (var piece in DeadPieces)
                {
                    Destroy(piece.gameObject);
                }
            }
            
            Vector3 piecePos = new Vector3(row * cellWidth, column * cellWidth, zeroPointPos.z) + zeroPointPos;

            //生成棋子
            GameObject newPiece;
            if (color == PieceColor.Black)
            {
                if (BlackPiece != null)
                {
                    newPiece = Instantiate(BlackPiece, piecePos, BlackPiece.transform.rotation);
                    color = PieceColor.White;

                    // 获取棋子代码脚本并设置行和列值
                    棋子代码 pieceScript = newPiece.GetComponent<棋子代码>();
                    pieceScript.SetRowColumnValue(new int[] { row, column });

                    currentPiece = newPiece.GetComponent<棋子代码>();
                    PreBlackPieceList = CurrentPieceList;
                }
            }
            else
            {
                if (WhitePiece != null)
                {
                    newPiece = Instantiate(WhitePiece, piecePos, WhitePiece.transform.rotation);
                    color = PieceColor.Black;

                    // 获取棋子代码脚本并设置行和列值
                    棋子代码 pieceScript = newPiece.GetComponent<棋子代码>();
                    pieceScript.SetRowColumnValue(new int[] { row, column });

                    currentPiece = newPiece.GetComponent<棋子代码>();
                    PreWhitePieceList = CurrentPieceList;
                }
            }
            
        }
    }

    //判断在当前棋子落下后需要提子的数量和位置
    List<棋子代码> TiZi(棋子代码 currentPiece, List<棋子代码> currentList)
    {
        List<棋子代码> result = new List<棋子代码>();
        var CurrentColorList = currentList.Where(en => en.color != currentPiece.color).ToList();
        foreach (var item in  CurrentColorList)
        {
            if ((item.row == currentPiece.row + 1 && item.column == currentPiece.column)||
                (item.row == currentPiece.row - 1 && item.column == currentPiece.column)||
                (item.column == currentPiece.column - 1 && item.row == currentPiece.row)||
                (item.column == currentPiece.column + 1 && item.row == currentPiece.row))
            {
                //将search_matrix清零
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        search_matrix[i, j] = 0;
                    }
                }
                var SamePieceBy = GetSamePieceBy(CurrentColorList, item);
                if(!ExistQi(item,currentList,SamePieceBy))
                {
                    result.AddRange(SamePieceBy);
                }
            }
        }
        
        return result;
    }

    //判断落子后是否有气
    bool ExistQi(棋子代码 currentPiece, List<棋子代码> currentList,List<棋子代码> SamePieceBy)
    {
        bool result = false;
        int flag;
        foreach(var item in SamePieceBy)
        {
            //用flag来代表当前位置棋子的气
            if (item.row == 0 || item.row == 14)
            {
                if (item.column == 0||item.column==14)
                {
                    flag = 2;
                }
                else
                {
                    flag = 3;
                }
            }
            else if(item.column == 14||item.column==0)
            {
                flag = 3;
            }
            else
            {
                flag = 4;
            }

            //判断当前棋子的四周是否有其他棋子，如果有就flag-1
            foreach (var it in currentList)
            {
                if (item.row == it.row && item.column + 1 == it.column)
                {
                    flag--;
                }
                else if (item.row == it.row && item.column - 1 == it.column)
                {
                    flag--;
                }
                else if (item.row + 1 == it.row && item.column == it.column)
                {
                    flag--;
                }
                else if (item.row - 1 == it.row && item.column == it.column) 
                {
                    flag--;
                }
            }

            if(flag != 0)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    //查找与落子颜色相同的棋子总共有多少相连
    List<棋子代码> GetSamePieceBy(List<棋子代码> currentColorList, 棋子代码 currentPiece)
    {
        List<棋子代码> result = new List<棋子代码>();
        search_matrix[currentPiece.row, currentPiece.column] = 1;
        result.Add(currentPiece);
        foreach (var item in currentColorList)
        {
            if (item.row >= 0 && item.row <= 14 && item.column >= 0 && item.column <= 14)
            {
                if ((item.row == currentPiece.row && item.column == currentPiece.column + 1 && search_matrix[item.row, item.column] == 0) ||
                (item.row == currentPiece.row && item.column == currentPiece.column - 1 && search_matrix[item.row, item.column] == 0) ||
                (item.row == currentPiece.row - 1 && item.column == currentPiece.column && search_matrix[item.row, item.column] == 0) ||
                (item.row == currentPiece.row + 1 && item.column == currentPiece.column && search_matrix[item.row, item.column] == 0))
                {
                    var resultList = GetSamePieceBy(currentColorList, item);
                    result.AddRange(resultList);
                }
            }
        }
        return result;
    }

    //判断打劫
    bool IsKo(List<棋子代码> currentPieceList, List<棋子代码> preBlackPieceList, List<棋子代码> preWhitePiecList)
    {
        bool res=false;
        int isBlack=preBlackPieceList.Count;
        int isWhite=preWhitePiecList.Count;
        foreach(var piece in currentPieceList)
        {
            if (isBlack != 0 && preBlackPieceList.Count == currentPieceList.Count) 
            {
                foreach(var piece2 in preBlackPieceList)
                {
                    if (piece2.row == piece.row && piece2.column == piece.column && piece2.color == piece.color) 
                    {
                        isBlack--;
                        break;
                    }
                }
            }
            if (isWhite != 0 && preWhitePiecList.Count == currentPieceList.Count) 
            {
                foreach( var piece2 in preWhitePiecList)
                {
                    if (piece2.row == piece.row && piece2.column == piece.column && piece2.color == piece.color) 
                    {
                        isWhite--;
                        break;
                    }
                }
            }
        }
        if (isBlack == 0 || isWhite == 0)
        {
            res = true;
        }

        return res;
    }
}
