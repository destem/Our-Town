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
    public Texture2D full;
    public Material growMat;
    public Material displayMat;
    public Material packer;
    RenderTexture buff;
    RenderTexture final;
    public Text t;
    float brushSize = 10f;
    OurTownGestureListener gesture;
    public GameObject screenModel;
    public float slowSpeed = 0f;
    public float mediumSpeed = 0f;
    public float fastSpeed = 0f;
    public int iterations = 1;
    public float growthThreshhold = 1f;
    public float chapelFadeTime = 2f;
    bool next = false;
    bool usingGrowth = true;
    
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
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        buff = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        Graphics.Blit(startMask, buff, packer);
        displayMat.SetTexture("_MainTex", buff);
        growMat.SetTexture("_MaskOneTex", black);
        growMat.SetTexture("_MaskTwoTex", black);
        growMat.SetTexture("_MaskThreeTex", black);
        growMat.SetTexture("_MaskFourTex", black);
        gesture = OurTownGestureListener.Instance;
        StartCoroutine(RunScene());
    }

    void Reset()
    {
        StopAllCoroutines();
        buff = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        Graphics.Blit(startMask, buff, packer);
        displayMat.SetTexture("_MainTex", buff);
        growMat.SetTexture("_MaskOneTex", black);
        growMat.SetTexture("_MaskTwoTex", black);
        growMat.SetTexture("_MaskThreeTex", black);
        growMat.SetTexture("_MaskFourTex", black);
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

#if BLIT_TO_SCREEN
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (usingGrowth)
        {
            for (int i = 0; i < iterations; i++)
            {
                Graphics.Blit(buff, final, growMat);
                Graphics.Blit(final, buff, growMat);
            }
            Graphics.Blit(buff, dest, displayMat);
            //Graphics.Blit(buff, dest);
        }
        else
        {
            //Graphics.Blit(buff, dest, chapelMat);
        }

    }

#endif

    IEnumerator RunScene()
    {
        screenModel.GetComponent<Renderer>().material = growMat;
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && gesture?(!gesture.IsCurrentGesture()):false)
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(FirstHouses());
        gesture.SetCurrentGesture(KinectGestures.Gestures.BrushHair);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
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
        StartCoroutine(Windows());
        //gesture.SetCurrentGesture(KinectGestures.Gestures.YogaTree);
        yield return new WaitForSeconds(2f);
        print("here come the detials for 1c");
        //FIRE OFF DETAILS
        gesture.SetCurrentGesture(KinectGestures.Gestures.Here);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("1d");
        //FIRE OFF BACKGROUND
        gesture.SetCurrentGesture(KinectGestures.Gestures.HeadTilt);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        //FIRE OFF WORDS
        print ("WORDS!");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Clench);
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        //words only
        print("TOWN DISAPPEARS!");
        yield return null;
        yield return new WaitForSeconds(2f); // time for professor to speak. 130 in rehearsal
        print("TREES COME IN");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Clap); //pop the town back in
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("POP! GOES THE TOWN");

        gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave); //houses only
        while (!next && gesture ? (!gesture.IsCurrentGesture()) : false)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("JUST THE HOUSES");
    }

    IEnumerator FirstHouses()
    {
        print("first houses");
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        SetMaskOne(.465f, .563f);
        yield return new WaitForSeconds(2.0f);
        SetMaskTwo(.464f, .565f);
        yield return new WaitForSeconds(1.5f);
        SetMaskOne(.526f, .565f);
        yield return new WaitForSeconds(2.0f);
        SetMaskTwo(.524f, .557f);
    }

    IEnumerator HouseOutlines()
    {
        float outlineDelay = 2f;
        //outlines are all on mask one. starting in lower-right and working left
        float[] coords = { 0.353f, 0.998f, 0.933f, 0.024f, 0.931f, 0.446f, 0.888f, 0.446f, 0.868f, 0.038f,
                           0.848f, 0.158f, 0.859f, 0.630f, 0.823f, 0.596f, 0.786f, 0.699f, 0.740f, 0.024f,
                           0.709f, 0.473f, 0.765f, 0.226f, 0.668f, 0.013f, 0.724f, 0.020f, 0.657f, 0.473f,
                           0.631f, 0.583f, 0.613f, 0.583f, 0.657f, 0.140f, 0.573f, 0.276f, 0.471f, 0.027f,
                           0.491f, 0.027f, 0.461f, 0.099f, 0.414f, 0.013f, 0.370f, 0.566f, 0.417f, 0.504f,
                           0.298f, 0.027f, 0.299f, 0.514f, 0.273f, 0.024f, 0.241f, 0.504f, 0.280f, 0.507f,
                           0.197f, 0.020f, 0.184f, 0.541f, 0.141f, 0.034f, 0.132f, 0.686f, 0.105f, 0.466f,
                           0.078f, 0.579f, 0.074f, 0.253f, 0.025f, 0.363f, 0.034f, 0.404f };
        print("starting house outlines");
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
        print("starting roofs");
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
        print("starting windows");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(windowDelay);
        }
    }

    IEnumerator HouseDetails()
    {
        yield return null;
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
        growMat.SetVector("_MaskThreeCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void SetMaskFour(float u, float v)
    {
        growMat.SetVector("_MaskFourCoords", new Vector4(u, v, brushSize, -1f));
    }

    public void ResetUVs()
    {
        growMat.SetVector("_MaskOneCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskTwoCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskThreeCoords", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_MaskFourCoords", new Vector4(-1, -1, -1, -1));
    }

    IEnumerator EnableScreenCollision()
    {
        yield return new WaitForSeconds(5f);
        screenModel.GetComponent<MeshCollider>().enabled = true;
    }

    void Blit()
    {
#if !BLIT_TO_SCREEN
        if (usingGrowth){
            for (int i = 0; i < iterations; i++)
                {
                    Graphics.Blit(buff, final, growMat);
                    Graphics.Blit(final, buff, growMat);
                }
        }
#endif
        return;
    }
    }
