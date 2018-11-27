using System.Collections.Generic;
using System.IO;
using PSPUtil.Singleton;
using PSPUtil.StaticUtil;
using UnityEngine;


public class Ctrl_XuLieTu : Singleton_Mono<Ctrl_XuLieTu>
{

    public void InitDealutData()              // 没任何保存 初始化最初值
    {
        for (ushort i = 0; i < 8; i++)
        {
            Dictionary<ushort, Dictionary<string, string[]>> bottomK_pathsV = new Dictionary<ushort, Dictionary<string, string[]>>();
            for (ushort j = 0; j < 5; j++)
            {
                bottomK_pathsV.Add(j, new Dictionary<string, string[]>());
            }
            indexK_KNameV.Add(i, bottomK_pathsV);
        }
        InitDealutColorData();
        IsInitFinish = true;
    }


    private void InitDealutColorData()
    {
        indexK_ColorV = new Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, ushort>>>();
        for (ushort i = 0; i < 8; i++)
        {
            Dictionary<ushort, Dictionary<ushort, ushort>> bottomK_IndexV = new Dictionary<ushort, Dictionary<ushort, ushort>>();
            for (ushort j = 0; j < 5; j++)
            {
                bottomK_IndexV.Add(j, new Dictionary<ushort, ushort>());
            }
            indexK_ColorV.Add(i, bottomK_IndexV);
        }
    }

    public void InitData()
    {
        // 先初始化 indexK_KNameV
        for (ushort i = 0; i < 8; i++)
        {
            Dictionary<ushort, Dictionary<string, string[]>> bottomK_pathsV= new Dictionary<ushort, Dictionary<string, string[]>>();

            for (ushort j = 0; j < 5; j++)
            {
                Dictionary<string, string[]> kNameK_PathsV = Load(i,j);
                bottomK_pathsV.Add(j,kNameK_PathsV);
            }
            indexK_KNameV.Add(i, bottomK_pathsV);
        }

        string colorFilePaht = MyDefine.Data_Path + ColorFileNames;
        if (ES3.FileExists(colorFilePaht) && ES3.KeyExists(PP_COLOR, colorFilePaht))
        {
            indexK_ColorV = ES3.Load(PP_COLOR, MyDefine.Data_Path + ColorFileNames,new Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, ushort>>>());
        }
        else
        {
            InitDealutColorData();
        }

        IsInitFinish = true;
    }


    public int GetEachCount(ushort bigIndex, ushort bottomIndex)                      // 获取每个有多少张序列图
    {
        return indexK_KNameV[bigIndex][bottomIndex].Count;
    }



    public List<string[]> GetPaths(ushort bigIndex,ushort bottomIndex)                // 获取
    {

        List<string[]> lsit = new List<string[]>();


        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            foreach (string[] strs in indexK_KNameV[bigIndex][bottomIndex].Values)
            {
                string[] tmps = new string[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    tmps[i] = MyDefine.TuJi_Path + strs[i];
                }
                lsit.Add(tmps);
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            lsit.AddRange(indexK_KNameV[bigIndex][bottomIndex].Values);
        }
        return lsit;
    }


