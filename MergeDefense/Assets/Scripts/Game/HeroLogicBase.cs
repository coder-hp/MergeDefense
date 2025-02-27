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
    [HideInInspector]
    public BoxCollider boxCollider;

    bool isDraging = false;

    [HideInInspector]
    public Transform heroQualityTrans;
    Vector3 heroQualityOffset = new Vector3(0, 0f, -0.01f);
    Material material_qualityBg;

    [HideInInspector]
    public Action Attack;

    HeroAniEvent heroAniEvent;
    Vector3 emojiOffset = new Vector3(-0.2f, 0.5f, 0);

    [HideInInspector]
    public HeroStarData heroStarData;

    public void Awake()
    {
        curStandGrid = GameLayer.s_instance.heroGrid.transform.Find(transform.parent.name);

        Transform modelTrans = transform.Find("model");
        heroAniEvent = modelTrans.GetComponent<HeroAniEvent>();
        animator = modelTrans.GetComponent<Animator>();
        boxCollider = transform.GetComponent<BoxCollider>();

        centerPoint = transform.Find("centerPoint");

        heroData = HeroEntity.getInstance().getData(id);
        heroStarData = HeroStarEntity.getInstance().getData(curStar);

        transform.localScale = Vector3.zero;
        Invoke("summonWaitShow", 0.5f);

        // 品质背景板
        {
            heroQualityTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroQuality"), GameLayer.s_instance.heroQualityPoint).transform;
            heroQualityTrans.position = curStandGrid.position + heroQualityOffset;
            heroQualityTrans.localScale = Vector3.zero;
            material_qualityBg = heroQualityTrans.GetChild(0).GetComponent<MeshRenderer>().material;
        }

        // 星星预设
        {
            starTrans = Instantiate(GameUILayer.s_instance.prefab_heroStar, GameUILayer.s_instance.heroStarPointTrans).transform;
            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
            starTrans.localScale = Vector3.zero;
            setStarUI();
        }

        // emoji
        {
            emojiTrans = Instantiate(GameUILayer.s_instance.prefab_heroEmoji, GameUILayer.s_instance.heroStarPointTrans).transform;
            emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position + new Vector3(-0.2f,0.7f,0));
            emojiTrans.localScale = Vector3.zero;
        }
    }

    void summonWaitShow()
    {
        if (heroQualityTrans)
        {
            heroQualityTrans.localScale = Vector3.one;
        }
        if (starTrans)
        {
            starTrans.localScale = Vector3.one;
        }

        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        transform.DOScale(0.7f, 0.2f).OnComplete(() =>
        {
            transform.DOScale(1f, 0.1f).OnComplete(() =>
            {
                isCanUpdate = true;
            });
        });
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

                    float moveTime = Vector3.Distance(minDisGridHeroPoint.position,curStandGrid.position) * 0.2f;

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
                        boxCollider.enabled = false;
                        transform.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(()=>
                        {
                            playAni(Consts.HeroAniNameIdle);
                            boxCollider.enabled = true;
                            isCanUpdate = true;
                            isAttacking = false;
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });

                        heroQualityTrans.DOMove(minDisGrid.position + heroQualityOffset, moveTime).SetEase(Ease.Linear);
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
                        heroLogicBase_other.boxCollider.enabled = false;
                        otherHero.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            heroLogicBase_other.boxCollider.enabled = true;
                            heroLogicBase_other.starTrans.localScale = Vector3.one;
                            heroLogicBase_other.starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, otherHero.position);
                        });
                        heroLogicBase_other.heroQualityTrans.DOMove(heroLogicBase_other.curStandGrid.position + heroQualityOffset, moveTime).SetEase(Ease.Linear);
                        heroLogicBase_other.emojiTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, heroLogicBase_other.curStandGrid.position + emojiOffset);

                        starTrans.localScale = Vector3.zero;
                        curStandGrid = minDisGrid;
                        transform.SetParent(minDisGridHeroPoint);
                        boxCollider.enabled = false;
                        transform.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            boxCollider.enabled = true;
                            starTrans.localScale = Vector3.one;
                            starTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position);
                        });
                        heroQualityTrans.DOMove(curStandGrid.position + heroQualityOffset, moveTime).SetEase(Ease.Linear);
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
            material_qualityBg.SetFloat("_ColorID", 4.0f);
        }
        // 紫色
        else if (curStar > 6)
        {
            material_qualityBg.SetFloat("_ColorID", 3.0f);
        }
        // 蓝色
        else if (curStar > 3)
        {
            material_qualityBg.SetFloat("_ColorID", 2.0f);
        }
        // 白色
        else if (curStar > 0)
        {
            material_qualityBg.SetFloat("_ColorID", 1.0f);
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

    public int getAddSkillRate()
    {
        int skillRate = 0;

        // 武器加成
        for (int i = 0; i < list_weapon.Count; i++)
        {
            // Buff3激活
            if ((heroData.badWeapon != -1) && (heroData.badWeapon != list_weapon[i].type))
            {
                if (list_weapon[i].buff3Type == Consts.BuffType.SkillRate)
                {
                    skillRate += Mathf.RoundToInt(float.Parse(list_weapon[i].buff3ValueStr));
                }
            }
        }

        return skillRate;
    }

    public bool isCanAddWeapon(int type)
    {
        if(list_weapon.Count >= 2)
        {
            return false;
        }

        //for (int i = 0; i < list_weapon.Count; i++)
        //{
        //    if (list_weapon[i].type == type)
        //    {
        //        return false;
        //    }
        //}

        return true;
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

        if (aniName == Consts.HeroAniNameIdle)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (crossFadeTime > 0)
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
        if (!HeroInfoPanel.s_instance.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }
        GameLayer.s_instance.attackRangeTrans.GetComponent<MeshRenderer>().material.SetFloat("_OutLineRange", 1.0f / heroData.atkRange);
        isTriggerMouseDown = true;
    }

    private void OnMouseUp()
    {
        if (!HeroInfoPanel.s_instance.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }

        if (isTriggerMouseDown)
        {
            if(HeroInfoPanel.s_instance.gameObject.activeInHierarchy && HeroInfoPanel.s_instance.heroLogicBase == this)
            {
                // 如果当前角色信息面板显示的是当前点击的角色，则关闭信息面板
                // 触发的是UI那边的HeroInfoPanel.onClickClose
            }
            else
            {
                if (HeroInfoPanel.s_instance.gameObject.activeInHierarchy)
                {
                    HeroInfoPanel.s_instance.isCanClose = false;
                }

                GameLayer.s_instance.attackRangeTrans.position = curStandGrid.position;
                HeroInfoPanel.s_instance.show(this);
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

        if(heroQualityTrans)
        {
            Destroy(heroQualityTrans.gameObject);
        }

        if(tween_emoji != null)
        {
            tween_emoji.Kill();
        }
    }
}
