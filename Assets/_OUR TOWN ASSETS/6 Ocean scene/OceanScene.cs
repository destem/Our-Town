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
    void Start()
    {
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

        }

    }

    IEnumerator RunScene()
    {
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
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
