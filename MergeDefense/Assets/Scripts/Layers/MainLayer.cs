using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLayer : MonoBehaviour
{
    public void onClickStartGame()
    {
        Destroy(gameObject);

        LayerManager.ShowLayer(Consts.Layer.GameLayer);
    }
}
