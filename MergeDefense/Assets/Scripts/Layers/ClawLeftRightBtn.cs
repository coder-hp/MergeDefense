using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLeftRightBtn : MonoBehaviour
{
    public ClawLayer clawLayer;
    public bool isLeft;

    private void OnMouseDrag()
    {
        clawLayer.clawMove(isLeft);
    }
}
