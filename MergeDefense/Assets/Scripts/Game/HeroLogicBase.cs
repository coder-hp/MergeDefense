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
    public bool isAttacking = false;
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
    [HideInInspector]
    public Transform flyWeaponPoint;
    [HideInInspector]
    public Transform modelTrans;
    [HideInInspector]
    public Transform rootTrans;

    bool isDraging = false;

    [HideInInspector]
    public Transform heroQualityTrans;
    Material material_qualityBg;

    [HideInInspector]
    public Action Attack;

    HeroAniEvent heroAniEvent;

    [HideInInspector]
    public HeroStarData heroStarData;
    [HideInInspector]
    public List<Consts.BuffData> list_buffDatas = new List<Consts.BuffData>();

    GameObject obj_attackEffect = null;

    Vector3 vec_atkRange = Vector3.one;

    private void Awake()
    {
        HeroManager.s_instance.addHero(this);
    }

    public void Start()
    {
        curStandGrid = GameLayer.s_instance.heroGrid.transform.Find(transform.parent.name);

        rootTrans = transform.Find("root");
        modelTrans = transform.Find("root/model");
        heroAniEvent = modelTrans.GetComponent<HeroAniEvent>();
        animator = modelTrans.GetComponent<Animator>();
        boxCollider = transform.GetComponent<BoxCollider>();
        flyWeaponPoint = transform.Find("flyWeaponPoint");

        heroData = HeroEntity.getInstance().getData(id);
        heroStarData = HeroStarEntity.getInstance().getData(curStar);

        vec_atkRange = new Vector3(heroData.atkRange, heroData.atkRange, heroData.atkRange);

        transform.localScale = Vector3.zero;
        Invoke("summonWaitShow", 0.5f);

        // 品质背景板
        {
            heroQualityTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroQuality"), transform).transform;
            heroQualityTrans.position = transform.position + Consts.heroQualityOffset;
            heroQualityTrans.rotation = Quaternion.Euler(0,0,0);
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
                checkMerge();
            });
        });
    }

    public void checkMerge()
    {
        isCanUpdate = false;

        // 检测是否可以合并
        for (int i = 0; i < HeroManager.s_instance.list_hero.Count; i++)
        {
            if (HeroManager.s_instance.list_hero[i] != this)
            {
                HeroLogicBase heroLogicBase_to = HeroManager.s_instance.list_hero[i];
                if ((heroLogicBase_to.isCanUpdate) && (heroLogicBase_to.curStar < heroLogicBase_to.heroData.maxStar) && (heroLogicBase_to.heroData.id == heroData.id) && (heroLogicBase_to.curStar == curStar))
                {
                    AudioScript.s_instance.playSound("heroMerge");

                    heroLogicBase_to.isCanUpdate = false;
                    playAni(Consts.HeroAniNameIdle);
                    boxCollider.enabled = false;
                    transform.SetParent(transform);
                    Destroy(heroUITrans.gameObject);

                    float moveTime = Vector3.Distance(transform.position, heroLogicBase_to.transform.position) / 10f;
                    if (moveTime > 0.3f)
                    {
                        moveTime = 0.3f;
                    }

                    float jumpHight = Vector3.Distance(heroLogicBase_to.transform.position, transform.position) / 5f;
                    if (jumpHight < 0.5f)
                    {
                        jumpHight = 0.5f;
                    }
                    else if (jumpHight > 1)
                    {
                        jumpHight = 1f;
                    }

                    transform.DOMove(heroLogicBase_to.transform.position, moveTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        heroLogicBase_to.addStar();
                        EffectManager.s_instance.heroMerge(heroLogicBase_to.transform.position);

                        // 升星角色的合并动画
                        {
                            Transform trans = heroLogicBase_to.rootTrans;
                            trans.DOLocalMoveY(0.3f, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
                            {
                                trans.DOLocalMoveY(0f, 0.1f).SetEase(Ease.InCubic);
                            });

                            trans.DOScaleY(1.2f, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
                            {
                                trans.DOScaleY(0.8f, 0.1f).SetEase(Ease.InCubic).OnComplete(() =>
                                {
                                    trans.DOScaleY(1f, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
                                    {
                                        heroLogicBase_to.checkMerge();
                                    });
                                });
                            });
                        }

                        Destroy(gameObject);
                        //Invoke("checkHeroMerge", 0.65f);
                    });

                    transform.GetChild(0).DOLocalMoveY(jumpHight, moveTime * 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        transform.GetChild(0).DOLocalMoveY(0, moveTime * 0.5f).SetEase(Ease.InSine);
                    });

                    return;
                }
            }
        }

        isCanUpdate = true;
        GameUILayer.s_instance.checkMythicHeroProgress();
    }

    private void Update()
    {
        if(!isCanUpdate)
        {
            return;
        }

        if(GameFightData.s_instance.isGameOver)
        {
            return;
        }

        // buff倒计时
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if (!list_buffDatas[i].isForever && list_buffDatas[i].time > 0)
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
                    GameLayer.s_instance.attackRangeTrans.localScale = vec_atkRange;
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

                    // 目标格子没有角色
                    if (minDisGridHeroPoint.childCount == 0)
                    {
                        changeParent(minDisGridHeroPoint, minDisGrid);
                    }
                    // 已有角色，交换位置
                    else
                    {
                        Transform otherHero = minDisGridHeroPoint.GetChild(0);
                        HeroLogicBase heroLogicBase_other = otherHero.GetComponent<HeroLogicBase>();

                        Transform otherHeroParent = otherHero.parent;
                        Transform otherHeroCurStandGrid = heroLogicBase_other.curStandGrid;

                        heroLogicBase_other.changeParent(transform.parent,curStandGrid);
                        changeParent(otherHeroParent, otherHeroCurStandGrid);
                    }
                }
            }
        }

        checkAttack();
    }

    Tween tween_heroMove = null;
    public void changeParent(Transform newParentTrans,Transform gridTrans)
    {
        float moveTime = Vector3.Distance(newParentTrans.position, curStandGrid.position) * 0.2f;

        isCanUpdate = false;
        playAni(Consts.HeroAniNameRun);

        float angle = -CommonUtil.twoPointAngle(curStandGrid.position, gridTrans.position);
        setAngle(new Vector3(0,angle,0));
        heroUITrans.localScale = Vector3.zero;
        curStandGrid = gridTrans;
        transform.SetParent(newParentTrans);
        boxCollider.enabled = false;

        if (tween_heroMove != null)
        {
            tween_heroMove.Kill();
        }
        tween_heroMove = transform.DOLocalMove(Vector3.zero, moveTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            playAni(Consts.HeroAniNameIdle);
            boxCollider.enabled = true;
            isCanUpdate = true;
            isAttacking = false;
            heroUITrans.localScale = Vector3.one;
            heroUITrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, curStandGrid.position);
            checkMerge();
        });
    }

    public bool checkAttack()
    {
        if (!isAttacking)
        {
            EnemyLogic enemyLogic = EnemyManager.s_instance.getHeroAtkTarget(this);
            if(enemyLogic)
            {
                heroAniEvent.enemyLogic = enemyLogic;
                float angleY = lookEnemy(enemyLogic);
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
                        case 112:
                            {
                                if(obj_attackEffect)
                                {
                                    obj_attackEffect.SetActive(false);
                                    obj_attackEffect.SetActive(true);
                                }
                                else
                                {
                                    obj_attackEffect = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/eff_attack_hero" + id), transform);
                                }
                                obj_attackEffect.transform.rotation = Quaternion.Euler(0,0, 360-angleY);
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
        
    }

    float lookEnemy(EnemyLogic enemyLogic)
    {
        float angle = -CommonUtil.twoPointAngle(curStandGrid.position, enemyLogic.transform.position);
        setAngle(new Vector3(0, angle, 0));
        return angle;
    }

    public int getAtk()
    {
        int atk = heroData.atk;
        float atkXiShu = heroStarData.baseAtkXiShu;

        // 武器加成
        for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
            {
                // 擅长
                if (heroData.goodWeapon == GameUILayer.s_instance.list_weaponBar[i].weaponData.type)
                {
                    atk += Mathf.RoundToInt(GameUILayer.s_instance.list_weaponBar[i].weaponData.buff1);
                    atkXiShu += GameUILayer.s_instance.list_weaponBar[i].weaponData.buff2;
                }
            }
        }

        // buff加成
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if(list_buffDatas[i].buffType == Consts.BuffType.AtkBaiFenBi)
            {
                atkXiShu += list_buffDatas[i].value;
            }
            else if (list_buffDatas[i].buffType == Consts.BuffType.Atk)
            {
                atk += (int)list_buffDatas[i].value;
            }
        }

        return Mathf.RoundToInt(atk * atkXiShu);
    }

    public float getAtkSpeed()
    {
        float atkSpeed = heroData.atkSpeed;

        // 武器加成
        for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
            {
                // 擅长、Buff3激活
                if (heroData.goodWeapon == GameUILayer.s_instance.list_weaponBar[i].weaponData.type)
                {
                    if (GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Type == Consts.BuffType.AtkSpeed)
                    {
                        atkSpeed += GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Value;
                    }
                }
            }
        }

        // 普通buff攻速
        for (int i = 0; i < list_buffDatas.Count; i++)
        {
            if (list_buffDatas[i].buffType == Consts.BuffType.AtkSpeed)
            {
                atkSpeed += list_buffDatas[i].value;
            }
        }

        // 全局buff加攻速
        for (int i = 0; i < GameFightData.s_instance.list_globalHeroBuff.Count; i++)
        {
            if (GameFightData.s_instance.list_globalHeroBuff[i].buffType == Consts.BuffType.AtkSpeed)
            {
                atkSpeed += GameFightData.s_instance.list_globalHeroBuff[i].value;
            }
        }

        return atkSpeed;
    }

    public int getCritRate()
    {
        int critRate = heroData.critRate;

        // 武器加成
        for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
            {
                // 擅长、Buff3激活
                if (heroData.goodWeapon == GameUILayer.s_instance.list_weaponBar[i].weaponData.type)
                {
                    if (GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Type == Consts.BuffType.CritRate)
                    {
                        critRate += Mathf.RoundToInt(GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Value);
                    }
                }
            }
        }

        return critRate;
    }

    public float getCritDamageXiShu()
    {
        float critDamage = heroData.critDamage;

        // 武器加成
        for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
            {
                // 擅长、Buff3激活
                if (heroData.goodWeapon == GameUILayer.s_instance.list_weaponBar[i].weaponData.type)
                {
                    if (GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Type == Consts.BuffType.CritDamage)
                    {
                        critDamage += Mathf.RoundToInt(GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Value);
                    }
                }
            }
        }

        return critDamage;
    }

    public int getAddSkillRate()
    {
        int skillRate = 0;

        // 武器加成
        for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
            {
                // 擅长、Buff3激活
                if (heroData.goodWeapon == GameUILayer.s_instance.list_weaponBar[i].weaponData.type)
                {
                    if (GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Type == Consts.BuffType.SkillRate)
                    {
                        skillRate += Mathf.RoundToInt(GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Value);
                    }
                }
            }
        }

        return skillRate;
    }

    public void addBuff(Consts.BuffData buffData)
    {
        if (buffData.isCanRepeatFrom)
        {
            list_buffDatas.Add(buffData);
        }
        else
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
            setAngle(Consts.heroIdleAngle);
        }
        else if (aniName == Consts.HeroAniNameRun)
        {
            if (tween_rotate != null)
            {
                tween_rotate.Kill();
            }
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
        if (GameFightData.s_instance.isGameOver)
        {
            return;
        }

        if (!HeroInfoPanel.s_instance.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }

        if(BtnSellHero.s_instance && BtnSellHero.s_instance.isClicked)
        {
            return;
        }

        isTriggerMouseDown = true;
    }

    private void OnMouseUp()
    {
        if (GameFightData.s_instance.isGameOver)
        {
            return;
        }

        if (!HeroInfoPanel.s_instance.gameObject.activeInHierarchy && isTouchInUI())
        {
            return;
        }

        if (BtnSellHero.s_instance && BtnSellHero.s_instance.isClicked)
        {
            return;
        }

        if (isTriggerMouseDown)
        {
            if (HeroInfoPanel.s_instance.gameObject.activeInHierarchy && HeroInfoPanel.s_instance.heroLogicBase == this)
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
        if (GameFightData.s_instance.isGameOver)
        {
            return;
        }

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
        bool isShowTween = false;

        // 优势武器
        if(weaponData.type == heroData.goodWeapon)
        {
            isShowTween = true;
            emojiTrans.localScale = Vector3.one;
            emojiTrans.GetChild(0).localScale = Vector3.one;
            emojiTrans.GetChild(1).localScale = Vector3.zero;
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

    Tween tween_rotate = null;
    public void setAngle(Vector3 angle)
    {
        if (tween_rotate != null)
        {
            tween_rotate.Kill();
        }
        //tween_rotate = transform.DOLocalRotate(angle, 0.2f, DG.Tweening.RotateMode.Fast);
        tween_rotate = modelTrans.DOLocalRotate(angle, 0.2f, DG.Tweening.RotateMode.Fast);
    }

    private void OnDestroy()
    {
        HeroManager.s_instance.removeHero(this);

        if (heroUITrans)
        {
            Destroy(heroUITrans.gameObject);
        }

        if(tween_emoji != null)
        {
            tween_emoji.Kill();
        }
    }
}
