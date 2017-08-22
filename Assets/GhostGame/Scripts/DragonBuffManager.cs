using UnityEngine;
using System.Collections;
using System;
using Script.Table;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class DragonBuffManager {

	private const int Invalid_Buff_ID = 0;
	private const string File_Name = "buff.txt";

	protected static DragonBuffManager s_instance = null;

	public static DragonBuffManager Instance()
	{
		if (s_instance == null) 
		{
			s_instance = new DragonBuffManager ();
		}

		return s_instance;
	}
		
	private int m_nBuffID;
	private int m_nPowerRate;
	private float m_fDuration;
	private DateTime m_BeginTime;

	// UI Controls
	UILabel m_LabelBuff;
	UILabel m_LabelTime;
	UIButton m_ButtonMoney;
	UIButton m_ButtonBack;
	UIButton m_ButtonMask;

	UISprite m_SpriteSuccess;
	UISprite[] m_aCorner;

	// 
	private Boolean m_bTrackEnable;

	private DragonBuffManager()
	{
		m_nBuffID = Invalid_Buff_ID;
		m_bTrackEnable = false;
	}

	public bool IsBuffExist()
	{
		return m_nBuffID != Invalid_Buff_ID;
	}

	public void Init()
	{
		_InitUIControls ();
		_ReadBuffFile ();

		if (m_nBuffID == Invalid_Buff_ID) {
			_ShowButtonMoney ();
		}
		else {
			_ShowBuffLabels ();
		}
	}

	public void Init2()
	{
		_InitUIControls2 ();
	}

	public void OnStart()
	{
		int nSceneID = SceneManager.GetActiveScene ().buildIndex;
		if (nSceneID == 0) 
		{
			// lobby
			m_bTrackEnable = false;
		} 
		else if (nSceneID == 1) 
		{
			// Ghost Game
			m_bTrackEnable = false;
		}
		else if (nSceneID == 2) 
		{
			// Money
			m_bTrackEnable = true;
		}
	}

	private void _ClearBuff()
	{
		m_nBuffID = Invalid_Buff_ID;
	}

	public void Update(float dt)
	{
		if (m_nBuffID == Invalid_Buff_ID)
			return;

		bool bTimeExpired = false;
		TimeSpan deltaTime = DateTime.Now - m_BeginTime;
		float fSeconds = (float)deltaTime.TotalSeconds;
		float fLeftTime = m_fDuration - fSeconds;
		if (fLeftTime < 0) {
			m_nBuffID = Invalid_Buff_ID;
			bTimeExpired = true;
			SaveBuff ();
		}

		int nSceneID = SceneManager.GetActiveScene ().buildIndex;
		if (nSceneID == 0) 
		{
			// lobby
			if (bTimeExpired) {
				_ShowButtonMoney ();
			}
			Update_InLobby();
		} 
		else if (nSceneID == 1) 
		{
			// Ghost Game
			Update_InGhostGame();

			if (bTimeExpired) {
				DragonGhostGame.Instance.SetPowerValue (1.0f);
			}
		}
		else if (nSceneID == 2) 
		{
			// Money
			Update_InMoney();
		}
	}

	private void Update_InLobby()
	{
		if (m_nBuffID != Invalid_Buff_ID) {

			_UpdateLabelTime ();
		}
	}

	private void Update_InGhostGame()
	{
	}

	private void Update_InMoney()
	{
	}

	public void ChooseBuff(int nBuffID)
	{
		if (!m_bTrackEnable)
			return;
		
		if (m_nBuffID != Invalid_Buff_ID)
			return;

		m_nBuffID = nBuffID;
		GhostPowerData ghostPowerData = GhostPowerTableManager.Instance ().GetGhostPowerData (m_nBuffID);
		m_nPowerRate = ghostPowerData.GetPowerRate ();
		m_fDuration = ghostPowerData.m_fDuration * 60.0f;

		m_BeginTime = DateTime.Now;

		SaveBuff ();
		//SceneManager.LoadScene (0);

		_ShowBuffLabels2 ();
		m_ButtonBack.gameObject.SetActive (false);
	}

	public float GetPowerValue()
	{
		if (m_nBuffID == Invalid_Buff_ID)
			return 1.0f;

		float fPowerValue = ((float)m_nPowerRate) / 100.0f;
		return fPowerValue;
	}

	private void _InitUIControls()
	{
		m_LabelBuff = GameObject.Find("UI Root/LabelBuff").GetComponent<UILabel>();
		m_LabelTime = GameObject.Find("UI Root/LabelTime").GetComponent<UILabel>();
		m_ButtonMoney = GameObject.Find("UI Root/Button_Money").GetComponent<UIButton>();

		_HideUIControls ();
	}

	private void _HideUIControls()
	{
		m_LabelBuff.gameObject.SetActive (false);
		m_LabelTime.gameObject.SetActive (false);
		m_ButtonMoney.gameObject.SetActive (false);
	}

	private void _ShowButtonMoney()
	{
		_HideUIControls ();
		m_ButtonMoney.gameObject.SetActive (true);
	}

	private void _ShowBuffLabels()
	{
		_HideUIControls ();

		m_LabelBuff.gameObject.SetActive (true);
		m_LabelTime.gameObject.SetActive (true);

		_UpdateLabelBuff ();
		_UpdateLabelTime ();
	}

	private void _InitUIControls2()
	{
		//m_LabelBuff = GameObject.Find("UI Root/LabelBuff").GetComponent<UILabel>();
		//m_LabelTime = GameObject.Find("UI Root/LabelTime").GetComponent<UILabel>();
		m_ButtonBack = GameObject.Find ("UI Root/ButtonBack").GetComponent<UIButton>();
		m_ButtonMask = GameObject.Find ("UI Root/ButtonMask").GetComponent<UIButton>();

		m_aCorner = new UISprite[4];
		for(int i = 0; i < 4; i++)
		{
			m_aCorner [i] = GameObject.Find ("UI Root/corner/sp" + i.ToString()).GetComponent<UISprite>();
		}

		m_SpriteSuccess = GameObject.Find ("UI Root/spSuccess").GetComponent<UISprite>();

		_HideUIControls2 ();
	}

	private void _HideUIControls2()
	{
		//m_LabelBuff.gameObject.SetActive (false);
		//m_LabelTime.gameObject.SetActive (false);
		m_ButtonMask.enabled = false;
		m_SpriteSuccess.gameObject.SetActive (false);
	}

	private void _ShowBuffLabels2()
	{
		_HideUIControls2 ();

		//m_LabelBuff.gameObject.SetActive (true);
		//m_LabelTime.gameObject.SetActive (true);
		m_ButtonMask.enabled = true;

		for(int i = 0; i < 4; i++)
		{
			m_aCorner [i].spriteName = "scan2";
		}

		m_SpriteSuccess.gameObject.SetActive (true);

		//_UpdateLabelBuff2 ();
	}

	private void _UpdateLabelBuff2()
	{
		//m_LabelBuff.text = "ATTACK " + m_nPowerRate.ToString () + "%";
		//int nMinute = (int)(m_fDuration / 60);
		//int nSecond = ((int)m_fDuration) % 60;

		//m_LabelTime.text = nMinute.ToString("00") + ":" + nSecond.ToString("00");
	}

	private void _UpdateLabelBuff()
	{
		m_LabelBuff.text = "ATTACK " + m_nPowerRate.ToString () + "%";
	}

	private void _UpdateLabelTime()
	{
		TimeSpan ts = DateTime.Now - m_BeginTime;
		float fLeftTime = (float)(m_fDuration - ts.TotalSeconds);

		int nMinute = (int)(fLeftTime / 60);
		int nSecond = ((int)fLeftTime) % 60;

		m_LabelTime.text = "DURATION " + nMinute.ToString("00") + ":" + nSecond.ToString("00");
	}

	public void SaveBuff()
	{
		string strFullFileName = Application.persistentDataPath + "/" + File_Name;
		if (File.Exists (strFullFileName)) {
			File.Delete (strFullFileName);
			return;
		}

		FileStream fs = new FileStream (strFullFileName, FileMode.Create);
		StreamWriter sw = new StreamWriter (fs, Encoding.UTF8);
		string line = "buffID=" + m_nBuffID.ToString ();
		sw.WriteLine (line);

		if (m_nBuffID == Invalid_Buff_ID) {
			sw.Close ();
			return;
		}

		line = "powerRate=" + m_nPowerRate.ToString ();
		sw.WriteLine (line);

		line = "duration=" + m_fDuration.ToString ();
		sw.WriteLine (line);

		line = "beginTime=" + m_BeginTime.ToString ("yyyy-MM-dd HH:mm:ss");
		sw.WriteLine (line);

		sw.Close ();
		fs.Close ();
	}

	private void _ReadBuffFile()
	{
		// read file 
		string strFullFileName = Application.persistentDataPath + "/" + File_Name;
		if (!File.Exists (strFullFileName)) {
			return;
		}

		StreamReader sr = new StreamReader (strFullFileName, Encoding.UTF8);
		String line;

		line = sr.ReadLine ();
		string [] aStr = line.Split ('=');
		m_nBuffID = Int32.Parse (aStr [1]);

		if (m_nBuffID == Invalid_Buff_ID) {
			sr.Close ();
			return;
		}

		// nPowerRate
		line = sr.ReadLine ();
		aStr = line.Split ('=');
		m_nPowerRate = Int32.Parse (aStr[1]);

		// fDuration
		line = sr.ReadLine ();
		aStr = line.Split ('=');
		m_fDuration = float.Parse (aStr[1]);

		// beginTime and leftTime
		line = sr.ReadLine ();
		aStr = line.Split ('=');

		m_BeginTime = DateTime.Parse (aStr[1]);
		TimeSpan ts = DateTime.Now - m_BeginTime;
		float fLeftTime = (float)(m_fDuration - ts.TotalSeconds);

		if (fLeftTime < 0) {
			_ClearBuff ();
		}

		sr.Close ();
	}
}
