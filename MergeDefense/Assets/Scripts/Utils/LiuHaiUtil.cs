using UnityEngine;

// Áõº£ÆÁÊÊÅä
public class LiuHaiUtil : MonoBehaviour
{
    void Awake()
    {
        Rect safeAreaRect = Screen.safeArea;
        float height = Screen.height;
        float liuhaiHeight = (height - safeAreaRect.height) * (1560f / height);
        float chazhi = liuhaiHeight;
        if (chazhi > 0)
        {
            transform.localPosition -= new Vector3(0, chazhi, 0);
        }
    }
}
