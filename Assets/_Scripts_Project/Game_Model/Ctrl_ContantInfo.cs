﻿using System.IO;
using PSPUtil.Singleton;
using PSPUtil.StaticUtil;
using UnityEngine;


public class Ctrl_ContantInfo : Singleton_Mono<Ctrl_ContantInfo>
{


    public void InitData()
    {


        fliePath = MyDefine.GetDataSaveDirPath() + SaveFileName;

        // 左边 Item 名称
        if (IsExists(PP_LEFT_NAME))
        {
            LeftItemNames = Load<string[]>(PP_LEFT_NAME);
        }
        else
        {
            LeftItemNames = new string[8];
            for (ushort i = 0; i < 8; i++)
            {
                SetLeftItemName(i, MyDefine.LeftName[i]);
            }
        }


        // 底下名称
        if (IsExists(PP_BOTTOM_NAMES))
        {
            BottomName = Load<string[][]>(PP_BOTTOM_NAMES);
        }
        else
        {
            BottomName = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                string[] tmpEach = new string[5];
                for (int j = 0; j < 5; j++)
                {
                    tmpEach[j] = MyDefine.LeftName[i] + (j + 1);
                }
                BottomName[i] = tmpEach;
            }
        }

    }


    public void SetLeftItemName(ushort bigIndex,string leftName)
    {
        switch (bigIndex)
        {
            case 0:
                LeftItemNames[bigIndex] = "<color=white>" + leftName + "</color>";
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                LeftItemNames[bigIndex] = "<color=blue>" + leftName + "</color>";
                break;
            case 5:
                LeftItemNames[bigIndex] = leftName;
                break;
            case 6:
                LeftItemNames[bigIndex] = "<color=#008080ff>" + leftName + "</color>";
                break;
            case 7:
                LeftItemNames[bigIndex] = "<color=white>" + leftName + "</color>";
                break;
        }
    }



    public string[] LeftItemNames { get; private set; }          // 总 左边Item

    public string[][] BottomName { get; private set; }           // 底下名称



    #region 私有



    private const string PP_LEFT_NAME = "PP_LEFT_NAME";
    private const string PP_BOTTOM_NAMES = "PP_BOTTOM_NAMES";
    private const string SaveFileName = "NormalData.res";        // 保存数据的文件名


    private string fliePath; 

    #endregion




    void OnApplicationQuit()
    {
        Save(PP_LEFT_NAME, LeftItemNames);
        Save(PP_BOTTOM_NAMES, BottomName);
    }



    //————————————————————————————————————



    private bool IsExists(string key)
    {
        return ES3.FileExists(fliePath) && ES3.KeyExists(key, fliePath);
    }


    private T Load<T>(string key)
    {
        return ES3.Load<T>(key, fliePath);
    }


    private void Save<T>(string key,T value)
    {
        ES3.Save<T>(key, value, fliePath);
    }


}