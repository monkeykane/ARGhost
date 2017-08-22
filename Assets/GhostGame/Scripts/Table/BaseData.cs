using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
//using DragonEngine;
using UnityEngine;

namespace Script.Table
{
    enum NetState_State
    {
        Net_Login = 0,      //登录状态
        Net_GameSever = 1,   //进入游戏服务器状态
        Net_GamePlaying = 2, //游戏中状态
    }
    struct CommonData
    {
        public static NetState_State NetState;      // [ALREADY REVIEWED] [tong： the game state]
        public const string FileName_Txt_Share = "Table/Share/";
        public const string FileName_Txt_Client = "Table/Client/";
        public const string FileName_Atals_NOSource = "UI/NoSourcePrefab/";

        public const int FileLoadDirType_Client = 0;   //客户端专用类型
        public const int FileLoadDirType_Share = 1;   //共享类型

        public const int Reconnet_Normal = 0;
        public const int Reconnet_Sever_Remove = 1;
        public const int Reconnet_Client_Failse = 2;
        public const int Reconnet_Sever_Save_Data = 3;
        public static int ReconnectType;    // [ALREADY REVIEWED] [tong： the net state]

        public static void ClearUp()
        {
            ReconnectType = 0;
        }
    }

    public class DBFile
    {
        enum FIELD_TYPE
        {
            T_INT = 0,	//Integer
            T_FLOAT = 1,	//Float
            T_STRING = 2,	//string
        };

        ArrayList m_DataTypeArray;   //每列的数据类型数组
        ArrayList m_DataList;         //文件数据数组
        ArrayList m_RowListNow;       //当前列数据
        public DBFile()
        {
            m_DataTypeArray = new ArrayList(16);
            m_DataList = new ArrayList(16);
        }
        public bool LoadFile(string filename, string filePath)
        {
            //            TextAsset ptext = (TextAsset)Resources.Load(filePath + filename);
            TextAsset ptext = Resources.Load<TextAsset>(filePath + filename);// (TextAsset)DragonEngine.ResourceManager.LoadAsset(filePath + filename, typeof(TextAsset));

            if (ptext == null || ptext.text == "")
            {
                return false;
            }
            string ss = ptext.text.ToString().Trim();
            int index = 0;
            int line = 1;
            int end = 0;
            string tempstr = string.Empty;

            while (end > -1)
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

                ProxyStr(tempstr, line);
                line++;

                index = end + 1;
            }
            return true;
        }
        public bool LoadFile(string filename, int Type)
        {
            string FILE_PATH = string.Empty;
            if (Type == CommonData.FileLoadDirType_Client)
            {
                //FILE_PATH += CommonData.FileName_Txt_Client;
                FILE_PATH = CommonData.FileName_Txt_Client;
            }
            else
            {
                //FILE_PATH += CommonData.FileName_Txt_Share;
                FILE_PATH = CommonData.FileName_Txt_Share;
            }
            return LoadFile(filename, FILE_PATH);
        }

        void ProxyStr(string str, int line)
        {
            int index = 0;
            int end = 0;
            int dataNum = 0;

            if (line == 1)
            {
                while (end > -1)
                {
                    end = str.IndexOf("\t", index);
                    string tempStr = string.Empty;
                    if (end > -1)
                    {
                        tempStr = str.Substring(index, end - index);
                    }
                    else
                    {
                        tempStr = str.Substring(index, str.Length - index);
                    }
                    index = end + 1;

                    FIELD_TYPE ptype = FIELD_TYPE.T_INT;
                    if (tempStr == "INT")
                    {
                        ptype = FIELD_TYPE.T_INT;
                    }
                    else if (tempStr == "FLOAT")
                    {
                        ptype = FIELD_TYPE.T_FLOAT;
                    }
                    else if (tempStr == "STRING")
                    {
                        ptype = FIELD_TYPE.T_STRING;
                    }
                    m_DataTypeArray.Add(ptype);
                }
            }
            else if (line > 2)
            {
                ArrayList mRowDataList = new ArrayList(16);  //某一行的数据
                m_DataList.Add(mRowDataList);
                dataNum = 0;
                while (dataNum < m_DataTypeArray.Count)
                {
                    end = str.IndexOf("\t", index);
                    string tempStr = string.Empty;
                    if (end > -1)
                    {
                        tempStr = str.Substring(index, end - index);
                    }
                    else
                    {
                        if (index < str.Length)
                        {
                            tempStr = str.Substring(index, str.Length - index);
                        }
                        else
                        {
                            tempStr = string.Empty;
                        }
                        end = str.Length - 1;
                    }
                    index = end + 1;

                    FIELD_TYPE ptype = (FIELD_TYPE)m_DataTypeArray[dataNum];
                    dataNum++;

                    if (ptype == FIELD_TYPE.T_INT)
                    {
                        if (tempStr == "")
                        {
                            int tempInt = 0;
                            mRowDataList.Add(tempInt);
                        }
                        else
                        {
                            int tempInt = Convert.ToInt32(tempStr);
                            mRowDataList.Add(tempInt);
                        }
                    }
                    else if (ptype == FIELD_TYPE.T_FLOAT)
                    {
                        if (tempStr == "")
                        {
                            float tempFloat = 0;
                            mRowDataList.Add(tempFloat);
                        }
                        else
                        {
                            float tempFloat = (float)Convert.ToDouble(tempStr);
                            mRowDataList.Add(tempFloat);
                        }
                    }
                    else if (ptype == FIELD_TYPE.T_STRING)
                    {
                        mRowDataList.Add(tempStr);
                    }
                }
            }
        }
        public int getRowNum()
        {
            return m_DataList.Count;
        }
        public void SetNowRowList(int col)
        {
            if (col < m_DataList.Count)
            {
                m_RowListNow = (ArrayList)m_DataList[col];
            }
        }
        public int getInt(int col)
        {
            if (col >= m_DataTypeArray.Count)
                return 0;

            FIELD_TYPE ptype = (FIELD_TYPE)m_DataTypeArray[col];

            if (ptype != FIELD_TYPE.T_INT)
            {
                return -1;
            }

            return (int)m_RowListNow[col];
        }
        public float getFloat(int col)
        {
            if (col >= m_DataTypeArray.Count)
                return 0;

            FIELD_TYPE ptype = (FIELD_TYPE)m_DataTypeArray[col];

            if (ptype != FIELD_TYPE.T_FLOAT)
            {
                return -1;
            }

            return (float)m_RowListNow[col];
        }
        public string getString(int col)
        {
            if (col >= m_DataTypeArray.Count)
                return null;

            FIELD_TYPE ptype = (FIELD_TYPE)m_DataTypeArray[col];

            if (ptype != FIELD_TYPE.T_STRING)
            {
                return null;
            }
            return (string)m_RowListNow[col];
            //            return AnsiToUnicode((string)m_RowListNow[col]);
        }


