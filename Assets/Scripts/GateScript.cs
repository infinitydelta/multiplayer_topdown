using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

	bool moveDown = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (moveDown)
		{
			transform.Translate(1 * Time.deltaTime, 0, 0, Space.World);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
            PlayerController pc = other.GetComponent<PlayerController>();
			if (Input.GetKey("e") && pc != null && pc.GetComponent<PlayerInventory>().Contains("shiny rock") >= 3)
			{
				moveDown = true;
				Destroy(this, 15);
			}
		}
	}
}
