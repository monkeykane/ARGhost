using UnityEngine;

public class XiaoliangMonoBehaviour : HiARBaseObjectMonoBehaviour {

	void OnTargetFound(string targetId)
	{
		for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(true);
//		LogUtil.Log("OnTargetFound  targetId:" + targetId);
	}
	
	void OnTargetTracked(string targetId)
	{
//		LogUtil.Log("OnTargetTracked  targetId:" + targetId);
	}
	
	void OnTargetLost(string targetId)
	{
		for (int i = 0; i < this.transform.childCount; i++) this.transform.GetChild(i).gameObject.SetActive(false);
//		LogUtil.Log("OnTargetLost targetId:"+ targetId);
	}
}
