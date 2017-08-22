using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Script.Table.Ghost;

public class DragonAirGhostManager : MonoBehaviour
{
    public List<DragonGhost> m_Ghostlist = new List<DragonGhost>(30);

    public DragonGhost  GetGhost( stGhostItem item )
    {
        for( int i = 0; i < m_Ghostlist.Count; ++i )
        {
            if ( m_Ghostlist[i].gameObject.name == item.m_prefabname )
            {
				m_Ghostlist [i].m_nGhostID = item.m_nId;
                m_Ghostlist[i].m_name = item.m_name;
                m_Ghostlist[i].m_rating = item.m_rating;
                m_Ghostlist[i].m_spawnrate = item.m_spawnrate;
                m_Ghostlist[i].m_power = item.m_power;
                m_Ghostlist[i].m_attacktime = item.m_attacktime;
                return m_Ghostlist[i];
            }
        }
        return m_Ghostlist[0];
    }
}
