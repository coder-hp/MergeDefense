using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderParamControl : MonoBehaviour
{
    public enum RendererType
    {
        MeshRenderer,
        SkinnedMeshRenderer,
        TrailRenderer,
    }

    public Material material;
    public bool isSmallToBig = true;
    public string paramName;
    public float minValue = 0;
    public float maxValue = 1;
    public float durTime;
    public RendererType rendererType = RendererType.MeshRenderer;

    bool isStart = false;
    Action callback = null;
    Vector3 curValueVec3;

    private void Awake()
    {
        switch (rendererType)
        {
            case RendererType.MeshRenderer:
                {
                    material = GetComponent<MeshRenderer>().material;
                    break;
                }

            case RendererType.SkinnedMeshRenderer:
                {
                    material = GetComponent<SkinnedMeshRenderer>().material;
                    break;
                }

            case RendererType.TrailRenderer:
                {
                    material = GetComponent<TrailRenderer>().material;
                    break;
                }
        }
    }

    void Update()
    {
        if(isStart)
        {
            material.SetFloat(paramName, curValueVec3.x);
        }
    }

    public void start(Ease ease, Action _callback = null)
    {
        callback = _callback;
        isStart = true;

        curValueVec3.x = isSmallToBig ? minValue : maxValue;
        DOTween.To(() => curValueVec3, x => curValueVec3 = x, new Vector3(isSmallToBig ? maxValue : minValue, 0, 0), durTime)
        .SetEase(ease)
        .OnComplete(() =>
        {
            Update();
            isStart = false;
            if (callback != null)
            {
                callback();
            }
        });
    }
}