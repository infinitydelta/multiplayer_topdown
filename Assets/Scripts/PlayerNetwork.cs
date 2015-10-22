using UnityEngine;
using System.Collections;

public class PlayerNetwork : MonoBehaviour {
	Rigidbody rb = null;
	Animator anim = null;
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponentInChildren<Animator>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (rb != null)
		{
			anim.SetFloat("Forward", rb.velocity.magnitude);
		}
	}
}
