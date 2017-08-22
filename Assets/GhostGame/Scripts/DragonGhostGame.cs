using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Script.Table;
using Script.Table.Ghost;

public class DragonGhostGame : MonoBehaviour
{
    // Singleton instance
    static DragonGhostGame          _self;    // [ALREADY REVIEWED] [WK COM]
    public static DragonGhostGame   Instance
    {
        get
        {
            return _self;
        }
    }


    public enum     EDragonGhostGameState
    {
        EDGGS_PRE,
        EDGGS_READY,
        EDGGS_PLAY,
        EDGGS_END,
		EDGGS_FINISH,
    };

    public EDragonGhostGameState m_GameState = EDragonGhostGameState.EDGGS_READY;
    // Current game state
    public EDragonGhostGameState CurGameState
    {
        get
        {
            return m_GameState;
        }
        set
        {
            SetGameState(value);
        }
    }

    public enum EPlayerState
    {
        EPS_LIVE,
        EPS_DEAD,
    };
    public EPlayerState m_PlayerState = EPlayerState.EPS_LIVE;


    public int                              m_NumGhostInGame;
    public List<DragonGhost.EGhostType>     m_GhostList;
    public List<DragonGhost.EGhostState>    m_GhostStateList;
    //public List<Material>                   m_GhostMaterialList;

    //public List<UI2DSprite>                 m_GhostPhotoLists = new List<UI2DSprite>(4);
    //public UI2DSprite m_masklable;
    

    public int m_curGhostIndex;
    public float m_TotalGameTime = 60;
    public float m_curGameTime;
    public int m_WaitingTime = 5;
    int _timeM, _timeS, _timeMS;

    //public DragonGhost                      m_Ghost;
	//public DragonMaskHold 					m_MaskHolder;

    public Transform                        m_ImageTarget;

    //public Transform                        m_GhostPlaceHolder;

	public Transform 						m_AirFlyTargetPosition;
    public Transform                        m_maskFlyTargetPosition;
   



    public int                              m_SpawnGhostRate = 0;
    public int                              m_defaultGhostRate = 10;
    public int                              m_AddSpawnGhostRate;

   
    
	public float 							m_PreSpawnGhostEffectTime;
	public Transform 						m_PreSpawnGhostEffect;

    public float                            m_curCatchGhostTime;
    public float                            m_curFlyTime;
   
    public Renderer                         m_RenderTarget;  
    
	public GameObject 						m_HiARCamera;
    public Camera                           m_3DCamera;




    //public Transform                        m_ghostsource;

	public ImageEffect_BrokenScreen 		m_ImageBroken;

	GameObject								m_camerago;
	GameObject								m_cameramain;

	public GameObject 						m_ButtonObj;
    public UISprite m_BtnWaiting;
    
	public UILabel 							m_TimeLabel, m_GameTimeLabel, m_GameTimeMSLabel;
    public UILabel                          m_Tip;
    //const string m_tipmask = "Searching Ghost...";
    //const string m_tipair = "In The Air...";
    //const string m_tipwait = "Waiting...";
	public UILabel							m_Res_S, m_Res_F;
    public UILabel m_GhostName;

    public UISprite m_PowerSlide;
    public UISprite m_PowerSliderBG;
    public GameObject m_WarningObj;

    public GameObject m_UIRoot;

    //const string m_attacktips = "WHAT A PITY!!!";
    //const string m_misstips = "ONE MORE TRY!!!";

    public List<string> m_tipslist = new List<string>(6);   // mask, air, wait, lose, miss, win


	public GameObject 						m_ButtonRelay;

    public UISprite                         m_Arrow;

    IEnumerator                             m_globalCoroutine;

    public TweenScale                       m_heartBeat;
    public float m_heartNormal;
    public float m_heartMiddle;
    public float m_heartEmg;

    public UILabel m_HeartText;


    public enum EHeartState
    {
        EHS_None,
        EHS_Wait,
        EHS_Find,
        EHS_Dead,
    };

    public EHeartState m_HeartState = EHeartState.EHS_None;
    public EHeartState CurHeartState
    {
        get
        {
            return m_HeartState;
        }
        set
        {
            SetHeartState(value);
        }
    }

    public TextAsset m_textfiles;

    public const int MAX_GHOST_COUNT = 30;
    public int[] ghostID = new int[MAX_GHOST_COUNT];
    public int[] ghostused = new int[MAX_GHOST_COUNT];

    public float m_curCachePower;
    public float m_powerspeed = 1000;
    public float m_powerValue = 1;
    public bool m_bPressed = false;

