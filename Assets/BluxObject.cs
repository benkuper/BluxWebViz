using OSCQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluxObject : MonoBehaviour
{
    public Transform target;
    public Light light;

    public Color lightColor = Color.white;
    public float dimmer;

    public virtual void Awake()
    {
        light = GetComponentInChildren<Light>();
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {

    }


    public virtual void clear()
    {
        if (target != null) Destroy(target.gameObject);
    }

    public virtual void loadJSONData(JSONObject data)
    {
        gameObject.name = data["niceName"].str;

        transform.position = new Vector3(data["stagePosition"][0].f, data["stagePosition"][1].f, data["stagePosition"][2].f);
        transform.rotation = Quaternion.Euler(data["stageRotation"][0].f, data["stageRotation"][1].f, data["stageRotation"][2].f);

        FindObjectOfType<BluxController>().registerAddress(data["controlAddress"].str, this);

        if (light != null)
        {
            JSONObject dComp = data["components"]["dimmer"];
            if (dComp)
            {
                JSONObject dO = dComp["computedParams"]["outValue"];
                dimmer = dO.f;
            }

            JSONObject cComp = data["components"]["color"];
            if (cComp != null)
            {
                //Debug.Log(cComp.ToString(cComp));
                JSONObject colO = cComp["computedParams"]["outMainColor"];
                lightColor = new Color(colO[0].f, colO[1].f, colO[2].f, colO[3].f);
            }
            else
            {
                lightColor = Color.white;
            }

            updateLightColor(); ;
        }




    }

    void updateLightColor()
    {
        if (light != null) light.color = lightColor * dimmer;
    }

    public virtual void updateFromJSONData(string name, JSONObject data)
    {
        JSONObject val = data["value"];

        switch (name)
        {
            case "stagePosition": transform.position = new Vector3(val[0].f, val[1].f, val[2].f); break;
            case "stageRotation": transform.rotation = Quaternion.Euler(val[0].f, val[1].f, val[2].f); break;
            case "outMainColor":
                if (light != null)
                {
                    lightColor = new Color(val[0].f, val[1].f, val[2].f, val[3].f);
                    updateLightColor();
                    //light.SetLightDirty();
                }
                break;

            case "outValue":
                dimmer = val.f;
                updateLightColor();
                break;
        }
    }
}
