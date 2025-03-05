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
    public Text text_time;
    public Transform weapinTrans0;
    public Transform weapinTrans1;
    public Transform weapinTrans2;

    WeaponData[] weaponArray = new WeaponData[3];
    bool[] buyStateArray = new bool[3];

    Vector2 bgStartPos;
    bool isClosed = false;

    private void Awake()
    {
        s_instance = this;

        bgStartPos = bgTrans.localPosition;
        gameObject.SetActive(false);
    }

    Vector3 posDownOffset = new Vector3(0,-740,0);
    public void show()
    {
        isClosed = false;

        bgTrans.localPosition += posDownOffset;
        bgTrans.DOLocalMoveY(bgStartPos.y,0.4f);

        text_curDiamond.text = GameUILayer.s_instance.curDiamond.ToString();
        gameObject.SetActive(true);

        refreshTime();

        InvokeRepeating("refreshTime",0.1f, 0.1f);
    }

    void refreshTime()
    {
        text_time.text = GameUILayer.s_instance.curBoCiRestTime + "s";
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
        int[] levelArray;

        if (GameUILayer.s_instance.curBoCi >= 71)
        {
            levelArray = new int[3];
            levelArray[0] = 7;
            levelArray[1] = 8;
            levelArray[2] = 9;
        }
        else if (GameUILayer.s_instance.curBoCi >= 61)
        {
            levelArray = new int[3];
            levelArray[0] = 6;
            levelArray[1] = 7;
            levelArray[2] = 8;
        }
        else if (GameUILayer.s_instance.curBoCi >= 51)
        {
            levelArray = new int[3];
            levelArray[0] = 5;
            levelArray[1] = 6;
            levelArray[2] = 7;
        }
        else if (GameUILayer.s_instance.curBoCi >= 41)
        {
            levelArray = new int[3];
            levelArray[0] = 4;
            levelArray[1] = 5;
            levelArray[2] = 6;
        }
        else if (GameUILayer.s_instance.curBoCi >= 31)
        {
            levelArray = new int[3];
            levelArray[0] = 3;
            levelArray[1] = 4;
            levelArray[2] = 5;
        }
        else if (GameUILayer.s_instance.curBoCi >= 21)
        {
            levelArray = new int[3];
            levelArray[0] = 2;
            levelArray[1] = 3;
            levelArray[2] = 4;
        }
        else if (GameUILayer.s_instance.curBoCi >= 11)
        {
            levelArray = new int[3];
            levelArray[0] = 1;
            levelArray[1] = 2;
            levelArray[2] = 3;
        }
        else
        {
            levelArray = new int[2];
            levelArray[0] = 1;
            levelArray[1] = 2;
        }

        for(int i = 0; i <= 2; i++)
        {
            buyStateArray[i] = false;

            int level = 0;
            if(levelArray.Length == 2)
            {
                int r = RandomUtil.getRandom(1, 100);
                if (r > 70)
                {
                    level = levelArray[1];
                }
                else
                {
                    level = levelArray[0];
                }
            }
            else
            {
                int r = RandomUtil.getRandom(1,100);
                if(r > 90)
                {
                    level = levelArray[2];
                }
                else if (r > 70)
                {
                    level = levelArray[1];
                }
                else
                {
                    level = levelArray[0];
                }
            }
            weaponArray[i] = WeaponEntity.getInstance().getData(RandomUtil.getRandom(1,5), level);

            Transform weaponTrans = transform.Find("bg/weapon" + i);
            weaponTrans.Find("btn_buy").localScale = Vector3.one;
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
        AudioScript.s_instance.playSound_btn();

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
                    weaponTrans.Find("btn_buy").localScale = Vector3.zero;

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
        if(isClosed)
        {
            return;
        }
        isClosed = true;
        CancelInvoke("refreshTime");
        AudioScript.s_instance.playSound_btn();

        bgTrans.DOLocalMoveY(bgStartPos.y + posDownOffset.y, 0.4f).OnComplete(()=>
        {
            gameObject.SetActive(false);
        });
    }
}
