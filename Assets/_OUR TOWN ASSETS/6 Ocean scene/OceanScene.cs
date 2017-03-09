using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanScene : MonoBehaviour
{

    public Texture2D startMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D black;

    public Texture2D paper;
    public Texture2D full;
    public Texture2D hurtFeelings;
    public Texture2D umbrellasOnly;


    public Material growMat;
    public Material displayMat;
    public Material fadeMat;

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
    float uvOffset = 0f;
    float imageLerp = 0f;
    bool rotate = false;

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
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        usingGrowth = true;
        uvOffset = 0f;
        imageLerp = 0;
        rotate = false;
        growMat2 = new Material(growMat);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        growMat2.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));

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

        fadeMat.SetTexture("_Chapel", umbrellasOnly);
        fadeMat.SetTexture("_Paper", full);
        ResetUVs();

        StartCoroutine(RunOceanScene());
    }

    // Update is called once per frame
    void Update()
    {
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

        if (rotate)
        {
            uvOffset += 0.002f * Time.deltaTime;
        }
    }

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
        else
        {
            fadeMat.SetVector("_Value", new Vector4(imageLerp, uvOffset, 0f, 0f));
            Graphics.Blit(source, dest, fadeMat);
        }

    }

    IEnumerator RunOceanScene()
    {
        print("Ocean scene started");
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        growMat2.SetTexture("_MaskTwoTex", MaskFourTex);
        print("Waiting to activate point forward gesture to start horizon");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for point forward");
        gesture.SetCurrentGesture(KinectGestures.Gestures.PointForward);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("Horizon");
        print("Waiting to activate mic drop gesture to start umbrellas");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for mic drop");
        gesture.SetCurrentGesture(KinectGestures.Gestures.MicDrop);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("Umbrellas");
        print("Waiting to activate T pose gesture to start weather and words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for T pose");
        gesture.SetCurrentGesture(KinectGestures.Gestures.Tpose);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("WeatherandWords");
        print("Waiting to activate yoga tree gesture to start rotation");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for yoga tree");
        gesture.SetCurrentGesture(KinectGestures.Gestures.YogaTree);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        //start rotating picture
        print("starting rotation");
        usingGrowth = false;
        rotate = true;
        print("Waiting to activate lean left gesture to fade to words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for lean left");
        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanLeft);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        rotate = false;
        print("Fade to umbrellas and words (60)");
        float startTime = Time.time;
        float fadeTime = 60f;
        while (Time.time - startTime < fadeTime)
        {
            imageLerp = (Time.time - startTime) / fadeTime;
            yield return null;
        }
        print("Waiting to activate The More You Know gesture to fade to last words");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for The More You Know");
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        gesture.ClearGestureSuccess();
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        imageLerp = 0;
        fadeMat.SetTexture("_Chapel", hurtFeelings);
        fadeMat.SetTexture("_Paper", umbrellasOnly);
        yield return null;
        print("2 minute fade to last words");
        startTime = Time.time;
        fadeTime = 120f;
        while (Time.time - startTime < fadeTime)
        {
            imageLerp = (Time.time - startTime) / fadeTime;
            yield return null;
        }

        fadeMat.SetTexture("_Chapel", paper);
        fadeMat.SetTexture("_Paper", hurtFeelings);
        yield return null;
        print("short final fade to blank");
        startTime = Time.time;
        fadeTime = 5f;
        while (Time.time - startTime < fadeTime)
        {
            imageLerp = (Time.time - startTime) / fadeTime;
            yield return null;
        }

        //yield return new WaitForSeconds(2f);
        print("Fade complete, moving to painting scene");
        OurTownManager.GotoPainting();
    }

    IEnumerator Horizon()
    {
        print("Horizon, sun, first umbrella");
        SetMaskOne(0.01f, 0.508f);
        yield return new WaitForSeconds(5f);
        SetMaskOne(.236f, .93f);
        yield return new WaitForSeconds(5f);
        SetMaskTwo(.973f, .122f);
    }

    IEnumerator Umbrellas()
    {
        print("Umbrellas");
        float umbrellaDelay = 2f;
        float[] coords = { 0.512f, 0.387f, 0.140f, 0.462f, 0.251f, 0.163f, 0.293f, 0.249f, 0.760f, 0.071f,
                           0.813f, 0.562f };
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(umbrellaDelay);
        }
    }

    IEnumerator WeatherandWords()
    {
        print("Weather and words");
        //3a sky
        SetMaskTwo(.257f, .853f);
        yield return null;
        SetMaskTwo(.192f, .987f);
        yield return new WaitForSeconds(3f);
        //3b water
        SetMaskFour(.248f, .59f);
        yield return new WaitForSeconds(3f);
        //3c storm
        SetMaskThree(.96f, .974f);
        yield return new WaitForSeconds(3f);
        //3d left rain
        SetMaskThree(.05f, .96f);
        //3e slanted rain
        SetMaskFour(.75f, .98f);
        yield return new WaitForSeconds(3f);
        //3f snow
        SetMaskThree(.46f, .986f);
        yield return new WaitForSeconds(3f);
        float[] coords = { 0.615f, 0.223f, 0.020f, 0.543f, 0.185f, 0.514f, 0.664f, 0.941f, 0.934f, 0.593f, 0.969f,
                           0.433f, 0.144f, 0.273f, 0.400f, 0.970f, 0.189f, 0.141f };
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        float[] coords2 = { 0.306f, 0.155f, 0.975f, 0.068f, 0.615f, 0.128f, 0.185f, 0.436f, 0.076f, 0.203f, 0.081f,
                            0.962f, 0.791f, 0.988f, 0.319f, 0.348f, 0.576f, 0.614f};
        for (int i = 0; i < coords2.Length; i += 2)
        {
            SetMaskTwo(coords2[i], coords2[i + 1]);
            yield return null;
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
        StopAllCoroutines();
    }

    void OnEnable()
    {
        Reset();
    }

    //public void Stop()
    //{
    //    StopAllCoroutines();
    //}
}
