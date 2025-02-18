using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMoveLine : MonoBehaviour
{
    public static HeroMoveLine s_instance = null;

    public Transform startTrans;
    public Transform endTrans;
    public Transform bodyTrans;

    Material material;

    private void Awake()
    {
        s_instance = this;
        material = bodyTrans.GetComponent<SkinnedMeshRenderer>().material;
    }

    public void setPos(Vector3 start,Vector3 end)
    {
        if(transform.localScale.x == 0)
        {
            transform.localScale = Vector3.one;
        }

        startTrans.position = start;
        endTrans.position = end;

        float angle = CommonUtil.twoPointAngle(start, end);
        startTrans.rotation = Quaternion.Euler(0,0,angle);
        endTrans.rotation = startTrans.rotation;

        material.SetFloat("_Tiling",(int)Vector3.Distance(start, end) * 2 + 1);
    }

    public void hide()
    {
        transform.localScale = Vector3.zero;
    }
}
