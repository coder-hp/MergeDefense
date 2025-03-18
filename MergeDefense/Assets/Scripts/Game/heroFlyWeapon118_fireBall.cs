using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon118_fireBall : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic)
    {
        heroLogicBase = _heroLogicBase;
        enemyLogic = _enemyLogic;

        if (enemyLogic)
        {
            targetTrans = enemyLogic.transform;
        }

        transform.position = heroLogicBase.flyWeaponPoint.position;

        if(enemyLogic == null)
        {
            transform.rotation = Quaternion.Euler(0, 0, RandomUtil.getRandom(1,360));
        }
    }

    bool isReadyDestroy = false;
    void Update()
    {
        if (enemyLogic)
        {
            float angle = CommonUtil.twoPointAngle(transform.position, targetTrans.position);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Vector3.MoveTowards(transform.position, targetTrans.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTrans.position) <= 0.1f)
            {
                if (heroLogicBase)
                {
                    atkEnemy(enemyLogic);
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if(!isReadyDestroy)
            {
                Destroy(gameObject, 5);
                transform.GetComponent<BoxCollider>().enabled = true;
            }

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime,Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyLogic _enemyLogic = other.transform.GetComponent<EnemyLogic>();
            atkEnemy(_enemyLogic);
            Destroy(gameObject);
        }
    }

    // 技能4：每攻击20次，喷射三个火球，每个火球对范围内的敌人造成攻击力2500%的伤害
    void atkEnemy(EnemyLogic _enemyLogic)
    {
        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            int atk = heroLogicBase.getAtk();
            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
            {
                EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
                {
                    --i;
                }
            }
        }
    }
}
