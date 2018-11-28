using PSPUtil.StaticUtil;
using UnityEngine;

public static class MyDefine         // 定义
{


    public const float DoubleClickTime = 0.25f;                  // 双击的控制时间（少于这个时间就算是双击）


    public static readonly string[] LeftName = { "施法前", "五行元素", "剑", "刀锤棍", "空手类", "命中效果", "整套技能", "加BUFF" };



    public static readonly string[] ColorChoose = { "#FF0000FF", "#5300FFFF", "#00B5FFFF", "#00FFFFFF", "#00FF21FF", "#E6FF00FF", "#FF8400FF", "#DBD8D8FF" };



    private static readonly string[] ColorKuangStrs =
    {
        "#FFFFFFFF","#8D999FFF","#38ABFDFF","#002CFFFF","#254D9AFF","#09FF00FF","#08A336FF","#FCFF04FF",
        "#9D9F00FF","#FF8400FF","#A25AECFF","#FF00CEFF","#FF001AFF","#960763FF","#624F00FF","#000000FF",
    };


    public static Color[] ColorKuange { get; private set; }



    public static string Data_Path
    {
        get { return DataPath + "/Data/"; }
    }


    public static string TuJi_Path
    {
        get { return DataPath + "/图集/"; }
    }


    private static readonly string DataPath;


    static MyDefine()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            DataPath = Application.persistentDataPath;

        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string dataPath = Application.dataPath;
            dataPath = dataPath.Replace("\\", "/");
            int lastIndex = dataPath.LastIndexOf('/');
            DataPath = dataPath.Substring(0, lastIndex);
        }
        ColorKuange = new Color[ColorKuangStrs.Length];
        for (int i = 0; i < ColorKuangStrs.Length; i++)
        {
            ColorKuange[i] = MyColor.GetColor(ColorKuangStrs[i]);
        }

    }




}
