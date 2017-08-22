using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Script.Table;

public class DragonApp : MonoBehaviour
{
	void Awake()
	{
		TableManager.Init ();
	}
	
	void Start()
	{
		_Init ();

		DragonBuffManager.Instance().OnStart();
	}

	void Update()
	{
		DragonBuffManager.Instance ().Update (0);
	}

	private void _Init()
	{
		int nSceneID = SceneManager.GetActiveScene ().buildIndex;
		if (nSceneID == 0) 
		{
			// lobby
			DragonBuffManager.Instance().Init();
			DragonAchievementManager.Instance ().Init ();
		} 
		else if (nSceneID == 1) 
		{
			// Ghost Game
		}
		else if (nSceneID == 2) 
		{
			// Money
			DragonBuffManager.Instance().Init2();
		}
	}
	
	public void OnClickButton()
	{
		SceneManager.LoadScene (1);
	}

	public void OnClickButtonMoney()
	{
		SceneManager.LoadScene (2);
	}

	public void OnClickButtonBack()
	{
		SceneManager.LoadScene (0);
	}

	public void OnClickButtonMask()
	{
		if (DragonBuffManager.Instance ().IsBuffExist ()) {
			SceneManager.LoadScene (0);
		}
	}

	public void OnClickButtonAchievement()
	{
		SceneManager.LoadScene (3);
	}
}
