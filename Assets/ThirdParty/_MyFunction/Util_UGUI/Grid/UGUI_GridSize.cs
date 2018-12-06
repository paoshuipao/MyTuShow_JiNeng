using PSPUtil.Attribute;
using PSPUtil.StaticUtil;
using UnityEngine;

[AddComponentMenu("我的组件/UI/UGUI_Grid(固定大小)", 19)]
public class UGUI_GridSize : GridBase          // 这个 Gird 已固定了大小
{


    [MyHead("每 Item 多大")]
    public Vector2 CallSize = new Vector2(100, 100);

    [MyHead("相隔")]
    public Vector2 Spacing = new Vector2(5, 5);

    [MyHead("左边边框距离")]
    public float LeftPadding = 0;



    protected override Vector2 OnUpdate2Setting(float cnavasWidth, int childCount, out int shu, out Vector2[] l_Position)
    {

        int hengCount = (int)(cnavasWidth / (CallSize.x + Spacing.x));    // 每行可以占几个

        shu = childCount / hengCount;
        int yu = childCount % hengCount;       // 如果有余数即竖多一行
        if (yu != 0)
        {
            shu++;
        }

        l_Position = new Vector2[shu * hengCount];

        int index = 0;
        for (int i = 0; i < shu; i++)
        {
            for (int j = 0; j < hengCount; j++)
            {
                l_Position[index] = new Vector2(LeftPadding + (CallSize.x + Spacing.x) * j, (CallSize.y + Spacing.y) * -i);
                index++;
            }
        }

        return CallSize;
    }
}
