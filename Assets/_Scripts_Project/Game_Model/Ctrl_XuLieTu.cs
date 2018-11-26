﻿using System.Collections.Generic;
using System.IO;
using PSPUtil.Singleton;
using PSPUtil.StaticUtil;
using UnityEngine;


public class Ctrl_XuLieTu : Singleton_Mono<Ctrl_XuLieTu>
{


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
        IsInitFinish = true;
    }


    public int GetEachCount(ushort bigIndex, ushort bottomIndex)                      // 获取每个有多少张序列图
    {
        Dictionary<string, string[]> dic = indexK_KNameV[bigIndex][bottomIndex];
        return dic.Count;
    }



    public List<string[]> GetPaths(ushort bigIndex,ushort bottomIndex)                // 获取
    {
        return new List<string[]>(indexK_KNameV[bigIndex][bottomIndex].Values);
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

        indexK_KNameV[bigIndex][bottomIndex].Add(kName,paths);

        return true;

    }


    public bool DeleteOne(ushort bigIndex, ushort bottomIndex, string kName)         // 删除单个 
    {
        if (indexK_KNameV[bigIndex][bottomIndex].ContainsKey(kName))
        {
            indexK_KNameV[bigIndex][bottomIndex].Remove(kName);

            string tmp = kName.ToLower();
            if (kNameK_ResultBeansV.ContainsKey(tmp))
            {
                kNameK_ResultBeansV.Remove(tmp);
            }
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
    }


    //————————————————————————————————————


    private Dictionary<string, string[]> Load(ushort bigIndex,ushort bottomIndex)
    {

        string fliePath = MyDefine.GetDataSaveDirPath() + FIleNames[bigIndex];
        string keyName = BottomKeyName[bottomIndex];
        return ES3.Load(keyName, fliePath, new Dictionary<string, string[]>()); ;
    }


    private void Save(ushort bigIndex, ushort bottomIndex)
    {
        string fliePath = MyDefine.GetDataSaveDirPath() + FIleNames[bigIndex];
        string keyName = BottomKeyName[bottomIndex];

        ES3.Save<Dictionary<string, string[]>>(keyName, indexK_KNameV[bigIndex][bottomIndex], fliePath);

    }
}
