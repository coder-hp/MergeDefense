using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyChangeTextPoint : MonoBehaviour
{
    public static MoneyChangeTextPoint s_instance = null;

    public GameObject prefab_moneyChangeText;

    List<Text> list_text = new List<Text>();

    void Awake()
    {
        s_instance = this;
    }

    // from:1金币  2钻石
    public void show(int num,int from)
    {
        Text changeNumText = null;
        for (int i = 0; i < list_text.Count; i++)
        {
            if(list_text[i].transform.localScale.x == 0)
            {
                changeNumText = list_text[i];
                changeNumText.transform.localScale = Vector3.one;
                if (num >= 0)
                {
                    changeNumText.text = "+"+num.ToString();
                    changeNumText.color = Color.white;
                }
                else
                {
                    changeNumText.text = num.ToString();
                    changeNumText.color = Color.red;
                }
                break;
            }
        }

        if(changeNumText == null)
        {
            changeNumText = Instantiate(prefab_moneyChangeText, transform).GetComponent<Text>();
            list_text.Add(changeNumText);
            changeNumText.text = num.ToString();
        }

        if (from == 1)
        {
            changeNumText.transform.position = GameUILayer.s_instance.text_gold.transform.position;
        }
        else if (from == 2)
        {
            changeNumText.transform.position = GameUILayer.s_instance.text_diamond.transform.position;
        }
        changeNumText.transform.DOLocalMoveY(changeNumText.transform.localPosition.y + 70, 0.8f).SetEase(Ease.OutCubic).OnComplete(()=>
        {
            changeNumText.transform.localScale = Vector3.zero;
        });
        changeNumText.DOFade(0, 0.8f).SetEase(Ease.OutCubic);
    }
}
