using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWeapon : MonoBehaviour
{
    public Image img_icon;
    public Text text_level;

    [HideInInspector]
    public WeaponData weaponData;
    [HideInInspector]
    public bool isCanDrag = false;

    Transform parentTrans;
    Transform mergeTarget = null;

    RaycastHit raycastHit;
    Transform dragTriggerWeaponBar = null;

    public void init(int type,int level)
    {
        parentTrans = transform.parent;

        weaponData = WeaponEntity.getInstance().getData(type, level);

        text_level.text = level.ToString();

        // 武器icon
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + type);
        img_icon.transform.localScale = Consts.weapinUIIconStartScale;
        img_icon.transform.DOScale(1, 0.25f).OnComplete(()=>
        {
            checkMerge();
        });
    }

    public void checkMerge()
    {
        isCanDrag = false;

        // 检测是否可以合并
        for (int i = 0; i < GameUILayer.s_instance.weaponGridTrans.childCount; i++)
        {
            if (GameUILayer.s_instance.weaponGridTrans.GetChild(i).childCount == 1 && GameUILayer.s_instance.weaponGridTrans.GetChild(i) != transform.parent)
            {
                UIItemWeapon uiItemWeapon_to = GameUILayer.s_instance.weaponGridTrans.GetChild(i).GetChild(0).GetComponent<UIItemWeapon>();
                if ((uiItemWeapon_to.isCanDrag) && (uiItemWeapon_to.weaponData.type == weaponData.type) && (uiItemWeapon_to.weaponData.level == weaponData.level) && (weaponData.level < 10))
                {
                    uiItemWeapon_to.isCanDrag = false;
                    transform.SetParent(GameUILayer.s_instance.transform);
                    
                    float moveTime = Vector3.Distance(transform.position, uiItemWeapon_to.transform.position) / 10f;
                    if (moveTime > 0.3f)
                    {
                        moveTime = 0.3f;
                    }

                    transform.DOMove(uiItemWeapon_to.transform.position, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        uiItemWeapon_to.init(RandomUtil.getRandom(1,(int)(Consts.WeaponType.End - 1)), uiItemWeapon_to.weaponData.level + 1);
                        Destroy(gameObject);
                    });

                    return;
                }
            }
        }

        isCanDrag = true;
    }

    public void addLevel()
    {
        if(weaponData.level >= 10)
        {
            return;
        }

        weaponData = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
        text_level.text = weaponData.level.ToString();
    }

    private void OnMouseDown()
    {
        if (GameLayer.s_instance.isGameOver)
        {
            return;
        }
        if (!isCanDrag)
        {
            return;
        }

        mergeTarget = null;
        transform.SetParent(GameUILayer.s_instance.transform);

        // 显示所有角色的武器适配性emoji
        for (int i = 0; i < GameLayer.s_instance.heroPoint.childCount; i++)
        {
            if(GameLayer.s_instance.heroPoint.GetChild(i).childCount > 0)
            {
                GameLayer.s_instance.heroPoint.GetChild(i).GetChild(0).GetComponent<HeroLogicBase>().showWeaponEmoji(weaponData);
            }
        }

        GameUILayer.s_instance.setIsShowBtnWeaponSell(true, weaponData);
    }

    private void OnMouseDrag()
    {
        if (GameLayer.s_instance.isGameOver)
        {
            return;
        }
        if (!isCanDrag)
        {
            return;
        }

        transform.localPosition = CommonUtil.getCurMousePosToUI() + Consts.weaponItemDragOffset;

        Transform trans = getMinDisItemWeapon();
        if (trans)
        {
            if (Vector2.Distance(trans.position, transform.position) <= 0.5f)
            {
                // 先重置之前选中的格子
                if (mergeTarget && mergeTarget != trans)
                {
                    mergeTarget.GetComponent<Image>().color = Color.white;
                    mergeTarget.DOScale(1f, 0.2f);
                }

                mergeTarget = trans;
                mergeTarget.GetComponent<Image>().color = Color.green;
                mergeTarget.DOScale(1.1f, 0.2f);
            }
            else if (mergeTarget)
            {
                mergeTarget.GetComponent<Image>().color = Color.white;
                mergeTarget.DOScale(1f, 0.2f);
                mergeTarget = null;
            }
        }
    }

    private void OnMouseUp()
    {
        if (GameLayer.s_instance.isGameOver)
        {
            return;
        }

        GameUILayer.s_instance.setIsShowBtnWeaponSell(false);

        if (!isCanDrag)
        {
            return;
        }

        // 关闭所有角色的武器适配性emoji
        for (int i = 0; i < GameLayer.s_instance.heroPoint.childCount; i++)
        {
            if (GameLayer.s_instance.heroPoint.GetChild(i).childCount > 0)
            {
                GameLayer.s_instance.heroPoint.GetChild(i).GetChild(0).GetComponent<HeroLogicBase>().hideWeaponEmoji();
            }
        }

        if (mergeTarget)
        {
            if (mergeTarget.childCount > 0)
            {
                UIItemWeapon uIItemWeapon = mergeTarget.GetChild(0).GetComponent<UIItemWeapon>();

                // 武器合成
                if (uIItemWeapon.weaponData.type == weaponData.type && uIItemWeapon.weaponData.level == weaponData.level && uIItemWeapon.weaponData.level <= 9)
                {
                    AudioScript.s_instance.playSound("weaponMerge");
                    mergeTarget.GetComponent<Image>().color = Color.white;
                    mergeTarget.DOScale(1f, 0.2f);
                    uIItemWeapon.addLevel();
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                parentTrans = mergeTarget;
                transform.SetParent(mergeTarget);
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;

                mergeTarget.GetComponent<Image>().color = Color.white;
                mergeTarget.DOScale(1f, 0.2f);
                return;
            }
        }

        if (mergeTarget)
        {
            mergeTarget.GetComponent<Image>().color = Color.white;
            mergeTarget.DOScale(1f, 0.2f);
        }

        // 未拖到角色上,检测是否拖到了卖出按钮上
        if (BtnWeaponSellEvent.s_instance.itemWeapon == this)
        {
            AudioScript.s_instance.playSound("sellWeapon");

            // 防止卖出按钮那边检测碰撞
            transform.tag = "Untagged";

            GameUILayer.s_instance.changeDiamond(weaponData.level);
            GameUILayer.s_instance.setIsShowBtnWeaponSell(false);
            Destroy(gameObject);
            return;
        }
        // 检测是否拖到了武器栏
        else if (dragTriggerWeaponBar != null)
        {
            // 防止卖出按钮那边检测碰撞
            transform.tag = "Untagged";

            dragTriggerWeaponBar.GetComponent<WeaponBar>().setData(weaponData);

            Destroy(gameObject);
            return;
        }

        isCanDrag = false;
        transform.DOMove(parentTrans.position, 0.2f).OnComplete(()=>
        {
            isCanDrag = true;
            transform.SetParent(parentTrans);
        });
    }

    Transform getMinDisItemWeapon()
    {
        float tempDis;
        float minDis = 999;
        Transform trans = null;
        for (int i = 0; i < GameUILayer.s_instance.weaponGridTrans.childCount; i++)
        {
            // 不限制是否为空格子
            // if (GameUILayer.s_instance.weaponGridTrans.GetChild(i).childCount == 1)
            {
                tempDis = Vector2.Distance(GameUILayer.s_instance.weaponGridTrans.GetChild(i).position, transform.position);
                if (tempDis < minDis)
                {
                    minDis = tempDis;
                    trans = GameUILayer.s_instance.weaponGridTrans.GetChild(i);
                }
            }
        }

        return trans;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WeaponBar"))
        {
            dragTriggerWeaponBar = collision.transform;

            for(int i = 0; i < GameUILayer.s_instance.weaponBarPointTrans.childCount; i++)
            {
                if(GameUILayer.s_instance.weaponBarPointTrans.GetChild(i) == dragTriggerWeaponBar)
                {
                    GameUILayer.s_instance.weaponBarPointTrans.GetChild(i).Find("choiced").localScale = Vector3.one;
                }
                else
                {
                    GameUILayer.s_instance.weaponBarPointTrans.GetChild(i).Find("choiced").localScale = Vector3.zero;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("WeaponBar"))
        {
            if(dragTriggerWeaponBar == collision.transform)
            {
                dragTriggerWeaponBar.Find("choiced").localScale = Vector3.zero;
                dragTriggerWeaponBar = null;
            }
        }
    }
}
