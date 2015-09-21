using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public GameObject cam;

	public float speed = 2;
    public float rotSpeed = 180f;

	Transform thisTransform = null;
	Rigidbody rb = null;

    Vector3 mousePosInWorldSpace;
    float targetYRotation;
    Camera playerCamera;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
        playerCamera = cam.GetComponent<Camera>();
	}

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			playerCamera.enabled = false;
			cam.SetActive(false);
			this.enabled = false;

		}
	}
	
	// Update is called once per frame
    void Update()
    {
        //mousePosInWorldSpace = playerCamera
        Ray mouseRay = playerCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        RaycastHit hit;
        int layermask = 1 << 8; //only hit layer 8 (Floor)

        if(Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, 100f, layermask))
        {
            mousePosInWorldSpace = hit.point;
            targetYRotation = Mathf.Atan2(-(mousePosInWorldSpace.z - thisTransform.position.z), mousePosInWorldSpace.x - thisTransform.position.x)*Mathf.Rad2Deg;
            //Debug.Log(targetYRotation);
        }


        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 100, Color.yellow);
    }
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

        thisTransform.rotation = Quaternion.Euler(0, targetYRotation, 0);

        //thisTransform.LookAt(mousePosInWorldSpace);
	}




}
