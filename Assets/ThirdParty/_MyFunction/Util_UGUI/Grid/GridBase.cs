using PSPUtil.Attribute;
using PSPUtil.StaticUtil;
using UnityEngine;


[ExecuteInEditMode]
[DisallowMultipleComponent]
public abstract class GridBase : MonoBehaviour 
{


    [MyHead("在这个范围内是显示的")]
    public Vector2 HidePosition = new Vector2(-8, 8);



    protected static readonly Vector2 ZERO_ONE = new Vector2(0, 1);


    void Reset()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition3D = Vector3.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }


    void Update()
    {
        RectTransform mRT = transform as RectTransform;
        if (null == mRT)
        {
            MyLog.Red("不可能为空吧");
            return;
        }
        float width = mRT.rect.width;                    // 总共的宽度

        int cout = transform.childCount;                            // 总共有几个 

        Vector2[] l_Position;
        int shu;
        Vector2 itemCallSize = OnUpdate2Setting(width, cout,out shu, out l_Position);


        for (int i = 0; i < cout; i++)
        {
            RectTransform rt = transform.GetChild(i) as RectTransform;
            if (null == rt)
            {
                MyLog.Red("为什么为空？" + transform.GetChild(i));
                continue;
            }
            rt.anchorMin = ZERO_ONE;
            rt.anchorMax = ZERO_ONE;
            rt.pivot = ZERO_ONE;
            rt.sizeDelta = itemCallSize;
            rt.anchoredPosition = l_Position[i];
        }

        // 设置每个 Item 大小
        float height = itemCallSize.y * shu;
        mRT.sizeDelta = new Vector2(0, height);


        // 隐藏
        if (Application.isPlaying)
        {
            for (int i = 0; i < mRT.childCount; i++)
            {
                if (mRT.GetChild(i).position.y > HidePosition.y)
                {
                    mRT.GetChild(i).gameObject.SetActive(false);
                }
                else if (mRT.GetChild(i).position.y < HidePosition.x)
                {
                    mRT.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    mRT.GetChild(i).gameObject.SetActive(true);
                }
            }

        }

    }




    protected abstract Vector2 OnUpdate2Setting(float cnavasWidth,int childCount,out int shu, out Vector2[] l_Position);  




}
