using PSPUtil.StaticUtil;
using UnityEngine;

public static class MyDefine         // 定义
{


    public const float DoubleClickTime = 0.25f;                  // 双击的控制时间（少于这个时间就算是双击）


    public static readonly string[] LeftName = { "施法前", "五行元素", "剑", "刀锤棍", "空手类", "命中效果", "整套技能", "加BUFF" };



    public static readonly string[] ColorChoose = { "#FF0000FF", "#5300FFFF", "#00B5FFFF", "#00FFFFFF", "#00FF21FF", "#E6FF00FF", "#FF8400FF", "#DBD8D8FF" };




    public static string GetDataSaveDirPath()
    {
        string fliePath ="";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            fliePath = MyAssetUtil.GetApplicationDataPathNoAssets() + "/Data/";

        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string dataPath = Application.dataPath;
            int lastIndex = dataPath.LastIndexOf('/');
            fliePath = dataPath.Substring(0, lastIndex) + "/Data/";
        }
        return fliePath;
    }



}
