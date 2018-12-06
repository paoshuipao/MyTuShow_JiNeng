using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PSPUtil;
using PSPUtil.Control;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : BaseUI
{

    protected override void OnStart(Transform root)
    {
        mLeftGroup = Get<UGUI_BtnToggleGroup>("Left");
        mLeftGroup.E_OnCloseOtherItem += E_OnLeftChangePre;     // 关闭其他 Item
        mLeftGroup.E_OnChooseItem += E_OnLeftGroupChange;       // 选中这个 Item
        mLeftGroup.E_OnDoubleClickItem += E_OnLeftDoubleClick;

        for (int i = 0; i < 8; i++)
        {
            L_LeftText[i] = Get<Text>("Left/Item"+(i+1)+"/TxLeft");
        }

        go_Loading = GetGameObject("LoadingAnim");

        // 等待选择文件或文件UI
        go_WaitBrowser = GetGameObject("OpenBrowser");
        tx_Wait = Get<Text>("OpenBrowser/Text");


        // 一开始把之前所有的都加载进来
        Ctrl_Coroutine.Instance.StartCoroutine(StartFirst());

    }


    protected override void OnEnable()
    {
        for (int i = 0; i < L_LeftText.Length; i++)
        {
            L_LeftText[i].text = Ctrl_ContantInfo.Instance.LeftItemNames[i];
        }
    }



    protected override void OnAddListener()
    {
        MyEventCenter.AddListener<ushort,ushort>(E_GameEvent.ChangeLeftItem, E_LeftChangeItem);     // 切换其他 Item
        MyEventCenter.AddListener(E_GameEvent.OpenFileContrl, OnShowGameWaitUI_File);               // 打开 文件 资源管理器
        MyEventCenter.AddListener(E_GameEvent.OpenFolderContrl, OnShowGameWaitUI_Folder);           // 打开 文件夹 资源管理器
        MyEventCenter.AddListener(E_GameEvent.CloseFileOrFolderContrl, OnHideGameWaitUI_Browser);   // 关闭 文件或者文件夹资源管理器
        MyEventCenter.AddListener<ushort,string>(E_GameEvent.OnSureChangeLeftName, E_ChangeLeftName);// 修改左边的名称

    }



    private ushort loadingIndex = 0;

    private void E_LoadTuFinishFromFile()
    {
        loadingIndex--;
//        isLoading = false;
    }

    IEnumerator StartFirst()
    {
        go_Loading.SetActive(true);
        MyEventCenter.AddListener(E_GameEvent.LoadTuFinishFromFile, E_LoadTuFinishFromFile);

        Button btn_DaoRu = Get<Button>("Left/Item9");
        Button btn_Search = Get<Button>("Left/Item10");
        Button btn_QuYuSearch = Get<Button>("Left/Item11");

        btn_DaoRu.interactable = false;
        btn_Search.interactable = false;
        btn_QuYuSearch.interactable = false;


        while (!Ctrl_XuLieTu.Instance.IsInitFinish)
        {
            yield return 0;
        }


        for (ushort bigIndex = 0; bigIndex < 8; bigIndex++)
        {
            for (ushort bottomIndex = 0; bottomIndex < 5; bottomIndex++)
            {
                List<string[]> psthList = Ctrl_XuLieTu.Instance.GetPaths(bigIndex, bottomIndex);
                for (int k = 0; k < psthList.Count; k++)
                {
                    while (loadingIndex>=MyDefine.MaxLoadCount)       // 有一个在加载中
                    {
                        yield return 0;
                    }
                    string[] tmpPaths = psthList[k];
                    List<FileInfo> fileInfos = new List<FileInfo>(tmpPaths.Length);
                    bool isChuZai = true; // 这些路径是否存在
                    for (int j = 0; j < tmpPaths.Length; j++)
                    {
                        FileInfo fileInfo = new FileInfo(tmpPaths[j]);
                        if (!fileInfo.Exists)
                        {
                            isChuZai = false;
                            break;
                        }
                        fileInfos.Add(fileInfo);
                    }
                    if (isChuZai) // 存在就导入进来
                    {
                        MyEventCenter.SendEvent(E_GameEvent.DaoRu_FromFile, bigIndex, bottomIndex, fileInfos);
                        loadingIndex++;
//                        isLoading = true;
                    }
                    else // 不存在就删除存储的
                    {
                        Ctrl_XuLieTu.Instance.DeleteOne(bigIndex, bottomIndex, Path.GetFileNameWithoutExtension(tmpPaths[0]));
                    }
                }
            }
        }

        yield return 0;
        MyEventCenter.RemoveListener(E_GameEvent.LoadTuFinishFromFile, E_LoadTuFinishFromFile);
        btn_DaoRu.interactable = true;
        btn_Search.interactable = true;
        btn_QuYuSearch.interactable = true;
        go_Loading.SetActive(false);

    }


    #region 私有

    private readonly Text[] L_LeftText = new Text[8];             // 左边的 8 个序列图文字
    private UGUI_BtnToggleGroup mLeftGroup;
    private GameObject go_Loading;




    // 底下的等待UI
    private GameObject go_WaitBrowser;
    private Text tx_Wait;
    private const string WAIT_FILE = "等待,选择文件中...";
    private const string WAIT_FOLDER = "等待,选择文件夹中...";



    private readonly Sub_ItemContant sub_ItemContant = new Sub_ItemContant();
    private readonly Sub_DaoRu sub_DaoRu = new Sub_DaoRu();
    private readonly Sub_Search sub_Search = new Sub_Search();
    private readonly Sub_QuYuSearch sub_QuYuSearch = new Sub_QuYuSearch();
    private readonly Sub_DaoRuResult sub_DaRuResult = new Sub_DaoRuResult();
    private readonly Sub_DuoTuInfo sub_DuoTuInfo = new Sub_DuoTuInfo();
    private readonly Sub_SingleTuInfo sub_SingleTuInfo = new Sub_SingleTuInfo();
    private readonly Sub_BeforeClick sub_BeforeClick = new Sub_BeforeClick();


    protected override SubUI[] GetSubUI()
    {
        return new SubUI[]
        {
            sub_ItemContant,sub_DaoRu,sub_Search,sub_QuYuSearch,sub_DaRuResult,sub_DuoTuInfo,sub_SingleTuInfo,sub_BeforeClick
        };
    }



    protected override void OnDisable()
    {
    }



    protected override void OnRemoveListener()
    {
    }

    protected override E_GameEvent GetShowEvent()
    {
        return E_GameEvent.ShowStartGameUI;
    }

    protected override E_GameEvent GetHideEvent()
    {
        return E_GameEvent.HideStartGameUI;
    }

    protected override string GetResName()
    {
        return "UI/UI_Game";
    }

    public override EF_Scenes GetSceneType()
    {
        return EF_Scenes._0_Main;
    }


    #endregion



    private void E_OnLeftChangePre(ushort preIndex)      // 切换前 先关闭之前的
    {
        switch (preIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                sub_ItemContant.Close(preIndex);
                break;
            case 8:
                sub_DaoRu.Close();
                break;
            case 9:
                sub_Search.Close();
                break;
            case 10:
                sub_QuYuSearch.Close();
                break;
            default:
                throw new Exception("未定义");
        }
    }


    private void E_OnLeftGroupChange(ushort bigIndex)       // 切换
    {
        switch (bigIndex)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                sub_ItemContant.Show(bigIndex); 
                break;
            case 8:
                sub_DaoRu.Show();
                break;
            case 9:
                sub_Search.Show();
                break;
            case 10:
                sub_QuYuSearch.Show();
                break;
            default:
                throw new Exception("未定义");
        }
        MyEventCenter.SendEvent(E_GameEvent.ItemChange);
    }


    private void E_OnLeftDoubleClick(ushort bigIndex)       // 双击左边的Item
    {
        MyEventCenter.SendEvent(E_GameEvent.ShowBeforeClick, (EBeforeShow)bigIndex);

    }




    //—————————————————— 事件 ——————————————————


    private void E_LeftChangeItem(ushort bigIndex,ushort bottomIndex)           // 不是点击切换 Item ，而是其他要求切换
    {
        mLeftGroup.ChangeItem(bigIndex);
        sub_ItemContant.Show(bigIndex, bottomIndex);
    }



    private void E_ChangeLeftName(ushort bigIndex,string newName)                // // 修改左边的名称
    {

        Ctrl_ContantInfo.Instance.SetLeftItemName(bigIndex, newName);
        L_LeftText[bigIndex].text = Ctrl_ContantInfo.Instance.LeftItemNames[bigIndex];

    }


    #region  文件、文件夹事件


    private void OnShowGameWaitUI_File()            // 接收 显示等待的界面 文件 事件
    {
        go_WaitBrowser.SetActive(true);
        tx_Wait.text = WAIT_FILE;

    }

    private void OnShowGameWaitUI_Folder()          // 接收 显示等待的界面 文件夹 事件
    {
        go_WaitBrowser.SetActive(true);
        tx_Wait.text = WAIT_FOLDER;

    }

    private void OnHideGameWaitUI_Browser()         // 接收 取消等待浏览器的界面 事件
    {
        go_WaitBrowser.SetActive(false);
    }

    #endregion


}
