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
                    ShowPlaneAt(position);
                }
                else//左键单击落子
                {
                    PlacePiece(position);
                }

            }     
        }
    }

    void ShowPlaneAt(Vector3 position)
    {
        // 根据x, y, z调整摄像机或隐藏/显示平面
        // 例如，如果你想显示x平面，可以设置y和z坐标相同
        Camera.main.transform.position = new Vector3(
            Camera.main.transform.position.x,
            position.y,
            position.z
        );

        // 确保摄像机朝向正确的方向，比如朝向原点
        Camera.main.transform.LookAt(position);
        Camera.main.enabled = true;

        // 这里可以添加代码来隐藏或显示其他平面
    }

    void PlacePiece(Vector3Int position)
    {
        Debug.Log("focused cell: " + position.ToString());
    }


}
