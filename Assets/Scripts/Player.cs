using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public GameObject boardController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)&& hit.collider.TryGetComponent<BoardUnit>(out var boardUnit))
            {
                Vector3Int position = boardUnit.Position;
                // 检查是否同时按下了Control键
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))//Ctrl+左键显示平面
                {
                    ShowPlaneAt(position,0);
                }
                else//左键单击落子
                {
                    PlacePiece(position);
                }

            }     
        }
    }

    void ShowPlaneAt(Vector3 position,int axis)
    {
        
    }

    void PlacePiece(Vector3Int position)
    {
        Debug.Log("focused cell: " + position.ToString());
    }


}
