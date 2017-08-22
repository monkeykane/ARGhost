using UnityEngine;
using System.Collections;

public class DragonCurrency : MonoBehaviour {

	public int m_nBuffID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		Debug.Log ("OnEnable " + m_nBuffID.ToString());

		DragonBuffManager.Instance ().ChooseBuff (m_nBuffID);
	}
}
