using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumManager : MonoBehaviour
{
    public static DamageNumManager s_instance = null;

    public GameObject prefab_damageNum;

    List<Transform> list_damageNum = new List<Transform>();

    void Awake()
    {
        s_instance = this;
    }

    public void showDamageNum(int num, Vector3 worldPos)
    {
        Transform damageNumTrans = null;
        for (int i = 0; i < list_damageNum.Count; i++)
        {
            if(list_damageNum[i].localScale.x == 0)
            {
                damageNumTrans = list_damageNum[i];
                damageNumTrans.localScale = Vector3.one;
                break;
            }
        }

        if(!damageNumTrans)
        {
            damageNumTrans = Instantiate(prefab_damageNum, transform).transform;
            list_damageNum.Add(damageNumTrans);
        }
        damageNumTrans.GetChild(0).GetComponent<Text>().text = num.ToString();
        damageNumTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, worldPos);
        damageNumTrans.DOLocalMoveY(damageNumTrans.localPosition.y + 80, 0.4f).OnComplete(()=>
        {
            damageNumTrans.localScale = Vector3.zero;
        });
    }
}
