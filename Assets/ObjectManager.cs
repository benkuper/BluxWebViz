using OSCQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject spotLightPrefab;
    public GameObject movingHeadPrefab;
    public GameObject lightStripPrefab;
    public GameObject targetPrefab;

    List<BluxObject> objects;
    Dictionary<BluxObject, Transform> objectsTargetMap;

    Transform objectsT;
    Transform targetsT;

    void Awake()
    {
        objects = new List<BluxObject>();
        objectsT = transform.Find("Objects");
        targetsT = transform.Find("Targets");

        objectsTargetMap = new Dictionary<BluxObject, Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void rebuildFromJSON(JSONObject data)
    {
        while (objects.Count > 0) removeObject(objects[0]);

        for (int i = 0; i < data.keys.Count; i++)
        {
            createObjectFromJSON(data.keys[i], data[i]);
        }
    }

    void createObjectFromJSON(string name, JSONObject data)
    {
        bool hasOrientation = false;

        JSONObject orientationComponent = data["components"]["orientation"];
        if(orientationComponent != null) hasOrientation = true;

        string type = data["type"].str;

        BluxObject o = null;
        switch (type)
        {
            case "Custom":
                bool isMovingHead = hasOrientation;
                if (isMovingHead)
                {
                    MovingHeadController mh = Instantiate(movingHeadPrefab).GetComponent<MovingHeadController>();
                    o = mh;
                }
                else
                {
                    o = Instantiate(spotLightPrefab).GetComponent<BluxObject>();
                }
                break;

            default:
                Debug.Log("Type not handled : " + type);
                break;
        }


        if (o != null)
        {
           // o.transform.SetParent(objectsT);
            o.loadJSONData(data);
            objects.Add(o);
        }
    }



    void removeObject(BluxObject o)
    {
        o.clear();
        objects.Remove(o);
        if (objectsTargetMap == null) objectsTargetMap = new Dictionary<BluxObject, Transform>();
        if (objectsTargetMap.ContainsKey(o)) objectsTargetMap.Remove(o);
        Destroy(o.gameObject);
    }


    public Transform createTarget(BluxObject source)
    {
        Transform t = Instantiate(targetPrefab).transform;
        t.SetParent(targetsT);
        if (objectsTargetMap == null) objectsTargetMap = new Dictionary<BluxObject, Transform>();
        objectsTargetMap.Add(source, t);

        return t;
    }


    public void handleControllableUpdate(JSONObject data)
    {
    }
}
