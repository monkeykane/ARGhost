using UnityEngine;
using System.Collections;

public class DragonGhost : MonoBehaviour
{
    public enum EGhostState
    {
        EGS_HIDDEN,
        EGS_CATCHABLE,
        EGS_CATCHED,
        EGS_ATTACK,
        EGS_MISS,
    }


    public enum EGhostType
    {
        EGT_MASK,
        EGT_AIR,
        EGT_SLAM,

    }
    public EGhostType                       m_ghostType = EGhostType.EGT_MASK;
    public EGhostState                      m_ghostState = EGhostState.EGS_HIDDEN;


	public float							m_extent;
	public float							m_curExtent;

	public float 							m_ghostDelayTime;
	public float 							m_ghostShowTime;
	public float 							m_ghostFlyTime;
	public float 							m_ghostAttackTime;
	public float							m_ghostTotalCatchTime;

	public Animator 						m_Animator;

    public Transform                        m_GhostMoveTarget;
    public Transform                        m_GhostSpawnLocator;

    public IEnumerator                      m_SpawnCorotine;


    // new Paramaters
    #region
	public int 								m_nGhostID;
    public string                           m_name;
    public string                           m_rating;
    public int                              m_spawnrate;
    public int                              m_power;
    public float                            m_attacktime;
    #endregion 



    public void Reset()
    {
        StopAllCoroutines();
        m_SpawnCorotine = null;
        m_curExtent = 0;
        transform.localScale = new Vector3(m_curExtent, m_curExtent, m_curExtent);
        transform.localPosition = m_GhostSpawnLocator.localPosition;
    }

    public void BeginSpawn()
    {
        if (m_SpawnCorotine == null)
        {
            m_SpawnCorotine = SpawnObject();
            StartCoroutine(m_SpawnCorotine);
        }
    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(m_ghostDelayTime);

        float showspeed = Vector3.SqrMagnitude(m_GhostMoveTarget.position - m_GhostSpawnLocator.position) / m_ghostShowTime;
        while (m_curExtent < m_extent )
        {
            MoveGhost(showspeed);
            ExpendGhost(showspeed);

            yield return new WaitForSeconds(Time.deltaTime);
        }
        m_ghostState = DragonGhost.EGhostState.EGS_CATCHABLE;
        yield break;
    }


    public void MoveGhost(float speed)
    {
        Vector3 pos = transform.localPosition;
        pos = Vector3.MoveTowards(pos, m_GhostMoveTarget.localPosition, speed * Time.deltaTime);
        transform.localPosition = pos;
    }

    public void ExpendGhost(float speed)
    {
        Vector3 localscale = transform.localScale;
        m_curExtent = Mathf.MoveTowards(m_curExtent, m_extent, speed * Time.deltaTime);
        localscale.x = m_curExtent;
        localscale.y = m_curExtent;
        localscale.z = m_curExtent;
        transform.localScale = localscale;
    }
}
