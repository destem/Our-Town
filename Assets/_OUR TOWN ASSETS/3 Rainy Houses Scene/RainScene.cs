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

    public Material growMat;
    public Material displayMat;

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
            Graphics.Blit(rotatedTown, dest);
        }

    }

    IEnumerator RunScene()
    {
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        growMat2.SetTexture("_MaskTwoTex", MaskFourTex);

        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        gesture.SetCurrentGesture(KinectGestures.Gestures.HeadTilt);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("FirstRain");

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeDown);
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
        StartCoroutine("WipeFirstHalf");

        gesture.SetCurrentGesture(KinectGestures.Gestures.SwipeUp);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("WipeSecondHalf");

        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanForward);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        StartCoroutine("EraseWords");

        gesture.SetCurrentGesture(KinectGestures.Gestures.Psi);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        usingGrowth = false;
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
        float wordDelay = 2f;
        float[] coords = { 0.443f, 0.263f, 0.258f, 0.042f, 0.261f, 0.510f, 0.348f, 0.751f, 0.343f, 0.617f,
                           0.261f, 0.991f, 0.933f, 0.255f };
        print("first rain and words");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(wordDelay);
        }

    }

    IEnumerator SecondRain()
    {
        float wordDelay = 2f;
        float[] coords = { 0.656f, 0.035f, 0.192f, 0.907f, 0.063f, 0.761f, 0.732f, 0.923f, 0.711f, 0.967f };
        print("second rain and words");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(wordDelay);
        }

    }

    IEnumerator LastRain()
    {
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
        float[] coords = { 0.25f, 0.75f, 0.75f, 0.75f, 0.5f, 0.75f, 0.465f, 0.010f, 0.99f, 0.99f };
        print("bring in the houses");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return null;
        }

    }

    IEnumerator WipeFirstHalf()
    {
        float[] coords = { 0.25f, 0.75f, 0.75f, 0.75f, 0.5f, 0.75f, 0.465f, 0.010f, 0.99f, 0.99f };
        print("wipe non-church side of screen");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return null;
        }

    }

    IEnumerator WipeSecondHalf()
    {
        float[] coords = { 0.25f, 0.75f, 0.75f, 0.75f, 0.5f, 0.75f, 0.465f, 0.010f, 0.99f, 0.99f };
        print("wipe other side of screen");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return null;
        }

    }

    IEnumerator EraseWords()
    {
        float[] coords = { 0.25f, 0.75f, 0.75f, 0.75f, 0.5f, 0.75f, 0.465f, 0.010f, 0.99f, 0.99f };
        print("erase all but four phrases");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
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
        Reset();
    }
}
