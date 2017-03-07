using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D black;

    public Texture2D paper;
    public Texture2D full;
    public Texture2D rotatedTown;
    public Texture2D wordsAndRain;
    public Texture2D wordsOnly;
    public Texture2D fourPhrases;

    public Material growMat;
    public Material displayMat;
    public Material wipeMat;
    public Material fadeMat;
    public Material wordFade;

    Material growMat2;
    RenderTexture buff;
    RenderTexture buff2;
    RenderTexture final;
    RenderTexture final2;

    float brushSize = 10f;
    OurTownGestureListener gesture;
    public float slowSpeed = 0f;
    public float mediumSpeed = 0f;
    public float fastSpeed = 0f;
    public int iterations = 1;
    public float growthThreshhold = 1f;
    bool next = false;
    bool usingGrowth = true;
    bool usingWipe = false;
    enum RainRenderType {FadeIn, Growth, HalfWipe, FadeWords, RotatedTown }
    RainRenderType rainRender = RainRenderType.FadeIn;

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
    void Start () {
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        usingGrowth = true;
        usingWipe = false;
        rainRender = RainRenderType.FadeIn;
        fadeMat.SetVector("_Value", Vector4.zero);
        wordFade.SetVector("_Value", Vector4.zero);
        wipeMat.SetFloat("_Value", -1f);
        growMat2 = new Material(growMat);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        growMat2.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));

        buff = _createTexture(startMask.width, startMask.height);
        buff2 = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        final2 = _createTexture(startMask.width, startMask.height);
        displayMat.SetTexture("_MainTex", buff);
        displayMat.SetTexture("_SecondTex", buff2);
        ResetUVs();
        fadeMat.SetTexture("_Chapel", paper);
        fadeMat.SetTexture("_Paper", black);

        growMat.SetTexture("_MaskOneTex", black);
        growMat.SetTexture("_MaskTwoTex", black);
        growMat2.SetTexture("_MaskOneTex", black);
        growMat2.SetTexture("_MaskTwoTex", black);
        StartCoroutine(RunScene());
    }

    // Update is called once per frame
    void Update () {
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
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
        switch (rainRender) {
            case RainRenderType.Growth:
            {
                for (int i = 0; i < iterations; i++)
                {
                    Graphics.Blit(buff, final, growMat);
                    Graphics.Blit(final, buff, growMat);
                    Graphics.Blit(buff2, final2, growMat2);
                    Graphics.Blit(final2, buff2, growMat2);
                }
                Graphics.Blit(buff, dest, displayMat);
                //Graphics.Blit(buff, dest);
                ResetUVs();
                break;
            }
            case RainRenderType.HalfWipe:
            {
                Graphics.Blit(source, dest, wipeMat);
                break;
            }
            case RainRenderType.RotatedTown:
            {
                Graphics.Blit(rotatedTown, dest);
                break;
            }
            case RainRenderType.FadeIn:
            {
                Graphics.Blit(source, dest, fadeMat);
                break;
            }
            case RainRenderType.FadeWords:
            {
                Graphics.Blit(source, dest, wordFade);
                break;
            }
        }

    }

    IEnumerator RunScene()
    {
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??

        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        growMat2.SetTexture("_MaskTwoTex", MaskFourTex);

        
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;

        float startTime = Time.time;
        float fadeDuration = 0.5f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeMat.SetVector("_Value", Vector4.one);
        rainRender = RainRenderType.Growth;
        StartCoroutine("FirstRain");

        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanForward);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("SecondRain");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Jump);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("LastRain");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Shrug);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("Houses");

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        //usingGrowth = false;
        // usingWipe = true;
        //rainRender = RainRenderType.HalfWipe;
        rainRender = RainRenderType.FadeIn;
        //wipeMat.SetFloat("_Value", -1f);
        fadeMat.SetVector("_Value", Vector4.zero);
        fadeMat.SetTexture("_Chapel", wordsAndRain);
        fadeMat.SetTexture("_Paper", full);
        startTime = Time.time;
        fadeDuration = 2f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f ,0f ,0f));
            yield return null;
        }
        fadeMat.SetVector("_Value", Vector4.one);

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        fadeMat.SetVector("_Value", Vector4.zero);
        fadeMat.SetTexture("_Chapel", wordsOnly);
        fadeMat.SetTexture("_Paper", wordsAndRain);
        startTime = Time.time;
        fadeDuration = 2f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeMat.SetVector("_Value", Vector4.one);

        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanForward);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        rainRender = RainRenderType.FadeWords;
        startTime = Time.time;
        fadeDuration = 2f;
        while (Time.time - startTime < fadeDuration)
        {
            wordFade.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        wordFade.SetVector("_Value", Vector4.one);
        gesture.SetCurrentGesture(KinectGestures.Gestures.Psi);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        rainRender = RainRenderType.RotatedTown;
        print("rotated town");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Clap);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        print("go to chapel");
        OurTownManager.GotoTree();
    }

    IEnumerator FirstRain()
    {
        print("First rain");
        float[] coords = { 0.028f, 0.728f, 0.028f, 0.653f, 0.032f, 0.406f, 0.019f, 0.226f, 0.043f, 0.504f,
                           0.048f, 0.594f, 0.049f, 0.419f, 0.049f, 0.393f, 0.049f, 0.363f, 0.059f, 0.622f,
                           0.059f, 0.827f, 0.059f, 0.185f, 0.062f, 0.116f, 0.078f, 0.592f, 0.078f, 0.707f,
                           0.080f, 0.160f, 0.092f, 0.206f, 0.089f, 0.319f, 0.089f, 0.352f, 0.092f, 0.519f,
                           0.089f, 0.623f, 0.091f, 0.714f, 0.094f, 0.880f, 0.098f, 0.746f, 0.099f, 0.704f,
                           0.099f, 0.651f, 0.099f, 0.564f, 0.100f, 0.314f, 0.100f, 0.134f, 0.100f, 0.036f,
                           0.113f, 0.985f, 0.112f, 0.715f, 0.113f, 0.499f, 0.113f, 0.242f, 0.126f, 0.672f,
                           0.126f, 0.867f, 0.126f, 0.533f, 0.126f, 0.316f, 0.126f, 0.174f, 0.126f, 0.083f,
                           0.134f, 0.975f, 0.134f, 0.947f, 0.134f, 0.901f, 0.134f, 0.865f, 0.134f, 0.697f,
                           0.135f, 0.534f, 0.134f, 0.427f, 0.135f, 0.251f, 0.134f, 0.135f, 0.134f, 0.020f,
                           0.143f, 0.972f, 0.143f, 0.942f, 0.143f, 0.901f, 0.143f, 0.835f, 0.143f, 0.765f,
                           0.143f, 0.717f, 0.143f, 0.666f, 0.143f, 0.570f, 0.144f, 0.488f, 0.143f, 0.446f,
                           0.143f, 0.397f, 0.143f, 0.313f, 0.143f, 0.251f, 0.143f, 0.162f, 0.143f, 0.105f,
                           0.143f, 0.045f, 0.143f, 0.015f, 0.151f, 0.926f, 0.151f, 0.861f, 0.151f, 0.803f,
                           0.151f, 0.714f, 0.151f, 0.641f, 0.151f, 0.546f, 0.151f, 0.469f, 0.151f, 0.345f,
                           0.151f, 0.131f, 0.169f, 0.879f, 0.169f, 0.806f, 0.169f, 0.765f, 0.170f, 0.527f,
                           0.170f, 0.488f, 0.170f, 0.389f, 0.170f, 0.326f, 0.170f, 0.268f, 0.170f, 0.222f,
                           0.170f, 0.167f, 0.170f, 0.096f, 0.170f, 0.042f, 0.190f, 0.994f, 0.191f, 0.937f,
                           0.190f, 0.848f, 0.191f, 0.721f, 0.190f, 0.622f, 0.190f, 0.482f, 0.190f, 0.371f,
                           0.190f, 0.292f, 0.191f, 0.196f, 0.200f, 0.810f, 0.201f, 0.666f, 0.201f, 0.610f,
                           0.201f, 0.469f, 0.200f, 0.370f, 0.200f, 0.273f, 0.200f, 0.175f, 0.200f, 0.099f,
                           0.201f, 0.028f, 0.208f, 0.942f, 0.208f, 0.851f, 0.209f, 0.778f, 0.209f, 0.629f,
                           0.209f, 0.494f, 0.209f, 0.379f, 0.209f, 0.274f, 0.209f, 0.156f, 0.209f, 0.047f,
                           0.227f, 0.996f, 0.227f, 0.856f, 0.227f, 0.786f, 0.227f, 0.707f, 0.227f, 0.601f,
                           0.227f, 0.495f, 0.228f, 0.389f, 0.227f, 0.275f, 0.247f, 0.906f, 0.247f, 0.768f,
                           0.247f, 0.598f, 0.247f, 0.508f, 0.247f, 0.433f, 0.247f, 0.311f, 0.246f, 0.177f,
                           0.282f, 0.925f, 0.282f, 0.894f, 0.282f, 0.861f, 0.282f, 0.834f, 0.282f, 0.721f,
                           0.282f, 0.496f, 0.283f, 0.305f, 0.286f, 0.354f, 0.287f, 0.611f, 0.320f, 0.955f,
                           0.321f, 0.806f, 0.321f, 0.698f, 0.321f, 0.373f, 0.321f, 0.314f, 0.338f, 0.869f,
                           0.338f, 0.928f, 0.342f, 0.520f, 0.342f, 0.293f, 0.345f, 0.244f, 0.367f, 0.875f,
                           0.369f, 0.556f, 0.369f, 0.286f, 0.369f, 0.108f, 0.390f, 0.922f, 0.391f, 0.865f,
                           0.391f, 0.749f, 0.391f, 0.579f, 0.391f, 0.310f, 0.425f, 0.812f, 0.425f, 0.712f,
                           0.426f, 0.553f, 0.426f, 0.484f, 0.426f, 0.301f, 0.426f, 0.200f, 0.425f, 0.117f,
                           0.454f, 0.929f, 0.454f, 0.674f, 0.454f, 0.537f, 0.454f, 0.397f, 0.453f, 0.178f,
                           0.457f, 0.099f, 0.476f, 0.989f, 0.478f, 0.783f, 0.476f, 0.698f, 0.476f, 0.587f,
                           0.479f, 0.463f, 0.478f, 0.380f, 0.477f, 0.358f, 0.476f, 0.143f, 0.483f, 0.167f,
                           0.504f, 0.955f, 0.505f, 0.764f, 0.505f, 0.563f, 0.505f, 0.352f, 0.505f, 0.166f,
                           0.522f, 0.987f, 0.522f, 0.668f, 0.523f, 0.596f, 0.522f, 0.495f, 0.523f, 0.241f,
                           0.523f, 0.178f, 0.536f, 0.998f, 0.536f, 0.939f, 0.536f, 0.871f, 0.536f, 0.836f,
                           0.536f, 0.741f, 0.536f, 0.699f, 0.538f, 0.563f, 0.538f, 0.469f, 0.538f, 0.418f,
                           0.538f, 0.327f, 0.537f, 0.211f, 0.537f, 0.144f, 0.537f, 0.051f, 0.560f, 0.989f,
                           0.560f, 0.835f, 0.560f, 0.788f, 0.561f, 0.715f, 0.561f, 0.639f, 0.561f, 0.449f,
                           0.560f, 0.349f, 0.561f, 0.167f, 0.560f, 0.040f, 0.581f, 0.999f, 0.581f, 0.962f,
                           0.581f, 0.880f, 0.581f, 0.834f, 0.581f, 0.748f, 0.581f, 0.651f, 0.581f, 0.570f,
                           0.581f, 0.525f, 0.581f, 0.461f, 0.581f, 0.304f, 0.581f, 0.254f, 0.581f, 0.065f,
                           0.582f, 0.016f, 0.603f, 0.994f, 0.603f, 0.947f, 0.603f, 0.767f, 0.603f, 0.684f,
                           0.603f, 0.518f, 0.604f, 0.389f, 0.604f, 0.218f, 0.604f, 0.090f, 0.604f, 0.036f,
                           0.620f, 0.996f, 0.620f, 0.761f, 0.621f, 0.661f, 0.619f, 0.407f, 0.621f, 0.301f,
                           0.621f, 0.234f, 0.621f, 0.180f, 0.621f, 0.090f, 0.621f, 0.030f, 0.647f, 0.975f,
                           0.647f, 0.802f, 0.647f, 0.702f, 0.647f, 0.566f, 0.647f, 0.478f, 0.647f, 0.286f,
                           0.647f, 0.179f, 0.647f, 0.056f, 0.674f, 0.986f, 0.673f, 0.861f, 0.673f, 0.791f,
                           0.673f, 0.699f, 0.673f, 0.551f, 0.674f, 0.450f, 0.673f, 0.357f, 0.674f, 0.275f,
                           0.673f, 0.167f, 0.700f, 0.996f, 0.700f, 0.816f, 0.701f, 0.725f, 0.701f, 0.678f,
                           0.701f, 0.522f, 0.701f, 0.327f, 0.701f, 0.218f, 0.701f, 0.118f, 0.701f, 0.008f,
                           0.740f, 0.979f, 0.739f, 0.772f, 0.739f, 0.521f, 0.739f, 0.459f, 0.740f, 0.389f,
                           0.740f, 0.286f, 0.740f, 0.253f, 0.740f, 0.185f, 0.740f, 0.108f, 0.740f, 0.017f,
                           0.763f, 0.988f, 0.764f, 0.818f, 0.764f, 0.721f, 0.765f, 0.610f, 0.765f, 0.405f,
                           0.765f, 0.273f, 0.766f, 0.213f, 0.766f, 0.123f, 0.765f, 0.028f, 0.790f, 0.996f,
                           0.791f, 0.815f, 0.791f, 0.767f, 0.791f, 0.651f, 0.791f, 0.557f, 0.791f, 0.475f,
                           0.791f, 0.397f, 0.792f, 0.301f, 0.792f, 0.197f, 0.792f, 0.169f, 0.818f, 0.981f,
                           0.819f, 0.829f, 0.821f, 0.701f, 0.820f, 0.578f, 0.820f, 0.419f, 0.823f, 0.317f,
                           0.820f, 0.253f, 0.823f, 0.188f, 0.859f, 0.995f, 0.859f, 0.908f, 0.859f, 0.790f,
                           0.860f, 0.735f, 0.860f, 0.548f, 0.861f, 0.379f, 0.860f, 0.156f, 0.860f, 0.074f,
                           0.898f, 0.992f, 0.898f, 0.924f, 0.898f, 0.733f, 0.900f, 0.558f, 0.900f, 0.394f,
                           0.902f, 0.303f, 0.904f, 0.186f, 0.904f, 0.109f, 0.905f, 0.053f, 0.931f, 0.996f,
                           0.931f, 0.929f, 0.932f, 0.881f, 0.932f, 0.830f, 0.932f, 0.722f, 0.931f, 0.375f,
                           0.931f, 0.173f, 0.958f, 0.980f, 0.959f, 0.816f, 0.959f, 0.661f, 0.959f, 0.587f,
                           0.960f, 0.371f, 0.962f, 0.183f, 0.962f, 0.090f, 0.978f, 0.917f, 0.978f, 0.747f,
                           0.978f, 0.624f, 0.981f, 0.362f, 0.981f, 0.255f, 0.981f, 0.126f, 0.981f, 0.034f,
                           0.997f, 0.974f, 0.997f, 0.859f, 0.997f, 0.701f, 0.997f, 0.585f, 0.997f, 0.382f,
                           0.997f, 0.244f, 0.998f, 0.092f };
        for (int i = 0; i < coords.Length; i += 4)
        {
            SetMaskThree(coords[i], coords[i + 1]);
            yield return null;
        }
        float wordDelay = 2f;
        float[] coords2 = { 0.443f, 0.263f, 0.258f, 0.042f, 0.261f, 0.510f, 0.348f, 0.751f, 0.343f, 0.617f,
                           0.261f, 0.991f, 0.933f, 0.255f };
        print("first words");
        for (int i = 0; i < coords2.Length; i += 2)
        {
            SetMaskOne(coords2[i], coords2[i + 1]);
            yield return new WaitForSeconds(wordDelay);
        }

    }

    IEnumerator SecondRain()
    {
        print("second rain");
        float[] coords = { 0.028f, 0.728f, 0.028f, 0.653f, 0.032f, 0.406f, 0.019f, 0.226f, 0.043f, 0.504f,
                           0.048f, 0.594f, 0.049f, 0.419f, 0.049f, 0.393f, 0.049f, 0.363f, 0.059f, 0.622f,
                           0.059f, 0.827f, 0.059f, 0.185f, 0.062f, 0.116f, 0.078f, 0.592f, 0.078f, 0.707f,
                           0.080f, 0.160f, 0.092f, 0.206f, 0.089f, 0.319f, 0.089f, 0.352f, 0.092f, 0.519f,
                           0.089f, 0.623f, 0.091f, 0.714f, 0.094f, 0.880f, 0.098f, 0.746f, 0.099f, 0.704f,
                           0.099f, 0.651f, 0.099f, 0.564f, 0.100f, 0.314f, 0.100f, 0.134f, 0.100f, 0.036f,
                           0.113f, 0.985f, 0.112f, 0.715f, 0.113f, 0.499f, 0.113f, 0.242f, 0.126f, 0.672f,
                           0.126f, 0.867f, 0.126f, 0.533f, 0.126f, 0.316f, 0.126f, 0.174f, 0.126f, 0.083f,
                           0.134f, 0.975f, 0.134f, 0.947f, 0.134f, 0.901f, 0.134f, 0.865f, 0.134f, 0.697f,
                           0.135f, 0.534f, 0.134f, 0.427f, 0.135f, 0.251f, 0.134f, 0.135f, 0.134f, 0.020f,
                           0.143f, 0.972f, 0.143f, 0.942f, 0.143f, 0.901f, 0.143f, 0.835f, 0.143f, 0.765f,
                           0.143f, 0.717f, 0.143f, 0.666f, 0.143f, 0.570f, 0.144f, 0.488f, 0.143f, 0.446f,
                           0.143f, 0.397f, 0.143f, 0.313f, 0.143f, 0.251f, 0.143f, 0.162f, 0.143f, 0.105f,
                           0.143f, 0.045f, 0.143f, 0.015f, 0.151f, 0.926f, 0.151f, 0.861f, 0.151f, 0.803f,
                           0.151f, 0.714f, 0.151f, 0.641f, 0.151f, 0.546f, 0.151f, 0.469f, 0.151f, 0.345f,
                           0.151f, 0.131f, 0.169f, 0.879f, 0.169f, 0.806f, 0.169f, 0.765f, 0.170f, 0.527f,
                           0.170f, 0.488f, 0.170f, 0.389f, 0.170f, 0.326f, 0.170f, 0.268f, 0.170f, 0.222f,
                           0.170f, 0.167f, 0.170f, 0.096f, 0.170f, 0.042f, 0.190f, 0.994f, 0.191f, 0.937f,
                           0.190f, 0.848f, 0.191f, 0.721f, 0.190f, 0.622f, 0.190f, 0.482f, 0.190f, 0.371f,
                           0.190f, 0.292f, 0.191f, 0.196f, 0.200f, 0.810f, 0.201f, 0.666f, 0.201f, 0.610f,
                           0.201f, 0.469f, 0.200f, 0.370f, 0.200f, 0.273f, 0.200f, 0.175f, 0.200f, 0.099f,
                           0.201f, 0.028f, 0.208f, 0.942f, 0.208f, 0.851f, 0.209f, 0.778f, 0.209f, 0.629f,
                           0.209f, 0.494f, 0.209f, 0.379f, 0.209f, 0.274f, 0.209f, 0.156f, 0.209f, 0.047f,
                           0.227f, 0.996f, 0.227f, 0.856f, 0.227f, 0.786f, 0.227f, 0.707f, 0.227f, 0.601f,
                           0.227f, 0.495f, 0.228f, 0.389f, 0.227f, 0.275f, 0.247f, 0.906f, 0.247f, 0.768f,
                           0.247f, 0.598f, 0.247f, 0.508f, 0.247f, 0.433f, 0.247f, 0.311f, 0.246f, 0.177f,
                           0.282f, 0.925f, 0.282f, 0.894f, 0.282f, 0.861f, 0.282f, 0.834f, 0.282f, 0.721f,
                           0.282f, 0.496f, 0.283f, 0.305f, 0.286f, 0.354f, 0.287f, 0.611f, 0.320f, 0.955f,
                           0.321f, 0.806f, 0.321f, 0.698f, 0.321f, 0.373f, 0.321f, 0.314f, 0.338f, 0.869f,
                           0.338f, 0.928f, 0.342f, 0.520f, 0.342f, 0.293f, 0.345f, 0.244f, 0.367f, 0.875f,
                           0.369f, 0.556f, 0.369f, 0.286f, 0.369f, 0.108f, 0.390f, 0.922f, 0.391f, 0.865f,
                           0.391f, 0.749f, 0.391f, 0.579f, 0.391f, 0.310f, 0.425f, 0.812f, 0.425f, 0.712f,
                           0.426f, 0.553f, 0.426f, 0.484f, 0.426f, 0.301f, 0.426f, 0.200f, 0.425f, 0.117f,
                           0.454f, 0.929f, 0.454f, 0.674f, 0.454f, 0.537f, 0.454f, 0.397f, 0.453f, 0.178f,
                           0.457f, 0.099f, 0.476f, 0.989f, 0.478f, 0.783f, 0.476f, 0.698f, 0.476f, 0.587f,
                           0.479f, 0.463f, 0.478f, 0.380f, 0.477f, 0.358f, 0.476f, 0.143f, 0.483f, 0.167f,
                           0.504f, 0.955f, 0.505f, 0.764f, 0.505f, 0.563f, 0.505f, 0.352f, 0.505f, 0.166f,
                           0.522f, 0.987f, 0.522f, 0.668f, 0.523f, 0.596f, 0.522f, 0.495f, 0.523f, 0.241f,
                           0.523f, 0.178f, 0.536f, 0.998f, 0.536f, 0.939f, 0.536f, 0.871f, 0.536f, 0.836f,
                           0.536f, 0.741f, 0.536f, 0.699f, 0.538f, 0.563f, 0.538f, 0.469f, 0.538f, 0.418f,
                           0.538f, 0.327f, 0.537f, 0.211f, 0.537f, 0.144f, 0.537f, 0.051f, 0.560f, 0.989f,
                           0.560f, 0.835f, 0.560f, 0.788f, 0.561f, 0.715f, 0.561f, 0.639f, 0.561f, 0.449f,
                           0.560f, 0.349f, 0.561f, 0.167f, 0.560f, 0.040f, 0.581f, 0.999f, 0.581f, 0.962f,
                           0.581f, 0.880f, 0.581f, 0.834f, 0.581f, 0.748f, 0.581f, 0.651f, 0.581f, 0.570f,
                           0.581f, 0.525f, 0.581f, 0.461f, 0.581f, 0.304f, 0.581f, 0.254f, 0.581f, 0.065f,
                           0.582f, 0.016f, 0.603f, 0.994f, 0.603f, 0.947f, 0.603f, 0.767f, 0.603f, 0.684f,
                           0.603f, 0.518f, 0.604f, 0.389f, 0.604f, 0.218f, 0.604f, 0.090f, 0.604f, 0.036f,
                           0.620f, 0.996f, 0.620f, 0.761f, 0.621f, 0.661f, 0.619f, 0.407f, 0.621f, 0.301f,
                           0.621f, 0.234f, 0.621f, 0.180f, 0.621f, 0.090f, 0.621f, 0.030f, 0.647f, 0.975f,
                           0.647f, 0.802f, 0.647f, 0.702f, 0.647f, 0.566f, 0.647f, 0.478f, 0.647f, 0.286f,
                           0.647f, 0.179f, 0.647f, 0.056f, 0.674f, 0.986f, 0.673f, 0.861f, 0.673f, 0.791f,
                           0.673f, 0.699f, 0.673f, 0.551f, 0.674f, 0.450f, 0.673f, 0.357f, 0.674f, 0.275f,
                           0.673f, 0.167f, 0.700f, 0.996f, 0.700f, 0.816f, 0.701f, 0.725f, 0.701f, 0.678f,
                           0.701f, 0.522f, 0.701f, 0.327f, 0.701f, 0.218f, 0.701f, 0.118f, 0.701f, 0.008f,
                           0.740f, 0.979f, 0.739f, 0.772f, 0.739f, 0.521f, 0.739f, 0.459f, 0.740f, 0.389f,
                           0.740f, 0.286f, 0.740f, 0.253f, 0.740f, 0.185f, 0.740f, 0.108f, 0.740f, 0.017f,
                           0.763f, 0.988f, 0.764f, 0.818f, 0.764f, 0.721f, 0.765f, 0.610f, 0.765f, 0.405f,
                           0.765f, 0.273f, 0.766f, 0.213f, 0.766f, 0.123f, 0.765f, 0.028f, 0.790f, 0.996f,
                           0.791f, 0.815f, 0.791f, 0.767f, 0.791f, 0.651f, 0.791f, 0.557f, 0.791f, 0.475f,
                           0.791f, 0.397f, 0.792f, 0.301f, 0.792f, 0.197f, 0.792f, 0.169f, 0.818f, 0.981f,
                           0.819f, 0.829f, 0.821f, 0.701f, 0.820f, 0.578f, 0.820f, 0.419f, 0.823f, 0.317f,
                           0.820f, 0.253f, 0.823f, 0.188f, 0.859f, 0.995f, 0.859f, 0.908f, 0.859f, 0.790f,
                           0.860f, 0.735f, 0.860f, 0.548f, 0.861f, 0.379f, 0.860f, 0.156f, 0.860f, 0.074f,
                           0.898f, 0.992f, 0.898f, 0.924f, 0.898f, 0.733f, 0.900f, 0.558f, 0.900f, 0.394f,
                           0.902f, 0.303f, 0.904f, 0.186f, 0.904f, 0.109f, 0.905f, 0.053f, 0.931f, 0.996f,
                           0.931f, 0.929f, 0.932f, 0.881f, 0.932f, 0.830f, 0.932f, 0.722f, 0.931f, 0.375f,
                           0.931f, 0.173f, 0.958f, 0.980f, 0.959f, 0.816f, 0.959f, 0.661f, 0.959f, 0.587f,
                           0.960f, 0.371f, 0.962f, 0.183f, 0.962f, 0.090f, 0.978f, 0.917f, 0.978f, 0.747f,
                           0.978f, 0.624f, 0.981f, 0.362f, 0.981f, 0.255f, 0.981f, 0.126f, 0.981f, 0.034f,
                           0.997f, 0.974f, 0.997f, 0.859f, 0.997f, 0.701f, 0.997f, 0.585f, 0.997f, 0.382f,
                           0.997f, 0.244f, 0.998f, 0.092f };
        for (int i = 0; i < coords.Length; i += 4)
        {
            SetMaskThree(coords[i+2], coords[i + 3]);
            yield return null;
        }
        float wordDelay = 2f;
        float[] coords2 = { 0.656f, 0.035f, 0.192f, 0.907f, 0.063f, 0.761f, 0.732f, 0.923f, 0.711f, 0.967f };
        print("second words");
        for (int i = 0; i < coords2.Length; i += 2)
        {
            SetMaskOne(coords2[i], coords2[i + 1]);
            yield return new WaitForSeconds(wordDelay);
        }

    }

    IEnumerator LastRain()
    {
        int[] xvalues = { 42, 13, 88, 23, 40, 26, 97, 74, 30, 16, 93, 75, 49, 78, 71, 94, 91, 56, 73, 20, 98, 82, 76, 45, 41
, 18, 62, 34, 64, 3, 1, 48, 81, 47, 9, 12, 61, 0, 54, 7, 70, 2, 15, 53, 55, 6, 27, 67, 89, 11, 24,
46, 31, 85, 66, 36, 51, 84, 83, 35, 59, 39, 80, 87, 32, 52, 25, 99, 21, 50, 60, 14, 44, 22, 69, 96,
 77, 29, 37, 10, 68, 17, 8, 5, 65, 72, 92, 38, 95, 43, 79, 19, 57, 4, 33, 58, 28, 63, 90, 86};
        for (int i = 0; i < 100; i++)
        {
            // STAGGER THIS
            SetMaskFour(i/100f, .95f);
            //SetMaskFour(xvalues[i]/100f, .95f);
            yield return null;
        }
        float wordDelay = 2f;
        float[] coords = { 0.292f, 0.084f, 0.148f, 0.092f, 0.056f, 0.343f, 0.860f, 0.981f, 0.579f, 0.788f,
                           0.500f, 0.020f, 0.516f, 0.038f };
        print("final rain and words");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(wordDelay);
        }

    }

    IEnumerator Houses()
    {
        float houseDelay = 1f;
        float[] coords = { 0.473f, 0.589f, 0.525f, 0.582f, 0.389f, 0.511f, 0.618f, 0.597f, 0.329f, 0.632f,
                           0.663f, 0.557f, 0.293f, 0.473f, 0.697f, 0.504f, 0.716f, 0.501f, 0.255f, 0.465f,
                           0.761f, 0.697f, 0.204f, 0.405f, 0.807f, 0.348f, 0.166f, 0.462f, 0.132f, 0.738f,
                           0.885f, 0.426f, 0.059f, 0.426f, 0.931f, 0.309f, 0.002f, 0.547f, 0.996f, 0.568f,
                           0.474f, 0.223f, 0.518f, 0.238f, 0.563f, 0.170f, 0.390f, 0.212f, 0.591f, 0.483f,
                           0.635f, 0.106f, 0.338f, 0.192f, 0.293f, 0.195f, 0.274f, 0.348f, 0.249f, 0.163f,
                           0.221f, 0.174f, 0.636f, 0.131f, 0.740f, 0.441f, 0.838f, 0.422f, 0.131f, 0.230f,
                           0.033f, 0.309f };
        print("bring in the houses");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(houseDelay);
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

    public void SetMaskThree(float u, float v)
    {
        growMat2.SetVector("_MaskOneCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskFour(float u, float v)
    {
        growMat2.SetVector("_MaskTwoCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void ResetUVs()
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
        growMat2.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat2.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
    }

    void OnDisable()
    {
        Reset();
    }
}
