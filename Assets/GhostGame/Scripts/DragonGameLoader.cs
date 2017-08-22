using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;


namespace Script.Table.Ghost
{
    // group item...
    public class stGhostItem : BaseData
    {
       

        public string m_name;
        public string m_prefabname;
        public string m_rating;
        public int m_spawnrate;
        public int m_power;
        public float m_attacktime;

        

        public override void LoadData(int nRowIndex, DBFile fileData)
        {
            int readIndex = 0;
            m_nId = fileData.getInt(readIndex); readIndex++;

            m_name = fileData.getString(readIndex); readIndex++;
            m_prefabname = fileData.getString(readIndex); readIndex++;
            m_rating = fileData.getString(readIndex); readIndex++;
            m_spawnrate = fileData.getInt(readIndex); readIndex++;
            m_power = fileData.getInt(readIndex); readIndex++;
            m_attacktime = fileData.getFloat(readIndex); readIndex++;
        }
    }


    // load manager...
    public class GhostItemManager<T> : BaseDataManager where T : BaseData, new()
    {
        private static GhostItemManager<T> s_Instance;// [ALREADY REVIEWED] [tong： JungleItemManager  data manager]
        public static GhostItemManager<T> Instance()
        {
            if (s_Instance == null)
            {
                s_Instance = new GhostItemManager<T>();
            }
            return s_Instance;
        }
        protected override BaseData NewItem()
        {
            return new T();
        }

        public T GetstItem(int nID)
        {
            if (m_DataMap.ContainsKey(nID))
                return (T)m_DataMap[nID];

            return null;
        }

        public int GetstItemCount()
        {
            return m_DataMap.Count;
        }

        public T GetstItemByIndex(int index, out int key)
        {
            int i = 0;
            foreach (KeyValuePair<int, BaseData> pair in m_DataMap)
            {
                if (i == index)
                {
                    key = pair.Key;
                    return (T)pair.Value;
                }
                i++;
            }

            // wrong index
            key = -1;
            return null;
            //			BaseData data = m_DataMap.ElementAt (index).Value;
            //			key = m_DataMap.ElementAt (index).Key;
            //			return (T)data;
        }

        public void Push(T item)
        {
            if (item != null)
                m_DataMap[item.m_nId] = item;
        }

        public void ClearUp()
        {
            s_Instance = null;
        }
    }
}