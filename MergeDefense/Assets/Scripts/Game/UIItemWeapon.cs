using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWeapon : MonoBehaviour
{
    public Image img_icon;
    public Image img_frame;
    public Image img_level_bg;
    public Text text_level;

    [HideInInspector]
    public WeaponData weaponData;

    Transform parentTrans;
    Transform mergeTarget = null;

    RaycastHit raycastHit;

    public void init(int type,int level)
    {
        parentTrans = transform.parent;

        weaponData = WeaponEntity.getInstance().getData(type, level);

        text_level.text = level.ToString();

        // 品质框
        transform.GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("dikuang_A");

        // 武器icon
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + type);
        img_icon.transform.localScale = Consts.weapinUIIconStartScale;
        img_icon.transform.DOScale(1, 0.25f);

        img_level_bg.color = Consts.list_weaponColor[weaponData.type];
        img_frame.color = Consts.list_weaponColor[weaponData.type];
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

    Transform curDragChoicedHero = null;
    private void OnMouseDown()
    {
        if (!GameUILayer.s_instance.isCanDragWeapon)
        {
            return;
        }

        mergeTarget = null;
        curDragChoicedHero = null;
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
        if(!GameUILayer.s_instance.isCanDragWeapon)
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
            // 检测是否拖到角色上
            else
            {
                raycastHit = RayUtil.getEndPoint(CommonUtil.mousePosToWorld(GameLayer.s_instance.camera3D) + Consts.mouseRayOffset);
                if (raycastHit.collider && raycastHit.collider.CompareTag("Hero"))
                {
                    if(curDragChoicedHero != raycastHit.collider.transform)
                    {
                        curDragChoicedHero = raycastHit.collider.transform;
                        HeroLogicBase heroLogicBase = curDragChoicedHero.GetComponent<HeroLogicBase>();

                        if (heroLogicBase.isCanAddWeapon(weaponData))
                        {
                            GameLayer.s_instance.weaponChoiceKuang.position = heroLogicBase.heroQualityTrans.position;
                            GameLayer.s_instance.weaponChoiceKuang.localScale = Vector3.one;
                        }
                        else
                        {
                            GameLayer.s_instance.weaponChoiceKuang.localScale = Vector3.zero;
                        }
                    }
                }
                else
                {
                    curDragChoicedHero = null;
                    GameLayer.s_instance.weaponChoiceKuang.localScale = Vector3.zero;
                }
            }
        }
    }

    private void OnMouseUp()
    {
        GameUILayer.s_instance.setIsShowBtnWeaponSell(false);

        if (!GameUILayer.s_instance.isCanDragWeapon)
        {
            return;
        }

        GameLayer.s_instance.weaponChoiceKuang.localScale = Vector3.zero;

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

        // 检测是否拖到角色上
        {
            raycastHit = RayUtil.getEndPoint(CommonUtil.mousePosToWorld(GameLayer.s_instance.camera3D) + Consts.mouseRayOffset);
            if (raycastHit.collider && raycastHit.collider.CompareTag("Hero"))
            {
                HeroLogicBase heroLogicBase = raycastHit.collider.GetComponent<HeroLogicBase>();
                if (heroLogicBase.isCanAddWeapon(weaponData))
                {
                    heroLogicBase.addWeapon(weaponData);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                // 未拖到角色上,检测是否拖到了卖出按钮上
                if(BtnWeaponSellEvent.s_instance.itemWeapon == this)
                {
                    AudioScript.s_instance.playSound("sellWeapom");

                    // 防止卖出按钮那边检测碰撞
                    transform.tag = "Untagged";

                    GameUILayer.s_instance.changeDiamond(weaponData.level);
                    GameUILayer.s_instance.setIsShowBtnWeaponSell(false);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        GameUILayer.s_instance.isCanDragWeapon = false;
        transform.DOMove(parentTrans.position, 0.2f).OnComplete(()=>
        {
            GameUILayer.s_instance.isCanDragWeapon = true;
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
}