    public AudioSource m_CleanerStart;
    public AudioSource m_CleanerLoop;

    private bool m_bGlobalPlay;

    void Awake()
    {
        _self = this;
        InitTable();
        SetPowerValue(DragonBuffManager.Instance().GetPowerValue());
    }

    void OnDestroy()
    {
        ClearUpTabel();
        _self = null;
    }

    void InitTable()
    {
		//GhostItemManager<stGhostItem>.Instance().LoadFile("ghostspawn", "Table/");
		//TableManager.Init ();
        _BuildAirGhostLists();
    }
    void ClearUpTabel()
    {
        //GhostItemManager<stGhostItem>.Instance().ClearUp();
    }

    void _BuildAirGhostLists()
    {
        for( int i = 0; i < MAX_GHOST_COUNT; ++i )
        {
            int key = 0;
            stGhostItem item = GhostItemManager<stGhostItem>.Instance().GetstItemByIndex(i, out key);
            if ( item != null )
            {
                ghostID[i] = key;
                if (i == 0)
                    ghostused[i] = item.m_spawnrate;
                else
                    ghostused[i] = item.m_spawnrate + ghostused[i - 1];
            }
        }
    }

    public int GetAirGhostID()
    {
        int rate = UnityEngine.Random.Range(1, 101);

        for (int i = 0; i < ghostused.Length; ++i)
        {
            if (rate < ghostused[i])
                return ghostID[i];
        }
        return ghostID[0];
    }

    public void InitGame()
    {
        m_cameramain = GameObject.Find("camera");
        StartCoroutine(TimeTick());
        CurGameState = EDragonGhostGameState.EDGGS_READY;
        // ghost type list
        m_GhostList = new List<DragonGhost.EGhostType>(m_NumGhostInGame);
        m_GhostStateList = new List<DragonGhost.EGhostState>(m_NumGhostInGame);
        //m_GhostMaterialList = new List<Material>(m_NumGhostInGame);
        for ( int i = 0; i < m_NumGhostInGame; ++i )
        {
            int seed = Random.Range(52, 101);
            m_GhostList.Add(seed > 51 ? DragonGhost.EGhostType.EGT_AIR : DragonGhost.EGhostType.EGT_MASK);
            m_GhostStateList.Add(DragonGhost.EGhostState.EGS_HIDDEN);
            //m_GhostMaterialList.Add(new Material(m_GhostPhotoLists[i].shader));
        }

        _ParseTextConfigFile();

        //m_AirGhost = m_AirGhostManager.GetGhost
        _CreateAirGhost();
       
    }


    void _ParseTextConfigFile()
    {
        m_tipslist.Clear();
        if (m_textfiles != null && m_textfiles.text != "")
        {
            string ss = m_textfiles.text.ToString().Trim();
            int index = 0;
            int line = 1;
            int end = 0;
            string tempstr = string.Empty;
            while ( end > -1 )
            {
                end = ss.IndexOf("\r\n", index);
                if (end == -1)
                {
                    tempstr = ss.Substring(index, ss.Length - index);
                }
                else
                {
                    tempstr = ss.Substring(index, end - index);
                }

                _ParseTextLine(tempstr);
                line++;

                index = end + 1;
            }
        }
    }

    void _ParseTextLine(string line )
    {
        int index = 0;
        int offset = 0;
        int length = line.Length;
        string content = string.Empty;
        offset = line.IndexOf("=", index);
        if ( offset != -1 )
        {
            offset += 1;
            content = line.Substring(offset, length - offset);
        }

        m_tipslist.Add(content);
    }

    void SetGameState(EDragonGhostGameState newstate)
    {
        if (m_GameState == newstate)
            return;
        m_GameState = newstate;
        if ( m_GameState == EDragonGhostGameState.EDGGS_READY)
        {
            m_Arrow.gameObject.SetActive(false);
            if ( !m_bGlobalPlay )
                HeartBeatNormal();
            m_Tip.gameObject.SetActive(false);
            StartCoroutine(WaitGamePlay());
            m_bGlobalPlay = true;
        }
        else if (m_GameState == EDragonGhostGameState.EDGGS_PLAY )
        {
            
            m_globalCoroutine = WaitingGhostSpawn();
            StartCoroutine(m_globalCoroutine);
        }
        else if ( m_GameState == EDragonGhostGameState.EDGGS_END )
        {
            StopAllCoroutines();

            StopSFX();

            m_Arrow.gameObject.SetActive(false);

            m_Tip.gameObject.SetActive (false);

            m_ButtonObj.SetActive (false);

            m_ButtonRelay.SetActive(true);

            m_PowerSliderBG.alpha = 0;
            m_BtnWaiting.alpha = 1;
        }
    }


