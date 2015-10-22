using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInventory : NetworkBehaviour 
{

    Queue<InventoryItem>[] playerItems;
    public Transform location;
    public Text guitext;

    Transform thisTransform;
    PlayerController owner;

    int currentlySelectedIndex;
    int maxInventorySize = 5;

    int currentlySelectedWeaponAmmoIndex;

	// Use this for initialization
    void Awake()
    {

    }
	void Start () 
    {
        playerItems = new Queue<InventoryItem>[maxInventorySize];
        thisTransform = this.GetComponent<Transform>();
        currentlySelectedIndex = 0;
        owner = GetComponent<PlayerController>();
        updateText();
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    public void selectUp()
    {
        int otherindex = currentlySelectedIndex - 1;
        if (otherindex < 0)
        {
            otherindex = maxInventorySize - 1;
        }
        selectIndex(otherindex);
    }
    public void selectDown()
    {
        int otherindex = currentlySelectedIndex + 1;
        if (otherindex == maxInventorySize)
        {
            otherindex = 0;
        }
        selectIndex(otherindex);
    }
    public void selectIndex(int index)
    {
        //hide and return old item
        if (playerItems[currentlySelectedIndex] != null)
        {
            InventoryItem oldItem = playerItems[currentlySelectedIndex].Peek();
            CmdHideItem(oldItem.gameObject);
        }
        currentlySelectedIndex = index;
        //show newly selected item and move into position
        if (playerItems[currentlySelectedIndex] != null)
        {
            InventoryItem newItem = playerItems[currentlySelectedIndex].Peek();
            CmdShowItem(newItem.gameObject);
            Weapon weapon = newItem.GetComponent<Weapon>();
            owner.weaponInHand = weapon;
            if(weapon != null)
            {
                //calculate amount of ammo carrying
                int amt = 0;
                currentlySelectedWeaponAmmoIndex = -1;
                for(int x = 0; x < maxInventorySize; x++)
                {
                    if(playerItems[x] != null && playerItems[x].Peek().itemName.Equals(weapon.ammotype))
                    {
                        amt += playerItems[x].Count;
                        currentlySelectedWeaponAmmoIndex = x;
                    }
                }
                weapon.currentammototal = amt;
            }
        }
        updateText();
        
    }
    public void shiftUp()
    {
        int otherindex = currentlySelectedIndex - 1;
        if(otherindex < 0)
        {
            otherindex= maxInventorySize - 1;
        }
        swap(otherindex);
    }
    public void shiftDown()
    {
        int otherindex = currentlySelectedIndex + 1;
        if (otherindex == maxInventorySize)
        {
            otherindex = 0;
        }
        swap(otherindex);
    }
    public void swap(int otherindex)
    {
        Queue<InventoryItem> temp = playerItems[otherindex];
        playerItems[otherindex] = playerItems[currentlySelectedIndex];
        playerItems[currentlySelectedIndex] = temp;
        currentlySelectedIndex = otherindex;
        updateText();
    }
    public void drop()
    {
        //eject item
        if(playerItems[currentlySelectedIndex] == null)
        {
            return;
        }
        InventoryItem item = playerItems[currentlySelectedIndex].Dequeue();
        CmdDropItem(item.gameObject);
        item.GetComponent<Rigidbody>().AddForce(this.transform.right * 20f, ForceMode.Impulse);
        item.enabled = true;
        //update count
        if (playerItems[currentlySelectedIndex].Count == 0)
        {
            playerItems[currentlySelectedIndex] = null;
            for(int i = currentlySelectedIndex; i < maxInventorySize; i++)
            {
                if(playerItems[i] != null)
                {
                    currentlySelectedIndex = i;
                    selectIndex(currentlySelectedIndex);
                    updateText();
                    return;
                }
            }
            for(int i = 0; i < currentlySelectedIndex; i++)
            {
                if (playerItems[i] != null)
                {
                    currentlySelectedIndex = i;
                    updateText();
                    return;
                }
            }
            currentlySelectedIndex = 0;
            selectIndex(currentlySelectedIndex);
            updateText();
            return;
        }
        selectIndex(currentlySelectedIndex);
        updateText();
    }
    public bool pickUp(InventoryItem item)
    {
		HUDscript.hidePickUpText();


		for (int i = 0; i < maxInventorySize; i++)
        {
            if(playerItems[i] != null && playerItems[i].Peek().itemName.Equals(item.itemName) && playerItems[i].Count < item.maxStack) //already have one in inventory, not at max stack
            {
                item.inInventory = true;
                playerItems[i].Enqueue(item);
                CmdPickUpItem(item.gameObject);
                selectIndex(currentlySelectedIndex);
                updateText();
                return true;
            }
        }
        for (int i = 0; i < maxInventorySize; i++) //none already in inventory, look for an empty slot
        {
            if(playerItems[i] == null) //empty spot
            {
                item.inInventory = true;
                playerItems[i] = new Queue<InventoryItem>();
                playerItems[i].Enqueue(item);
                CmdPickUpItem(item.gameObject);
                selectIndex(currentlySelectedIndex);
                updateText();
                return true;
            }
        }
        return false;
    }
    [Command]
    public void CmdPickUpItem(GameObject item)
    {
        InventoryItem ii = item.GetComponent<InventoryItem>();
        if(ii == null)
        {
            return;
        }
        ii.RpcSetTransform(thisTransform.position, thisTransform.rotation, ii.transform.localScale);
        ii.RpcParent(this.gameObject);
        ii.RpcRigidbodyKinematic(true);
        ii.RpcInInventory(true);
        ii.RpcColliders(false);
        ii.RpcActive(false);
    }
    [Command]
    public void CmdDropItem(GameObject item)
    {
        InventoryItem ii = item.GetComponent<InventoryItem>();
        if (ii == null)
        {
            return;
        }
        ii.RpcSetTransform(location.position + thisTransform.right * 2f, thisTransform.rotation, ii.transform.localScale);
        ii.RpcParent(null);
        ii.RpcRigidbodyKinematic(false);
        ii.RpcInInventory(false);
        ii.RpcColliders(true);
        ii.RpcActive(true);
    }
    [Command]
    public void CmdShowItem(GameObject item)
    {
        InventoryItem ii = item.GetComponent<InventoryItem>();
        if (ii == null)
        {
            return;
        }
        ii.RpcSetTransform(location.position, thisTransform.rotation, ii.transform.localScale);
        ii.RpcActive(true);
    }
    [Command]
    public void CmdHideItem(GameObject item)
    {
        InventoryItem ii = item.GetComponent<InventoryItem>();
        if (ii == null)
        {
            return;
        }
        ii.RpcSetTransform(thisTransform.position, thisTransform.rotation, ii.transform.localScale);
        ii.RpcActive(false);
    }

    private void updateText()
    {
        string text = "";
        for(int i = 0; i < maxInventorySize; i++)
        {
            if(i == currentlySelectedIndex)
            {
                text += "[";
            }
            if(playerItems[i] == null)
            {
                text += "-----";
            }
            else
            {
                text += playerItems[i].Peek().itemName + (playerItems[i].Count > 1 ? " x" + playerItems[i].Count : ""); 
            }
            if (i == currentlySelectedIndex)
            {
                text += "]";
            }
            text += "\n";
        }
        guitext.text = text;
    }

    public void fireWeapon()
    {
        if(currentlySelectedWeaponAmmoIndex > -1) //sanity check
        {
            InventoryItem toDestroy = playerItems[currentlySelectedWeaponAmmoIndex].Dequeue();
            string ammoname = toDestroy.itemName;
            if(playerItems[currentlySelectedWeaponAmmoIndex].Count == 0) //last one in slot, change index if possible
            {
                playerItems[currentlySelectedWeaponAmmoIndex] = null;
                currentlySelectedWeaponAmmoIndex = -1;
                for(int x = 0; x < maxInventorySize; x++)
                {
                    if(playerItems[x] != null && playerItems[x].Peek().itemName.Equals(ammoname))
                    {
                        currentlySelectedWeaponAmmoIndex = x;
                    }
                }
            }
            //destroy the used ammo object;
            CmdDestroyItem(toDestroy.gameObject);
        }
        updateText();
    }
    [Command]
    public void CmdDestroyItem(GameObject item)
    {
        item.GetComponent<InventoryItem>().RpcDestroySelf();
    }
    public int Contains(string itemname)
    {
        int output = 0;
        for (int x = 0; x < maxInventorySize; x++)
        {
            if(playerItems[x] != null && playerItems[x].Peek().itemName.Equals(itemname))
            {
                output += playerItems[x].Count;
            }
        }
        return output;
    }
}
