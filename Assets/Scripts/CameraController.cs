using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Board3DController board3DController;
    public float rotationSpeed = 5.0f; // 旋转速度
    public float zoomSpeed = 1f;
    public float minZoom = 1f; // 最小缩放距离
    public float maxZoom = 30f; // 最大缩放距离

    private float _rotationX = 0.0f;
    private float _rotationY = 0.0f;
    private float _distance = 10f; // 初始距离

    void Update()
    {
        // 检测鼠标滚轮的输入，并进行缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll);

        // 当鼠标左键被按下时，进行旋转
        if (Input.GetMouseButton(0))
        {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        // 计算鼠标移动的差值
        _rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        _rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // 限制垂直方向的旋转，避免翻转
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        // 根据旋转角度计算新的摄像机位置
        Quaternion rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        transform.position = board3DController.centerPosition + rotation * Vector3.back * _distance;
        transform.LookAt(board3DController.centerPosition);
    }

    void ZoomCamera(float scroll)
    {

        // 获取摄像机Transform组件
        Transform cameraTransform = Camera.main.transform;

        // 计算摄像机与目标点之间的距离
        float distance = Vector3.Distance(cameraTransform.position, board3DController.centerPosition);

        // 根据鼠标滚轮的滚动方向调整距离
        distance -= scroll * zoomSpeed * distance; // 使用相对缩放
        distance = Mathf.Clamp(distance, minZoom, maxZoom); // 限制缩放范围

        // 计算新的摄像机位置
        Vector3 newPosition = board3DController.centerPosition - cameraTransform.forward * distance;
        cameraTransform.position = newPosition;
    }
}


