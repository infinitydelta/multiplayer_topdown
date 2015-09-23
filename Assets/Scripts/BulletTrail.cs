using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

 [RequireComponent(typeof (LineRenderer))]
public class BulletTrail : NetworkBehaviour 
{

    LineRenderer lr;
    float lifespan = 0.1f;
    float starttime;
    bool initialized = false;
    Vector3 targetendpoint;
    Vector3 targetstartpoint;
    Vector3 endpoint;
    Vector3 startpoint;


	// Use this for initialization
     void Awake()
     {
         lr = GetComponent<LineRenderer>();
     }
	void Start () 
    {
        starttime = Time.time;
        Destroy(this.gameObject, lifespan);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(!initialized)
        {
            return;
        }
        if(endpoint != targetendpoint)
        {
            endpoint = Vector3.Lerp(targetstartpoint, targetendpoint, ((Time.time - starttime)*2f) / lifespan);
            lr.SetPosition(1, endpoint);
            if (endpoint == targetendpoint)
            {
                starttime = Time.time;
            }
        }
        else if(startpoint != targetendpoint)
        {
            startpoint = Vector3.Lerp(targetstartpoint, targetendpoint, ((Time.time - starttime)*2f) / lifespan);
            lr.SetPosition(0, startpoint);
        }
	
	}
     [ClientRpc]
    public void RpcSetStartEnd(Vector3 start, Vector3 end)
    {
        targetstartpoint = start;
        targetendpoint = end;
        startpoint = start;
        endpoint = start;
        lr.SetPosition(0, start);
        lr.SetPosition(1, start);
        initialized = true;
    }
}
