using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	Rigidbody rb = null;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	// Use this for initialization
	void Start () {
		rb.AddForce(transform.right * 10, ForceMode.VelocityChange);
		Destroy(this.gameObject, 5);

	}
	
}
