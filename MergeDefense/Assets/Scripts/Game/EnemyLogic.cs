using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    int curTargetPosIndex = 1;
    float moveSpeed = 2;

    private void Awake()
    {
        EnemyManager.s_instance.addEnemy(this);
        transform.position = GameLayer.s_instance.list_enemyMoveFourPos[0];
    }

    public void move()
    {
        transform.position = Vector3.MoveTowards(transform.position, GameLayer.s_instance.list_enemyMoveFourPos[curTargetPosIndex], moveSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, GameLayer.s_instance.list_enemyMoveFourPos[curTargetPosIndex]) <= 0.1f)
        {
            if(++curTargetPosIndex > 3)
            {
                curTargetPosIndex = 0;
            }

            if(curTargetPosIndex == 3)
            {
                transform.localScale = new Vector3(-1,1,1);
            }
            else if (curTargetPosIndex == 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
