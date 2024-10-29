using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Board3DController board3DController;
    //public Transform pivotPoint; // 中心点，摄像头将围绕它旋转
    public float rotationSpeed = 5.0f; // 旋转速度

    private float _rotationX = 0.0f;
    private float _rotationY = 0.0f;

    void Update()
    {
        // 当鼠标左键被按下时
        if (Input.GetMouseButton(0))
        {
            // 计算鼠标移动的差值
            _rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
            _rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;

            // 限制垂直方向的旋转，避免翻转
            _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
        }

        Vector3 pivotPosition = board3DController.centerPosition;
        // 更新摄像头的旋转
        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        transform.position = pivotPosition + rotation * new Vector3(0, 0, -10); // 调整距离以适应您的场景
        transform.LookAt(pivotPosition);
    }
}

