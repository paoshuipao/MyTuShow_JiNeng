﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using PSPUtil;
using PSPUtil.Control;
using PSPUtil.StaticUtil;
using UnityEngine;
using UnityEngine.UI;

public class Sub_ItemContant : SubUI            // 包含全部的内容
{

    protected override void OnStart(Transform root)
    {
        MyEventCenter.AddListener<ushort,ushort,List<FileInfo>>(E_GameEvent.DaoRu_FromFile, E_OnDaoRuFromFile);
        MyEventCenter.AddListener<ushort,ushort,List<ResultBean>,ushort>(E_GameEvent.DaoRu_FromResult, E_OnDaoRuFromResult);
        MyEventCenter.AddListener<EDuoTuInfoType>(E_GameEvent.CloseDuoTuInfo, E_CloseDuoTuInfo);                       // 关闭多图信息
        MyEventCenter.AddListener<EDuoTuInfoType,string[]>(E_GameEvent.OnClickNoSaveThisDuoTu, E_DeleteOne);           // 多图信息中删除一个
        MyEventCenter.AddListener(E_GameEvent.OnClickSureDeleteAll, E_SureDeleteAl);                                   // 删除所有
        MyEventCenter.AddListener<List<ResultBean>>(E_GameEvent.DaoRuSucees2Delete, E_DaoRuSucees2Delete);             // 转换成功
        MyEventCenter.AddListener<ushort>(E_GameEvent.OnChangeKuangColor, E_OnChangeKuangColor);

        // 模版
        go_MoBan = GetGameObject("MoBan");


        for (ushort i = 0; i < 8; i++)
        {
            ushort bigIndex = i;
            // 8 个 Item
            l_ItemGOs[bigIndex] = GetGameObject("Item"+ bigIndex);

            // 8 个 内容 RectTransform
            RectTransform[] rts = new RectTransform[5];
            for (int j = 1; j < rts.Length+1; j++)
            {
                rts[j - 1] = Get<RectTransform>("Item" + bigIndex + "/SrcollRect/FenLie" + j);
       
            }

            l_TopContant[i]= rts;

            // 给每个 UGUI_BtnToggleGroup 添加事件
            ScrollRect scroll = Get<ScrollRect>("Item" + bigIndex + "/SrcollRect");
            UGUI_BtnToggleGroup bottomGroup = Get<UGUI_BtnToggleGroup>("Item" + bigIndex + "/Bottom/Contant");
            bottomGroup.E_OnCloseOtherItem += (bottomIndex) =>
            {
                E_OnBottomClosePre(bigIndex, bottomIndex);
            };
            bottomGroup.E_OnChooseItem += (bottomIndex) =>
            {
                E_OnBottomChangeItem(bigIndex, bottomIndex, scroll);
            };
            bottomGroup.E_OnDoubleClickItem += E_OnBottomDoubleClick;
            l_ToggleGroup[i] = bottomGroup;

            // 添加所有底下的字
            Text[] txNames = new Text[5];
            for (int j = 0; j < txNames.Length; j++)
            {
                txNames[j] = Get<Text>("Item"+i+"/Bottom/Contant/GeShiItem"+(j+1)+"/TxBottomName");

            }
            l_BottomNames[i] = txNames;

        }


        // 改名
        go_GaiNing = GetGameObject("GaiMing");
        tx_YuanMing = Get<Text>("GaiMing/Contant/Grid/Middle/TxYuan");
        tx_GaiMing = Get<Text>("GaiMing/Contant/Grid/Middle/TxGaiName");
        input_GaiMIng = Get<InputField>("GaiMing/Contant/Grid/Top/InputField");
        AddInputOnValueChanged(input_GaiMIng, (str) =>
        {
            if (mGaiMingcolorIndex >= 0)
            {
                inputEditName = "<color=" + MyDefine.ColorChoose[mGaiMingcolorIndex] + ">" + str + " </color>";
            }
            else
            {
                inputEditName = str;
            }
            tx_GaiMing.text = inputEditName;

        });
        AddButtOnClick("GaiMing/Contant/Grid/Bottom/BtnSure", Btn_SureGaiMing);
        AddButtOnClick("GaiMing/Contant/Grid/Bottom/BtnFalse", Btn_CloseGaiMing);

        for (int i = 0; i < 9; i++)    // 颜色按钮刚好 9 
        {
            ushort cIndex = (ushort) i;
            AddButtOnClick("GaiMing/Contant/Grid/AddColor/Contant/BtnColor"+i, () =>
            {
                ManyBtn_OnAddColorClick(cIndex);
            });
        }


        // 最右边
        AddButtOnClick("RightContrl/DaoRu", Btn_DaoRu);
        AddButtOnClick("RightContrl/DeleteAll", Btn_DaoClear);



        // 清空一行
        go_ClearOneLine = GetGameObject("ClearOneLine");
        tx_ClearTittle = Get<Text>("ClearOneLine/Contant/Tittle");
        tx_ClearTip = Get<Text>("ClearOneLine/Contant/Tip");
        AddButtOnClick("ClearOneLine/Contant/Bottom/BtnSure", Btn_SureClearOneLine);
        AddButtOnClick("ClearOneLine/Contant/Bottom/BtnFalse", Btn_CloseClearUI);


        // 速度滑动条
        slider_Speed = Get<Slider>("RightContrl/Slider_Speed/Slider");
        AddSliderOnValueChanged(slider_Speed, Slider_ChangeSpeed);



        // 删除失败
        go_DeleteError = GetGameObject("DeleteError");
        tx_DeleteErrorName = Get<Text>("DeleteError/Contant/Name/NameText");
        tx_DeleteErrorLength = Get<Text>("DeleteError/Contant/Length/LengthText");

        AddButtOnClick("DeleteError", () =>
        {
            go_DeleteError.SetActive(false);
        });
    }


