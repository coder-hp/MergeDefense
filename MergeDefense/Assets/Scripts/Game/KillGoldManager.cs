using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillGoldManager : MonoBehaviour
{
    public static KillGoldManager s_instance = null;

    public GameObject prefab_killGold;

    List<Transform> list_killGold = new List<Transform>();
    List<Text> list_killGoldText = new List<Text>();

    void Awake()
    {
        s_instance = this;
    }

    public void showKillGold(int num, Vector3 worldPos)
    {
        Transform killGoldTrans = null;
        for (int i = 0; i < list_killGold.Count; i++)
        {
            if(list_killGold[i].localScale.x == 0)
            {
                killGoldTrans = list_killGold[i];
                killGoldTrans.localScale = Vector3.one;
                list_killGoldText[i].text = num.ToString();
                break;
            }
        }

        if(killGoldTrans == null)
        {
            killGoldTrans = Instantiate(prefab_killGold, transform).transform;
            list_killGold.Add(killGoldTrans);
            list_killGoldText.Add(killGoldTrans.GetChild(1).GetComponent<Text>());
            list_killGoldText[list_killGoldText.Count - 1].text = num.ToString();
        }

        killGoldTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, worldPos);
        killGoldTrans.DOLocalMoveY(killGoldTrans.localPosition.y + RandomUtil.getRandom(80, 100), 0.4f).OnComplete(()=>
        {
            killGoldTrans.localScale = Vector3.zero;
        });
    }
}