    IEnumerator WaitingGhostSpawn()
    {
        // reset
        ResetGhostSpawnRate();
        DragonGhost.EGhostType type = m_GhostList[m_curGhostIndex];

        CurHeartState = EHeartState.EHS_Wait;

        int rad = Random.Range(0, 100);
        //m_Ghost.m_ghostState = DragonGhost.EGhostState.EGS_HIDDEN;
        m_AirGhost.m_ghostState = DragonGhost.EGhostState.EGS_HIDDEN;
        m_curCatchGhostTime = 0;
        m_Tip.gameObject.SetActive(true);
        m_Tip.text = m_tipslist.Count > 2 ? m_tipslist[2] : string.Empty; ;
        m_Arrow.gameObject.SetActive(false);
        while ( true )
        {
            if (m_SpawnGhostRate > rad)
            {
                m_SpawnGhostRate = 100;
                //if (type == DragonGhost.EGhostType.EGT_MASK)
                //{
                //    StartCoroutine(SpawnMaskGhost());
                //}
                //else if (type == DragonGhost.EGhostType.EGT_AIR)
                    StartCoroutine(SpawnAirGhost());
                yield break;
            }
            else
            {
                AddSpawnGhostRate();
                if ( m_SpawnGhostRate > rad )
                {
                    m_SpawnGhostRate = 100;
                    //if (type == DragonGhost.EGhostType.EGT_MASK)
                    //{
                    //    StartCoroutine(SpawnMaskGhost());
                    //}
                    //else if (type == DragonGhost.EGhostType.EGT_AIR)
                        StartCoroutine(SpawnAirGhost());
                    yield break;
                }
            }
            float time = Random.Range(3, 5);
            yield return new WaitForSeconds(time);
        }

        yield break;
    }

