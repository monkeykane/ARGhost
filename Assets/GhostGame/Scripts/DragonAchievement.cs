using UnityEngine;
using System.Collections;
using Script.Table;
using Script.Table.Ghost;
using UnityEngine.SceneManagement;

public class DragonAchievement : MonoBehaviour {

	UISprite [] m_aSp1;
	UISprite [] m_aSp2;

	public DragonAchievement()
	{
		
	}

	// Use this for initialization
	void Start () 
	{
		_Init ();
	}

	private void _Init()
	{
		m_aSp1 = new UISprite[GameConst.Achievement_Num];
		m_aSp2 = new UISprite[GameConst.Achievement_Num];

		for(int i = 0; i < GameConst.Achievement_Num; i++)
		{
			m_aSp1 [i] = GameObject.Find ("UI Root/Sprite" + i.ToString() + "/sp1").GetComponent<UISprite>();
			m_aSp2 [i] = GameObject.Find ("UI Root/Sprite" + i.ToString() + "/sp2").GetComponent<UISprite>();

			_ShowSingleSprite (m_aSp1[i], m_aSp2[i], i);
		}
	}

	private void _ShowSingleSprite(UISprite sp1, UISprite sp2, int nIndex)
	{
		AchievementData data = AchievementTableManager.Instance ().GetAchievementDataByIndex (nIndex);
		int nStatus = DragonAchievementManager.Instance ().GetAchievementStatus (nIndex);

		sp2.spriteName = data.m_strBackgroundIcon;
		if (nStatus == 0) 
		{
			sp1.spriteName = "Question";
		} 
		else 
		{
			sp1.spriteName = data.m_strIcon;
		}
	}

	public void OnClickButtonBack()
	{
		SceneManager.LoadScene (0);
	}
}
