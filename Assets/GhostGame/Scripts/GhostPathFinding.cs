using UnityEngine;
using System.Collections;

public class GhostPathFinding : MonoBehaviour
{
	public Transform[]			m_MoveTarget = new Transform[4];

	public float				m_speed;

	public float				m_rotspeed;

	public Transform 			m_curTarget;

	// Use this for initialization
	void Start ()
	{
		int id = Random.Range (0, 4);
		m_curTarget = m_MoveTarget [id];
	}
	
	// Update is called once per frame
	void Update ()
	{
		float dis = Vector3.SqrMagnitude (m_curTarget.position - transform.position);
		if (dis < 0.5f) 
		{
			int id = Random.Range (0, 4);
			m_curTarget = m_MoveTarget [id];
		}

		transform.position = Vector3.MoveTowards (transform.position, m_curTarget.position, m_speed * Time.deltaTime);

		Vector3 dir = m_curTarget.position - transform.position;
		dir.Normalize ();

		Vector3 ghostdir = transform.right;
		transform.right = Vector3.RotateTowards (ghostdir, dir, m_rotspeed * Time.deltaTime, 0.5f );

	}
}
