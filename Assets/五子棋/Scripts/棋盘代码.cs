using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

public struct QiPu
{
    public int[,] CurrentBoard_Matrix;
    public int step;

    public QiPu(int[,] board,int num)
    {
        CurrentBoard_Matrix = board;
        step = num;
    }
}

public class 棋盘代码 : MonoBehaviour
{
    private int[,] board_matrix = new int[15, 15];
    private int count = 0;
    public QiPu[] qipu = new QiPu[300];

    public List<棋子代码> CurrentBoardState = new List<棋子代码>();
    // Start is called before the first frame update
    void Start()
    {
        //开始时将board_matrix重置为0
        for(int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++) 
            {
                board_matrix[i, j] = 0;
            }
        }
        qipu[count]=new QiPu(board_matrix,count);
        count++;
    }

/*    void PrintBoardMatrix()
    {
        string matrixString = "";
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                // 将每个元素转换为字符串，并在每个元素后添加一个空格
                matrixString += board_matrix[i, j].ToString() + " ";
            }
            // 在每行的末尾添加换行符
            matrixString += "\n";
        }
        // 使用Debug.Log输出整个矩阵字符串
        Debug.Log(matrixString);
    }*/

    // Update is called once per frame
    void Update()
    {
        //点击鼠标左键落子后，在数组board_matrix对应位置标识对应的棋子，黑子为1，白子为2
        if (Input.GetMouseButtonDown(0))
        {
            CurrentBoardState = GameObject.FindObjectsOfType<棋子代码>().ToList();
            foreach (var piece in CurrentBoardState)
            {
                if (piece.color == PieceColor.Black)
                {
                    board_matrix[14 - piece.column, piece.row] = 1;
                }
                else
                {
                    board_matrix[14 - piece.column, piece.row] = 2;
                }
            }
            qipu[count] = new QiPu(board_matrix, count);
            count++;
            /*            PrintBoardMatrix();*/
        }
    }
}
