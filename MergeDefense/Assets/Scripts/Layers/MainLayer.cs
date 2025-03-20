using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLayer : MonoBehaviour
{
    public static MainLayer s_instance = null;

    public Transform childLayerPoint;
    public Transform bottomPoint;

    private void Awake()
    {
        s_instance = this;
    }

    public void onClickBottomTab(int index)
    {
        AudioScript.s_instance.playSound_btn();

        //switch(index)
        //{
        //    // 角色养成
        //    case 0:
        //        {
        //            LayerManager.ShowLayer(Consts.Layer.HeroLayer);
        //            break;
        //        }

        //    // 战斗
        //    case 1:
        //        {
        //            LayerManager.ShowLayer(Consts.Layer.BattleLayer);
        //            break;
        //        }

        //    // 抓娃娃机
        //    case 2:
        //        {
        //            LayerManager.ShowLayer(Consts.Layer.ClawLayer);
        //            break;
        //        }
        //}

        for(int i = 0; i < childLayerPoint.childCount; i++)
        {
            if(i == index)
            {
                childLayerPoint.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                childLayerPoint.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < bottomPoint.childCount; i++)
        {
            if (i == index)
            {
                bottomPoint.GetChild(i).localScale = new Vector3(1.1f, 1.1f, 1.1f);
                bottomPoint.GetChild(i).Find("Text").localScale = Vector3.one;
                bottomPoint.GetChild(i).Find("Image").localPosition = new Vector3(0,16,0);
                bottomPoint.GetChild(i).Find("Image").localScale = new Vector3(0.8f, 0.8f, 0.8f);
                bottomPoint.GetChild(i).GetComponent<Image>().sprite = AtlasUtil.getAtlas_main().GetSprite("button_nav_2");
            }
            else
            {
                bottomPoint.GetChild(i).localScale = Vector3.one;
                bottomPoint.GetChild(i).Find("Text").localScale = Vector3.zero;
                bottomPoint.GetChild(i).Find("Image").localPosition = Vector3.zero;
                bottomPoint.GetChild(i).Find("Image").localScale = Vector3.one;
                bottomPoint.GetChild(i).GetComponent<Image>().sprite = AtlasUtil.getAtlas_main().GetSprite("button_nav_1");
            }
        }
    }
}
