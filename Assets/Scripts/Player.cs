using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting;
using System;
using static UnityEngine.UI.CanvasScaler;

public class Player : MonoBehaviour
{
    //建立一个三维数组防止重复寻找
    private int[,,] search_matrix;

    public GameObject boardController;//合并待修改
    // 记录当前步数
    private int currentStep;
    private GameObject selectedObj;

    Board3DController board3DController;

    // 当前棋子列表
    List<BoardUnit> currentBoardUnits = new List<BoardUnit>();

    // 记录上一步的分别为黑棋与白棋时的棋盘列表
    List<BoardUnit> preBlackPieceList = new List<BoardUnit>();
    List<BoardUnit> preWhitePieceList = new List<BoardUnit>();

    // Start is called before the first frame update
    void Start()
    {
        // 获取Board3DController实例
        this.board3DController = FindObjectOfType<Board3DController>();
        Search_Matrix_Init();
        // 将步数初始化
        currentStep = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.TryGetComponent<BoardUnit>(out var boardUnit))
        {
            GameObject newObj = boardUnit.gameObject;
            if (selectedObj != null&&selectedObj!= newObj)
            {
                selectedObj.GetComponent<Outline>().enabled = false;
            }
            selectedObj = newObj;
            selectedObj.GetComponent<Outline>().enabled = true;
            
            if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
            {
                Vector3Int position = boardUnit.Position;
                // 检查是否同时按下了Control键
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))//Ctrl+左键显示平面
                {
                    board3DController.ShowPlaneAt(position);
                }
                else//左键单击落子
                {
                    // 重置search_matrix
                    Search_Matrix_Init();

                    //判断是否可以落子
                    if (CanPlace(boardUnit))
                    {
                        //可以落子
                        PlacePiece(boardUnit);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<Outline>().enabled = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            board3DController.Show();
        }
        /*if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
               
        }
        else if (Input.GetMouseButtonDown(1))
        {
            board3DController.Show();
        }*/
    }
    // 判断是否可以落子
    bool CanPlace(BoardUnit unit)
    {
        bool res = true;
        // 是否存在棋子
        if (ExistPiece(unit)){
            res = false;
        }
        else
        {
            // 获取当前棋子颜色
            var type = BoardUnit.PieceType.None;
            if (currentStep % 2 == 0)
            {
                type = BoardUnit.PieceType.Black;
            }
            else
            {
                type = BoardUnit.PieceType.White;
            }
            unit.CurrentPieceType = type;
            List<BoardUnit> currentPiece = new List<BoardUnit>();
            currentPiece.Add(unit);
            board3DController.ChangeBoardUnits(currentPiece, type);

            currentBoardUnits = board3DController.GetCurrenBoardUnitList();

            // 落子后提子列表
            var deadPieces = TiZi(unit);

            if (ExistQi(unit))
            {
                res = true;
            }
            else
            {
                if (deadPieces.Count != 0) 
                {
                    if (deadPieces.Count == 1)
                    {
                        if (IsKo(unit))
                        {
                            board3DController.ChangeBoardUnits(currentPiece, BoardUnit.PieceType.None);
                            unit.CurrentPieceType = BoardUnit.PieceType.None;
                            return false;
                        }
                        else
                        {
                            res = true;
                        }
                    }
                }
                else
                {
                    res = false;
                }
            }
            board3DController.ChangeBoardUnits(deadPieces, BoardUnit.PieceType.None);
            board3DController.ChangeBoardUnits(currentPiece, BoardUnit.PieceType.None);
            unit.CurrentPieceType = BoardUnit.PieceType.None;
        }

        return res;
    }// ok

