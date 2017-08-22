using System;
using System.Collections.Generic;
using System.Text;
using Script.Table.Ghost;

namespace Script.Table
{
    class TableManager
    {   
        public static void Init()
        {
			GhostItemManager<stGhostItem>.Instance().LoadFile("ghostspawn", "Table/");
			GhostPowerTableManager.Instance ().LoadFile ("GhostPowerRate", "Table/");
			AchievementTableManager.Instance ().LoadFile ("achievement", "Table/");
        }

		public static void ClearUp()
		{
			GhostItemManager<stGhostItem>.Instance ().ClearUp();
			GhostPowerTableManager.Instance ().ClearUp ();
			AchievementTableManager.Instance ().ClearUp ();
		}
    }
}
