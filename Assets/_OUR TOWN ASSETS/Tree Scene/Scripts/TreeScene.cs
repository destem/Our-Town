﻿#define BLIT_TO_SCREEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScene : MonoBehaviour {

    public Texture2D startMask;
    public Texture2D chapelMask;
    //public Texture2D nextMask;
    public Texture2D leftHandMask;
    public Texture2D rightHandMask;
    public Material growMat;
    public Material displayMat;
    RenderTexture buff;
    RenderTexture final;
    public Text t;
    float brushSize = 10f;
    bool addingToMask = false;
    TreeGestureListener gesture;
    public GameObject screenModel;
    public float slowSpeed = 0f;
    public float mediumSpeed = 0f;
    public float fastSpeed = 0f;
    public float growthThreshhold = 1f;

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
        growMat.SetTexture("_RightHandMask", chapelMask);
        growMat.SetTexture("_LeftHandMask", chapelMask);
        growMat.SetVector("_Speeds", new Vector4(slowSpeed, mediumSpeed, fastSpeed, growthThreshhold));
        buff = _createTexture(startMask.width, startMask.height);
        final = _createTexture(startMask.width, startMask.height);
        Graphics.Blit(startMask, buff);//, growMat);
        displayMat.SetTexture("_MainTex", buff);
        gesture = TreeGestureListener.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || addingToMask)
        //{
        //    growMat.SetVector("_LeftHand", new Vector4(-1, -1, -1, -1));
        //    growMat.SetVector("_RightHand", new Vector4(-1, -1, -1, -1));
        //    addingToMask = false;
        //}
        ResetUVs();
        if (Input.GetKeyDown(KeyCode.N) || gesture.IsPsi())
        {
            growMat.SetTexture("_RightHandMask", rightHandMask);
            growMat.SetTexture("_LeftHandMask", leftHandMask);
            //StartCoroutine(EnableScreenCollision());
        }

        if (gesture.IsClap() || Input.GetKeyDown(KeyCode.S))
        {
            print("CLAP!!");
            growMat.SetVector("_LeftHand", new Vector4(0.481f, 0.05f, brushSize, -1f));
            growMat.SetVector("_RightHand", new Vector4(0.52f, 0.05f, brushSize, -1f));
            addingToMask = true;
        }
       
        Vector3 mousePos = Input.mousePosition;
        t.text = string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);

        if (Input.GetMouseButtonDown(0))

        {
            float u = Input.mousePosition.x / Screen.width;
            float v = Mathf.Min(Input.mousePosition.y / Screen.height, 0.08f);
            //print("Click at " + u + ", " + v);
            growMat.SetVector("_RightHand", new Vector4(u, v, brushSize, -1f));
        }
        if (Input.GetMouseButton(1))

        {
            float u = Input.mousePosition.x / Screen.width;
            float v = Mathf.Min(Input.mousePosition.y / Screen.height, 0.08f);

            growMat.SetVector("_LeftHand", new Vector4(u, v, brushSize, -1f));
        }


#if !BLIT_TO_SCREEN
        for (int i = 0; i < 1; i++)
        {
            Graphics.Blit(buff, final, growMat);
            Graphics.Blit(final, buff, growMat);
        }
#endif
    }

#if BLIT_TO_SCREEN
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        for (int i = 0; i < 1; i++)
        {
            Graphics.Blit(buff, final, growMat);
            Graphics.Blit(final, buff, growMat);
        }
        ////Graphics.Blit(buff, dest);
        Graphics.Blit(buff, dest, displayMat);
    }

#endif

    public void SetLeftHand(Vector2 coords)
    {
        float u = coords.x;
        float v = Mathf.Min(coords.y, 0.08f);

        growMat.SetVector("_LeftHand", new Vector4(u, v, brushSize, -1f));
    }

    public void SetRightHand(Vector2 coords)
    {
        float u = coords.x;
        float v = Mathf.Min(coords.y, 0.08f);

        growMat.SetVector("_RightHand", new Vector4(u, v, brushSize, -1f));
    }

    public void ResetUVs()
    {
        growMat.SetVector("_LeftHand", new Vector4(-1, -1, -1, -1));
        growMat.SetVector("_RightHand", new Vector4(-1, -1, -1, -1));
    }

    IEnumerator EnableScreenCollision()
    {
        yield return new WaitForSeconds(5f);
        screenModel.GetComponent<MeshCollider>().enabled = true;
    }

 
}