    //落子函数
    void PlacePiece(BoardUnit unit)
    {
        if (currentStep % 2 == 0)
        {
            unit.CurrentPieceType = BoardUnit.PieceType.Black;
        }
        else
        {
            unit.CurrentPieceType = BoardUnit.PieceType.White;
        }
        /*string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        Debug.Log($"{timestamp} - Piece placed at: {unit.Position.ToString()}");
        Debug.Log($"{timestamp} - Piece type: {unit.CurrentPieceType}");*/
        //当前步数加1
        currentStep++;
        currentStep %= 2;

        List<BoardUnit> pieces = new List<BoardUnit>();
        pieces.Add( unit );
        board3DController.ChangeBoardUnits(pieces,unit.CurrentPieceType);

        if(unit.CurrentPieceType == BoardUnit.PieceType.Black)
        {
            preBlackPieceList = board3DController.GetCurrenBoardUnitList().Where(en => en.CurrentPieceType == BoardUnit.PieceType.Black).ToList();
        }
        else
        {
            preWhitePieceList = board3DController.GetCurrenBoardUnitList().Where(en => en.CurrentPieceType == BoardUnit.PieceType.White).ToList();
        }
    }// ok

    // 判断是否存在棋子
    bool ExistPiece(BoardUnit unit)
    {
        bool res = false;
        if (unit.CurrentPieceType == BoardUnit.PieceType.None)
        {
            res = false;
        }
        else
        {
            res = true;
        }
        return res;
    }// ok

    // 判断是否有气
    bool ExistQi(BoardUnit unit)
    {
        bool res = false;

        List<BoardUnit> currentColorList = currentBoardUnits.Where(en => en.CurrentPieceType == unit.CurrentPieceType).ToList();

        Search_Matrix_Init();
        List<BoardUnit> samePieceBy = GetSamePieceBy(currentColorList, unit);

        var type = BoardUnit.PieceType.None;

        foreach (var item in samePieceBy)
        {
            int flag = 6;
            //用flag来代表当前位置棋子的气
            flag = flag - ((item.Position[0] == 0 || item.Position[0] == search_matrix.GetLength(0) - 1) ? 1 : 0)
                        - ((item.Position[1] == 0 || item.Position[1] == search_matrix.GetLength(1) - 1) ? 1 : 0)
                        - ((item.Position[2] == 0 || item.Position[2] == search_matrix.GetLength(2) - 1) ? 1 : 0);

            //判断当前棋子的四周是否有其他棋子，如果有就flag-1
            foreach (var it in currentBoardUnits)
            {
                if (it.CurrentPieceType != type)
                {
                    if ((item.Position[0] == it.Position[0] - 1 && item.Position[1] == it.Position[1] && item.Position[2] == it.Position[2]) ||
                        (item.Position[0] == it.Position[0] + 1 && item.Position[1] == it.Position[1] && item.Position[2] == it.Position[2]) ||
                        (item.Position[0] == it.Position[0] && item.Position[1] == it.Position[1] - 1 && item.Position[2] == it.Position[2]) ||
                        (item.Position[0] == it.Position[0] && item.Position[1] == it.Position[1] + 1 && item.Position[2] == it.Position[2]) ||
                        (item.Position[0] == it.Position[0] && item.Position[1] == it.Position[1] && item.Position[2] == it.Position[2] - 1) ||
                        (item.Position[0] == it.Position[0] && item.Position[1] == it.Position[1] && item.Position[2] == it.Position[2] + 1))
                    {
                        flag--;
                        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                        Debug.Log($"{timestamp} - Piece placed at: {it.Position.ToString()}");
                        Debug.Log($"{timestamp} - Piece type: {it.CurrentPieceType}");
                    }
                }
            }

            if (flag != 0)
            {
                res = true;
                break;
            }
        }

        return res;
    }// ok

