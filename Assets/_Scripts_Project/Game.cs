using System.Collections;
using System.IO;
using PSPUtil;
using PSPUtil.StaticUtil;
using UnityEngine;

public class Game : MonoBehaviour
{

    public GameObject LOGO;

    void Awake()
    {
        Manager.Init();                      // 初始化所有的 Manager
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;



//        MyEventCenter.SendEvent(E_GameEvent.ShowLog);      // 显示 Log

    }


    void Start()
    {
        if (!LOGO.activeSelf)
        {
            LOGO.SetActive(true);
        }
        LOGO.transform.localPosition = Vector3.zero;
        LOGO.transform.localScale =Vector3.one;
        StartCoroutine(JumpScene());

    }

    void OnDestroy()
    {
        MyEventCenter.SendEvent(E_GameEvent.LogoExit);
    }



    IEnumerator JumpScene()
    {
        bool isStart = true;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {

            string dataPath = Application.dataPath;
            int lastIndex = dataPath.LastIndexOf('/');
            string tuJiPath = dataPath.Substring(0, lastIndex) + "/图集";
            if (!Directory.Exists(tuJiPath))
            {
                MyEventCenter.SendEvent(E_GameEvent.NoExistsTuJi, tuJiPath);
                isStart = false;
            }
        }

        if (isStart)
        {
            yield return new WaitForSeconds(0.5f);
            Ctrl_ContantInfo.Instance.InitData();
            Ctrl_XuLieTu.Instance.InitData();
            yield return new WaitForSeconds(2f);
            Manager.Get<MySceneManager>(EF_Manager.MyScene).LoadScene(EF_Scenes._1_Start);
        }

    }

}