        public string AnsiToUnicode(string ansiText)
        {
            byte[] gb = System.Text.Encoding.GetEncoding("GB2312").GetBytes(ansiText);
            string un = System.Text.Encoding.GetEncoding("Unicode").GetString(gb);
            return un;
        }
        public string UnicodeToAnsi(string unText)
        {
            byte[] un = System.Text.Encoding.GetEncoding("Unicode").GetBytes(unText);
            string gb = System.Text.Encoding.GetEncoding("GB2312").GetString(un);
            return gb;
        }
    }
    public interface ISingletonManager
    {
        // Called when the instance is destroyed, for clearance
        void DoDestroy();
    }


    public class BaseData
    {
        public int m_nId;
        public virtual void LoadData(int nRowIndex, DBFile fileData)
        {

        }


    }

    public abstract class BaseDataManager : ISingletonManager
    {
        protected Dictionary<int, BaseData> m_DataMap;
        protected int m_RowNum;
        public BaseDataManager()
        {
            m_DataMap = new Dictionary<int, BaseData>(64);
        }
        public void LoadFile(string fileName, int Type)
        {
            DBFile filedata = new DBFile();
            if (filedata.LoadFile(fileName, Type) == false)
            {
                //string message = Utils.BuildFormatString("Load {0} Error,Please check file", fileName);
                Debug.LogError("filedata.LoadFile(fileName, Type) == false");
            }

            m_RowNum = filedata.getRowNum();

            for (int i = 0; i < m_RowNum; i++)
            {
                BaseData item = NewItem();
                filedata.SetNowRowList(i);
                try
                {
                    item.LoadData(i, filedata);
                    m_DataMap[item.m_nId] = item;
                }
                catch (System.Exception)
                {
                    //string message = Utils.BuildFormatString("Load File {0} Error: row={1}", fileName, i);
                    //DebugTools.LogError(DragonEngine.LogCategory.GameLogic, message);
                    Debug.LogError("System.Exception");
                    return;
                }
            }

            _OnLoadComplete();
        }
        public int getRowNum()
        {
            return m_RowNum;
        }
        public void LoadFile(string fileName, string filePath)
        {
            DBFile filedata = new DBFile();
            if (filedata.LoadFile(fileName, filePath) == false)
            {
                //string message = Utils.BuildFormatString("Load {0} Error,Please check file", fileName);
                //DebugTools.LogError(DragonEngine.LogCategory.GameLogic, message);
                Debug.LogError("LoadFile");
            }

            m_RowNum = filedata.getRowNum();

            for (int i = 0; i < m_RowNum; i++)
            {
                BaseData item = NewItem();
                filedata.SetNowRowList(i);
                try
                {
                    item.LoadData(i, filedata);
                    m_DataMap[item.m_nId] = item;

					_OnLoadItem(item);
                }
                catch (System.Exception)
                {
                    //string message = Utils.BuildFormatString("Load File {0}{1} Error: row={2}", filePath, fileName, i);
                    //DebugTools.LogError(DragonEngine.LogCategory.GameLogic, message);
                    Debug.LogError("LoadFile: System.Exception");
                    return;
                }

                _OnLoadComplete();
            }
        }

        protected virtual void _OnLoadComplete()
        {

        }

		protected virtual void _OnLoadItem(BaseData item)
		{
			
		}

        protected abstract BaseData NewItem();
        public virtual void DoDestroy()
        {

        }
    }
}