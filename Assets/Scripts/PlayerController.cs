using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public bool editorOnly = false;

	public GameObject cam;
    public GameObject cameraAnchor;
	public GameObject bulletTrail;
	public Transform muzzle;

    public float maxSpeed = 10;
    //public float speed = 2;
    public float accel = 1;
    public bool enableMaxRotationSpeed = true;
    public float rotSpeed = 360f;

	Transform thisTransform = null;
	Rigidbody rb = null;

    Vector3 inputVector;

    Vector3 mousePosInWorldSpace;
    float targetYRotation;
    Vector3 targetRotVector;
    Camera playerCamera;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.maxAngularVelocity = 1000;
        playerCamera = cam.GetComponent<Camera>();
		if (editorOnly)
		{
			this.gameObject.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			playerCamera.enabled = false;
			cam.SetActive(false);
			this.enabled = false;

		}
        thisTransform.Translate(new Vector3(0, 2, 0));
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
            targetRotVector = new Vector3(mousePosInWorldSpace.x - thisTransform.position.x, 0, (mousePosInWorldSpace.z - thisTransform.position.z));
            targetRotVector.Normalize();
            //Debug.Log(targetYRotation);
        }

        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 100, Color.yellow);

        //shoot
		if (Input.GetMouseButtonDown(0))
		{
            Vector3 shootHitPoint = muzzle.position + (100f * thisTransform.right);
            GameObject objectHit = null;
            RaycastHit shoot;
            if(Physics.Raycast(muzzle.position, thisTransform.right, out shoot, 100f))
            {
                shootHitPoint = shoot.point;
                objectHit = shoot.collider.gameObject;
            }

			CmdShootRay(muzzle.position, shootHitPoint, objectHit);
			CameraShake.cameraShake.startShake();
		}

        //get movement input
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputVector = Vector3.ClampMagnitude(inputVector, 1f);

    }

	[Command]
	void CmdShootRay(Vector3 rayStart, Vector3 rayEnd, GameObject hit)
	{
        GameObject bTrail = (GameObject)(Instantiate(bulletTrail));
        BulletTrail bt = bTrail.GetComponent<BulletTrail>();
       
		NetworkServer.Spawn(bTrail);
        bt.RpcSetStartEnd(rayStart, rayEnd);
	}

	void FixedUpdate () {

		Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 influence = (inputVector * maxSpeed - vel);
        if (influence.sqrMagnitude > accel*accel)
        {
            influence = influence.normalized * accel;
        }
        rb.AddForce(influence, ForceMode.VelocityChange);
		rb.velocity = new Vector3(Mathf.Min(rb.velocity.x, maxSpeed), rb.velocity.y, Mathf.Min(rb.velocity.z, maxSpeed));
		//Vector3.ClampMagnitude(rb.velocity, maxSpeed);

		//thisTransform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        if (enableMaxRotationSpeed)
        {
            Vector3 newRot = Vector3.RotateTowards(thisTransform.right, targetRotVector, rotSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);
            rb.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(-newRot.z, newRot.x) * Mathf.Rad2Deg, 0));
        }
        else
        {
            rb.rotation = Quaternion.Euler(0, targetYRotation, 0);
        }
        
        

        //thisTransform.LookAt(mousePosInWorldSpace);
	}

    public Vector3 getMousePosInWorldSpace()
    {
        return mousePosInWorldSpace;
    }


}
