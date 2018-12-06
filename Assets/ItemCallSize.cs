using UnityEngine;

public class ItemCallSize : MonoBehaviour
{


    private float parentX;

    private RectTransform thisRT;
    private RectTransform parentRT;

    private float mWidth, mHeight;

    public void SetSize(float width,float height)
    {
        mWidth = width;
        mHeight = height;
    }



    void Awake()
    {
        thisRT = transform as RectTransform;
        parentRT = (RectTransform) transform.parent;

    }

    void Update()
    {
        float sizeX = parentRT.sizeDelta.x;
        if (sizeX != parentX || thisRT.sizeDelta.x< 10)
        {
            parentX = sizeX;
            if (mWidth > sizeX)
            {
                mWidth = sizeX - 5;
            }
            if (mHeight > sizeX)
            {
                mHeight = sizeX - 5;
            }
            thisRT.sizeDelta = new Vector2(mWidth, mHeight);

        }

    }




}
