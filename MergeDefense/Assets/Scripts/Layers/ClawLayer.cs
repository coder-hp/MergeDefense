using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLayer : MonoBehaviour
{
    public Transform clawTrans;

    void Start()
    {
        
    }

    public void onClickClaw()
    {
        clawTrans.DOLocalMoveY(-600, 3).SetEase(Ease.Linear).OnComplete(()=>
        {
            clawTrans.Find("left").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0,0, 90),0.5f).SetDelay(1);
            clawTrans.Find("right").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -90), 0.5f).SetDelay(1).OnComplete(()=>
            {
                clawTrans.DOLocalMoveY(860, 3).SetDelay(1).SetEase(Ease.Linear);
            });
        });
    }
}
