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

    [HideInInspector]
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
    [HideInInspector]
    public Transform emojiTrans;
    [HideInInspector]
    public Transform curStandGrid;
    [HideInInspector]
    public bool isCanUpdate = false;

    bool isDraging = false;

    [HideInInspector]
    public Transform heroQualityTrans;
    Vector3 heroQualityOffset = new Vector3(0, 0f, -0.01f);
    Material material_qualityBg;

    [HideInInspector]
    public Action Attack;

    HeroAniEvent heroAniEvent;
    Vector3 emojiOffset = new Vector3(-0.2f, 0.5f, 0);

    HeroStarData heroStarData;

    public void Awake()
    {
        curStandGrid = GameLayer.s_instance.heroGrid.transform.Find(transform.parent.name);
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
        heroStarData = HeroStarEntity.getInstance().getData(curStar);

        // 品质背景板
        {
            heroQualityTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroQuality"), GameLayer.s_instance.heroQualityPoint).transform;
            heroQualityTrans.position = curStandGrid.position + heroQualityOffset;
            material_qualityBg = heroQualityTrans.GetComponent<MeshRenderer>().material;
        }

        // 星星预设
        {
            starTrans = Instantiate(GameUILayer.s_instance.prefab_heroStar, GameUILayer.s_instance.heroStarPointTrans).transform;
            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
            setStarUI();
        }

        // emoji
        {
            emojiTrans = Instantiate(GameUILayer.s_instance.prefab_heroEmoji, GameUILayer.s_instance.heroStarPointTrans).transform;
            emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position + new Vector3(-0.2f,0.7f,0));
            emojiTrans.localScale = Vector3.zero;
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
                if(minDis <= 1.4f)
                {
                    GameLayer.s_instance.heroGridChoiced.localScale = Vector3.one;
                    GameLayer.s_instance.heroGridChoiced.transform.position = minDisGrid.position;

                    GameLayer.s_instance.attackRangeTrans.position = minDisGrid.position;
                    GameLayer.s_instance.attackRangeTrans.localScale = new Vector3(heroData.atkRange, heroData.atkRange, heroData.atkRange);

                    HeroMoveLine.s_instance.setPos(curStandGrid.position, minDisGrid.position);
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

                if (minDis <= 1.4f)
                {
                    Transform minDisGridHeroPoint = GameLayer.s_instance.heroPoint.Find(minDisGrid.name);

                    // 目标格子没有角色
                    if (minDisGridHeroPoint.childCount == 0)
                    {
                        isCanUpdate = false;
                        playAni(Consts.HeroAniNameRun);

                        float angle = -CommonUtil.twoPointAngle(curStandGrid.position, minDisGrid.position);
                        transform.localRotation = Quaternion.Euler(0, angle, 0);
                        starTrans.localScale = Vector3.zero;
                        curStandGrid = minDisGrid;
                        transform.SetParent(minDisGridHeroPoint);
                        transform.DOLocalMove(Vector3.zero, 1).OnComplete(()=>
                        {
                            playAni(Consts.HeroAniNameIdle);
                            isCanUpdate = true;
                            isAttacking = false;
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });

                        heroQualityTrans.DOMove(minDisGrid.position + heroQualityOffset, 1);
                        emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position + emojiOffset);
                    }
                    // 已有角色，交换位置
                    else
                    {
                        Transform otherHero = minDisGridHeroPoint.GetChild(0);
                        HeroLogicBase heroLogicBase_other = otherHero.GetComponent<HeroLogicBase>();
                        heroLogicBase_other.starTrans.localScale = Vector3.zero;
                        heroLogicBase_other.curStandGrid = curStandGrid;
                        otherHero.SetParent(transform.parent);
                        otherHero.DOLocalMove(Vector3.zero, 1).OnComplete(() =>
                        {
                            heroLogicBase_other.starTrans.localScale = Vector3.one;
                            heroLogicBase_other.starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, otherHero.position);
                        });
                        heroLogicBase_other.heroQualityTrans.DOMove(heroLogicBase_other.curStandGrid.position + heroQualityOffset, 1);
                        heroLogicBase_other.emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, heroLogicBase_other.curStandGrid.position + emojiOffset);

                        starTrans.localScale = Vector3.zero;
                        curStandGrid = minDisGrid;
                        transform.SetParent(minDisGridHeroPoint);
                        transform.DOLocalMove(Vector3.zero, 1).OnComplete(() =>
                        {
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });
                        heroQualityTrans.DOMove(curStandGrid.position + heroQualityOffset, 1);
                        emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position + emojiOffset);
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
            EnemyLogic enemyLogic = EnemyManager.s_instance.getMinDisTarget(curStandGrid);
            if (enemyLogic && Vector3.Distance(curStandGrid.position, enemyLogic.transform.position) <= heroData.atkRange)
            {
                heroAniEvent.enemyLogic = enemyLogic;
                lookEnemy(enemyLogic);
                isAttacking = true;
                playAni(Consts.HeroAniNameAttack);

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
            material_qualityBg.SetColor("_Color",new Color(1,0.6f,0.15f));
        }
        // 紫色
        else if (curStar > 6)
        {
            material_qualityBg.SetColor("_Color", new Color(0.75f,0.25f,1));
        }
        // 蓝色
        else if (curStar > 3)
        {
            material_qualityBg.SetColor("_Color", new Color(0.25f,0.5f,1));
        }
        // 白色
        else if (curStar > 0)
        {
            material_qualityBg.SetColor("_Color", new Color(0.8f,0.75f,0.75f));
        }
    }

    void lookEnemy(EnemyLogic enemyLogic)
    {
        float angle = -CommonUtil.twoPointAngle(curStandGrid.position, enemyLogic.transform.position);
        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    public int getAtk()
    {
        int atk = heroData.atk;
        float atkXiShu = heroStarData.baseAtkXiShu;

        // 武器加成
        for (int i = 0; i < list_weapon.Count; i++)
        {
            // 擅长
            if ((heroData.goodWeapon == -1) || (heroData.goodWeapon == list_weapon[i].type))
            {
                atk += Mathf.RoundToInt(list_weapon[i].buff1 * 1.2f);
                atkXiShu += list_weapon[i].buff2 * 1.2f;
            }
            // 不擅长
            else if ((heroData.badWeapon == -1) || (heroData.badWeapon == list_weapon[i].type))
            {
                atk += Mathf.RoundToInt(list_weapon[i].buff1 * 0.8f);
                atkXiShu += list_weapon[i].buff2 * 0.8f;
            }
            else
            {
                atk += list_weapon[i].buff1;
                atkXiShu += list_weapon[i].buff2;
            }
        }

        return Mathf.RoundToInt(atk * atkXiShu);
    }

    public float getAtkSpeed()
    {
        float atkSpeed = heroData.atkSpeed;

        // 武器加成
        for (int i = 0; i < list_weapon.Count; i++)
        {
            // Buff3激活
            if ((heroData.badWeapon != -1) && (heroData.badWeapon != list_weapon[i].type))
            {
                if(list_weapon[i].buff3Type == Consts.BuffType.AtkSpeed)
                {
                    atkSpeed += float.Parse(list_weapon[i].buff3ValueStr);
                }
            }
        }

        return atkSpeed;
    }

    public int getCritRate()
    {
        int critRate = heroData.critRate;

        // 武器加成
        for (int i = 0; i < list_weapon.Count; i++)
        {
            // Buff3激活
            if ((heroData.badWeapon != -1) && (heroData.badWeapon != list_weapon[i].type))
            {
                if (list_weapon[i].buff3Type == Consts.BuffType.CritRate)
                {
                    critRate += Mathf.RoundToInt(float.Parse(list_weapon[i].buff3ValueStr));
                }
            }
        }

        return critRate;
    }

    public float getCritDamageXiShu()
    {
        float critDamage = heroData.critDamage;

        // 武器加成
        for (int i = 0; i < list_weapon.Count; i++)
        {
            // Buff3激活
            if ((heroData.badWeapon != -1) && (heroData.badWeapon != list_weapon[i].type))
            {
                if (list_weapon[i].buff3Type == Consts.BuffType.CritDamage)
                {
                    critDamage += float.Parse(list_weapon[i].buff3ValueStr);
                }
            }
        }

        return critDamage;
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
        heroStarData = HeroStarEntity.getInstance().getData(curStar);
        setStarUI();
    }

    public void playAni(string aniName,float crossFadeTime = 0)
    {
        if(aniName == Consts.HeroAniNameAttack)
        {
            animator.speed = getAtkSpeed();
        }
        else
        {
            animator.speed = 1;
        }

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

                GameLayer.s_instance.attackRangeTrans.position = curStandGrid.position;
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

    DG.Tweening.Sequence tween_emoji = null;
    public void showWeaponEmoji(int type)
    {
        bool isShowTween = false;

        // 优势武器
        if(heroData.goodWeapon == -1 || type == heroData.goodWeapon)
        {
            isShowTween = true;
            emojiTrans.localScale = Vector3.one;
            emojiTrans.GetChild(0).localScale = Vector3.one;
            emojiTrans.GetChild(1).localScale = Vector3.zero;
        }
        // 劣势武器
        else if (type == heroData.badWeapon)
        {
            isShowTween = true;
            emojiTrans.localScale = Vector3.one;
            emojiTrans.GetChild(0).localScale = Vector3.zero;
            emojiTrans.GetChild(1).localScale = Vector3.one;
        }
        else
        {
            emojiTrans.localScale = Vector3.zero;
        }

        if(isShowTween)
        {
            if(tween_emoji == null)
            {
                tween_emoji = DOTween.Sequence();
                tween_emoji.Append(emojiTrans.DOLocalRotateQuaternion(Quaternion.Euler(0,0,5),0.5f))
                           .Append(emojiTrans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -5), 0.5f)).SetLoops(-1);
                tween_emoji.SetAutoKill(false);
            }
            else
            {
                tween_emoji.Restart();
            }
        }
    }

    public void hideWeaponEmoji()
    {
        emojiTrans.localScale = Vector3.zero;

        if(tween_emoji != null)
        {
            tween_emoji.Pause();
        }
    }

    private void OnDestroy()
    {
        if(starTrans)
        {
            Destroy(starTrans.gameObject);
        }

        if(emojiTrans)
        {
            Destroy(emojiTrans.gameObject);
        }

        if(tween_emoji != null)
        {
            tween_emoji.Kill();
        }
    }
}
