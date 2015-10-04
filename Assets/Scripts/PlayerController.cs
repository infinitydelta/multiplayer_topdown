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
    float relMoveScaleFactor;
    bool aiming;

    Vector3 mousePosInWorldSpace;
    float targetYRotation;
    Vector3 targetRotVector;
    Camera playerCamera;
    Canvas canvas;
    PlayerInventory playerInventory;
    bool holding = false;

    public InventoryItem item;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.maxAngularVelocity = 1000;
        playerCamera = cam.GetComponent<Camera>();
        playerInventory = GetComponent<PlayerInventory>();
        canvas = thisTransform.FindChild("Canvas").GetComponent<Canvas>();
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
            canvas.enabled = false;
		}
        else
        {
            canvas.transform.parent = null;
            canvas.transform.position = Vector3.zero;
        }
        thisTransform.Translate(new Vector3(0, 2, 0));
	}
	
	// Update is called once per frame
    void Update()
    {
        //raycast the mouse position in screenspace from the camera to find where on the floor the mouse is over in worldspace
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
		if (Input.GetButtonDown("Fire"))
		{
            Vector3 shootHitPoint = muzzle.position + (100f * thisTransform.right);
            GameObject objectHit = null;
            RaycastHit shoot;
            if(Physics.Raycast(muzzle.position, thisTransform.right, out shoot, 100f))
            {
                shootHitPoint = shoot.point;
                objectHit = shoot.collider.gameObject;
            }

			CmdShootRay(muzzle.position, shootHitPoint, objectHit); //Call on Server
			CameraShake.cameraShake.startShake();
		}
        //aim
        if(Input.GetButtonDown("Aim"))
        {
            aiming = true;
        }
        if(Input.GetButtonUp("Aim"))
        {
            aiming = false;
        }

        //get movement input
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputVector = Vector3.ClampMagnitude(inputVector, 1f); //inputVector.Normalize();

        //movement scaling factor based on angle between movement input and direction facing
        float currentRot = thisTransform.rotation.eulerAngles.y;
        float movementRot = Mathf.Atan2(-inputVector.z, inputVector.x)*Mathf.Rad2Deg;
        Debug.DrawLine(thisTransform.position, thisTransform.position + inputVector, Color.blue);
        //Debug.Log(currentRot - movementRot);
        relMoveScaleFactor = 0.5f + 0.5f * Mathf.Cos((currentRot - movementRot) * Mathf.Deg2Rad); //normalized to 0-1
        //Debug.Log(relMoveScaleFactor);

        //inventory input
        if(Input.GetButtonDown("Inventory Up") || Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if(holding)
            {
                playerInventory.shiftUp();
            }
            else
            {
                playerInventory.selectUp();
            }
        }
        if (Input.GetButtonDown("Inventory Down") || Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if(holding)
            {
                playerInventory.shiftDown();
            }
            else
            {
                playerInventory.selectDown();
            }
        }
        if(Input.GetButtonDown("Inventory Hold"))
        {
            holding = true;
        }
        if(Input.GetButtonUp("Inventory Hold"))
        {
            holding = false;
        }
        if(Input.GetButtonDown("Inventory Throw"))
        {
            playerInventory.drop();
        }
        if(Input.GetButtonDown("Interact"))
        {
            if(item != null)
            {
                playerInventory.pickUp(item);
                item = null;
            }
        }
    }

	[Command]
	void CmdShootRay(Vector3 rayStart, Vector3 rayEnd, GameObject hit)
	{
        GameObject bTrail = (GameObject)(Instantiate(bulletTrail));
        BulletTrail bt = bTrail.GetComponent<BulletTrail>();

        if (hit != null)
        {
            Rigidbody hitrb = hit.GetComponent<Rigidbody>();
            if (hitrb != null)
            {
                hitrb.AddForce((rayEnd - rayStart).normalized * 5f, ForceMode.Impulse);
            }
        }
       
		NetworkServer.Spawn(bTrail);
        bt.RpcSetStartEnd(rayStart, rayEnd); //Call on all clients
	}

	void FixedUpdate () {

		Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 influence = (inputVector * maxSpeed - vel);
        if (influence.sqrMagnitude > accel*accel)
        {
            influence = influence.normalized * accel; 
        }
        rb.AddForce(influence, ForceMode.VelocityChange);
        float moveScale = 0.5f + 0.5f * relMoveScaleFactor; //50% move speed when facing opposite direction
        if(aiming)
        {
            moveScale *= 0.5f; //50% move speed when aiming
        }
        rb.velocity = new Vector3((rb.velocity.x > 0? Mathf.Min(rb.velocity.x, maxSpeed * moveScale): Mathf.Max(rb.velocity.x, -1 * maxSpeed * moveScale)), rb.velocity.y, (rb.velocity.z > 0? Mathf.Min(rb.velocity.z, maxSpeed * moveScale): Mathf.Max(rb.velocity.z, -1 * maxSpeed * moveScale)));
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
    public bool isAiming()
    {
        return aiming;
    }


}
