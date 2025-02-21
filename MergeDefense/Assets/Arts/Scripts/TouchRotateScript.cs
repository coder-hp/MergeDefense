using UnityEngine;

public class TouchRotateScript : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotateSpeed = 0.5f;    // 旋转速度系数
    public float inertiaDamping = 0.95f; // 惯性阻尼系数 (0-1)
    public bool enableInertia = true;   // 是否启用惯性效果

    [Header("旋转轴限制")]
    public bool allowX = true;         // 允许绕X轴旋转
    public bool allowY = true;         // 允许绕Y轴旋转

    private Vector2 lastTouchPos;      // 上次触摸位置
    private Vector2 deltaTouch;        // 触摸位移差
    private Vector3 currentRotation;   // 当前旋转速度
    private bool isDragging = false;   // 是否正在拖动

    void Update()
    {
        // 处理触摸/鼠标输入
        HandleInput();

        // 旋转
        ApplyRotation();
    }

    void HandleInput()
    {
        // 移动端触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastTouchPos = touch.position;
                    isDragging = true;
                    currentRotation = Vector3.zero; // 停止惯性
                    break;

                case TouchPhase.Moved:
                    deltaTouch = touch.position - lastTouchPos;
                    lastTouchPos = touch.position;
                    CalculateRotation(deltaTouch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
        // PC端鼠标输入
        else if (Input.GetMouseButtonDown(0))
        {
            lastTouchPos = Input.mousePosition;
            isDragging = true;
            currentRotation = Vector3.zero;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            deltaTouch = (Vector2)Input.mousePosition - lastTouchPos;
            lastTouchPos = Input.mousePosition;
            CalculateRotation(deltaTouch);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void CalculateRotation(Vector2 delta)
    {
        // 计算旋转速度（根据设备DPI调整）
        float dpiFactor = Screen.dpi > 0 ? Screen.dpi / 160f : 1f;

        Vector3 rotationDelta = new Vector3(
            allowX ? delta.y * rotateSpeed / dpiFactor : 0,
            allowY ? -delta.x * rotateSpeed / dpiFactor : 0,
            0
        );

        // 平滑过渡
        currentRotation = Vector3.Lerp(currentRotation, rotationDelta, 0.5f);
    }

    void ApplyRotation()
    {
        if (enableInertia)
        {
            // 惯性效果
            if (!isDragging && currentRotation.magnitude > 0.01f)
            {
                transform.Rotate(currentRotation * Time.deltaTime * 50, Space.World);
                currentRotation *= inertiaDamping;
            }
            else if (isDragging)
            {
                transform.Rotate(currentRotation * Time.deltaTime * 50, Space.World);
            }
        }
        else
        {
            // 无惯性直接旋转
            if (isDragging)
            {
                transform.Rotate(currentRotation * Time.deltaTime * 50, Space.World);
            }
        }
    }
}
