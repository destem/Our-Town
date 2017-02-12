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
           
        Vector3 mousePos = Input.mousePosition;
        t.text = string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);

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
        while (!next)// && !gesture.IsClap())
        {
            Blit();
            yield return null;
        }
        next = false;
        StartCoroutine(HouseOutlines());
    }

    IEnumerator HouseOutlines()
    {
        float outlineDelay = 2f;
        //outlines are all on mask one. starting in lower-right and working left
        /*

         */
        print("starting house outlines");
        SetMaskOne(.933f, .009f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.931f, .445f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.888f, .44f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.869f, .009f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.859f, .604f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.848f, .15f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.823f, .59f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.785f, .7f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.74f, .005f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.725f, .452f);
        yield return new WaitForSeconds(outlineDelay);
        SetMaskOne(.69f, .005f);
        yield return new WaitForSeconds(outlineDelay);
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