    public override void OnEnable()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j <5; j++)
            {
                l_BottomNames[i][j].text = Ctrl_ContantInfo.Instance.BottomName[i][j];
            }
        }
    }


    #region 私有


    public void Show(ushort bigIndex)
    {
        mCurrentBigIndex = bigIndex;
        mCurrentBottomIndex = (ushort)l_ToggleGroup[bigIndex].mCurrentIndex;
        mUIGameObject.SetActive(true);
        l_ItemGOs[bigIndex].SetActive(true);
    }


    public void Show(ushort bigIndex, ushort bottomIndex)
    {
        mCurrentBigIndex = bigIndex;
        mCurrentBottomIndex = bottomIndex;

        mUIGameObject.SetActive(true);
        l_ToggleGroup[bigIndex].ChangeItem(bottomIndex);
        l_ItemGOs[bigIndex].SetActive(true);
        l_TopContant[bigIndex][bottomIndex].gameObject.SetActive(true);
    }


    public void Close(ushort index)
    {
        mUIGameObject.SetActive(false);
        l_ItemGOs[index].SetActive(false);
        CloseSliderSpeed();


    }



    private GameObject go_CurrentSelect; // 当前选择的对象
    private bool isSelect;               // 是否之前点击了


    private ushort mCurrentBigIndex,mCurrentBottomIndex,mCurrentItemIndex;    // 当前处于那个大的Item索引，和小的索引
    private readonly GameObject[] l_ItemGOs = new GameObject[8];                        // 8个总对象
    private readonly RectTransform[][] l_TopContant = new RectTransform[8][]; // 8个下的分别5个RectTransform
    private readonly Text[][] l_BottomNames = new Text[8][];                  // 8个下的分别5个底下名称
    private readonly UGUI_BtnToggleGroup[] l_ToggleGroup = new UGUI_BtnToggleGroup[8];




    // 模版
    private GameObject go_MoBan;



    // 改名
    private GameObject go_GaiNing;
    private InputField input_GaiMIng;
    private Text tx_YuanMing,tx_GaiMing;


    // 清空一行
    private GameObject go_ClearOneLine;
    private Text tx_ClearTittle,tx_ClearTip;


    // 速度滑动条
    private Slider slider_Speed;



    // 删除失败
    private GameObject go_DeleteError;
    private Text tx_DeleteErrorName,tx_DeleteErrorLength;



    public override string GetUIPathForRoot()
    {
        return "Right/EachContant/ItemContant";
    }



    public override void OnDisable()
    {
    }


    private IEnumerator CheckoubleClick() // 检测是否双击
    {
        isSelect = true;
        yield return new WaitForSeconds(MyDefine.DoubleClickTime);
        isSelect = false;
    }

    #endregion




    private void E_OnBottomClosePre(ushort bigIndex,ushort bottomIndex)                        // 关闭之前的 Item
    {
        l_TopContant[bigIndex][bottomIndex].gameObject.SetActive(false);
    }


    private void E_OnBottomChangeItem(ushort bigIndex, ushort bottomIndex, ScrollRect scroll)  // 切换 Item
    {
        l_TopContant[bigIndex][bottomIndex].gameObject.SetActive(true);
        scroll.content = l_TopContant[bigIndex][bottomIndex];
        mCurrentBottomIndex = bottomIndex;
        CloseSliderSpeed();
    }


    //—————————————————— 改名 ——————————————————


    private void E_OnBottomDoubleClick(ushort index)            // 双击 要改名
    {
        go_GaiNing.SetActive(true);
        tx_YuanMing.text = l_BottomNames[mCurrentBigIndex][mCurrentBottomIndex].text;
        inputEditName = tx_YuanMing.text;
    }


    private void Btn_SureGaiMing()                             // 确定改名
    {
        if (!string.IsNullOrEmpty(input_GaiMIng.text))
        {
            l_BottomNames[mCurrentBigIndex][mCurrentBottomIndex].text = inputEditName;
            Ctrl_ContantInfo.Instance.BottomName[mCurrentBigIndex][mCurrentBottomIndex] = inputEditName;
            MyEventCenter.SendEvent(E_GameEvent.GaiBottomName, mCurrentBigIndex, mCurrentBottomIndex, inputEditName);
        }
        Btn_CloseGaiMing();
    }


    private void Btn_CloseGaiMing()                            // 关闭改名
    {
        go_GaiNing.SetActive(false);
        mGaiMingcolorIndex = -1;
        input_GaiMIng.text = "";
    }



    private int mGaiMingcolorIndex = -1;
    private string inputEditName;
    private void ManyBtn_OnAddColorClick(ushort index)        // 点击了增加颜色的按钮
    {

        if (index == 8)
        {
            inputEditName = input_GaiMIng.text;
            mGaiMingcolorIndex = -1;
        }
        else
        {
            mGaiMingcolorIndex = index;
            inputEditName = "<color="+MyDefine.ColorChoose[index]+">" + input_GaiMIng.text + " </color>";
        }
        tx_GaiMing.text = inputEditName;
    }


    //—————————————————— 最右边 ——————————————————


    private void Btn_DaoRu()                           // 点击导入
    {
        MyOpenFileOrFolder.OpenFile(Ctrl_DaoRuInfo.Instance.ShowFirstPath, "选择多个文件（序列图）", EFileFilter.TuAndAll,
            (filePaths) =>
            {
                List<FileInfo> fileInfos = new List<FileInfo>(filePaths.Length);
                foreach (string filePath in filePaths)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (MyFilterUtil.IsTu(fileInfo))
                    {
                        fileInfos.Add(fileInfo);
                    }
                    else
                    {
                        MyLog.Red("选择了其他的格式文件 —— " + fileInfo.Name);
                    }
                }
                MyEventCenter.SendEvent(E_GameEvent.RealyDaoRu_File, EDuoTuInfoType.SearchShow,mCurrentBigIndex, mCurrentBottomIndex, fileInfos);
            });
    }


    private void Btn_DaoClear()                        // 点击清空
    {
        go_ClearOneLine.SetActive(true);
        int num = l_TopContant[mCurrentBigIndex][mCurrentBottomIndex].childCount;
        if (num<=0)
        {
            tx_ClearTip.text = "当前页一张序列图都没有,不用清空";
        }
        else
        {
            tx_ClearTittle.text = "清空当前<color=white> " + l_BottomNames[mCurrentBigIndex][mCurrentBottomIndex].text + " </color>页所有？";
            tx_ClearTip.text = "当前页总共有序列图<color=white> " + num + " </color>张";
        }
        
    }


    private void Btn_SureClearOneLine()                // 确定清空一行
    {
        DeleteOneLine(mCurrentBigIndex, mCurrentBottomIndex);
        Btn_CloseClearUI();
    }



    private void DeleteOneLine(ushort bigIndex,ushort bottomIndex)     // 删除一行
    {
        RectTransform rt = l_TopContant[bigIndex][bottomIndex];
        int num = rt.childCount;
        if (num > 0)
        {
            for (int i = 0; i < rt.childCount; i++)
            {
                Object.Destroy(rt.GetChild(i).gameObject);
            }
            Ctrl_XuLieTu.Instance.ClearOneLine(bigIndex, bottomIndex);
        }
    }



    private void Btn_CloseClearUI()                                    // 关闭清空的界面
    {
        go_ClearOneLine.SetActive(false);

    }



    private UGUI_SpriteAnim[] l_Anims;


    private void Slider_ChangeSpeed(float value)                      // 需要改变速度
    {
        if (null == l_Anims)
        {
            l_Anims = l_TopContant[mCurrentBigIndex][mCurrentBottomIndex].GetComponentsInChildren<UGUI_SpriteAnim>();
        }

        if (null!= l_Anims && l_Anims.Length >0)
        {
            for (int i = 0; i < l_Anims.Length; i++)
            {
                l_Anims[i].FPS = 0.5f / value;
            }
        }
       
    }



    private void CloseSliderSpeed()                                   // 还原这个速度的Slider
    {
        if (null != l_Anims)
        {
            foreach (UGUI_SpriteAnim anim in l_Anims)
            {
                anim.FPS = 0.1f;
            }
            l_Anims = null;
            slider_Speed.value = 5;
        }

    }




    //—————————————————— 事件 ——————————————————


    private void E_OnDaoRuFromFile(ushort bigIndex, ushort bottomIndex, List<FileInfo> fileInfos)      // 通过 FileInfo 导入
    {
        // 1. 创建一个实例
        RectTransform rt = l_TopContant[bigIndex][bottomIndex];
        Transform t = InstantiateMoBan(go_MoBan, rt);
        ushort index = (ushort)(rt.childCount - 1);

        // 2. 加载图片
        MyLoadTu.LoadMultipleTu(fileInfos, (resBean) =>
        {
            MyEventCenter.SendEvent(E_GameEvent.LoadTuFinishFromFile);
            // 3. 完成后把图集加上去
            ushort color = Ctrl_XuLieTu.Instance.GetColor(bigIndex, bottomIndex, index);
            InitMoBan(bigIndex,bottomIndex, index, color, t, resBean);
        });
    }


    private void E_OnDaoRuFromResult(ushort bigIndex,ushort bottomIndex,List<ResultBean> resultBeans,ushort colorIndex)  // 通过 ResultBean 导入
    {

        RectTransform rt = l_TopContant[bigIndex][bottomIndex];
        Transform t = InstantiateMoBan(go_MoBan, rt);
        InitMoBan(bigIndex, bottomIndex, (ushort)(rt.childCount - 1), colorIndex, t, resultBeans.ToArray());
    }

    private float SetSize(ushort bigIndex,ushort bottomInd,float value)
    {
        float res = value;
        if (bigIndex == 5|| bigIndex == 6)     // 6、7 两个是施法过程击中 
        {
            if (value > 350)
            {
                res = 350;
            }
        }else if (bigIndex == 7 && bottomInd == 1)    // 状态
        { 
            if (value > 220)
            {
                res = 200;
            }
        }
        else
        {
            if (value > 300)
            {
                res = 300;
            }
        }
        if (value<4)
        {
            res = 4;
        }
        return res;
    }



    private void InitMoBan(ushort bigIndex,ushort bottomIndex,ushort index,ushort colorIndex, Transform t,ResultBean[] resultBeans)         // 初始化模版
    {
        GameObject go = t.gameObject;
        Ctrl_XuLieTu.Instance.CeateMobanInitThis(resultBeans);
        Ctrl_XuLieTu.Instance.SetColor(bigIndex,bottomIndex,index,colorIndex);


        t.Find("AnimTu/Anim").GetComponent<UGUI_SpriteAnim>().ChangeAnim(resultBeans.ToSprites());
        Color c = MyDefine.ColorKuange[colorIndex];

        t.Find("AnimTu/Kuang").GetComponent<Image>().color = c;
        t.Find("TxName").GetComponent<Text>().color = c;

        float width = resultBeans[0].Width;
        float height = resultBeans[0].Height;
        if (width <= 20)
        {
            for (int i = 1; i < resultBeans.Length; i++)
            {
                if (resultBeans[i].Width > 20)
                {
                    width = resultBeans[i].Width;
                    height = resultBeans[i].Height;
                    break;
                }
            }
        }
        t.Find("AnimTu").GetComponent<ItemCallSize>().SetSize(width, height);
//        t.Find("AnimTu").GetComponent<RectTransform>().sizeDelta = new Vector2(SetSize(bigIndex, bottomIndex, width),SetSize(bigIndex, bottomIndex, height));

        string kName = Path.GetFileNameWithoutExtension(resultBeans[0].File.Name);
        int lastIndex = kName.LastIndexOf('_');        // 最后的 _01 不要
        if (lastIndex>0)
        {
            kName = kName.Substring(0, lastIndex);
        }
        t.Find("TxName").GetComponent<Text>().text = kName;

        t.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (go.Equals(go_CurrentSelect) && isSelect) // 双击
            {
                mCurrentItemIndex = index;
                mUIGameObject.SetActive(false);       // 显示信息时把整个给隐藏了
                ushort color = Ctrl_XuLieTu.Instance.GetColor(bigIndex, bottomIndex, index);
                MyEventCenter.SendEvent(E_GameEvent.ShowDuoTuInfo, resultBeans, EDuoTuInfoType.InfoShow, color);
            }
            else // 单击
            {
                go_CurrentSelect = go;
                Ctrl_Coroutine.Instance.StartCoroutine(CheckoubleClick());
            }
        });
    }

    //————————————————————————————————————

    
    private void E_CloseDuoTuInfo(EDuoTuInfoType type)                  // 关闭显示多图信息
    {
        if (type == EDuoTuInfoType.InfoShow)
        {
            mUIGameObject.SetActive(true);
        }
    }


    private void E_DeleteOne(EDuoTuInfoType type, string[] paths)      // 多图信息中删除一个 
    {
        if (type == EDuoTuInfoType.InfoShow)
        {
            DeleteOne(Path.GetFileNameWithoutExtension(paths[0]));
        }
    }



    private void E_SureDeleteAl()                                     // 确定清空所有图片
    {
        for (ushort i = 0; i < 8; i++)
        {
            for (ushort j = 0; j < 5; j++)
            {
                DeleteOneLine(i,j);
            }
        }
    }


    private void E_DaoRuSucees2Delete(List<ResultBean>  resultBeans) // 转换成功，需要把转换前的删除
    {
        DeleteOne(Path.GetFileNameWithoutExtension(resultBeans[0].File.FullName));
        go_CurrentSelect = null;

    }


    private void DeleteOne(string kName)                            // 删除单个
    {
        if (string.IsNullOrEmpty(kName))
        {
            go_DeleteError.SetActive(true);
            tx_DeleteErrorName.text = "为空？ 玩毛啊？";
            return;
        }

        bool isDelete = Ctrl_XuLieTu.Instance.DeleteOne(mCurrentBigIndex, mCurrentBottomIndex, kName);
        if (isDelete)
        {
            Object.Destroy(go_CurrentSelect);
        }
        else
        {
            go_DeleteError.SetActive(true);
            tx_DeleteErrorName.text = kName;
            tx_DeleteErrorLength.text = kName.Length.ToString();
        }
    }



    private void E_OnChangeKuangColor(ushort colorIndex)            // 修改了框框颜色
    {
        Ctrl_XuLieTu.Instance.SetColor(mCurrentBigIndex, mCurrentBottomIndex, mCurrentItemIndex, colorIndex);
        Color color = MyDefine.ColorKuange[colorIndex];
        go_CurrentSelect.transform.Find("AnimTu/Kuang").GetComponent<Image>().color = color;
        go_CurrentSelect.transform.Find("TxName").GetComponent<Text>().color = color;
    }


}


