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

    // Update is called once per frame
    void Update()
    {
      
        ResetUVs();
        next = false;
           
        if (Input.GetButtonDown("Jump"))
        {
            next = true;
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
        while (!next)// && !gesture.IsClap())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(FirstHouses());
        while (!next)// && !gesture.IsClap())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(HouseOutlines());
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
                           0.631f, 0.583f, 0.613f, 0.583f, 0.657f, 0.140f, 0.573f, 0.274f, 0.471f, 0.027f,
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
        //SetMaskOne(.933f, .009f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.931f, .445f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.888f, .44f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.869f, .009f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.859f, .604f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.848f, .15f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.823f, .59f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.785f, .7f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.74f, .005f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.725f, .452f);
        //yield return new WaitForSeconds(outlineDelay);
        //SetMaskOne(.69f, .005f);
        //yield return new WaitForSeconds(outlineDelay);
    }

    IEnumerator Roofs()
    {
        float roofDelay = 1f;
        yield return null;
    }

    IEnumerator Windows()
    {
        yield return null;
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