    IEnumerator TimeTick()
    {
       
        while ( m_curGameTime < m_TotalGameTime )
        {
            m_curGameTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CurGameState = EDragonGhostGameState.EDGGS_END;
        yield break;
    }


    void ResetGhostSpawnRate()
    {
        m_SpawnGhostRate = m_defaultGhostRate;
    }


	public void AdjustMoveGhost( DragonGhost ghost, float speed, Transform flytarget )
	{
		Vector3 pos = ghost.transform.localPosition;
		Quaternion rot = ghost.transform.localRotation;
		Quaternion target = flytarget.rotation;

		pos = Vector3.MoveTowards(pos, Vector3.zero, speed * Time.deltaTime );
		rot = Quaternion.Slerp (rot, Quaternion.identity, speed * Time.deltaTime);
        ghost.transform.localPosition = pos;
        ghost.transform.localRotation = rot;
	}

	//IEnumerator  SpawnMaskGhost()
 //   {
 //       //HeartBeatEmg();
 //       CurHeartState = EHeartState.EHS_Find;
 //       m_Tip.gameObject.SetActive(true);
 //       m_Tip.text = m_tipslist.Count > 0 ? m_tipslist[0] : string.Empty;

 //       m_AirGyroController.EnableGyro( false );

 //       if ( m_PreSpawnGhostEffect != null )
	//		m_PreSpawnGhostEffect.gameObject.SetActive (true);

	//	yield return new WaitForSeconds (m_PreSpawnGhostEffectTime);

 //       //StartCoroutine(MobileVibrate());
 //       StartCoroutine(SearchingMaskGhost());
 //       m_Ghost.m_ghostState = DragonGhost.EGhostState.EGS_CATCHABLE;
 //       yield break;
 //   }

    void CatchedGhost( DragonGhost ghost )
    {
        ghost.m_ghostState = DragonGhost.EGhostState.EGS_CATCHED;
        m_GhostStateList[m_curGhostIndex] = DragonGhost.EGhostState.EGS_CATCHED;
        m_curCatchGhostTime = ghost.m_attacktime - Time.deltaTime * 3;

		DragonAchievementManager.Instance ().OnCatchGhost (ghost.m_nGhostID);
    }

    IEnumerator WaitGamePlay()
    {
        if (!m_bGlobalPlay)
        {
            float time = m_WaitingTime;
            float oldTime = time;
            m_TimeLabel.text = time.ToString();
            m_TimeLabel.gameObject.SetActive(true);
            m_GameTimeLabel.gameObject.SetActive(false);
            m_GameTimeMSLabel.gameObject.SetActive(false);
            while (time > 0)
            {
                yield return new WaitForSeconds(1.0f);
                time -= 1;
                m_TimeLabel.text = time.ToString();
            }
            m_TimeLabel.text = "GO";
            yield return new WaitForSeconds(1.0f);
            m_curGameTime -= oldTime;
            m_curGameTime = Mathf.Clamp(m_curGameTime, 0, m_TotalGameTime);
        }
        CurGameState = EDragonGhostGameState.EDGGS_PLAY;
		m_TimeLabel.gameObject.SetActive (false);
        m_GameTimeLabel.gameObject.SetActive(true);
        m_GameTimeMSLabel.gameObject.SetActive(true);

        yield break;
    }


    //IEnumerator SearchingMaskGhost()
    //{
    //    while (m_curCatchGhostTime < m_Ghost.m_ghostTotalCatchTime )
    //    {
    //        yield return new WaitForSeconds(Time.deltaTime);
    //        m_curCatchGhostTime += Time.deltaTime;
    //    }
    //    m_Tip.gameObject.SetActive(false);
    //    if (m_Ghost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHABLE )
    //    {
    //        if (m_GhostPlaceHolder.gameObject.activeSelf) // attack 
    //        {
    //            m_Ghost.m_ghostState = DragonGhost.EGhostState.EGS_ATTACK;
    //            m_Ghost.transform.parent = null;

    //            if (m_Ghost.transform.parent == null)
    //            {
    //                m_Ghost.m_Animator.SetBool("bFly", true);
    //                float speed = Vector3.SqrMagnitude(m_Ghost.transform.position - m_maskFlyTargetPosition.position) / m_Ghost.m_ghostFlyTime;
    //                while (m_curFlyTime < m_Ghost.m_ghostFlyTime)
    //                {
    //                    AdjustMoveGhost( m_Ghost, speed, m_maskFlyTargetPosition);
    //                    yield return new WaitForSeconds(Time.deltaTime);
    //                    m_curFlyTime += Time.deltaTime;

    //                }
    //                m_curFlyTime = 0;
    //                m_Ghost.m_Animator.SetBool("bAttack", true);
    //                yield return new WaitForSeconds(m_Ghost.m_ghostAttackTime);

    //                m_GhostStateList[m_curGhostIndex] = DragonGhost.EGhostState.EGS_ATTACK;

    //                CurGameState = EDragonGhostGameState.EDGGS_END;
    //                yield break;
    //            }
    //            yield break;
    //        }
    //        else // miss
    //        {
    //            m_Ghost.m_ghostState = DragonGhost.EGhostState.EGS_MISS;
    //            // reset spawn mask ghost
    //            m_GhostStateList[m_curGhostIndex] = DragonGhost.EGhostState.EGS_MISS;
    //            //if (Application.platform == RuntimePlatform.WindowsEditor)
    //            //{
    //            //    if (m_curGhostIndex < m_GhostPhotoLists.Count)
    //            //    {
    //            //        m_GhostPhotoLists[m_curGhostIndex].sprite2D = m_masklable.sprite2D;
    //            //    }
    //            //}
    //        }
    //    }
    //    else if (m_Ghost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHED )
    //    {
    //        yield return new WaitForSeconds(0.15f);
    //        CurGameState = EDragonGhostGameState.EDGGS_END;
    //        yield break;
    //    }

    //    m_Ghost.Reset();
    //    m_MaskHolder.Reset();

    //    ++m_curGhostIndex;
    //    if ( m_curGhostIndex < m_GhostList.Count )
    //    {
    //        CurGameState = EDragonGhostGameState.EDGGS_READY;
    //    }
    //    else
    //    {
    //        CurGameState = EDragonGhostGameState.EDGGS_END;
    //    }
    //    yield break;
    //}



    #region Air GPP

    public float m_AirRaidus;
    public DragonAirGhostManager m_AirGhostManager;
    DragonGhost m_AirGhost;
    public DragonGyro m_AirGyroController;

    void _CreateAirGhost()
    {
        int ghostid = GetAirGhostID();
        stGhostItem item = GhostItemManager<stGhostItem>.Instance().GetstItem(ghostid);
        if (item != null)
        {
            m_AirGhost = m_AirGhostManager.GetGhost(item);
        }
    }

    IEnumerator SpawnAirGhost()
    {
        //HeartBeatEmg();
        CurHeartState = EHeartState.EHS_Find;
        _CreateAirGhost();
        //m_AirGhost = m_AirGhostManager.GetGhost();


        m_Tip.gameObject.SetActive(true);
        m_Tip.text = m_tipslist.Count > 1 ? m_tipslist[1] : string.Empty; ;
        yield return new WaitForSeconds(0.5f);
        Vector3 pos = Random.insideUnitSphere * m_AirRaidus;
        m_AirGhost.transform.position = pos;
        Vector3 dir = pos.normalized;
        m_AirGhost.transform.rotation = Quaternion.LookRotation(-dir);
        m_AirGhost.gameObject.SetActive(true);
        m_AirGyroController.EnableGyro( true );

      
       
        {
            m_Arrow.gameObject.SetActive(true);
        }
        //StartCoroutine(MobileVibrate());
        StartCoroutine(SearchingAirGhost());
        m_AirGhost.m_ghostState = DragonGhost.EGhostState.EGS_CATCHABLE;

        yield break;
    }

    IEnumerator SearchingAirGhost()
    {
        while (m_curCatchGhostTime < m_AirGhost.m_attacktime)
        {
            if ( m_curCatchGhostTime + 1.5f > m_AirGhost.m_attacktime )
            {
                bool bshowwaring = false;
                SkinnedMeshRenderer mesh = m_AirGhost.GetComponentInChildren<SkinnedMeshRenderer>();
                if (mesh != null && mesh.isVisible)
                {
                    bshowwaring = true;
                }
                m_WarningObj.SetActive(bshowwaring);
            }
            Vector3 pos = m_AirGhost.transform.position;
            Vector3 localpos = m_3DCamera.transform.InverseTransformPoint(pos);
            Vector3 screenpos = m_3DCamera.WorldToScreenPoint(m_AirGhost.transform.position);
            screenpos.z = 0;

            if (localpos.z < 0)
            {
                screenpos.x = Screen.width - screenpos.x;
                screenpos.y = Screen.height - screenpos.y;
            }

            Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 icondir = (screenpos - center).normalized;
            //Debug.DrawRay(Vector3.zero, icondir, Color.blue, 10000);

            m_Arrow.transform.up = icondir;

            if (m_AirGhost.gameObject.activeSelf == true)
            {
                // SkinnedMeshRenderer mesh = m_AirGhost.GetComponentInChildren<SkinnedMeshRenderer>();
                //if (mesh != null && mesh.isVisible)
                m_Arrow.gameObject.SetActive(!m_ButtonObj.activeSelf);
            }


            yield return new WaitForSeconds(Time.deltaTime);
            m_curCatchGhostTime += Time.deltaTime;
        }
        m_Tip.gameObject.SetActive(false);
        m_ButtonObj.SetActive(false);
        m_BtnWaiting.alpha = 1;
        m_PowerSliderBG.alpha = 0;
        if (m_AirGhost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHABLE)
        {
            bool bAttack = false;

            if ( m_AirGhost.gameObject.activeSelf == true )
            {
                SkinnedMeshRenderer mesh = m_AirGhost.GetComponentInChildren<SkinnedMeshRenderer>();
                if ( mesh != null && mesh.isVisible )
                {
                    bAttack = true;
                    m_Arrow.gameObject.SetActive(false);
                }
            }

            if ( bAttack )
            {
                StopSFX();
                m_WarningObj.SetActive(false);
                m_AirGhost.m_ghostState = DragonGhost.EGhostState.EGS_ATTACK;
                m_AirGhost.m_Animator.SetBool("bFly", true);
                m_AirGhost.transform.parent = m_AirFlyTargetPosition;
                float speed = Vector3.SqrMagnitude(m_AirGhost.transform.position - m_AirFlyTargetPosition.position) / m_AirGhost.m_ghostFlyTime;
                while (m_curFlyTime < m_AirGhost.m_ghostFlyTime)
                {
                    AdjustMoveGhost(m_AirGhost, speed, m_AirFlyTargetPosition);
                    yield return new WaitForSeconds(Time.deltaTime);
                    m_curFlyTime += Time.deltaTime;

                }
                m_curFlyTime = 0;
                m_AirGhost.m_Animator.SetBool("bAttack", true);
                yield return new WaitForSeconds(m_AirGhost.m_ghostAttackTime);
                m_GhostStateList[m_curGhostIndex] = DragonGhost.EGhostState.EGS_ATTACK;
                CurGameState = EDragonGhostGameState.EDGGS_END;
                m_WarningObj.SetActive(false);
                yield break;

            }
            else // miss
            {
                m_WarningObj.SetActive(false);
                m_AirGhost.m_ghostState = DragonGhost.EGhostState.EGS_MISS;
                // reset spawn mask ghost
                //m_GhostStateList[m_curGhostIndex] = DragonGhost.EGhostState.EGS_MISS;
                //if (m_curGhostIndex < m_GhostPhotoLists.Count)
                //{
                //    m_GhostPhotoLists[m_curGhostIndex].sprite2D = m_masklable.sprite2D;
                //}
            }
        }
        else if (m_AirGhost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHED)
        {
            m_WarningObj.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            CurGameState = EDragonGhostGameState.EDGGS_END;
            yield break;
        }
        m_AirGhost.gameObject.SetActive(false);

        ++m_curGhostIndex;
        if (m_curGhostIndex < m_GhostList.Count)
        {
            CurGameState = EDragonGhostGameState.EDGGS_READY;
        }
        else
        {
            CurGameState = EDragonGhostGameState.EDGGS_END;
        }
        yield break;
    }
    #endregion Air GPP


    IEnumerator	MobileVibrate()
	{
		while (CurGameState == EDragonGhostGameState.EDGGS_PLAY) 
		{
			Handheld.Vibrate ();
			yield return new WaitForSeconds (Random.Range (2.3f, 3.7f));
		}
		yield break; 
	}


    void AddSpawnGhostRate()
    {
        m_SpawnGhostRate += m_AddSpawnGhostRate;
        m_SpawnGhostRate = Mathf.Min(m_SpawnGhostRate, 100);
    }


	// Use this for initialization
	void Start ()
    {
        InitGame();
	}

    void OnGUI()
    {
        float localTime = m_TotalGameTime - m_curGameTime;
        _timeM = (int)localTime / 60;
        _timeS = (int)localTime;
        _timeMS = (int)((localTime - _timeS) * 100);
        _timeMS = Mathf.Clamp(_timeMS, 0, 100);
        m_GameTimeLabel.text = string.Format("{0:D1}:{1:00}", _timeM.ToString(), _timeS.ToString() );   
        m_GameTimeMSLabel.text = string.Format("{0:00}", _timeMS.ToString());
        if ( CurGameState == EDragonGhostGameState.EDGGS_READY )
        {
        }

        else if (CurGameState == EDragonGhostGameState.EDGGS_PLAY)
        {
            //if (m_curGhostIndex < m_GhostList.Count && m_GhostList[m_curGhostIndex] == DragonGhost.EGhostType.EGT_MASK)
            //{
            //    if (m_GhostPlaceHolder.gameObject.activeSelf && m_Ghost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHABLE)
            //    {
            //        if (m_ButtonObj.activeSelf == false)
            //        {
            //            m_ButtonObj.SetActive(true);
            //            m_PowerSliderBG.alpha = 1;
            //            m_BtnWaiting.alpha = 0;
            //            m_Arrow.alpha = 0;
            //        }
            //    }
            //    else
            //    {
            //        m_ButtonObj.SetActive(false);
            //        m_PowerSliderBG.alpha = 0;
            //        m_BtnWaiting.alpha = 1;
            //        m_Arrow.alpha = 1;
            //    }
            //}
            //else 
            if ( m_AirGhost.gameObject.activeSelf &&  m_curGhostIndex < m_GhostList.Count && m_GhostList[m_curGhostIndex] == DragonGhost.EGhostType.EGT_AIR )
            {
                if ( m_cameramain == null )
                {
                    m_cameramain = GameObject.Find("camera");
                }

                Ray ray = new Ray( m_cameramain.transform.position, m_cameramain.transform.forward );
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 20);
                if ( hit.transform != null && hit.transform == m_AirGhost.transform && m_AirGhost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHABLE)
                {
                    m_ButtonObj.SetActive(true);
                    m_PowerSliderBG.alpha = 1;
                    m_BtnWaiting.alpha = 0;
                }
                else
                {
                    StopSFX();
                    m_ButtonObj.SetActive(false);
                    m_PowerSliderBG.alpha = 0;
                    m_BtnWaiting.alpha = 1;
                }
                
            }
        }

		else if (CurGameState == EDragonGhostGameState.EDGGS_END )
		{
            bool bCatchGhost = false;
            bool bAttack = false;
            for( int i = 0; i < m_GhostStateList.Count; ++i )
            {
                if ( m_GhostStateList[i] == DragonGhost.EGhostState.EGS_CATCHED )
                {
                    bCatchGhost = true;
                    continue;
                }
                if ( m_GhostStateList[i] == DragonGhost.EGhostState.EGS_ATTACK )
                {
                    bAttack = true;
                    break;
                }
            }
			if (bCatchGhost && !bAttack )
			{
                if (m_Res_S.gameObject.activeSelf == false)
                {
                    m_GhostName.text = m_AirGhost.m_name;
                    m_Res_S.text = m_tipslist.Count > 5 ? m_tipslist[5] : string.Empty;
                    m_Res_S.gameObject.SetActive(true);                
                }
			} 
			else
			{
                if (m_Res_F.gameObject.activeSelf == false)
                {
                    m_Res_F.text = bAttack ? (m_tipslist.Count > 3 ? m_tipslist[3] : string.Empty) : (m_tipslist.Count > 4 ? m_tipslist[4] : string.Empty);
                    m_Res_F.gameObject.SetActive(true);
                }
			}
		}
    }

