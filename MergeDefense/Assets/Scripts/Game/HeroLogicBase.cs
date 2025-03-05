using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public List<WeaponData> list_weapon = new List<WeaponData>();
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isMerge = false;
    [HideInInspector]
    public Transform heroUITrans;
    [HideInInspector]
    public Transform emojiTrans;
    [HideInInspector]
    public Transform weaponUITrans;
    [HideInInspector]
    public Transform curStandGrid;
    [HideInInspector]
    public bool isCanUpdate = false;
    [HideInInspector]
    public BoxCollider boxCollider;

    bool isDraging = false;

    [HideInInspector]
    public Transform heroQualityTrans;
    Material material_qualityBg;

    [HideInInspector]
    public Action Attack;

    HeroAniEvent heroAniEvent;

    [HideInInspector]
    public HeroStarData heroStarData;

    List<Consts.BuffData> list_buffDatas = new List<Consts.BuffData>();

    public void Start()
    {
        curStandGrid = GameLayer.s_instance.heroGrid.transform.Find(transform.parent.name);

        Transform modelTrans = transform.Find("model");
        heroAniEvent = modelTrans.GetComponent<HeroAniEvent>();
        animator = modelTrans.GetComponent<Animator>();
        boxCollider = transform.GetComponent<BoxCollider>();

        heroData = HeroEntity.getInstance().getData(id);
        heroStarData = HeroStarEntity.getInstance().getData(curStar);

        transform.localScale = Vector3.zero;
        Invoke("summonWaitShow", 0.5f);

        // 品质背景板
        {
            heroQualityTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroQuality"), GameLayer.s_instance.heroQualityPoint).transform;
            heroQualityTrans.position = curStandGrid.position + Consts.heroQualityOffset;
            heroQualityTrans.localScale = Vector3.zero;
            material_qualityBg = heroQualityTrans.GetChild(0).GetComponent<MeshRenderer>().material;
        }

        // 角色模型上的UI
        {
            heroUITrans = Instantiate(GameUILayer.s_instance.prefab_heroUI, GameUILayer.s_instance.heroUIPointTrans).transform;
            heroUITrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position);
            heroUITrans.localScale = Vector3.zero;

            emojiTrans = heroUITrans.Find("emoji");
            weaponUITrans = heroUITrans.Find("weapon");

            setStarUI();
            setWeaponUI();
        }
    }

    void summonWaitShow()
    {
        AudioScript.s_instance.playSound("summonHero" + RandomUtil.getRandom(1, 3));

        if (heroQualityTrans)
        {
            heroQualityTrans.localScale = Vector3.one;
        }

        if (heroUITrans)
        {
            heroUITrans.localScale = Vector3.one;
        }

        transform.localScale = Consts.summonHeroBigScale;
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

        // buff倒计时
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if (list_buffDatas[i].time > 0)
            {
                list_buffDatas[i].time -= Time.deltaTime;
                if (list_buffDatas[i].time <= 0)
                {
                    list_buffDatas.RemoveAt(i);
                    --i;
                }
            }
        }

        if (isDraging)
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
                    GameLayer.s_instance.matrial_attackRange.SetFloat("_OutLineRange", 1.0f / heroData.atkRange);

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
                        heroUITrans.localScale = Vector3.zero;
                        curStandGrid = minDisGrid;
                        transform.SetParent(minDisGridHeroPoint);
                        boxCollider.enabled = false;
                        transform.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(()=>
                        {
                            playAni(Consts.HeroAniNameIdle);
                            boxCollider.enabled = true;
                            isCanUpdate = true;
                            isAttacking = false;
                            heroUITrans.localScale = Vector3.one;
                            heroUITrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position);
                        });

                        heroQualityTrans.DOMove(minDisGrid.position + Consts.heroQualityOffset, moveTime).SetEase(Ease.Linear);
                    }
                    // 已有角色，交换位置
                    else
                    {
                        Transform otherHero = minDisGridHeroPoint.GetChild(0);
                        HeroLogicBase heroLogicBase_other = otherHero.GetComponent<HeroLogicBase>();
                        heroLogicBase_other.heroUITrans.localScale = Vector3.zero;
                        heroLogicBase_other.curStandGrid = curStandGrid;
                        otherHero.SetParent(transform.parent);
                        heroLogicBase_other.boxCollider.enabled = false;
                        otherHero.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            heroLogicBase_other.boxCollider.enabled = true;
                            heroLogicBase_other.heroUITrans.localScale = Vector3.one;
                            heroLogicBase_other.heroUITrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, heroLogicBase_other.curStandGrid.position);
                        });
                        heroLogicBase_other.heroQualityTrans.DOMove(heroLogicBase_other.curStandGrid.position + Consts.heroQualityOffset, moveTime).SetEase(Ease.Linear);
                        
                        heroUITrans.localScale = Vector3.zero;
                        curStandGrid = minDisGrid;
                        transform.SetParent(minDisGridHeroPoint);
                        boxCollider.enabled = false;
                        transform.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            boxCollider.enabled = true;
                            heroUITrans.localScale = Vector3.one;
                            heroUITrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position);
                        });
                        heroQualityTrans.DOMove(curStandGrid.position + Consts.heroQualityOffset, moveTime).SetEase(Ease.Linear);
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

                // 攻击特效
                if (!isDraging)
                {
                    switch (id)
                    {
                        case 101:
                        case 104:
                        case 106:
                        case 108:
                            {
                                EffectManager.heroAttack(this);
                                break;
                            }
                    }
                }

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
                heroUITrans.GetChild(0).Find(i.ToString()).gameObject.SetActive(true);
            }
            else
            {
                heroUITrans.GetChild(0).Find(i.ToString()).gameObject.SetActive(false);
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

    void setWeaponUI()
    {
        Transform weapon1 = weaponUITrans.GetChild(0);
        Transform weapon2 = weaponUITrans.GetChild(1);

        if (list_weapon.Count >= 1)
        {
            weapon1.localScale = Vector3.one;
            weapon1.GetComponent<Image>().color = Consts.list_weaponColor[list_weapon[0].type];
            weapon1.GetChild(0).GetComponent<Text>().text = list_weapon[0].level.ToString();
        }
        else
        {
            weapon1.localScale = Vector3.zero;
        }

        if (list_weapon.Count >= 2)
        {
            weapon2.localScale = Vector3.one;
            weapon2.GetComponent<Image>().color = Consts.list_weaponColor[list_weapon[1].type];
            weapon2.GetChild(0).GetComponent<Text>().text = list_weapon[1].level.ToString();
        }
        else
        {
            weapon2.localScale = Vector3.zero;
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

        // buff加成
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if(list_buffDatas[i].buffType == Consts.BuffType.AtkBaiFenBi)
            {
                atkXiShu += list_buffDatas[i].value;
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

    public void addBuff(Consts.BuffData buffData)
    {
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            // 如果已存在该buff,则重置时间
            if (list_buffDatas[i].buffType == buffData.buffType && list_buffDatas[i].from == buffData.from)
            {
                list_buffDatas[i].time = buffData.time;
                return;
            }
        }

        list_buffDatas.Add(buffData);
    }

    public bool isCanAddWeapon(WeaponData weaponData)
    {
        if (list_weapon.Count == 0)
        {
            return true;
        }
        else if (list_weapon.Count == 1)
        {
            if (list_weapon[0].type == weaponData.type && list_weapon[0].level > weaponData.level)
            {
                return false;
            }
            else if(list_weapon[0].level >= 10)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (list_weapon.Count == 2)
        {
            for (int i = 0; i < list_weapon.Count; i++)
            {
                if (list_weapon[i].level <= 9 && list_weapon[i].type == weaponData.type && list_weapon[i].level <= weaponData.level)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    public void addWeapon(WeaponData weaponData)
    {
        if (list_weapon.Count == 0)
        {
            list_weapon.Add(weaponData);
        }
        else if (list_weapon.Count == 1)
        {
            if (list_weapon[0].type == weaponData.type)
            {
                if(list_weapon[0].level == weaponData.level)
                {
                    list_weapon[0] = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
                }
                else
                {
                    list_weapon[0] = weaponData;
                }
            }
            else
            {
                list_weapon.Add(weaponData);
            }
        }
        else if (list_weapon.Count == 2)
        {
            for (int i = 0; i < list_weapon.Count; i++)
            {
                if (list_weapon[i].type == weaponData.type)
                {
                    if (list_weapon[i].level == weaponData.level)
                    {
                        list_weapon[i] = WeaponEntity.getInstance().getData(weaponData.type, weaponData.level + 1);
                    }
                    else
                    {
                        list_weapon[i] = weaponData;
                    }
                    break;
                }
            }
        }

        setWeaponUI();
        AudioScript.s_instance.playSound("equipWeapon");
    }

    public void addStar()
    {
        ++curStar;
        heroStarData = HeroStarEntity.getInstance().getData(curStar);
        setStarUI();
    }

    public void mergeWeapon(List<WeaponData> weaponDatas)
    {
        //for(int i = 0; i < list_weapon.Count; i++)
        //{
        //    if(heroData.goodWeapon != -1 && (heroData.goodWeapon != list_weapon[i].type))
        //    {
        //        list_weapon.RemoveAt(i);
        //        --i;
        //    }
        //}
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

            if(HeroInfoPanel.s_instance.gameObject.activeInHierarchy)
            {
                HeroInfoPanel.s_instance.btn_sellHeroTrans.localScale = Vector3.zero;
            }
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
    public void showWeaponEmoji(WeaponData weaponData)
    {
        if(!isCanAddWeapon(weaponData))
        {
            return;
        }

        bool isShowTween = false;

        // 优势武器
        if(heroData.goodWeapon == -1 || weaponData.type == heroData.goodWeapon)
        {
            isShowTween = true;
            emojiTrans.localScale = Vector3.one;
            emojiTrans.GetChild(0).localScale = Vector3.one;
            emojiTrans.GetChild(1).localScale = Vector3.zero;
        }
        // 劣势武器
        else if (weaponData.type == heroData.badWeapon)
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
        if(heroUITrans)
        {
            Destroy(heroUITrans.gameObject);
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
