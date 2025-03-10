using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossNoticeLayer : MonoBehaviour
{
    void Start()
    {
        AudioScript.s_instance.playSound("bossNotice");

        Destroy(gameObject,1.5f);
    }
}
