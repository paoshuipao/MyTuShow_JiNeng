using System;
using PSPUtil;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public enum EBeforeShow
{
    Input1,
    Input2,
    Input3,
    Input4,
    Input5,
    Input6,
    Input7,
    Input8,
    DeleteAllSure,   // 删除所有的确定

}




public class Sub_BeforeClick : SubUI 
{


    protected override void OnStart(Transform root)
    {

        MyEventCenter.AddListener<EBeforeShow>(E_GameEvent.ShowBeforeClick, E_Show);
        Get<Button>().onClick.AddListener(Btn_OnBigClick);


        for (ushort i = 0; i < 8; i++)
        {
            ushort bigIndex = i;
            InputField input = Get<InputField>("Left/Item" + (i + 1) + "/InputChange");
            inputs[i] = input;
            AddInputOnEndEdit(input, (str) =>
            {
                if (!string.IsNullOrEmpty(str))
                {
                    MyEventCenter.SendEvent(E_GameEvent.OnSureChangeLeftName,bigIndex, str);
                }
                input.text = "";
                Btn_OnBigClick();
            });
        }

        // 删除所有
        go_DeleteAll = GetGameObject("Right/DeleteAll");
        AddButtOnClick("Right/DeleteAll/Contant/Btn", Btn_DeleteSure);

    }


    #region 私有


    private readonly InputField[] inputs = new InputField[8];

    // 颜色
    private GameObject mCurrentShowGO;

    // 删除所有
    private GameObject go_DeleteAll;



    public override string GetUIPathForRoot()
    {
        return "BeforeClick";
    }


    public override void OnEnable()
    {
    }

    public override void OnDisable()
    {
    }


    #endregion

    private void Btn_OnBigClick()          // 点击了最大的按钮
    {
        mUIGameObject.SetActive(false);
        mCurrentShowGO.SetActive(false);
        mCurrentShowGO = null;
    }



    private void Btn_DeleteSure()           // 点击 确定删除
    {
        MyEventCenter.SendEvent(E_GameEvent.OnClickSureDeleteAll);
        Btn_OnBigClick();
    }




    //—————————————————— 事件——————————————————


    private void E_Show(EBeforeShow showWhich)
    {
        mUIGameObject.SetActive(true);

        switch (showWhich)
        {

            case EBeforeShow.Input1:
            case EBeforeShow.Input2:
            case EBeforeShow.Input3:
            case EBeforeShow.Input4:
            case EBeforeShow.Input5:
            case EBeforeShow.Input6:
            case EBeforeShow.Input7:
            case EBeforeShow.Input8:
                mCurrentShowGO = inputs[(int)showWhich].gameObject;
                break;
            case EBeforeShow.DeleteAllSure:
                mCurrentShowGO = go_DeleteAll;
                break;
            default:
                throw new Exception("未定义");
        }
        mCurrentShowGO.SetActive(true);
    }


}
