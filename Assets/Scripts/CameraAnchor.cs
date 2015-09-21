using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour {

	Transform thisTransform = null;
	Transform player = null;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
	}
	// Use this for initialization
	void Start () {
		player = thisTransform.parent;
		thisTransform.parent = null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		thisTransform.position = player.position;
	}
}
