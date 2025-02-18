using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBallLogic : MonoBehaviour
{
    int atk;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;
    float damageRange = 1.9f;

    public void init(int _atk, EnemyLogic _enemyLogic)
    {
        atk = _atk;
        enemyLogic = _enemyLogic;
        targetTrans = enemyLogic.centerPoint;
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
                for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                {
                    if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].centerPoint.position) <= damageRange)
                    {
                        if(EnemyManager.s_instance.list_enemy[i].damage(atk))
                        {
                            --i;
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
