using OSCQuery;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class MovingHeadController : BluxObject
{
    public enum Mode { TARGET, PANTILT };
    public Mode mode = Mode.PANTILT;

    public Transform panT;
    Transform tiltT;

    //[Range(0, 1)]
    public float pan;
    //[Range(0, 1)]
    public float tilt;

    [Header("Calibration")]
    public float minPanAngle;
    public float maxPanAngle;

    public float minTiltAngle;
    public float maxTiltAngle;

    public float headYOffset = 0;

    [Header("Target")]
    public float panMin;
    public float panMax;
    public float tiltMin;
    public float tiltMax;

    Vector3 cTarget;
    public Vector3 debugPos;

    //public Renderer beamMat;

    bool targetIsAccessible;

    override public void Awake()
    {
        base.Awake();
        //spanT = transform.Find("Axis");
        tiltT = panT.Find("Head");
    }
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        if (mode == Mode.TARGET && target != null) setPanTiltFromTarget2(target.position);

        panT.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(minPanAngle, maxPanAngle, pan));
        tiltT.localRotation = Quaternion.Euler(Mathf.Lerp(minTiltAngle, maxTiltAngle, tilt), 0, 0);

        //beamMat.sharedMaterial.SetColor("_EmissionColor", targetIsAccessible ? new Color(.2f, 1, 0) : new Color(1, .2f, 0));
    }

    override public void clear()
    {
        if (target != null) Destroy(target.gameObject);
    }


    void setPanTiltFromTarget2(Vector3 worldTarget)
    {

        Vector3 position = Vector3.up * headYOffset;// InverseTransformPoint(transform.position, transform.rotation, transform.localScale, tiltT.position);
        Vector3 target = InverseTransformPoint(transform.position, transform.rotation, transform.localScale, worldTarget);

        tilt = 0;
        //Vector2 xzPos = new Vector2(position.x, position.z);
        Vector2 relXZTarget = new Vector2(target.x, target.z) - new Vector2(position.x, position.z);

        float xzAngle = Vector2.SignedAngle(Vector2.right, relXZTarget);
        pan = ((-xzAngle + 90) / 360 + .5f) % 1;


        //Rotate around zero of the calculated pan to be able to simplify tilt computation to a XY angle
        Quaternion xzRot = Quaternion.Euler(new Vector3(0, xzAngle, 0));
        cTarget = xzRot * target;



        Vector2 relXYTarget = new Vector2(cTarget.x, cTarget.y) - new Vector2(position.x, position.y);
        targetIsAccessible = relXYTarget.y >= 0;

        if (targetIsAccessible)
        {
            float xyAngle = Vector3.Angle(Vector3.right, relXYTarget);
            tilt = xyAngle / 180;
            if (tilt > .5) tilt = 1 - tilt;


        }
        else
        {
            tilt = 0;
        }
    }

    Vector3 InverseTransformPoint(Vector3 localPos, Quaternion localRotation, Vector3 localScale, Vector3 targetPos)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(localPos, localRotation, localScale);
        Matrix4x4 inverse = matrix.inverse;
        return inverse.MultiplyPoint3x4(targetPos);
    }



    override public void loadJSONData(JSONObject data)
    {
        base.loadJSONData(data); 

        //Debug.Log("Moving Head Load JSON data " + data);

        JSONObject oComp = data["components"]["orientation"];
        if (oComp != null)
        {

            target = FindObjectOfType<ObjectManager>().createTarget(this);


            JSONObject targetO = oComp["computedParams"]["outTarget"];
            JSONObject panO = oComp["computedParams"]["outPan"];
            JSONObject tiltO = oComp["computedParams"]["outTilt"];

            
            target.position = new Vector3(targetO[0].f, targetO[1].f, targetO[2].f);
            pan = panO.f;
            tilt = tiltO.f;
        }



    }

    override public void updateFromJSONData(string name, JSONObject data)
    {
        base.updateFromJSONData(name, data);
        JSONObject val = data["value"];


        switch (name)
        {
            case "outTarget": target.position = new Vector3(val[0].f, val[1].f, val[2].f); break;
            case "outPan": pan = val.f; break;
            case "outTilt": tilt = val.f; break;
            case "outDebug": debugPos = new Vector3(val[0].f, val[1].f, val[2].f); break;
        }
    }


    void OnDrawGizmos()
    {
        if(debugPos != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(debugPos, .1f);
        }
    }
}
