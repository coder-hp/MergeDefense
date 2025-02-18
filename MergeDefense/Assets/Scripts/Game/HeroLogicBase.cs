using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroLogicBase : MonoBehaviour
{
    public int id;
    public int curStar = 1;

    [HideInInspector]
    public HeroData heroData;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Transform centerPoint;
    [HideInInspector]
    public List<WeaponData> list_weapon = new List<WeaponData>();
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isMerge = false;
    [HideInInspector]
    public Transform starTrans;

    bool isDraging = false;
    bool isCanUpdate = false;

    [HideInInspector]
    public Transform heroQualityTrans;
    Vector3 heroQualityOffset = new Vector3(0, 0.44f, 0.8f);
    Material material_qualityBg;

    [HideInInspector]
    public Action Attack;

    HeroAniEvent heroAniEvent;


    public void Awake()
    {
        heroAniEvent = transform.Find("model").GetComponent<HeroAniEvent>();
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        transform.DOScale(0.7f, 0.2f).OnComplete(()=>
        {
            transform.DOScale(1f, 0.1f).OnComplete(()=>
            {
                isCanUpdate = true;
            });
        });
        animator = transform.Find("model").GetComponent<Animator>();
        centerPoint = transform.Find("centerPoint");

        heroData = HeroEntity.getInstance().getData(id);

        // 品质背景板
        {
            heroQualityTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroQuality"), GameLayer.s_instance.heroQualityPoint).transform;
            heroQualityTrans.position = transform.position + heroQualityOffset;
            material_qualityBg = heroQualityTrans.GetComponent<MeshRenderer>().material;
        }

        // 星星预设
        {
            starTrans = Instantiate(GameUILayer.s_instance.prefab_heroStar, GameUILayer.s_instance.heroStarPointTrans).transform;
            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
            setStarUI();
        }
    }

    private void Update()
    {
        if(!isCanUpdate)
        {
            return;
        }

        if(isMerge)
        {
            return;
        }

        if(isDraging)
        {
            Vector3 mousePos = CommonUtil.mousePosToWorld(GameLayer.s_instance.camera3D);
            float minDis = 100;
            Transform minDisGrid = null;
            for(int i = 0; i < GameLayer.s_instance.heroGrid.transform.childCount; i++)
            {
                float tempDis = Vector3.Distance(mousePos, GameLayer.s_instance.heroGrid.transform.GetChild(i).position);
                if (tempDis < minDis)
                {
                    minDis = tempDis;
                    minDisGrid = GameLayer.s_instance.heroGrid.transform.GetChild(i);
                }
            }

            if(Input.GetMouseButton(0))
            {
                if(minDis <= 1.2f)
                {
                    GameLayer.s_instance.heroGridChoiced.localScale = Vector3.one;
                    GameLayer.s_instance.heroGridChoiced.transform.position = minDisGrid.position;

                    GameLayer.s_instance.attackRangeTrans.position = minDisGrid.position;
                    GameLayer.s_instance.attackRangeTrans.localScale = new Vector3(heroData.atkRange, heroData.atkRange, heroData.atkRange);

                    HeroMoveLine.s_instance.setPos(GameLayer.s_instance.heroGrid.transform.Find(transform.parent.name).position, minDisGrid.position);
                }
                else
                {
                    GameLayer.s_instance.heroGridChoiced.localScale = Vector3.zero;
                    GameLayer.s_instance.attackRangeTrans.localScale = Vector3.zero;
                    HeroMoveLine.s_instance.hide();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDraging = false;

                GameLayer.s_instance.heroGrid.SetActive(false);
                GameLayer.s_instance.heroGridChoiced.localScale = Vector3.zero;
                GameLayer.s_instance.attackRangeTrans.localScale = Vector3.zero;
                HeroMoveLine.s_instance.hide();

                if (minDis <= 1.2f)
                {
                    Transform minDisGridTrans = GameLayer.s_instance.heroPoint.Find(minDisGrid.name);

                    // 目标格子没有角色
                    if (minDisGridTrans.childCount == 0)
                    {
                        starTrans.localScale = Vector3.zero;
                        transform.SetParent(minDisGridTrans);
                        transform.DOLocalMove(Vector3.zero, 1).OnComplete(()=>
                        {
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });
                        heroQualityTrans.DOMove(minDisGridTrans.position + heroQualityOffset, 1);
                    }
                    // 已有角色，交换位置
                    else
                    {
                        Transform otherHero = minDisGridTrans.GetChild(0);
                        HeroLogicBase heroLogicBase_other = otherHero.GetComponent<HeroLogicBase>();
                        heroLogicBase_other.starTrans.localScale = Vector3.zero;
                        otherHero.SetParent(transform.parent);
                        otherHero.DOLocalMove(Vector3.zero, 1).OnComplete(() =>
                        {
                            heroLogicBase_other.starTrans.localScale = Vector3.one;
                            heroLogicBase_other.starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, otherHero.position);
                        });
                        heroLogicBase_other.heroQualityTrans.DOMove(transform.parent.position + heroQualityOffset, 1);

                        starTrans.localScale = Vector3.zero;
                        transform.SetParent(minDisGridTrans);
                        transform.DOLocalMove(Vector3.zero, 1).OnComplete(() =>
                        {
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });
                        heroQualityTrans.DOMove(minDisGridTrans.position + heroQualityOffset, 1);
                    }
                }
            }
        }

        checkAttack();
    }

    public bool checkAttack()
    {
        if (!isAttacking)
        {
            EnemyLogic enemyLogic = EnemyManager.s_instance.getMinDisTarget(transform);
            if (enemyLogic && Vector3.Distance(transform.position, enemyLogic.transform.position) <= heroData.atkRange)
            {
                heroAniEvent.enemyLogic = enemyLogic;
                lookEnemy(enemyLogic);
                isAttacking = true;
                playAni("attack");

                return true;
            }
        }

        return false;
    }

    void setStarUI()
    {
        int showCount = curStar % 3;
        if (showCount == 0)
        {
            showCount = 3;
        }
        for (int i = 1; i <= 3; i++)
        {
            if (i <= showCount)
            {
                starTrans.Find(i.ToString()).gameObject.SetActive(true);
            }
            else
            {
                starTrans.Find(i.ToString()).gameObject.SetActive(false);
            }
        }

        // 橙色
        if (curStar > 9)
        {
            material_qualityBg.SetColor("_Color",new Color(1, 0.6f,0.24f));
        }
        // 紫色
        else if (curStar > 6)
        {
            material_qualityBg.SetColor("_Color", new Color(1, 0.34f, 0.88f));
        }
        // 蓝色
        else if (curStar > 3)
        {
            material_qualityBg.SetColor("_Color", new Color(0.46f, 0.57f,1));
        }
    }

    void lookEnemy(EnemyLogic enemyLogic)
    {
        float angle = -CommonUtil.twoPointAngle(centerPoint.position, enemyLogic.centerPoint.position);
        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    public int getAtk()
    {
        return heroData.atk;
    }

    public void addWeapon(WeaponData weaponData)
    {
        ToastScript.show("增加武器:" + weaponData.name + " level" + weaponData.level);
        Debug.Log(transform.name + "增加武器:" + weaponData.name + " level" + weaponData.level);
        list_weapon.Add(weaponData);
    }

    public void addStar()
    {
        ++curStar;
        setStarUI();
    }

    public void playAni(string aniName,float crossFadeTime = 0)
    {
        if(crossFadeTime > 0)
        {
            animator.CrossFade(aniName, crossFadeTime);
        }
        else
        {
            animator.Play(aniName,0,0);
        }
    }

    bool isTriggerMouseDown = false;
    private void OnMouseDown()
    {
        if (!GameUILayer.s_instance.heroInfoPanel.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }

        isTriggerMouseDown = true;
    }

    private void OnMouseUp()
    {
        if (!GameUILayer.s_instance.heroInfoPanel.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }

        if (isTriggerMouseDown)
        {
            if(GameUILayer.s_instance.heroInfoPanel.gameObject.activeInHierarchy && GameUILayer.s_instance.heroInfoPanel.heroLogicBase == this)
            {
                // 如果当前角色信息面板显示的是当前点击的角色，则关闭信息面板
                // 触发的是UI那边的HeroInfoPanel.onClickClose
            }
            else
            {
                if (GameUILayer.s_instance.heroInfoPanel.gameObject.activeInHierarchy)
                {
                    GameUILayer.s_instance.heroInfoPanel.isCanClose = false;
                }

                GameLayer.s_instance.attackRangeTrans.position = transform.parent.position;
                GameUILayer.s_instance.heroInfoPanel.show(this);
            }
        }

        isTriggerMouseDown = false;
    }

    private void OnMouseExit()
    {
        if (isTriggerMouseDown)
        {
            isTriggerMouseDown = false;
            isDraging = true;

            GameLayer.s_instance.heroGrid.SetActive(true);
        }
    }

    bool isTouchInUI()
    {
        // 检测触摸是否在UI上（Button、Image等）
        {
            // 电脑端用法
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            // 手机端用法
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        if(starTrans)
        {
            Destroy(starTrans.gameObject);
        }
    }
}