    // 判断是否产生提子
    List<BoardUnit> TiZi(BoardUnit unit)
    {
        List<BoardUnit> res = new List<BoardUnit>();

        var currentColorList = currentBoardUnits.Where(en => en.CurrentPieceType != unit.CurrentPieceType && en.CurrentPieceType != BoardUnit.PieceType.None).ToList();
        foreach (var item in currentColorList)
        {
            if ((item.Position[0] == unit.Position[0] + 1 && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2]) ||
                (item.Position[0] == unit.Position[0] - 1 && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2]) ||
                (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] + 1 && item.Position[2] == unit.Position[2]) ||
                (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] - 1 && item.Position[2] == unit.Position[2]) ||
                (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] + 1) ||
                (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] - 1)) 
            {

                Search_Matrix_Init();

                var SamePieceBy = GetSamePieceBy(currentColorList, item);
                if (!ExistQi(item))
                {
                    res.AddRange(SamePieceBy);
                }
            }
        }

        return res;
    }// ok

    // 判断是否处于打劫
    bool IsKo(BoardUnit unit)
    {
        bool res = false;

        int isBlack = preBlackPieceList.Count;
        int isWhite = preWhitePieceList.Count;
        foreach (var piece in currentBoardUnits.Where(en => en.CurrentPieceType == BoardUnit.PieceType.Black).ToList())
        {
            if (isBlack != 0 && preBlackPieceList.Count == (currentBoardUnits.Where(en => en.CurrentPieceType == BoardUnit.PieceType.Black).ToList()).Count)
            {
                foreach (var piece2 in preBlackPieceList)
                {
                    if (piece2.Position[0] == piece.Position[0] && piece2.Position[1] == piece.Position[1] && piece2.Position[2] == piece.Position[2])
                    {
                        isBlack--;
                        break;
                    }
                }
            }
        }
        foreach (var piece in currentBoardUnits.Where(en => en.CurrentPieceType == BoardUnit.PieceType.White).ToList())
        {
            if (isWhite != 0 && preWhitePieceList.Count == (currentBoardUnits.Where(en => en.CurrentPieceType == BoardUnit.PieceType.White).ToList()).Count)
            {
                foreach (var piece2 in preWhitePieceList)
                {
                    if (piece2.Position[0] == piece.Position[0] && piece2.Position[1] == piece.Position[1] && piece2.Position[2] == piece.Position[2])
                    {
                        isWhite--;
                        break;
                    }
                }
            }
        }
        if (isBlack == 0 && isWhite == 0)
        {
            res = true;
        }

        return res;
    }// ok

    // 查找相连的所有棋子
    List<BoardUnit> GetSamePieceBy(List<BoardUnit> currentColorList, BoardUnit unit)
    {
        List<BoardUnit> result = new List<BoardUnit>();
        search_matrix[unit.Position[0], unit.Position[1], unit.Position[2]] = 1;
        result.Add(unit);
        foreach (var item in currentColorList)
        {
            if (item.Position[0] >= 0 && item.Position[0] <= search_matrix.GetLength(0) && 
                item.Position[1] >= 0 && item.Position[1] <= search_matrix.GetLength(1) &&
                item.Position[2] >= 0 && item.Position[2] <= search_matrix.GetLength(2))
            {
                if ((item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] + 1 && item.Position[2] == unit.Position[2] && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0) ||
                    (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] - 1 && item.Position[2] == unit.Position[2] && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0) ||
                    (item.Position[0] == unit.Position[0] - 1 && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0) ||
                    (item.Position[0] == unit.Position[0] + 1 && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0) ||
                    (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] + 1 && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0) ||
                    (item.Position[0] == unit.Position[0] && item.Position[1] == unit.Position[1] && item.Position[2] == unit.Position[2] - 1 && search_matrix[item.Position[0], item.Position[1], item.Position[2]] == 0)) 
                {
                    var resultList = GetSamePieceBy(currentColorList, item);
                    result.AddRange(resultList);
                }
            }
        }
        return result;
    }// ok

    // 重置search_matrix
    void Search_Matrix_Init()
    {
        search_matrix = new int[board3DController.boardSize, board3DController.boardSize, board3DController.boardSize];
        for (int i = 0; i < board3DController.boardSize; i++) 
        {
            for (int j = 0; j < board3DController.boardSize; j++) 
            {
                for(int k = 0; k < board3DController.boardSize; k++)
                {
                    search_matrix[i, j, k] = 0;
                }
            }
        }
    }// ok
}
