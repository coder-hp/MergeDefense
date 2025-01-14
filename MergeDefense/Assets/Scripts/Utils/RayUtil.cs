using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayUtil
{
    // offsetPos是为了减少透视带来的视觉误差，优化拖动放块的手感
    static Vector3 offsetPos = new Vector3(0, 0, 0.5f);
    public static RaycastHit getEndPoint(Transform trans)
    {
        //创建一条射线，产生的射线是在世界空间中，从相机的近裁剪面开始并穿过屏幕position(x,y)像素坐标（position.z被忽略）  
        Ray ray = new Ray(trans.position + offsetPos, Vector3.down);

        //RaycastHit是一个结构体对象，用来储存射线返回的信息  
        RaycastHit hit;
        //如果射线碰撞到对象，把返回信息储存到hit中  
        if (Physics.Raycast(ray, out hit,6))
        {
            return hit;
        }

        // 显示射线，只能在Scene面板中看到
        //Debug.DrawLine(ray.origin, hit.point, Color.red);

        return hit;
    }
}
