using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopPanel : MonoBehaviour
{
    public static WeaponShopPanel s_instance = null;

    public Transform bgTrans;
    public Text text_curDiamond;
    public Transform weapinTrans0;
    public Transform weapinTrans1;
    public Transform weapinTrans2;

    WeaponData[] weaponArray = new WeaponData[3];
    bool[] buyStateArray = new bool[3];

    Vector2 bgStartPos;

    private void Awake()
    {
        s_instance = this;

        bgStartPos = bgTrans.localPosition;
        gameObject.SetActive(false);
    }

    Vector3 posDownOffset = new Vector3(0,-740,0);
    public void show()
    {
        bgTrans.localPosition += posDownOffset;
        bgTrans.DOLocalMoveY(bgStartPos.y,0.4f);

        text_curDiamond.text = GameUILayer.s_instance.curDiamond.ToString();
        gameObject.SetActive(true);
    }

    public void diamondChanged()
    {
        text_curDiamond.text = GameUILayer.s_instance.curDiamond.ToString();

        for (int i = 0; i <= 2; i++)
        {
            Transform weaponTrans = transform.Find("bg/weapon" + i);
            int price = weaponArray[i].level * 5;
            weaponTrans.Find("btn_buy/price").GetComponent<Text>().text = price.ToString();
            if (price <= GameUILayer.s_instance.curDiamond)
            {
                weaponTrans.Find("btn_buy/price").GetComponent<Text>().color = Color.white;
            }
            else
            {
                weaponTrans.Find("btn_buy/price").GetComponent<Text>().color = Color.red;
            }
        }
    }

    public void refreshWeapon()
    {
        int minLevel = 1;
        int maxLevel = 1;

        if (GameUILayer.s_instance.curBoCi >= 71)
        {
            minLevel = 7;
            maxLevel = 9;
        }
        else if (GameUILayer.s_instance.curBoCi >= 61)
        {
            minLevel = 6;
            maxLevel = 7;
        }
        else if (GameUILayer.s_instance.curBoCi >= 51)
        {
            minLevel = 5;
            maxLevel = 7;
        }
        else if (GameUILayer.s_instance.curBoCi >= 41)
        {
            minLevel = 4;
            maxLevel = 6;
        }
        else if (GameUILayer.s_instance.curBoCi >= 31)
        {
            minLevel = 3;
            maxLevel = 5;
        }
        else if (GameUILayer.s_instance.curBoCi >= 21)
        {
            minLevel = 2;
            maxLevel = 4;
        }
        else if (GameUILayer.s_instance.curBoCi >= 11)
        {
            minLevel = 1;
            maxLevel = 3;
        }
        else
        {
            minLevel = 1;
            maxLevel = 2;
        }

        for(int i = 0; i <= 2; i++)
        {
            buyStateArray[i] = false;

            weaponArray[i] = WeaponEntity.getInstance().getData(RandomUtil.getRandom(1,5),RandomUtil.getRandom(minLevel,maxLevel));

            Transform weaponTrans = transform.Find("bg/weapon" + i);
            weaponTrans.Find("buyed").localScale = Vector3.zero;
            weaponTrans.Find("name").GetComponent<Text>().text = weaponArray[i].name;

            int price = weaponArray[i].level * 5;
            weaponTrans.Find("btn_buy/price").GetComponent<Text>().text = price.ToString();
            if(price <= GameUILayer.s_instance.curDiamond)
            {
                weaponTrans.Find("btn_buy/price").GetComponent<Text>().color = Color.white;
            }
            else
            {
                weaponTrans.Find("btn_buy/price").GetComponent<Text>().color = Color.red;
            }

            weaponTrans.Find("weapon/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponArray[i].type);
            weaponTrans.Find("weapon/frame").GetComponent<Image>().color = Consts.list_weaponColor[weaponArray[i].type];
            weaponTrans.Find("weapon/level_bg").GetComponent<Image>().color = Consts.list_weaponColor[weaponArray[i].type];
            weaponTrans.Find("weapon/level_bg/level").GetComponent<Text>().text = weaponArray[i].level.ToString();
        }
    }

    public void onClickBuy(int index)
    {
        int price = weaponArray[index].level * 5;
        if (GameUILayer.s_instance.curDiamond >= price)
        {
            // 检测是否有空格子
            for (int i = 0; i < GameUILayer.s_instance.weaponGridTrans.childCount; i++)
            {
                if (GameUILayer.s_instance.weaponGridTrans.GetChild(i).childCount == 0)
                {
                    Transform weaponTrans = transform.Find("bg/weapon" + index);
                    weaponTrans.Find("buyed").localScale = Vector3.one;

                    GameUILayer.s_instance.changeDiamond(-price);
                    GameUILayer.s_instance.addWeapon(weaponArray[index]);
                    return;
                }
            }

            ToastScript.show("Weapon grid is full");
        }
        else
        {
            ToastScript.show("Diamond Not Enough!");
        }
    }

    public void onClickClose()
    {
        bgTrans.DOLocalMoveY(bgStartPos.y + posDownOffset.y, 0.4f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }
}
