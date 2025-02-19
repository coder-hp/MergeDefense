using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLogic : MonoBehaviour
{
    int atk;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;

    public void init(int _atk,EnemyLogic _enemyLogic)
    {
        atk = _atk;
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
                enemyLogic.damage(atk);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
