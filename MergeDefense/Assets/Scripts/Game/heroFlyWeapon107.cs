using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon107 : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic)
    {
        heroLogicBase = _heroLogicBase;
        enemyLogic = _enemyLogic;
        targetTrans = enemyLogic.transform;
    }

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
                    bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                    int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

                    EffectManager.enemyDamage(enemyLogic.transform.position, heroLogicBase.id);

                    if (!enemyLogic.damage(atk, isCrit))
                    {
                        // 如果没死，则判定技能:攻击时，10%概率造成0.8s[眩晕]
                        bool isTriggerSkill = RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()) ? true : false;
                        if (isTriggerSkill)
                        {
                            enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.Stun, 0, 0.8f, "107"));
                        }
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
