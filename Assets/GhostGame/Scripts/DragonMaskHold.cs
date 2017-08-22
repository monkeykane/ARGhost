using UnityEngine;
using System.Collections;

public class DragonMaskHold : MonoBehaviour
{
	public float							m_extent;
	public float							m_curExtent;

	public float 							m_ghostMaskShowTime;

	public Transform						m_ghostMaskEffect;

    public IEnumerator                      m_SpawnCorotine;

    public void Reset()
    {
        StopAllCoroutines();
        m_SpawnCorotine = null;
        m_curExtent = 0;
        transform.localScale = new Vector3( m_curExtent, m_curExtent, m_curExtent );
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
        float speed = (m_extent - m_curExtent) / m_ghostMaskShowTime;

        if (m_ghostMaskEffect != null)
            m_ghostMaskEffect.gameObject.SetActive(true);

        while (m_curExtent < m_extent)
        {
            ExpendMaskHold(speed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield break;
    }

    public void ExpendMaskHold(float speed)
    {
        Vector3 localscale = transform.localScale;
        m_curExtent = Mathf.MoveTowards(m_curExtent, m_extent, speed * Time.deltaTime);
        localscale.x = m_curExtent;
        localscale.y = m_curExtent;
        localscale.z = m_curExtent;
        transform.localScale = localscale;
    }


}
