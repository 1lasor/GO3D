using UnityEngine;
public class Board3DController : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject boardcellPrefab; // 棋盘单元预制体

    public int boardSize = 8; // 棋盘大小
    public float spacing = 1.0f; // 棋盘单元间隔
    public Vector3 centerPosition;


    void Start()
    {
        GenerateBoard();
        GetCenterPosition();
        //MoveCameraToCenter();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for(int z = 0;z < boardSize; z++)
                {
                    // 计算位置
                    Vector3 position = new Vector3(x * spacing, y * spacing, z * spacing);

                    // 实例化预制体
                    GameObject boardCell = Instantiate(boardcellPrefab, position, Quaternion.identity);
                    BoardUnit boardUnit = boardCell.GetComponent<BoardUnit>();
                    boardUnit.Position = new Vector3Int(x, y, z);
                    Debug.Log(boardUnit.Position.ToString());

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

    void GetCenterPosition()
    {
        float radius = (boardSize - 1) * spacing / 2.0f;
        centerPosition = new Vector3(radius,radius,radius);
    }

    void MoveCameraToCenter()
    {
        if (mainCamera != null)
        {
            // 计算棋盘中心位置
            Vector3 centerPosition = new Vector3((boardSize - 1) * spacing / 2.0f, mainCamera.transform.position.y, (boardSize - 1) * spacing / 2.0f);

            // 设置摄像头位置
            mainCamera.transform.position = centerPosition;
        }
        else
        {
            Debug.LogError("Main Camera is not assigned in the inspector!");
        }
    }

}

