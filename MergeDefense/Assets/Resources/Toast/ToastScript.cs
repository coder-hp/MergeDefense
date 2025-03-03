using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class ToastScript : MonoBehaviour
{
    static GameObject prefab = null;
    static Vector3 moveOffset = new Vector3(0, 200, 0);

    public static void show (string text)
    {
        try
        {
            if (prefab == null)
            {
                prefab = Resources.Load("Toast/Toast") as GameObject;
            }
            GameObject obj = Instantiate(prefab, LaunchScript.s_instance.canvas_top);
            obj.transform.Find("Text").GetComponent<Text>().text = text;

            obj.transform.DOLocalMove(moveOffset, 1).OnComplete(() =>
            {
                Destroy(obj);
            });
        }
        catch(Exception exp)
        {
            Debug.Log("ToastScript.show异常：" + exp.ToString());
        }
    }
}
