using PSPUtil.Attribute;
using PSPUtil.StaticUtil;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

using UnityEngine;


[AddComponentMenu("我的组件/UI/UGUI_Grid(固定每行数)", 20)]     
public class UGUI_GridCount : GridBase              // 这个 Gird 是固定每行的数量
{

    [MyHead("每行有几个 Item")]
#if UNITY_EDITOR
    [MinValue(2)]
#endif
    public ushort HengCount = 3;    

    [MyHead("最少相隔")]
    public Vector2 Spacing = new Vector2(4, 4);


    protected override Vector2 OnUpdate2Setting(float cnavasWidth, int childCount, out int shu, out Vector2[] l_Position)
    {

        int itemWidth = (int)((cnavasWidth - Spacing.x * (HengCount - 1)) / HengCount);  // item 的宽度
        Vector2 itemCallSize = new Vector2(itemWidth, itemWidth);

        shu = childCount / HengCount;
        int yu = childCount % HengCount;       // 如果有余数即竖多一行
        if (yu != 0)
        {
            shu++;
        }


        l_Position = new Vector2[shu * HengCount];

        int index = 0;
        for (int i = 0; i < shu; i++)
        {
            for (int j = 0; j < HengCount; j++)
            {
                l_Position[index] = new Vector2((itemWidth + Spacing.x) * j, (itemWidth + Spacing.y) * -i);
                index++;
            }
        }

        return itemCallSize;

    }
}
