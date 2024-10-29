using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUnit : MonoBehaviour
{
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
        Transparency = 0.2f;

        // 初始化棋子种类为无
        CurrentPieceType = PieceType.None;
        SetOpacity(Transparency);
    }

    // Update方法用于在每一帧更新透明度
    void Update()
    {

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
    }
}
