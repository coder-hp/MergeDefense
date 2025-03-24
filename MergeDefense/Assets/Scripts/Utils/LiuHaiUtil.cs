using UnityEngine;

// 刘海屏适配
public class LiuHaiUtil : MonoBehaviour
{
    void Awake()
    {
        Rect safeAreaRect = Screen.safeArea;
        float liuhaiHeight = (Screen.height - safeAreaRect.yMax);
        //liuhaiHeight *= ((float)Screen.height / (float)Screen.width) / (1920f / 1080f);
        //liuhaiHeight *= (float)Screen.height / 1920f;
        if (liuhaiHeight > 0)
        {
            transform.localPosition -= new Vector3(0, liuhaiHeight, 0);
        }
    }
}
