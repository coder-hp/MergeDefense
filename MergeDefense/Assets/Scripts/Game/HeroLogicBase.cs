using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogicBase : MonoBehaviour
{
    public int id;

    [HideInInspector]
    public HeroData heroData;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Transform centerPoint;

    List<WeaponData> list_weapon = new List<WeaponData>();

    float atkRange = 0f;
    [HideInInspector]
    public bool isAttacking = false;

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
}
