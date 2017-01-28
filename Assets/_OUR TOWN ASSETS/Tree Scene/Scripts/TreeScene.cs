#define BLIT_TO_SCREEN

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
    public Texture2D full;
    public Material chapelMat;
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
    public int iterations = 1;
    public float growthThreshhold = 1f;
    bool next = false;
   // bool useGrowMat = false;
    //Texture2D chapelTex;

    enum TreeStates {TREE_START, TREE_CHAPEL, TREE_FIRST_WORDS, TREE_CHAPEL_BRANCH, TREE_CENTRAL_SLOW, TREE_SIDE_GROWTH, TREE_SECOND_WORDS, TREE_ANGRY_FAST, TREE_THIRD_WORDS, TREE_FINAL_WORDS }
    TreeStates state = TreeStates.TREE_START;

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
        //chapelTex = startMask;
        StartCoroutine(RunScene());
    }

    // Update is called once per frame
    void Update()
    {
      
        ResetUVs();
        next = false;
        if (Input.GetKeyDown(KeyCode.N) || gesture.IsPsi())
        {
            growMat.SetTexture("_RightHandMask", rightHandMask);
            growMat.SetTexture("_LeftHandMask", leftHandMask);
            //StartCoroutine(EnableScreenCollision());
        }

           
        Vector3 mousePos = Input.mousePosition;
        t.text = string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }

        //if (Input.GetMouseButtonDown(0))

        //{
            //float u = Input.mousePosition.x / Screen.width;
            //float v = Mathf.Min(Input.mousePosition.y / Screen.height, 0.08f);
            ////print("Click at " + u + ", " + v);
            //growMat.SetVector("_RightHand", new Vector4(u, v, brushSize, -1f));
        //}
        //if (Input.GetMouseButton(1))

        //{
        //    float u = Input.mousePosition.x / Screen.width;
        //    float v = Mathf.Min(Input.mousePosition.y / Screen.height, 0.08f);

        //    growMat.SetVector("_LeftHand", new Vector4(u, v, brushSize, -1f));
        //}



    }

#if BLIT_TO_SCREEN
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
      
        for (int i = 0; i < iterations; i++)
        {
            Graphics.Blit(buff, final, growMat);
            Graphics.Blit(final, buff, growMat);
        }
        Graphics.Blit(buff, dest, displayMat);
        //Graphics.Blit(full, dest);

    }

#endif

    IEnumerator RunScene()
    {
        while (!next && !gesture.IsClap())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("starting chapel");
        SetRightHand(.52f, 0.025f);
        SetLeftHand(.48f, 0.025f);
        while (!next && !gesture.IsPsi())
        {
            Blit();
            yield return null;
        }
        next = false;
        print("first branches");
        growMat.SetTexture("_RightHandMask", rightHandMask);
        growMat.SetTexture("_LeftHandMask", leftHandMask);
        while (!next)
        {
            Blit();
            yield return null;
        }
        next = false;
        print("First words");
        SetRightHand(.215f, .994f);
        yield return null;
        SetRightHand(.222f, .942f);
        yield return null;
        SetRightHand(.24f, .886f);
        yield return null;
        SetRightHand(.58f, .065f);
        yield return null;
        SetRightHand(.335f, .15f);
    }

    public void SetLeftHand(float u, float v)
    {
        //float u = coords.x;
        //float v = Mathf.Min(coords.y, 0.08f);

        growMat.SetVector("_LeftHand", new Vector4(u, v, brushSize, -1f));
    }

    public void SetRightHand(float u, float v)
    {
        //float u = coords.x;
        //float v = Mathf.Min(coords.y, 0.08f);

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

    void Blit()
    {
#if !BLIT_TO_SCREEN
        for (int i = 0; i < iterations; i++)
        {
            Graphics.Blit(buff, final, growMat);
            Graphics.Blit(final, buff, growMat);
        }
#endif
        return;
    }
    }
