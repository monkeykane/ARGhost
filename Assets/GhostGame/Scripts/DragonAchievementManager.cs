using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using Script.Table;
using Script.Table.Ghost;

public class DragonAchievementManager {

	private const string File_Name = "achievement.txt";

	private static DragonAchievementManager s_instance;

	public static DragonAchievementManager Instance()
	{
		if (s_instance == null) 
		{
			s_instance = new DragonAchievementManager ();
		}

		return s_instance;
	}
		
	private int[] m_aAchievementStatus;

	private DragonAchievementManager()
	{
		m_aAchievementStatus = new int[GameConst.Achievement_Num];
	}

	public void Init()
	{
		_ReadFile ();
		_UpdateUI ();
	}

	private void _UpdateUI()
	{
		UILabel labelAchievement = GameObject.Find ("UI Root/Label_Achievement").GetComponent<UILabel>();

		int nFinsihedCount = GetFinsihedAchievementCount ();
		labelAchievement.text = nFinsihedCount.ToString("00") + "/" + GameConst.Achievement_Num.ToString();
	}

	public void SaveFile()
	{
		string strFullFileName = Application.persistentDataPath + "/" + File_Name;
		if (File.Exists (strFullFileName)) {
			File.Delete (strFullFileName);
			return;
		}

		FileStream fs = new FileStream (strFullFileName, FileMode.Create);
		StreamWriter sw = new StreamWriter (fs, Encoding.UTF8);

		string line;
		for (int i = 0; i < GameConst.Achievement_Num; i++) {

			AchievementData data = AchievementTableManager.Instance ().GetAchievementDataByIndex (i);
			line = data.m_nId.ToString () + ":" + m_aAchievementStatus [i].ToString ();
			sw.WriteLine (line);
		}

		sw.Close ();
		fs.Close ();
	}

	private void _ReadFile()
	{
		// read file 
		string strFullFileName = Application.persistentDataPath + "/" + File_Name;
		if (!File.Exists (strFullFileName)) {

			SaveFile ();
			return;
		}

		StreamReader sr = new StreamReader (strFullFileName, Encoding.UTF8);
		string line;
		for (int i = 0; i < GameConst.Achievement_Num; i++) {

			line = sr.ReadLine ();
			string[] aItem = line.Split (':');
			m_aAchievementStatus [i] = int.Parse (aItem [1]);
		}

		sr.Close ();
	}

	public int GetAchievementStatus(int nIndex)
	{
		return m_aAchievementStatus [nIndex];
	}

	public void OnCatchGhost(int nGhostID)
	{
		int nIndex = AchievementTableManager.Instance ().GetAchievementIndex (nGhostID);
		if (m_aAchievementStatus [nIndex] == 0) 
		{
			m_aAchievementStatus [nIndex] = 1;
			SaveFile ();
		}


	}

	public int GetFinsihedAchievementCount()
	{
		int nCount = 0;
		for(int i = 0; i < GameConst.Achievement_Num; i++)
		{
			if (m_aAchievementStatus [i] == 1) 
			{
				nCount++;
			}
		}

		return nCount;
	}
}
