using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawEnterLayer : MonoBehaviour
{
    void Start()
    {
        
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound_btn();
        LayerManager.ShowLayer(Consts.Layer.ClawLayer);
    }
}
