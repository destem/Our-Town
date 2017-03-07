#define BLIT_TO_SCREEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D black;
    public Texture2D wordsOnly;
    public Texture2D housesOnly;
    public Texture2D paper;
    public Texture2D full;

    public Material growMat;
    public Material displayMat;
    public Material packer;
    public Material imageFade;
    public Material wipeMat;
    Material growMat2;
    RenderTexture buff;
    RenderTexture buff2;
    RenderTexture final;
    RenderTexture final2;
    public Text t;
    float brushSize = 10f;
    OurTownGestureListener gesture;
    //public GameObject screenModel;
    public float slowSpeed = 0f;
    public float mediumSpeed = 0f;
    public float fastSpeed = 0f;
    public int iterations = 1;
    public float growthThreshhold = 1f;
    bool next = false;
    bool usingGrowth = false;
    bool usingWipe = false;
    
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
        //screenModel.GetComponent<Renderer>().material = displayMat;
        //growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));      
        gesture = OurTownGestureListener.Instance;
        Reset();

    }

    void Reset()
    {
        StopAllCoroutines();
        
        usingGrowth = false;
        usingWipe = false;
        wipeMat.SetFloat("_Value", 1.05f);
        growMat2 = new Material(growMat);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        growMat2.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        ResetUVs();

        buff = _createTexture(startMask.width, startMask.height);
        buff2 = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        final2 = _createTexture(startMask.width, startMask.height);
        displayMat.SetTexture("_MainTex", buff);
        displayMat.SetTexture("_SecondTex", buff2);

        growMat.SetTexture("_MaskOneTex", black);
        growMat.SetTexture("_MaskTwoTex", black);
        growMat2.SetTexture("_MaskOneTex", black);
        growMat2.SetTexture("_MaskTwoTex", black);
        //screenModel.GetComponent<Renderer>().material = imageFade;
        imageFade.SetTexture("_Paper", black);
        imageFade.SetTexture("_Chapel", paper);
        imageFade.SetVector("_Value", Vector4.zero);
        //screenModel.GetComponent<Renderer>().material = displayMat;
        StartCoroutine(RunScene());
    }

    // Update is called once per frame
    void Update()
    {
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        //ResetUVs();
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

#if BLIT_TO_SCREEN
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (usingGrowth)
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
        }
        else if (usingWipe)
        {
            Graphics.Blit(buff, dest, wipeMat);
        }
        else
        {
            Graphics.Blit(buff, dest, imageFade);
        }

    }

