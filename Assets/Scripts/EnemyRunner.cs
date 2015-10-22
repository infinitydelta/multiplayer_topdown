using UnityEngine;
using System.Collections;

public class EnemyRunner : Enemy 
{

    Transform currentPlayerTarget;
    Vector3 targetMovePosition;
    float attackTimer;
    float attackdelay = 1f;

	// Use this for initialization
	void Start () 
    {
        health = 30;
        targetMovePosition = this.transform.position;
        attackTimer = attackdelay;
	}
	
	// Update is called once per frame
	void Update () 
    {
        GameObject[] playersInScene = GameObject.FindGameObjectsWithTag("Player");
        if (playersInScene.Length > 0)
        {
            //find closest player
            currentPlayerTarget = playersInScene[0].transform;
            foreach (GameObject player in playersInScene)
            {
                if(Vector3.Distance(this.transform.position, player.transform.position) < Vector3.Distance(this.transform.position, currentPlayerTarget.position))
                {
                    currentPlayerTarget = player.transform;
                }
            }
            //only act if within a certain range
            if(Vector3.Distance(this.transform.position, currentPlayerTarget.position) < 30f) //30 units?
            {
                //raycast to player
                RaycastHit hit;
                if(Physics.Raycast(this.transform.position, currentPlayerTarget.position - this.transform.position, out hit, 50f))
                {
                    if(hit.collider.gameObject.Equals(currentPlayerTarget.gameObject))//can see the player
                    {
                        targetMovePosition = hit.point;
                    }
                }
            }
            //move towards point
            this.transform.LookAt(targetMovePosition);
            Debug.DrawLine(this.transform.position, targetMovePosition, Color.red);
            this.GetComponent<Rigidbody>().AddForce((targetMovePosition - this.transform.position).normalized * 20f);
            this.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 10f);
        }
        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
	
	}
    void OnTriggerStay(Collider other)
    {
        if(attackTimer <= 0)
        {
            if(other.CompareTag("Player"))
            {
                PlayerController pc = other.GetComponent<PlayerController>();
                if (pc != null)
                {
                    RpcTakeDamage(5);
                    attackTimer = attackdelay;
                }
            }
        }
    }
}
