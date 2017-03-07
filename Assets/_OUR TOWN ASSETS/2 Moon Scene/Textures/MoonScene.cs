#define BLIT_TO_SCREEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D MaskOneTex;
    public Texture2D MaskTwoTex;
    public Texture2D MaskThreeTex;
    public Texture2D MaskFourTex;
    public Texture2D black;
    public Texture2D MaskOneFinalTex;
    public Texture2D MaskTwoFinalTex;
    public Texture2D firstTargetTex;
    public Texture2D finalTargetTex;

    public Texture2D paper;
    public Texture2D full;

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
    //public GameObject screenModel;
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
    void Start()
    {
        //screenModel.GetComponent<Renderer>().material = displayMat;
        gesture = OurTownGestureListener.Instance;
        Reset();

    }

    void Reset()
    {
        StopAllCoroutines();
        usingGrowth = true;
        fadeMat.SetVector("_Value", Vector4.zero);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));

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
        displayMat.SetTexture("_FinalTex", firstTargetTex);
        displayMat.SetTexture("_BGTex", paper);
        //screenModel.GetComponent<Renderer>().material = displayMat;
        ResetUVs();

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
        else
        {
            Graphics.Blit(source, dest, fadeMat);
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
        growMat.SetTexture("_MaskOneTex", MaskOneTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoTex);
        growMat2.SetTexture("_MaskOneTex", MaskThreeTex);
        StartCoroutine(HouseSilhouettes());

        gesture.SetCurrentGesture(KinectGestures.Gestures.Here);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(SmallMoonSky());

        gesture.SetCurrentGesture(KinectGestures.Gestures.Shrug);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(OuterDetails());

        yield return new WaitForSeconds(5f);

        StartCoroutine(Words());

        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanLeft);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine("InnerDetails");

        gesture.SetCurrentGesture(KinectGestures.Gestures.StepAndSweep);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Changeover");
        ResetUVs();
        StopCoroutine(HouseSilhouettes());
        StopCoroutine(SmallMoonSky());
        StopCoroutine(OuterDetails());
        StopCoroutine("InnerDetails");
        StopCoroutine(Words());
        displayMat.SetTexture("_FinalTex", finalTargetTex);
        displayMat.SetTexture("_BGTex", firstTargetTex);
        buff = _createTexture(startMask.width, startMask.height);
        buff2 = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        final2 = _createTexture(startMask.width, startMask.height);
        Blit();
        displayMat.SetTexture("_MainTex", buff);
        displayMat.SetTexture("_SecondTex", buff2);
        growMat.SetTexture("_MaskOneTex", MaskOneFinalTex);
        growMat.SetTexture("_MaskTwoTex", MaskTwoFinalTex);

        // final masks are in place, fire off the moon
        SetMaskOne(0.475f, .667f);
        //yield return new WaitForSeconds(20f);
        //SetMaskTwo(0.475f, .667f);
        yield return null;
        gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(Blackout());

        gesture.SetCurrentGesture(KinectGestures.Gestures.LeanLeft);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(LastWindows());

        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("Wipeout");
        usingGrowth = false;
        float startTime = Time.time;
        float fadeDuration = 1.5f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeMat.SetVector("_Value", Vector4.one);

    }

    IEnumerator HouseSilhouettes()
    {
        float houseDelay = 1;
        //float[] coords = { 0.492f, 0.036f, 0.510f, 0.031f, 0.548f, 0.031f, 0.569f, 0.042f, 0.373f, 0.031f,
        //                   0.442f, 0.026f, 0.596f, 0.042f, 0.636f, 0.036f, 0.293f, 0.026f, 0.338f, 0.036f,
        //                   0.649f, 0.031f, 0.720f, 0.026f, 0.198f, 0.042f, 0.263f, 0.042f, 0.775f, 0.031f,
        //                   0.819f, 0.051f, 0.116f, 0.046f, 0.154f, 0.036f, 0.845f, 0.042f, 0.878f, 0.057f,
        //                   0.055f, 0.026f, 0.072f, 0.057f, 0.930f, 0.042f, 0.974f, 0.046f, 0.01f, 0.062f,
        //                   0.01f, 0.460f, 0.992f, 0.051f, 0.993f, 0.387f };
        float[] coords = { 0.010f, 0.028f, 0.030f, 0.028f, 0.056f, 0.021f, 0.073f, 0.028f, 0.096f, 0.028f,
                           0.116f, 0.021f, 0.138f, 0.021f, 0.154f, 0.028f, 0.173f, 0.021f, 0.199f, 0.025f,
                           0.226f, 0.028f, 0.246f, 0.021f, 0.264f, 0.028f, 0.279f, 0.025f, 0.291f, 0.028f,
                           0.307f, 0.028f, 0.324f, 0.028f, 0.340f, 0.032f, 0.352f, 0.021f, 0.374f, 0.025f,
                           0.392f, 0.021f, 0.413f, 0.025f, 0.428f, 0.014f, 0.442f, 0.021f, 0.457f, 0.025f,
                           0.486f, 0.021f, 0.497f, 0.014f, 0.509f, 0.021f, 0.521f, 0.021f, 0.545f, 0.025f,
                           0.553f, 0.021f, 0.560f, 0.014f, 0.567f, 0.014f, 0.572f, 0.014f, 0.596f, 0.014f,
                           0.613f, 0.021f, 0.620f, 0.021f, 0.636f, 0.021f, 0.640f, 0.021f, 0.649f, 0.025f,
                           0.660f, 0.017f, 0.671f, 0.025f, 0.681f, 0.014f, 0.691f, 0.014f, 0.703f, 0.021f,
                           0.710f, 0.014f, 0.719f, 0.017f, 0.746f, 0.017f, 0.754f, 0.021f, 0.759f, 0.014f,
                           0.775f, 0.014f, 0.783f, 0.014f, 0.792f, 0.021f, 0.803f, 0.021f, 0.812f, 0.014f,
                           0.819f, 0.017f, 0.839f, 0.021f, 0.848f, 0.025f, 0.858f, 0.021f, 0.867f, 0.021f,
                           0.874f, 0.021f, 0.883f, 0.017f, 0.894f, 0.021f, 0.902f, 0.011f, 0.930f, 0.028f,
                           0.949f, 0.017f, 0.956f, 0.014f, 0.973f, 0.014f, 0.988f, 0.028f, 0.995f, 0.028f };
        print("starting house silhouettes");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            //yield return null;
           // SetMaskOne(coords[i+2], coords[i + 3]);

            yield return new WaitForSeconds(houseDelay);
        }

    }

    IEnumerator SmallMoonSky()
    {
        float[] coords = { 0.463f, 0.051f, 0.044f, 0.910f, 0.029f, 0.759f, 0.042f, 0.523f, 0.082f, 0.554f,
                           0.092f, 0.817f, 0.108f, 0.565f, 0.126f, 0.465f, 0.132f, 0.701f, 0.159f, 0.900f,
                           0.161f, 0.665f, 0.160f, 0.460f, 0.187f, 0.465f, 0.173f, 0.728f, 0.204f, 0.900f,
                           0.206f, 0.701f, 0.228f, 0.612f, 0.224f, 0.454f, 0.248f, 0.365f, 0.272f, 0.507f,
                           0.253f, 0.617f, 0.248f, 0.874f, 0.284f, 0.859f, 0.281f, 0.712f, 0.281f, 0.434f,
                           0.305f, 0.665f, 0.302f, 0.890f, 0.337f, 0.868f, 0.321f, 0.712f, 0.333f, 0.601f,
                           0.362f, 0.612f, 0.353f, 0.445f, 0.363f, 0.324f, 0.402f, 0.387f, 0.387f, 0.523f,
                           0.416f, 0.701f, 0.390f, 0.806f, 0.438f, 0.915f, 0.434f, 0.696f, 0.450f, 0.586f,
                           0.473f, 0.429f, 0.503f, 0.596f, 0.478f, 0.701f, 0.478f, 0.937f, 0.522f, 0.962f,
                           0.520f, 0.743f, 0.531f, 0.454f, 0.533f, 0.256f, 0.568f, 0.460f, 0.551f, 0.559f,
                           0.565f, 0.859f, 0.586f, 0.801f, 0.587f, 0.607f, 0.599f, 0.429f, 0.634f, 0.439f,
                           0.652f, 0.701f, 0.634f, 0.806f, 0.681f, 0.921f, 0.693f, 0.654f, 0.673f, 0.554f,
                           0.686f, 0.356f, 0.723f, 0.382f, 0.715f, 0.225f, 0.714f, 0.576f, 0.739f, 0.837f,
                           0.756f, 0.670f, 0.750f, 0.282f, 0.768f, 0.104f, 0.827f, 0.146f, 0.815f, 0.559f,
                           0.790f, 0.544f, 0.769f, 0.879f, 0.809f, 0.842f, 0.834f, 0.953f, 0.868f, 0.853f,
                           0.899f, 0.534f, 0.933f, 0.465f, 0.962f, 0.544f, 0.938f, 0.779f, 0.956f, 0.921f,
                           0.984f, 0.915f };
        print("small moon stippling");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskThree(coords[i], coords[i + 1]);
            yield return null;
        }

    }

    IEnumerator OuterDetails()
    {
        float[] coords = { 0.043f, 0.017f, 0.208f, 0.430f, 0.278f, 0.068f, 0.448f, 0.200f, 0.461f, 0.150f,
                           0.532f, 0.011f, 0.578f, 0.089f, 0.587f, 0.014f, 0.905f, 0.158f, 0.917f, 0.014f,
                           0.927f, 0.075f };
        print("outer details");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(2f);
        }

    }

    IEnumerator InnerDetails()
    {
        float[] coords = { 0.500f, 0.204f, 0.653f, 0.032f, 0.299f, 0.405f, 0.058f, 0.171f, 0.103f, 0.060f,
                           0.798f, 0.275f, 0.140f, 0.171f, 0.435f, 0.060f, 0.843f, 0.165f, 0.603f, 0.049f,
                           0.556f, 0.017f, 0.511f, 0.078f, 0.335f, 0.100f, 0.103f, 0.171f, 0.780f, 0.025f,
                           0.948f, 0.219f, 0.359f, 0.039f, 0.206f, 0.197f, 0.081f, 0.075f, 0.490f, 0.078f,
                           0.296f, 0.190f, 0.327f, 0.408f, 0.784f, 0.211f, 0.812f, 0.197f, 0.560f, 0.143f,
                           0.854f, 0.427f, 0.170f, 0.200f, 0.058f, 0.075f, 0.627f, 0.035f, 0.506f, 0.269f,
                           0.729f, 0.032f, 0.563f, 0.064f, 0.259f, 0.057f, 0.314f, 0.186f, 0.127f, 0.107f,
                           0.418f, 0.021f, 0.082f, 0.165f, 0.211f, 0.014f, 0.329f, 0.319f, 0.146f, 0.100f,
                           0.501f, 0.064f, 0.843f, 0.448f, 0.268f, 0.025f, 0.446f, 0.025f, 0.807f, 0.017f,
                           0.330f, 0.043f };
        print("inner details");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(1.03f);
        }

    }

    IEnumerator Words()
    {
        float[] coords = { 0.399f, 0.214f, 0.047f, 0.663f, 0.338f, 0.700f, 0.742f, 0.484f, 0.354f, 0.362f,
                           0.901f, 0.635f, 0.045f, 0.075f, 0.592f, 0.265f, 0.637f, 0.273f, 0.868f, 0.932f };
        print("moon words");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskOne(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(2f);
        }

    }
    IEnumerator Blackout()
    {
        float[] coords = { 0.952f, 0.171f, 0.941f, 0.034f, 0.966f, 0.030f, 0.450f, 0.051f, 0.140f, 0.182f,
                           0.686f, 0.034f, 0.813f, 0.248f, 0.515f, 0.083f, 0.333f, 0.106f, 0.085f, 0.096f,
                           0.654f, 0.027f, 0.704f, 0.114f, 0.364f, 0.030f, 0.131f, 0.148f, 0.256f, 0.048f,
                           0.302f, 0.344f, 0.382f, 0.092f, 0.879f, 0.307f, 0.733f, 0.038f, 0.064f, 0.089f,
                           0.148f, 0.030f, 0.188f, 0.038f, 0.420f, 0.041f, 0.606f, 0.038f, 0.672f, 0.162f,
                           0.698f, 0.030f, 0.784f, 0.044f, 0.849f, 0.486f, 0.331f, 0.341f, 0.130f, 0.030f,
                           0.317f, 0.109f, 0.797f, 0.230f, 0.847f, 0.203f, 0.618f, 0.196f, 0.107f, 0.100f,
                           0.217f, 0.051f, 0.148f, 0.168f, 0.677f, 0.030f, 0.784f, 0.251f, 0.139f, 0.279f,
                           0.166f, 0.044f, 0.403f, 0.030f, 0.812f, 0.048f, 0.893f, 0.086f, 0.209f, 0.220f,
                           0.629f, 0.041f, 0.299f, 0.128f, 0.175f, 0.168f, 0.236f, 0.044f, 0.866f, 0.078f,
                           0.665f, 0.030f, 0.503f, 0.069f, 0.436f, 0.078f, 0.209f, 0.431f };
        print("Blackout");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(2.5f);
        }
    }

    IEnumerator LastWindows()
    {
        float[] coords = { 0.492f, 0.096f, 0.559f, 0.176f, 0.561f, 0.041f, 0.499f, 0.237f, 0.510f, 0.237f };
        print("Last 5 windows");
        for (int i = 0; i < coords.Length; i += 2)
        {
            SetMaskTwo(coords[i], coords[i + 1]);
            yield return new WaitForSeconds(.5f);
        }
        SetMaskTwo(.467f, .423f);
        yield return null;
        SetMaskTwo(.531f, .423f);
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


    void Blit()
    {
#if !BLIT_TO_SCREEN
        if (usingGrowth)
        {
            for (int i = 0; i < iterations; i++)
            {
                //print("update blitting");
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
