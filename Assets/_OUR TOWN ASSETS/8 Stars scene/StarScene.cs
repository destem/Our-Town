using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScene : MonoBehaviour
{

    public Texture2D full;
    public Texture2D[] stars;
    public Texture2D startMask;

    public Texture2D maskOneTex;
    public Texture2D maskTwoTex;
    public Material displayMat;
    public Material growMat;
    public Material fadeInMat;
    public Material fadeOutMat;
    public Material starBlitMat;

    //public GameObject screenModel;
    public Texture2D black;
    public float slowSpeed = 0.05f;
    public float mediumSpeed = 0.5f;
    public float fastSpeed = 10f;
    public int iterations = 5;
    public float growthThreshhold = 2f;
    Material growMat2;
    RenderTexture buff;
    RenderTexture buff2;
    RenderTexture finalBuff;
    RenderTexture final;
    RenderTexture final2;
    float brushSize = 2f;
    OurTownGestureListener gesture;
    bool next = false;
    bool usingGrowth = true;
    enum StarRenderType { FadeIn, Grow, BlitStars, FadeOut}
    StarRenderType starRender = StarRenderType.FadeIn;
    int clusterSize = 1;
    float blitTimeDelay = 0.5f;

    Color32[] starBlock;
    Texture2D dynamicBackground;
    Vector2[] coordArray;
    List<Vector2> coordList;
    System.Random rng = new System.Random();

    List<Vector2> Shuffle(List<Vector2> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector2 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    RenderTexture _createTexture(int w, int h)
    {
        RenderTexture t = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
        t.filterMode = FilterMode.Point;
        t.anisoLevel = 1;
        t.autoGenerateMips = false;
        t.useMipMap = false;
        t.Create();
        return t;
    }

    // Use this for initialization
    void Start()
    {
        rng = new System.Random();
        coordArray = new Vector2[8192 / 16 * 1024 / 16];
        for (int i = 0; i < coordArray.Length; i++)
        {
            coordArray[i] = new Vector2((i % 512) * 16, (i / 512) * 16);
        }
        coordList = new List<Vector2>(coordArray);
        coordList = Shuffle(coordList);
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        fadeOutMat.SetVector("_Value", Vector4.zero);
        fadeInMat.SetVector("_Value", Vector4.zero);
        clusterSize = 1;
        blitTimeDelay = 0.5f;
        usingGrowth = true;
        starRender = StarRenderType.FadeIn;
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        dynamicBackground = Instantiate(full);
        buff = _createTexture(startMask.width, startMask.height);
        buff2 = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        final2 = _createTexture(startMask.width, startMask.height);
        finalBuff = _createTexture(startMask.width, startMask.height); 
        displayMat.SetTexture("_MainTex", buff);
        displayMat.SetTexture("_SecondTex", buff2);
        fadeOutMat.SetTexture("_Paper", finalBuff);
        growMat.SetTexture("_MaskOneTex", maskOneTex);
        growMat.SetTexture("_MaskTwoTex", maskTwoTex);
        ResetUVs();

        StartCoroutine(RunScene());
    }

    // Update is called once per frame
    void Update()
    {
        ResetUVs();
        next = false;

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        switch (starRender)
        {
            case StarRenderType.Grow:
                for (int i = 0; i < iterations; i++)
                {
                    Graphics.Blit(buff, final, growMat);
                    Graphics.Blit(final, buff, growMat);

                }
                Graphics.Blit(buff, dest, displayMat);
                //Graphics.Blit(buff, dest);
                break;
            case StarRenderType.BlitStars:
                Graphics.Blit(dynamicBackground, dest, starBlitMat);
                break;
            case StarRenderType.FadeIn:
                Graphics.Blit(source, dest, fadeInMat);
                break;
            case StarRenderType.FadeOut:
                Graphics.Blit(dynamicBackground, finalBuff, starBlitMat);
                Graphics.Blit(source, dest, fadeOutMat);
                break;
        }
       

    }

    IEnumerator RunScene()
    {
        yield return new WaitForSeconds(2f); //gesture not getting initialized fast enough??
        gesture.SetCurrentGesture(KinectGestures.Gestures.Behold);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        float startTime = Time.time;
        float fadeDuration = 0.5f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeInMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeInMat.SetVector("_Value", Vector4.one);
        starRender = StarRenderType.Grow;
        StartCoroutine("StartStarfield");

        yield return new WaitForSeconds(10f);
        StartCoroutine("MillionYears");

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        //usingGrowth = false;
        starRender = StarRenderType.BlitStars;
        StartCoroutine("MakeStars");

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        blitTimeDelay = .2f;
        //clusterSize = 2;

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        blitTimeDelay = 0f;
        //clusterSize = 4;

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        clusterSize = 4;

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        clusterSize = 20;

        yield return new WaitForSeconds(10f);
        starRender = StarRenderType.FadeOut;
        startTime = Time.time;
        fadeDuration = 10f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeOutMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeOutMat.SetVector("_Value", Vector4.one);
    }

    IEnumerator StartStarfield()
    {
        print("Start starfield");
        float[] coords = { 0.469f, 0.392f, 0.479f, 0.454f, 0.489f, 0.439f, 0.499f, 0.413f, 0.509f, 0.392f,
                           0.505f, 0.345f, 0.497f, 0.329f, 0.490f, 0.298f, 0.475f, 0.277f, 0.467f, 0.314f,
                           0.080f, 0.864f, 0.058f, 0.685f, 0.083f, 0.376f, 0.126f, 0.450f, 0.127f, 0.811f,
                           0.188f, 0.837f, 0.196f, 0.445f, 0.144f, 0.277f, 0.198f, 0.115f, 0.256f, 0.365f,
                           0.229f, 0.638f, 0.264f, 0.826f, 0.323f, 0.801f, 0.295f, 0.596f, 0.320f, 0.340f,
                           0.292f, 0.178f, 0.351f, 0.125f, 0.349f, 0.365f, 0.393f, 0.628f, 0.412f, 0.832f,
                           0.486f, 0.826f, 0.442f, 0.204f, 0.541f, 0.198f, 0.548f, 0.759f, 0.513f, 0.648f,
                           0.601f, 0.937f, 0.708f, 0.670f, 0.631f, 0.465f, 0.575f, 0.173f, 0.590f, 0.570f,
                           0.650f, 0.298f, 0.717f, 0.125f, 0.792f, 0.392f, 0.754f, 0.633f, 0.715f, 0.759f,
                           0.726f, 0.471f, 0.803f, 0.864f, 0.819f, 0.277f, 0.882f, 0.287f, 0.860f, 0.460f,
                           0.893f, 0.842f, 0.933f, 0.303f, 0.955f, 0.779f, 0.979f, 0.272f, 0.336f, 0.601f,
                           0.256f, 0.785f, 0.302f, 0.309f, 0.183f, 0.365f, 0.148f, 0.717f, 0.196f, 0.837f,
                           0.099f, 0.235f, 0.547f, 0.675f, 0.479f, 0.670f, 0.438f, 0.770f, 0.379f, 0.225f,
                           0.342f, 0.859f, 0.721f, 0.900f, 0.728f, 0.628f, 0.735f, 0.251f, 0.804f, 0.570f,
                           0.842f, 0.309f, 0.987f, 0.633f, 0.987f, 0.376f, 0.982f, 0.182f, 0.838f, 0.868f,
                           0.714f, 0.895f, 0.732f, 0.151f, 0.542f, 0.554f, 0.499f, 0.099f, 0.417f, 0.968f,
                           0.004f, 0.953f, 0.037f, 0.198f, 0.016f, 0.178f, 0.159f, 0.879f, 0.130f, 0.361f,
                           0.663f, 0.968f, 0.648f, 0.884f, 0.032f, 0.968f, 0.018f, 0.801f, 0.693f, 0.570f};
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return null;
        }
    }

    IEnumerator MillionYears()
    {
        print("Million years");
        float[] coords = { 0.471f, 0.382f, 0.478f, 0.389f, 0.495f, 0.372f, 0.504f, 0.379f, 0.475f, 0.325f,
                           0.479f, 0.329f, 0.494f, 0.314f };
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return null;
        }
    }

    IEnumerator MakeStars()
    {
        int n = coordList.Count - 1;
        while (n > 0)
        {
            int cluster = n > clusterSize ? clusterSize : n;
            for (int i = 0; i < cluster; i++)
            {
                BlitStar((int)coordList[n].x, (int)coordList[n].y);
                n--;
            }
            dynamicBackground.Apply();
            if (blitTimeDelay > 0f)
            {
                yield return new WaitForSeconds(blitTimeDelay);
            }
            else yield return null;
        }
    }

    public void SetMaskOne(float u, float v)
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskTwo(float u, float v)
    {
        growMat.SetVector("_MaskTwoCoords", new Vector4(u, v, brushSize, -1f));
    }


    public void ResetUVs()
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
    }

    void BlitStar(int x, int y)
    {
        starBlock = stars[Random.Range(0, 3)].GetPixels32();
        Color32[] blendedBlock = new Color32[1024];
        //loop through 32 x 32, get bg color, combine with corresponding star color
        for (int j = 0; j < 32; j++)
        {
            for (int i = 0; i < 32; i++)
            {
                Color bgColor = dynamicBackground.GetPixel(x + i, y + j);
                Color32 starColor = starBlock[j * 32 + i];
                Color blendedColor = new Color(starColor.r / 255f * starColor.a / 255f + bgColor.r * (1 - starColor.a / 255f), starColor.g / 255f * starColor.a / 255f + bgColor.g * (1 - starColor.a / 255f), starColor.b / 255f * starColor.a / 255f + bgColor.b * (1 - starColor.a / 255f));


                blendedBlock[j * 32 + i] = (Color32)blendedColor;
            }
        }

        dynamicBackground.SetPixels32(x, y, 32, 32, blendedBlock);

    }

    void OnDisable()
    {
        Reset();
    }
}

