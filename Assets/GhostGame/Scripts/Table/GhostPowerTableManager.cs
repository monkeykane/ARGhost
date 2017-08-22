using System;

namespace Script.Table
{
	public class GhostPowerData : BaseData
	{
		public const int Max_Num = 5;
		
		public float m_fDuration;
		public int[] m_aPowerRates;
		public int[] m_aProbabilitys;
		public int m_nNum;

		public GhostPowerData()
		{
			m_nNum = 0;
			m_aPowerRates = new int[Max_Num];
			m_aProbabilitys = new int[Max_Num];
		}
		
		public override void LoadData(int nRowIndex,DBFile fileData)
		{
			int nIndex = 0;

			m_nId = fileData.getInt (nIndex); nIndex++;
			m_fDuration = fileData.getFloat (nIndex); nIndex++;

			int nTotalProbablity = 0;

			for (int i = 0; i < Max_Num; i++) 
			{
				m_aPowerRates [i] = fileData.getInt (nIndex); nIndex++;
				int nProbablity = fileData.getInt (nIndex); nIndex++;
				if (nProbablity == 0) 
				{
					break;
				}

				nTotalProbablity += nProbablity;
				m_aProbabilitys [i] = nTotalProbablity;
			}
		}

		public int GetPowerRate()
		{
			int nProbability = UnityEngine.Random.Range (0, 100);

			for (int i = 0; i < Max_Num; i++) 
			{
				if (nProbability < m_aProbabilitys [i])
					return m_aPowerRates [i];
			}

			return 100;
		}
	}

	public class GhostPowerTableManager : BaseDataManager
	{
		private static GhostPowerTableManager s_instance;
		public static GhostPowerTableManager Instance()
		{
			if (s_instance == null) 
			{
				s_instance = new GhostPowerTableManager();
			}
			return s_instance;
		}

		protected override BaseData NewItem()
		{
			return new GhostPowerData();
		}

		public GhostPowerData GetGhostPowerData(int nID)
		{
			return (GhostPowerData)m_DataMap [nID];
		}

		public void ClearUp()
		{
			s_instance = null;
		}
	}
}

