using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillGoldManager : MonoBehaviour
{
    public static KillGoldManager s_instance = null;

    public GameObject prefab_killGold;

    List<Text> list_killGold = new List<Text>();

    void Awake()
    {
        s_instance = this;
    }

    public void showKillGold(int num)
    {
        Text killGoldText = null;
        for (int i = 0; i < list_killGold.Count; i++)
        {
            if(list_killGold[i].transform.localScale.x == 0)
            {
                killGoldText = list_killGold[i];
                killGoldText.transform.localScale = Vector3.one;
                if (num >= 0)
                {
                    killGoldText.text = "+"+num.ToString();
                    killGoldText.color = Color.white;
                }
                else
                {
                    killGoldText.text = num.ToString();
                    killGoldText.color = Color.red;
                }
                break;
            }
        }

        if(killGoldText == null)
        {
            killGoldText = Instantiate(prefab_killGold, transform).GetComponent<Text>();
            list_killGold.Add(killGoldText);
            killGoldText.text = num.ToString();
        }

        killGoldText.transform.position = GameUILayer.s_instance.text_gold.transform.position;
        killGoldText.transform.DOLocalMoveY(killGoldText.transform.localPosition.y + 70, 0.8f).SetEase(Ease.OutCubic).OnComplete(()=>
        {
            killGoldText.transform.localScale = Vector3.zero;
        });
        killGoldText.DOFade(0, 0.8f).SetEase(Ease.OutCubic);
    }
}
