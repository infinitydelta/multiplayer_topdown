using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDscript : MonoBehaviour {
	//public GameObject HUD;
	public static HUDscript thisHUD;
	Text text;

	void Awake()
	{
		thisHUD = this;
	}

	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void showPickUpText(Vector3 position, string name)
	{
		if (thisHUD == null)
		{
			//create prefab
			createHUD();

		}
		thisHUD.text.text = "Press E to pick up " + name;
		thisHUD.text.transform.position = position + Vector3.up;
		thisHUD.text.gameObject.SetActive(true);
	}

	public static void hidePickUpText()
	{
		if (thisHUD == null)
		{
			createHUD();
		}
		thisHUD.text.gameObject.SetActive(false);
	}

	static void createHUD()
	{
		GameObject g = Instantiate(Resources.Load("HUD")) as GameObject;
		thisHUD = g.GetComponent<HUDscript>();
		thisHUD.text = g.GetComponentInChildren<Text>();
	}
}