#endif

    IEnumerator RunScene()
    {
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;

        float startTime = Time.time;
        float fadeDuration = 0.5f;
        while (Time.time - startTime < fadeDuration)
        {
            imageFade.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        imageFade.SetVector("_Value", Vector4.one);
       // screenModel.GetComponent<Renderer>().material = displayMat;
        usingGrowth = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(FirstHouses());
        gesture.SetCurrentGesture(KinectGestures.Gestures.Shrug);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(HouseOutlines());
        
        next = false;
        yield return new WaitForSeconds(2f);
        StartCoroutine(Roofs());
        yield return null;
        gesture.SetCurrentGesture(KinectGestures.Gestures.Clap);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(Landscape());
        yield return null;
        StartCoroutine(Windows());

        yield return new WaitForSeconds(10f);

        StartCoroutine(HouseDetails());
        gesture.SetCurrentGesture(KinectGestures.Gestures.Here);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;

        //FIRE OFF BACKGROUND
        StartCoroutine(BackgroundDetails());
        yield return new WaitForSeconds(20f);
        Debug.Log("full background, no words");
        SetMaskFour(.99f, .99f);
        gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(Words());
        
        gesture.SetCurrentGesture(KinectGestures.Gestures.Clench);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        usingGrowth = false;
        //screenModel.GetComponent<Renderer>().material = imageFade;
        imageFade.SetTexture("_Chapel", wordsOnly);
        imageFade.SetVector("_Value", Vector4.one);

        Debug.Log("Words only");
       // yield return null;
        yield return new WaitForSeconds(2f); // time for professor to speak. 130 in rehearsal
        //Debug.Log("TREES COME IN");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Clap); //pop the town back in
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        Debug.Log("Town comes back");
        imageFade.SetTexture("_Chapel", full);
        gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave); //houses only
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        Debug.Log("JUST THE HOUSES");
        imageFade.SetTexture("_Chapel", housesOnly);
        yield return null;
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow); //wipe
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        Debug.Log("Wipe");
        //TODO: Actually implement the wipe
        usingGrowth = false;
        usingWipe = true;
        wipeMat.SetFloat("_Value", 1.05f);
        startTime = Time.time;
        fadeDuration = 10f;
        while (Time.time - startTime < fadeDuration + .1f)
        {
            wipeMat.SetFloat("_Value", 1 - (Time.time - startTime) / fadeDuration);
            yield return null;
        }
        OurTownManager.GotoMoon();
    }

    IEnumerator FirstHouses()
    {
        Debug.Log("first houses");
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        growMat2.SetTexture("_MaskTwoTex", MaskFourTex);
        SetMaskOne(.465f, .563f);
        yield return new WaitForSeconds(2.0f);
        Blit();
        SetMaskTwo(.464f, .565f);
        yield return new WaitForSeconds(1.5f);
        Blit();
        SetMaskOne(.526f, .565f);
        yield return new WaitForSeconds(2.0f);
        Blit();
        SetMaskTwo(.524f, .557f);
        yield return new WaitForSeconds(1.5f);
        Blit();
        SetMaskOne(0.353f, 0.998f); //sun
    }

    IEnumerator HouseOutlines()
    {
        float outlineDelay = 2f;
        //outlines are all on mask one. starting in lower-right and working left
        float[] coords = { 0.933f, 0.024f, 0.931f, 0.446f, 0.888f, 0.446f, 0.868f, 0.038f,
                           0.848f, 0.158f, 0.859f, 0.630f, 0.823f, 0.596f, 0.786f, 0.699f, 0.740f, 0.024f,
                           0.709f, 0.473f, 0.765f, 0.226f, 0.668f, 0.013f, 0.724f, 0.020f, 0.657f, 0.473f,
                           0.631f, 0.583f, 0.613f, 0.583f, 0.657f, 0.140f, 0.573f, 0.276f, 0.471f, 0.027f,
                           0.491f, 0.027f, 0.461f, 0.099f, 0.414f, 0.013f, 0.370f, 0.566f, 0.417f, 0.504f,
                           0.298f, 0.027f, 0.299f, 0.514f, 0.273f, 0.024f, 0.241f, 0.504f, 0.280f, 0.507f,
                           0.197f, 0.020f, 0.184f, 0.541f, 0.141f, 0.034f, 0.132f, 0.686f, 0.105f, 0.466f,
                           0.078f, 0.579f, 0.074f, 0.253f, 0.025f, 0.363f, 0.034f, 0.404f };
        Debug.Log("starting house outlines");
        for (int i = 0; i< coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(outlineDelay);
        }      
    }

    IEnumerator Roofs()
    {
        float roofDelay = 2f;
        float[] coords = { 0.885f, 0.090f, 0.910f, 0.451f, 0.857f, 0.465f, 0.819f, 0.048f, 0.815f, 0.521f,
                           0.800f, 0.625f, 0.798f, 0.215f, 0.778f, 0.013f, 0.769f, 0.722f, 0.765f, 0.375f,
                           0.745f, 0.013f, 0.724f, 0.465f, 0.709f, 0.479f, 0.666f, 0.101f, 0.689f, 0.302f, 
                           0.706f, 0.236f, 0.661f, 0.521f, 0.625f, 0.010f, 0.629f, 0.333f, 0.622f, 0.848f, 0.571f, 0.285f,
                           0.567f, 0.069f, 0.524f, 0.563f, 0.507f, 0.354f, 0.484f, 0.386f, 0.464f, 0.570f,
                           0.470f, 0.104f, 0.396f, 0.247f, 0.413f, 0.110f, 0.405f, 0.514f, 0.369f, 0.584f,
                           0.319f, 0.618f, 0.297f, 0.034f, 0.297f, 0.521f, 0.272f, 0.006f, 0.255f, 0.180f,
                           0.240f, 0.510f, 0.208f, 0.020f, 0.182f, 0.556f, 0.227f, 0.465f, 0.140f, 0.211f,
                           0.148f, 0.768f, 0.133f, 0.688f, 0.127f, 0.538f, 0.086f, 0.020f, 0.093f, 0.020f,
                           0.112f, 0.013f, 0.082f, 0.612f, 0.085f, 0.663f, 0.073f, 0.267f, 0.025f, 0.375f,
                           0.051f, 0.389f, 0.337f, 0.994f };
        Debug.Log("starting roofs");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(roofDelay);
        }
    }

    IEnumerator Windows()
    {
        float windowDelay = .7f;
        float[] coords = { 0.919f, 0.493f, 0.919f, 0.401f, 0.893f, 0.192f, 0.909f, 0.192f, 0.924f, 0.198f,
                           0.859f, 0.459f, 0.875f, 0.463f, 0.847f, 0.678f, 0.843f, 0.617f, 0.859f, 0.459f,
                           0.828f, 0.013f, 0.841f, 0.010f, 0.858f, 0.013f, 0.789f, 0.030f, 0.776f, 0.761f,
                           0.776f, 0.727f, 0.779f, 0.706f, 0.758f, 0.010f, 0.720f, 0.510f, 0.720f, 0.394f,
                           0.730f, 0.412f, 0.728f, 0.278f, 0.743f, 0.380f, 0.757f, 0.373f, 0.757f, 0.270f,
                           0.773f, 0.284f, 0.697f, 0.236f, 0.678f, 0.212f, 0.678f, 0.055f, 0.715f, 0.164f,
                           0.715f, 0.058f, 0.640f, 0.013f, 0.668f, 0.514f, 0.675f, 0.518f, 0.667f, 0.432f,
                           0.675f, 0.435f, 0.626f, 0.672f, 0.619f, 0.452f, 0.627f, 0.449f, 0.643f, 0.398f,
                           0.635f, 0.322f, 0.641f, 0.322f, 0.647f, 0.329f, 0.652f, 0.326f, 0.588f, 0.349f,
                           0.578f, 0.274f, 0.598f, 0.274f, 0.535f, 0.576f, 0.537f, 0.569f, 0.535f, 0.487f,
                           0.537f, 0.487f, 0.512f, 0.309f, 0.501f, 0.144f, 0.521f, 0.140f, 0.531f, 0.081f,
                           0.564f, 0.086f, 0.475f, 0.579f, 0.475f, 0.466f, 0.442f, 0.069f, 0.423f, 0.061f,
                           0.425f, 0.109f, 0.452f, 0.116f, 0.419f, 0.295f, 0.405f, 0.301f, 0.403f, 0.212f,
                           0.374f, 0.576f, 0.385f, 0.569f, 0.396f, 0.572f, 0.378f, 0.483f, 0.386f, 0.483f,
                           0.300f, 0.130f, 0.305f, 0.078f, 0.316f, 0.137f, 0.316f, 0.078f, 0.328f, 0.137f,
                           0.328f, 0.075f, 0.338f, 0.137f, 0.338f, 0.078f, 0.347f, 0.126f, 0.356f, 0.130f,
                           0.366f, 0.133f, 0.375f, 0.133f, 0.353f, 0.041f, 0.361f, 0.047f, 0.371f, 0.044f,
                           0.380f, 0.041f, 0.312f, 0.541f, 0.303f, 0.412f, 0.320f, 0.415f, 0.328f, 0.583f,
                           0.340f, 0.576f, 0.280f, 0.044f, 0.288f, 0.038f, 0.268f, 0.192f, 0.262f, 0.103f,
                           0.273f, 0.095f, 0.250f, 0.572f, 0.260f, 0.476f, 0.279f, 0.412f, 0.189f, 0.473f,
                           0.203f, 0.192f, 0.211f, 0.192f, 0.218f, 0.195f, 0.220f, 0.095f, 0.233f, 0.095f,
                           0.144f, 0.158f, 0.169f, 0.161f, 0.161f, 0.167f, 0.140f, 0.737f, 0.152f, 0.713f,
                           0.110f, 0.456f, 0.118f, 0.459f, 0.128f, 0.459f, 0.082f, 0.552f, 0.094f, 0.682f,
                           0.089f, 0.621f, 0.089f, 0.558f, 0.094f, 0.562f, 0.098f, 0.624f, 0.098f, 0.558f,
                           0.083f, 0.339f, 0.077f, 0.195f, 0.088f, 0.184f, 0.040f, 0.421f, 0.035f, 0.360f,
                           0.043f, 0.367f, 0.046f, 0.236f, 0.031f, 0.329f, 0.031f, 0.281f };
        Debug.Log("starting windows");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(windowDelay);
        }
    }

    IEnumerator Landscape()
    {
        float landDelay = 5.07f;
        float[] coords = { 0.995f, 0.882f, 0.753f, 0.951f, 0.614f, 0.643f, 0.297f, 0.549f, 0.243f, 0.592f, 0.161f, 0.714f, 0.130f, 0.728f };
        Debug.Log("starting landscape");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(landDelay);
        }
    }

    IEnumerator HouseDetails()
    {
        float detailDelay = 2.09f;
        float[] coords = { 0.930f, 0.459f, 0.928f, 0.237f, 0.860f, 0.573f, 0.823f, 0.474f, 0.841f, 0.154f,
                           0.837f, 0.602f, 0.813f, 0.610f, 0.788f, 0.093f, 0.777f, 0.732f, 0.749f, 0.301f,
                           0.756f, 0.078f, 0.696f, 0.326f, 0.657f, 0.480f, 0.614f, 0.581f, 0.623f, 0.309f,
                           0.587f, 0.283f, 0.512f, 0.387f, 0.536f, 0.674f, 0.476f, 0.674f, 0.439f, 0.294f,
                           0.418f, 0.355f, 0.399f, 0.463f, 0.414f, 0.398f, 0.343f, 0.225f, 0.300f, 0.014f,
                           0.343f, 0.667f, 0.325f, 0.341f, 0.283f, 0.014f, 0.278f, 0.176f, 0.253f, 0.595f,
                           0.249f, 0.459f, 0.245f, 0.104f, 0.210f, 0.258f, 0.183f, 0.563f, 0.196f, 0.541f,
                           0.210f, 0.344f, 0.162f, 0.500f, 0.152f, 0.746f, 0.156f, 0.656f, 0.139f, 0.753f, 0.128f, 0.352f,
                           0.114f, 0.416f, 0.125f, 0.531f, 0.094f, 0.071f, 0.093f, 0.724f, 0.079f, 0.394f, 0.078f, 0.344f,
                           0.085f, 0.237f, 0.030f, 0.445f, 0.026f, 0.301f, 0.034f, 0.294f, 0.036f, 0.258f };
        Debug.Log("starting house details");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskThree(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(detailDelay);
        }
    }

    IEnumerator BackgroundDetails()
    {
        float bgDelay = 2.17f;
        float[] coords = { 0.962f, 0.021f, 0.978f, 0.014f, 0.956f, 0.297f, 0.901f, 0.275f, 0.872f, 0.150f,
                           0.774f, 0.394f, 0.789f, 0.466f, 0.703f, 0.671f, 0.599f, 0.068f, 0.607f, 0.158f,
                           0.570f, 0.613f, 0.464f, 0.132f, 0.468f, 0.039f, 0.502f, 0.413f, 0.427f, 0.391f,
                           0.349f, 0.373f, 0.286f, 0.330f, 0.291f, 0.064f, 0.173f, 0.341f, 0.136f, 0.344f,
                           0.103f, 0.121f, 0.052f, 0.498f };
        Debug.Log("background elements");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskThree(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(bgDelay);
        }
    }

    IEnumerator Words()
    {
        float wordDelay = 1.17f;
        float[] coords = { 0.648f, 0.066f, 0.228f, 0.287f, 0.258f, 0.277f, 0.324f, 0.238f, 0.324f, 0.198f,
                           0.191f, 0.688f, 0.432f, 0.352f, 0.670f, 0.263f, 0.453f, 0.227f, 0.549f, 0.699f,
                           0.627f, 0.834f, 0.737f, 0.564f, 0.986f, 0.184f, 0.871f, 0.066f, 0.750f, 0.209f,
                           0.018f, 0.514f, 0.115f, 0.607f, 0.105f, 0.273f, 0.774f, 0.117f, 0.728f, 0.109f,
                           0.727f, 0.020f, 0.862f, 0.269f, 0.552f, 0.166f, 0.834f, 0.507f, 0.974f, 0.976f,
                           0.006f, 0.831f, 0.636f, 0.585f, 0.617f, 0.792f, 0.498f, 0.362f, 0.006f, 0.714f,
                           0.785f, 0.315f, 0.235f, 0.202f };
        Debug.Log("starting words");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(wordDelay);
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

    IEnumerator EnableScreenCollision()
    {
        yield return new WaitForSeconds(5f);
        //screenModel.GetComponent<MeshCollider>().enabled = true;
    }

    void Blit()
    {
#if !BLIT_TO_SCREEN
        if (usingGrowth){
            for (int i = 0; i < iterations; i++)
                {
                //Debug.Log("update blitting");
                    Graphics.Blit(buff, final, growMat);
                    Graphics.Blit(final, buff, growMat);
                }
            ResetUVs();
        }
#endif
        return;
    }

    void OnDisable()
    {
        Reset();
    }
}
