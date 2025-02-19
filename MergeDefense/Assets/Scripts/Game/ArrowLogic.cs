using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLogic : MonoBehaviour
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
            //transform.LookAt(targetTrans);
            transform.position = Vector3.MoveTowards(transform.position, targetTrans.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTrans.position) <= 0.1f)
            {
                bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                int atk = (int)(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                enemyLogic.damage(atk, isCrit);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
