using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class InventoryItem : NetworkBehaviour 
{
    public string itemName = "item";
    public int maxStack = 1;
    public bool consumable = false;

    [SyncVar]
    bool inInventory = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(!inInventory && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().item = this;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.GetComponent<PlayerController>().item == this)
            {
                other.GetComponent<PlayerController>().item = null;
            }
        }
    }
    [ClientRpc]
    public void RpcInInventory(bool val)
    {
        inInventory = val;
    }
    [ClientRpc]
    public void RpcActive(bool val)
    {
        this.gameObject.SetActive(val);
    }
    [ClientRpc]
    public void RpcRigidbodyKinematic(bool val)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = val;
        }
    }
    [ClientRpc]
    public void RpcSetTransform(Vector3 pos, Quaternion rot, Vector3 sca)
    {
        this.transform.position = pos;
        this.transform.rotation = rot;
        this.transform.localScale = sca;
    }
    [ClientRpc]
    public void RpcParent(GameObject newParent)
    {
        if (newParent == null)
        {
            this.transform.SetParent(null);
            this.GetComponent<NetworkTransform>().enabled = true;
        }
        else
        {
            Transform parent = newParent.transform;
            this.transform.SetParent(parent);
            this.GetComponent<NetworkTransform>().enabled = false;
        }
    }
    [ClientRpc]
    public void RpcColliders(bool enabled)
    {
        foreach(Collider c in GetComponents<Collider>())
        {
            c.enabled = enabled;
        }
    }
}
