using UnityEngine;
using System.Collections;

namespace Script.Table
{
	public class AchievementData : BaseData
	{
		public int m_nGhostID;
		public string m_strIcon;
		public string m_strBackgroundIcon;

		public AchievementData()
		{
			
		}

		public override void LoadData(int nRowIndex,DBFile fileData)
		{
			int nIndex = 0;

			m_nId = fileData.getInt (nIndex); nIndex++;
			m_nGhostID = fileData.getInt (nIndex); nIndex++;
			m_strIcon = fileData.getString (nIndex); nIndex++;
			m_strBackgroundIcon = fileData.getString (nIndex); nIndex++;
		}
	}

	public class AchievementTableManager : BaseDataManager
	{
		private static AchievementTableManager s_instance;
		public static AchievementTableManager Instance()
		{
			if (s_instance == null) 
			{
				s_instance = new AchievementTableManager();
			}
			return s_instance;
		}

		ArrayList m_AchievementDataArray;

		protected AchievementTableManager()
		{
			m_AchievementDataArray = new ArrayList ();
		}

		protected override BaseData NewItem()
		{
			return new AchievementData();
		}

		public AchievementData GetAchievementData(int nID)
		{
			return (AchievementData)m_DataMap [nID];
		}

		protected override void _OnLoadItem(BaseData item)
		{
			m_AchievementDataArray.Add (item);
		}

		public void ClearUp()
		{
			s_instance = null;
		}

		public AchievementData GetAchievementDataByIndex(int nIndex)
		{
			return (AchievementData)m_AchievementDataArray [nIndex];
		}

		public int GetAchievementIndex(int nGhostID)
		{
			for (int i = 0; i < GameConst.Achievement_Num; i++) 
			{
				AchievementData data = (AchievementData)m_AchievementDataArray [i];
				if (data.m_nGhostID == nGhostID) 
				{
					return i;
				}
			}

			return 0;
		}
	}
}