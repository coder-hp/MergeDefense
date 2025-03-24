using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMissionLayer : MonoBehaviour
{
    public Transform btn_take;
    public Transform timeTrans;
    public Transform rewardTrans;
    public Text text_desc;

    void Start()
    {
        if(BattleMission.s_instance.isTakeMission)
        {
            btn_take.localScale = Vector3.zero;
        }
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if (isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(gameObject);
        });
    }

    public void onClickTake()
    {
        onClickClose();
        BattleMission.s_instance.takeMission();
    }
}