    public bool Save(ushort bigIndex,ushort bottomIndex,string[] paths)              // 保存,成功返回 true
    {
        string kName = Path.GetFileNameWithoutExtension(paths[0]);
        if (string.IsNullOrEmpty(kName))
        {
            return false;
        }
        if (indexK_KNameV[bigIndex][bottomIndex].ContainsKey(kName))
        {
            return false;
        }


        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // 如 C:\Users\Administrator\Desktop\我的工具\工具_技能\图集\霸王拳\出招\图1.png
            // KName -> 图1
            // 保存  -> 霸王拳\出招\图1.png 的集合
            string[] savePaths = new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                string tmp = paths[i].Replace("\\", "/");
                savePaths[i] = tmp.Replace(MyDefine.TuJi_Path, "");

            }
            indexK_KNameV[bigIndex][bottomIndex].Add(kName, savePaths);
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            indexK_KNameV[bigIndex][bottomIndex].Add(kName, paths);
        }
        return true;

    }


    public bool DeleteOne(ushort bigIndex, ushort bottomIndex, string kName)         // 删除单个
    {
        List<string> tmpList = new List<string>(indexK_KNameV[bigIndex][bottomIndex].Keys);


        if (tmpList.Contains(kName))
        {
            DeleteSearchOne(kName);   // 删除搜索单个
            DeleteColorOne(bigIndex, bottomIndex, (ushort)tmpList.IndexOf(kName));  // 删除颜色单个
            indexK_KNameV[bigIndex][bottomIndex].Remove(kName);
            return true;
        }
        else
        {

            MyLog.Red("没有包含这个名称 —— "+ kName+" 长度 —— "+ kName.Length);
            MyLog.Red("大索引 —— "+bigIndex +"    小索引 —— "+bottomIndex);
            return false;
        }
    }


    public void ClearOneLine(ushort bigIndex, ushort bottomIndex)                    // 删除一行
    {
        foreach (string kName in indexK_KNameV[bigIndex][bottomIndex].Keys)
        {
            string tmp = kName.ToLower();
            if (kNameK_ResultBeansV.ContainsKey(tmp))
            {
                kNameK_ResultBeansV.Remove(tmp);
            }
        }
        indexK_KNameV[bigIndex][bottomIndex].Clear();
        ClearColorOneLine(bigIndex, bottomIndex);
    }




    //—————————————————— 用于搜索 ——————————————————


    private readonly Dictionary<string, ResultBean[]> kNameK_ResultBeansV = new Dictionary<string, ResultBean[]>();   // 用于搜索


    public Dictionary<string, ResultBean[]> Search(string searchName)
    {
        Dictionary<string, ResultBean[]> tmpRes = new Dictionary<string, ResultBean[]>();
        if (!string.IsNullOrEmpty(searchName))
        {
            foreach (string key in kNameK_ResultBeansV.Keys)
            {
                if (key.ToLower().Contains(searchName))
                {
                    tmpRes.Add(key, kNameK_ResultBeansV[key]);
                }
            }
        }
        return tmpRes;
    }


    public void CeateMobanInitThis(ResultBean[] resultBeans)    // 只要创建一个模版就加进来
    {
        string kName = Path.GetFileNameWithoutExtension(resultBeans[0].File.FullName).ToLower();
        int addIndex = 0;
        while (kNameK_ResultBeansV.ContainsKey(kName))
        {
            addIndex++;
            kName +="("+ addIndex+")";
        }
        kNameK_ResultBeansV.Add(kName, resultBeans);
    }



    private void DeleteSearchOne(string kName)
    {

        string tmp = kName.ToLower();
        if (kNameK_ResultBeansV.ContainsKey(tmp))
        {
            kNameK_ResultBeansV.Remove(tmp);
        }
    }

    //—————————————————— 添加颜色 ——————————————————


    private Dictionary<ushort,Dictionary<ushort,Dictionary<ushort,ushort>>> indexK_ColorV;


    public void SetColor(ushort bigIndex, ushort bottomIndex, ushort index,ushort colorIndex)
    {
        if (indexK_ColorV[bigIndex][bottomIndex].ContainsKey(index))
        {
            indexK_ColorV[bigIndex][bottomIndex][index] = colorIndex;
        }
        else 
        {
            indexK_ColorV[bigIndex][bottomIndex].Add(index,colorIndex);
        }
    }

    public ushort GetColor(ushort bigIndex, ushort bottomIndex, ushort index)
    {
        if (indexK_ColorV[bigIndex][bottomIndex].ContainsKey(index))
        {
            return indexK_ColorV[bigIndex][bottomIndex][index];
        }
        else  // 没有这个就直接添加进来
        {
            indexK_ColorV[bigIndex][bottomIndex].Add(index,0);
            return 0;
        }
    }


    private void DeleteColorOne(ushort bigIndex, ushort bottomIndex, ushort index)
    {
        indexK_ColorV[bigIndex][bottomIndex].Remove(index);
    }

    private void ClearColorOneLine(ushort bigIndex, ushort bottomIndex)
    {
        indexK_ColorV[bigIndex][bottomIndex].Clear();
    }



    #region 私有


    public bool IsInitFinish = false;
    private readonly Dictionary<ushort,Dictionary<ushort,Dictionary<string,string[]>>> indexK_KNameV = new Dictionary<ushort, Dictionary<ushort, Dictionary<string, string[]>>>();


    private static readonly string[] FIleNames = new[]
    {
        "data0.res", "data1.res", "data2.res", "data3.res",
        "data4.res", "data5.res", "data6.res", "data7.res",
    };


    private static readonly string[] BottomKeyName = new[]
    {
        "Item0", "Item1",  "Item2", "Item3", "Item4",
    };


    private const string ColorFileNames = "ColorData.res";
    private const string PP_COLOR = "PP_COLOR";

    #endregion



    void OnApplicationQuit()
    {
        for (ushort i = 0; i < 8; i++)
        {
            for (ushort j = 0; j < 5; j++)
            {
                Save(i,j);
            }
        }

        ES3.Save<Dictionary<ushort, Dictionary<ushort, Dictionary<ushort, ushort>>>>(PP_COLOR, indexK_ColorV, MyDefine.Data_Path + ColorFileNames);

    }


    //————————————————————————————————————


    private Dictionary<string, string[]> Load(ushort bigIndex,ushort bottomIndex)
    {
        string fliePath = MyDefine.Data_Path + FIleNames[bigIndex];
        string keyName = BottomKeyName[bottomIndex];
        return ES3.Load(keyName, fliePath, new Dictionary<string, string[]>()); ;
    }


    private void Save(ushort bigIndex, ushort bottomIndex)
    {
        string fliePath = MyDefine.Data_Path + FIleNames[bigIndex];
        string keyName = BottomKeyName[bottomIndex];
        ES3.Save<Dictionary<string, string[]>>(keyName, indexK_KNameV[bigIndex][bottomIndex], fliePath);
    }



}