    //public void OnEnableMaskGhost()
    //{
    //    if (CurGameState != EDragonGhostGameState.EDGGS_PLAY)
    //        return;
    //    if (m_GhostList[m_curGhostIndex] != DragonGhost.EGhostType.EGT_MASK)
    //        return;


    //    m_MaskHolder.BeginSpawn();
    //    m_Ghost.BeginSpawn();

    //    if (m_SpawnGhostRate == 100)
    //        return;
    //    m_SpawnGhostRate = 100;
  
    //    if (m_globalCoroutine != null)
    //    {
    //        StopCoroutine(m_globalCoroutine);
    //        m_globalCoroutine = null;
    //    }
    //    StartCoroutine( SpawnMaskGhost() );

    //}

    #region Capture
    public void OnCaptureScreen()
    {
        if ( Application.platform == RuntimePlatform.WindowsEditor )
        {
            //Application.CaptureScreenshot(Application.dataPath + "/catch_ghost.png");
            StartCoroutine(GetCapture());
        }
        else if ( Application.platform == RuntimePlatform.Android )
        {
            StartCoroutine(GetCapture());
            StartCoroutine(SaveCapture());
        }
    }

    IEnumerator GetCapture()

    {
        m_UIRoot.SetActive(false);

        yield return new WaitForEndOfFrame();

        int width = Screen.width;

        int height = Screen.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0, true);

