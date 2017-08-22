using UnityEngine;

public class HiARObjectMonoBehaviour : HiARBaseObjectMonoBehaviour {

	void OnTargetFound(string targetId)
	{
		for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(true);

	}
	
	void OnTargetTracked(string targetId)
	{

	}
	
	void OnTargetLost(string targetId)
	{
		for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(false);

	}

}
