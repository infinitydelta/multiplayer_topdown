using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncTransform : NetworkBehaviour {

	public int tickRate = 120;

	[SyncVar]
	public Vector3 syncPosition;
	[SyncVar]
	public Quaternion syncRotation;

	float timer = 0;

	Transform thisTransform = null;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		syncPosition = thisTransform.position;
		syncRotation = thisTransform.rotation;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateTransform();
		if (!isLocalPlayer)
		{
			thisTransform.position = syncPosition;
			thisTransform.rotation = syncRotation;
		}
	}

	//every 1/120 sec send position and rotation to server
	//server reads network input and redistributes
	[Command]
	void CmdUpdateTransform(Vector3 pos, Quaternion rot)
	{
		syncPosition = pos;
		syncRotation = rot;
	}

	[ClientCallback]
	void UpdateTransform()
	{
		if (isLocalPlayer)
		{
			if (timer > 1/tickRate)
			{
				CmdUpdateTransform(thisTransform.position, thisTransform.rotation);
				timer = 0;
			}
			timer += Time.deltaTime;
		}
	}
}
