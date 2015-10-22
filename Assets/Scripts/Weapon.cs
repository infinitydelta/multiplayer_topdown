using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
    public string ammotype;
    public int damage;  //not yet used
    public float accuracy; //not yet used
    public float firerate;
    public bool fullauto;
    public float force;
    public int clipsize; //not yet used
    public float reloadtime; //not yet used
    public int currentammototal;
    public int currentammoinclip; //not yet used

    float firetimer;
    bool released;
    bool reloading; //not yet used

	// Use this for initialization
	void Start () 
    {
        firetimer = firerate;
        released = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(firetimer > 0)
        {
            firetimer -= Time.deltaTime;
        }
	
	}
    public bool fire()
    {
        if (currentammototal > 0 && firetimer <= 0) //can fire
        {
            if (!fullauto)
            {
				if (released)
				{
					firetimer = 1 / firerate;
					released = false;
					currentammototal--;
					return true;
				}
				return false;
            }
            else //full auto
            {
                firetimer = firerate;
                currentammototal--;
                return true;
            }
        }
        return false;
    }
    public void release()
    {
        released = true;
    }
}
