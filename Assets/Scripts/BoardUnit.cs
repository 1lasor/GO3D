using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUnit : MonoBehaviour
{
    // 单元格
    public GameObject boardCell;

    // 棋子的材质
    public Material blackMaterial;
    public Material whiteMaterial;
    public Material defaultMaterial;

    // 三维位置
    public Vector3Int Position { get; set; }
    // 透明度
    [Range(0.0f, 1.0f)]
    public float Transparency;
    // 棋子种类枚举
    public enum PieceType
    {
        None,
        Black,
        White
    }

    // 当前棋子种类
    public PieceType CurrentPieceType { get; set; }

    // 在Start方法中初始化位置和透明度
    void Start()
    {
        // 初始化位置，这里可以设置为预制体在场景中的位置

        // 初始化透明度，这里设置为完全不透明

        // 初始化棋子种类为无
        CurrentPieceType = PieceType.None;
        SetOpacity(0.2f);
    }

    void Update()
    {
        SetPieceType(CurrentPieceType);
    }

    // 更新透明度的方法
    public void SetOpacity(float newOpacity)
    {
        // 获取预制体上的所有Renderer组件
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // 遍历所有Renderer组件
        foreach (Renderer renderer in renderers)
        {
            // 获取并修改材质的颜色属性
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                Color color = materials[i].color;
                color.a = newOpacity; // 设置新的透明度值
                materials[i].color = color;
            }
        }
    }

    // 设置棋子种类的公共方法
    public void SetPieceType(PieceType type)
    {
        CurrentPieceType = type;
        // 这里可以添加逻辑来根据棋子种类改变棋子的外观，比如颜色
        // 从BoardCell上获取piece组件
        GameObject piece = boardCell.transform.Find("Piece").gameObject;
        // 获取piece上的所有Renderer组件
        Renderer renderer = piece.GetComponent<Renderer>();
        Material materials = defaultMaterial;
        switch (type)
        {
            case PieceType.Black:
                materials = blackMaterial;
                break;
            case PieceType.White:
                materials = whiteMaterial;
                break;
            case PieceType.None:
                materials = defaultMaterial;
                break;
        }
        renderer.material = materials;
    }
}
