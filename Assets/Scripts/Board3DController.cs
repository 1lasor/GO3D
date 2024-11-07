using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class Board3DController : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject boardcellPrefab; // 棋盘单元预制体

    public int boardSize; // 棋盘大小
    public float spacing = 1.0f; // 棋盘单元间隔
    public Vector3 centerPosition;

    public Toggle toggleX;
    public Toggle toggleY;
    public Toggle toggleZ;

    public float targetSpacing; // 目标间距值
    public float duration; // 动画持续时间

    private float startSpacing; // 初始间距值
    private float elapsedTime; // 已经过的时间

    // 记录当前棋盘状态
    public List<BoardUnit> boardUnits = new List<BoardUnit>();

    void Start()
    {
        toggleX.isOn = true; 
        GenerateBoard();
        GetCenterPosition();
        //MoveCameraToCenter();
        //StartAnimation(1.0f, 30.0f, 100);
    }

    void GenerateBoard()
    {
        for (int x = 0; x < boardSize; x++) 
        {
            for (int y = 0; y < boardSize; y++) 
            {
                for (int z = 0; z < boardSize; z++) 
                {
                    // 计算位置
                    Vector3 position = new Vector3(x * spacing, y * spacing, z * spacing);

                    // 实例化预制体
                    GameObject boardCell = Instantiate(boardcellPrefab, position, Quaternion.identity);
                    BoardUnit boardUnit = boardCell.GetComponent<BoardUnit>();
                    boardUnit.Position = new Vector3Int(x, y, z);
                    Debug.Log(boardUnit.Position.ToString());

                    // 将新增单元格添加到当前棋盘状态
                    boardUnits.Add(boardUnit);

                    // 可以选择性地设置父对象，以便于管理
                    boardCell.transform.SetParent(gameObject.transform);

                    // 可以根据需要改变颜色或其它属性
                    // if ((x + y) % 2 == 0)
                    // {
                    //     square.GetComponent<Renderer>().material.color = Color.white;
                    // }
                    // else
                    // {
                    //     square.GetComponent<Renderer>().material.color = Color.black;
                    // }
                }
            }
        }
    }

    // boardUnits的一些公用函数
    /* ---------------------------------------------------------------------------------------------------*/
    // 获取boardUnits列表
    public List<BoardUnit> GetCurrenBoardUnitList()
    {
        return boardUnits;
    }

    // 将需要更改的棋子以列表的形式传给boardUnits,通过查找相同的下标进行更换
    public void ChangeBoardUnits(List<BoardUnit> changeUnits, BoardUnit.PieceType type)
    {
        foreach(var changeUnit in changeUnits)
        {
            // 查找boardUnits中具有相同Location的BoardUnit
            int index = boardUnits.FindIndex(unit => unit.Position == changeUnit.Position);
            if (index != -1)
            {
                // 替换找到的BoardUnit
                boardUnits[index].CurrentPieceType = type;
            }
        }
    }

    /* ---------------------------------------------------------------------------------------------------*/

    public void ShowPlaneAt(Vector3 position)
    {
        this.Show(false);
        if (toggleX.isOn)
        {
            foreach (var boardUnit in boardUnits)
            {
                if (boardUnit.Position.x == position.x)
                {
                    boardUnit.gameObject.SetActive(true);
                }
            }
            Debug.Log("x is on");
        }
        if (toggleY.isOn)
        {
            foreach (var boardUnit in boardUnits)
            {
                if (boardUnit.Position.y == position.y)
                {
                    boardUnit.gameObject.SetActive(true);
                }
            }
            Debug.Log("y is on");
        }
        if (toggleZ.isOn)
        {
            foreach (var boardUnit in boardUnits)
            {
                if (boardUnit.Position.z == position.z)
                {
                    boardUnit.gameObject.SetActive(true);
                }
            }
            Debug.Log("z is on");
        }
    }

    public void Show(bool active = true)
    {
        foreach (var boardUnit in boardUnits)
        {
            boardUnit.gameObject.SetActive(active);
        }
    }

    void GetCenterPosition()
    {
        float radius = (boardSize - 1) * spacing / 2.0f;
        centerPosition = new Vector3(radius,radius,radius);
    }

    public void StartAnimation(float startValue, float targetValue, float animDuration)
    {
        startSpacing = startValue;
        targetSpacing = targetValue;
        duration = animDuration;
        StartCoroutine(AnimateSpacing());
    }
    IEnumerator AnimateSpacing()
    {
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            // 更新spacing值
            float t = elapsedTime / duration;
            // 使用插值计算当前spacing值
            float currentSpacing = Mathf.Lerp(startSpacing, targetSpacing, t);
            // 更新你的对象以反映新的spacing值
            UpdateSpacing(currentSpacing);

            // 等待下一帧
            yield return null;
            // 更新已过时间
            elapsedTime += Time.deltaTime;
        }

        // 确保最终达到目标值
        UpdateSpacing(targetSpacing);
    }

    // 更新spacing值的方法，你需要根据你的游戏逻辑来定义这个方法
    void UpdateSpacing(float spacing)
    {
        this.spacing = spacing;

        // 这里写上你更新spacing的逻辑
        // 例如，如果是改变物体之间的间隔，可能需要循环遍历所有物体并更新它们的位置
        Debug.Log("Spacing updated to: " + spacing);
    }

}