        byte[] imagebytes = tex.EncodeToPNG();

        //tex.Compress(false);
        tex.Apply();

        m_RenderTarget.material.mainTexture = tex;
        //if (m_curGhostIndex < m_GhostPhotoLists.Count)
        //{ 
        //    //Material mat = new Material(m_GhostPhotoLists[m_curGhostIndex].shader);
        //    m_GhostMaterialList[m_curGhostIndex].mainTexture = tex;
        //    m_GhostPhotoLists[m_curGhostIndex].material = m_GhostMaterialList[m_curGhostIndex];
        //}
        m_HiARCamera.SetActive(false);
        if (m_camerago == null)
        {
            m_camerago = GameObject.Find("camera background");
        }
        if (m_camerago != null)
        {
            m_camerago.SetActive(false);
        }

        

        File.WriteAllBytes(Application.dataPath + "/screencapture.png", imagebytes);

        

        CurGameState = EDragonGhostGameState.EDGGS_END;

        yield break;

    }

    IEnumerator SaveCapture()
    {
        Application.CaptureScreenshot("catch_ghost.png");
        while (!IsFileExist("catch_ghost.png"))
        {
            yield return new WaitForSeconds(0.05f);
        }

		//yield return new WaitForSeconds( 0.4f );
		//m_ButtonRelay.SetActive (true);
        yield break;
    }

    public static string GetDefaultFilePath()
    {
        return Application.persistentDataPath + "/";
    }

    public static bool IsFileExist(string name)
    {
        return IsFileExistByPath(GetDefaultFilePath() + name); ;
    }

    public static bool IsFileExistByPath(string path)
    {
        FileInfo info = new FileInfo(path);
        if (info == null || info.Exists == false)
        {
            return false;
        };
        return true;
    }
    #endregion Capture


	public void OnGhostAttack()
	{
		if (m_cameramain == null) 
		{
			m_cameramain = GameObject.Find ("camera");
		}

        if (m_cameramain != null && m_cameramain.GetComponent<ImageEffect_BrokenScreen>() == null )
        {
            ImageEffect_BrokenScreen comp = m_cameramain.AddComponent<ImageEffect_BrokenScreen>();
            comp.BumpMap = m_ImageBroken.BumpMap;

        }

        CurHeartState = EHeartState.EHS_Dead;
    }


	public void OnClickButton()
	{
        CatchedGhost(m_AirGhost);//m_GhostList[m_curGhostIndex] == DragonGhost.EGhostType.EGT_AIR? m_AirGhost : m_Ghost );
		OnCaptureScreen();
    }

	public void OnRelpayGame()
	{
		Application.LoadLevel (0);
	}


    public void HeartBeatNormal()
    {
        m_heartBeat.duration = m_heartNormal;
    }

    public void HeartBeatEmg()
    {
        m_heartBeat.duration = m_heartEmg;
    }

    public void HeartBeatFind()
    {
        m_heartBeat.duration = m_heartMiddle;
    }

    public void HeartBeatDead()
    {
        m_heartBeat.style = UITweener.Style.Once;
    }

    IEnumerator WaitHeartBeatNumber()
    {
        while( m_HeartState == EHeartState.EHS_Wait )
        {
            m_HeartText.text = Random.Range(75, 82).ToString();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
        }
        yield break;
    }

    IEnumerator FindHeartBeatNumber()
    {
        int basehartnum = 120;
        while ( m_HeartState == EHeartState.EHS_Find )
        {
            if (m_GhostList[m_curGhostIndex] == DragonGhost.EGhostType.EGT_MASK )
            {
                m_HeartText.text = Random.Range(90, 100).ToString();
                yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
            }
            else if (m_GhostList[m_curGhostIndex] == DragonGhost.EGhostType.EGT_AIR  )
            {
                SkinnedMeshRenderer mesh = m_AirGhost.GetComponentInChildren<SkinnedMeshRenderer>();
                if (mesh != null && mesh.isVisible)
                {
                    HeartBeatEmg();
                    basehartnum++;
                    m_HeartText.text = basehartnum.ToString();
                    yield return new WaitForSeconds(1.0f);
                }
                else
                {
                    HeartBeatFind();
                    Vector3 forward = m_3DCamera.gameObject.transform.forward;
                    Vector3 dir = m_AirGhost.transform.position.normalized;
                    float cx = Vector3.Dot(forward, dir);
                    int heart = (int)(90 + cx * (150 - 90));
                    heart = Mathf.Max(heart, 90);
                    m_HeartText.text = heart.ToString();
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        yield break;
    }

    void DeadHeartBeatNumber()
    {
        HeartBeatDead();
        m_HeartText.text = 0.ToString();
    }
    void SetHeartState( EHeartState value )
    {
        if (value == m_HeartState)
            return;
        m_HeartState = value;
        switch (value)
        {
            case EHeartState.EHS_None:
                m_HeartText.text = 75.ToString();
                break;
            case EHeartState.EHS_Wait:
                HeartBeatNormal();
                StartCoroutine("WaitHeartBeatNumber");
                break;
            case EHeartState.EHS_Find:
                StartCoroutine("FindHeartBeatNumber");
                break;
            case EHeartState.EHS_Dead:
                DeadHeartBeatNumber();
                break;
        }
    }

    public void StopSFX()
    {
        m_CleanerLoop.Stop();
        m_CleanerStart.Stop();
    }

    public void OnCatchPressed()
    {
        m_bPressed = true;
        m_CleanerLoop.Stop();
        m_CleanerStart.Play();
        Invoke("PlayLoop", m_CleanerLoop.time);
    }


    public void OnCatchRelease()
    {
        m_bPressed = false;
        StopSFX();
    }

    public void PlayLoop()
    {
        if (m_bPressed)
        {
            m_CleanerLoop.loop = true;
            m_CleanerLoop.Play();
        }
    }



    void Update()
    {
        if ( m_AirGhost == null || m_AirGhost.m_ghostState == DragonGhost.EGhostState.EGS_CATCHED )
        {
            m_bPressed = false;
        }

        if (m_ButtonObj == null || m_ButtonObj.activeSelf == false )
        {
            m_curCachePower = 0;
            m_BtnWaiting.alpha = 1;
            m_bPressed = false;
        }


        if (m_bPressed)
        {
            m_curCachePower += m_powerspeed * Time.deltaTime * m_powerValue;
            if (m_curCachePower >= m_AirGhost.m_power )
            {
                CatchedGhost(m_AirGhost);
                StopSFX();
                OnCaptureScreen();
                m_curCachePower = m_AirGhost.m_power;
            }
        }
        else
            m_curCachePower = 0;

        m_PowerSlide.fillAmount = m_curCachePower / m_AirGhost.m_power;
    }


    public void SetPowerValue( float value )
    {
        m_powerValue = value;
    }
}
