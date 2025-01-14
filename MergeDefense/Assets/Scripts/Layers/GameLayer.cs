using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    void Start()
    {
        LayerManager.ShowLayer(Consts.Layer.GameUILayer);
    }
}
