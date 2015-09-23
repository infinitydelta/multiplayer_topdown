using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour {

	public float scalingFactor = 0.25f; //0 = follow player
	public float smoothing = 0.1f; //1 = no smoothing

	Transform thisTransform = null;
	Transform playerTransform = null;
    PlayerController playerController = null;
    Vector3 targetPos;


	void Awake()
	{
		thisTransform = GetComponent<Transform>();
	}
	// Use this for initialization
	void Start () {
		playerTransform = thisTransform.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
		thisTransform.parent = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//thisTransform.position = playerTransform.position;
        targetPos = playerTransform.position + ((playerController.getMousePosInWorldSpace() - playerTransform.position) * scalingFactor);
		//thisTransform.position = new Vector3(Mathf.Lerp(thisTransform.position.x, targetPos.x, smoothing), Mathf.Lerp(thisTransform.position.y, targetPos.y, smoothing), Mathf.Lerp(thisTransform.position.z, targetPos.z, smoothing));
		thisTransform.position  = Vector3.Lerp(thisTransform.position, targetPos, smoothing);
	}
}
