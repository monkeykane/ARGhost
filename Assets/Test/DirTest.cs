using UnityEngine;
using System.Collections;

public class DirTest : MonoBehaviour
{

    public Transform m_Target;
    public UISprite m_Arrow;
    public Camera m_3DCamera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pos = m_Target.position;

        Vector3 localpos = m_3DCamera.transform.InverseTransformPoint(pos);

        //pos.z = 0.01f;
        Vector3 screenpos = m_3DCamera.WorldToScreenPoint(pos);

        if ( localpos.z < 0 )
        {
            screenpos.x = Screen.width - screenpos.x;
            screenpos.y = Screen.height - screenpos.y;
        }



        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 icondir = (screenpos - center).normalized;
        //Debug.DrawRay(Vector3.zero, icondir, Color.blue, 10000);

        m_Arrow.transform.up = icondir;
    }
}
