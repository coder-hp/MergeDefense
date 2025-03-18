using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroFlyWeaponBase : MonoBehaviour
{
    [HideInInspector]
    public HeroLogicBase heroLogicBase;
    [HideInInspector]
    public int fixedAtk = 0;

    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic, int _fixedAtk = 0)
    {
        heroLogicBase = _heroLogicBase;
        enemyLogic = _enemyLogic;
        fixedAtk = _fixedAtk;

        if (enemyLogic)
        {
            targetTrans = enemyLogic.transform;
            transform.position = heroLogicBase.flyWeaponPoint.position;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -heroLogicBase.modelTrans.eulerAngles.y);
            transform.position = heroLogicBase.flyWeaponPoint.position;
            transform.position = new Vector3(transform.position.x, transform.position.y,0);
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
            if (!isReadyDestroy)
            {
                Destroy(gameObject, 3);
                transform.GetComponent<BoxCollider>().enabled = true;
            }

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (heroLogicBase)
            {
                EnemyLogic _enemyLogic = other.transform.GetComponent<EnemyLogic>();
                atkEnemy(_enemyLogic);
            }
            Destroy(gameObject);
        }
    }

    public virtual void atkEnemy(EnemyLogic _enemyLogic)
    {
        // TODO
    }
}
