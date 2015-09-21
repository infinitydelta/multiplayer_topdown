using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject cam;

	public float speed = 2;

	Transform thisTransform = null;
	Rigidbody rb = null;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			cam.GetComponent<Camera>().enabled = false;
			cam.SetActive(false);
			this.enabled = false;

		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey("w"))
		{
			rb.AddForce(Vector3.forward * speed, ForceMode.Impulse);
		}
		if (Input.GetKey("s"))
		{
			rb.AddForce(-Vector3.forward * speed, ForceMode.Impulse);
		}
		if (Input.GetKey("a"))
		{
			rb.AddForce(Vector3.left * speed, ForceMode.Impulse);
		}
		if (Input.GetKey("d"))
		{
			rb.AddForce(-Vector3.left * speed, ForceMode.Impulse);
		}

		if (Input.GetKey("left"))
		{
			thisTransform.Rotate(0, 1, 0);
		}


		Vector3.ClampMagnitude(rb.velocity, 10);
	}




}
