using PSPUtil;
using PSPUtil.StaticUtil;
using UnityEngine;
using UnityEngine.UI;

public class LOGO : MonoBehaviour
{

    private GameObject go_Bottom;


    void Awake()
    {
        MyEventCenter.AddListener<string>(E_GameEvent.NoExistsTuJi, E_NoExistsTuJi);
        go_Bottom = transform.Find("Bottom").gameObject;
    }


    private void E_NoExistsTuJi(string tuJiPath)         // 不存在图集文件夹
    {
        go_Bottom.SetActive(true);
        transform.Find("Bottom/ErrorText").GetComponent<Text>().text = "找不到路径： "+ tuJiPath;
        transform.Find("Bottom/BtnGoHead").GetComponent<Button>().onClick.AddListener(() =>
        {
            Ctrl_ContantInfo.Instance.InitDealutData();
            Ctrl_XuLieTu.Instance.InitDealutData();
            Manager.Get<MySceneManager>(EF_Manager.MyScene).LoadScene(EF_Scenes._1_Start);
        });

    }




    void OnDestroy()
    {
        MyEventCenter.RemoveListener<string>(E_GameEvent.NoExistsTuJi, E_NoExistsTuJi);

    }





}
