using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroLogicBase : MonoBehaviour
{
    public int id;

    [HideInInspector]
    public HeroData heroData;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Transform centerPoint;
    [HideInInspector]
    public List<WeaponData> list_weapon = new List<WeaponData>();

    float atkRange = 0f;
    [HideInInspector]
    public bool isAttacking = false;

    bool isDraging = false;

    public void Awake()
    {
        animator = transform.Find("model").GetComponent<Animator>();
        centerPoint = transform.Find("centerPoint");

        heroData = HeroEntity.getInstance().getData(id);

        // 近战
        if (heroData.isJinZhan == 1)
        {
            atkRange = 1.9f;
        }
        // 远程
        else
        {
            atkRange = 10;
        }
    }

    private void Update()
    {
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
                }
                else
                {
                    GameLayer.s_instance.heroGridChoiced.localScale = Vector3.zero;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDraging = false;

                GameLayer.s_instance.heroGrid.SetActive(false);
                GameLayer.s_instance.heroGridChoiced.localScale = Vector3.zero;

                if (minDis <= 1.2f)
                {
                    transform.SetParent(GameLayer.s_instance.heroPoint.Find(minDisGrid.name));
                    transform.DOLocalMove(Vector3.zero,1);
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
            if (enemyLogic && Vector3.Distance(transform.position, enemyLogic.transform.position) <= atkRange)
            {
                lookEnemy(enemyLogic);
                isAttacking = true;
                playAni("attack");

                // 单体攻击
                if (heroData.isAtkSingle == 1)
                {
                    enemyLogic.damage(getAtk());
                }
                // 群体攻击
                else
                {

                }

                return true;
            }
        }

        return false;
    }

    void lookEnemy(EnemyLogic enemyLogic)
    {
        float angle = -CommonUtil.twoPointAngle(centerPoint.position, enemyLogic.centerPoint.position);
        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    int getAtk()
    {
        return heroData.atk;
    }

    public void addWeapon(WeaponData weaponData)
    {
        ToastScript.show("增加武器:" + weaponData.name + " level" + weaponData.level);
        Debug.Log(transform.name + "增加武器:" + weaponData.name + " level" + weaponData.level);
        list_weapon.Add(weaponData);
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
}
