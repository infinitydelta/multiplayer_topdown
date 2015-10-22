using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour 
{
    protected int health = 20;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
			//Destroy(this.gameObject);
			this.enabled = false;
        }
    }
}
