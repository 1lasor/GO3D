using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    Black=0, White=1,
}
public class 棋子代码 : MonoBehaviour
{
    public int row;
    public int column; 
    public PieceColor color=PieceColor.Black;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRowColumnValue(int[] RowColumnValue)
    {
        if(RowColumnValue.Length!=2) return;
        row= RowColumnValue[0];
        column= RowColumnValue[1];
    }
}
