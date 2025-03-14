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
    List<Text> list_damageNumText = new List<Text>();

    void Awake()
    {
        s_instance = this;
    }

    public void showDamageNum(int num, Vector3 worldPos,bool isCrit)
    {
        Transform damageNumTrans = null;
        for (int i = 0; i < list_damageNum.Count; i++)
        {
            if(list_damageNum[i].localScale.x == 0)
            {
                damageNumTrans = list_damageNum[i];
                damageNumTrans.localScale = Vector3.one;
                list_damageNumText[i].text = CommonUtil.intToStrK(num);

                if (isCrit)
                {
                    list_damageNumText[i].color = Consts.color_critText;
                }
                else
                {
                    list_damageNumText[i].color = Color.white;
                }
                break;
            }
        }

        if(damageNumTrans == null)
        {
            damageNumTrans = Instantiate(prefab_damageNum, transform).transform;
            list_damageNum.Add(damageNumTrans);
            list_damageNumText.Add(damageNumTrans.GetChild(0).GetComponent<Text>());
            list_damageNumText[list_damageNumText.Count - 1].text = CommonUtil.intToStrK(num);

            if (isCrit)
            {
                list_damageNumText[list_damageNumText.Count - 1].color = Consts.color_critText;
            }
        }

        damageNumTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, worldPos);
        damageNumTrans.DOLocalMoveY(damageNumTrans.localPosition.y + RandomUtil.getRandom(60, 100), 0.4f).OnComplete(()=>
        {
            damageNumTrans.localScale = Vector3.zero;
        });

        if(isCrit)
        {
            damageNumTrans.DOScale(1.6f, 0.15f).OnComplete(()=>
            {
                damageNumTrans.DOScale(1.2f, 0.1f);
            });
        }
    }
}
