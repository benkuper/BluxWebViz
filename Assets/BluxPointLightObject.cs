using OSCQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluxPointLightObject : BluxObject
{
    public Material fogMat;

    public virtual void Awake()
    {
        base.Awake();
        fogMat = GetComponent<MeshRenderer>().material;

    }

    public virtual void Update()
    {
        base.Update();
        fogMat.color = lightColor * dimmer;
        fogMat.SetFloat("_Density", dimmer * .05f);
        fogMat.SetVector("_FogCenter", new Vector4(transform.position.x, transform.position.y, transform.position.z, transform.localScale.x * .5f));

    }
}
