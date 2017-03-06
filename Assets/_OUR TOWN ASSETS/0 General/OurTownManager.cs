using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurTownManager : MonoBehaviour {

    public Texture2D testPattern;
    public Material fadeMat;
    public Texture2D[] showImages;
    bool justStarted = true;
    static TownScene townScene;
    static MoonScene moonScene;
    static RainScene rainScene;
    static TreeScene treeScene;
    static MistScene mistScene;
    static OceanScene oceanScene;
    static PaintingScene paintingScene;
    static StarScene starScene;
    enum MasterRenderType { logo, fade, emergency}
    MasterRenderType masterRender = MasterRenderType.logo;
    Texture2D currentTex;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        townScene = GetComponent<TownScene>();
        moonScene = GetComponent<MoonScene>();
        rainScene = GetComponent<RainScene>();
        treeScene = GetComponent<TreeScene>();
        mistScene = GetComponent<MistScene>();
        oceanScene = GetComponent<OceanScene>();
        paintingScene = GetComponent<PaintingScene>();
        starScene = GetComponent<StarScene>();
        DisableAll();
	}

    // Update is called once per frame
    void Update()
    {
        if (justStarted && Input.GetKeyDown(KeyCode.Return))
        {
            StopAllCoroutines();
            justStarted = false;
            masterRender = MasterRenderType.fade;
            //GotoTown();
            StartCoroutine("FadeToTown");
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopAllCoroutines();
            DisableAll();
            GotoTown();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopAllCoroutines();
            DisableAll();
            GotoMoon();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopAllCoroutines();
            DisableAll();
            GotoRain();
        }

        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha4))
        {
            StopAllCoroutines();
            DisableAll();
            GotoTree();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopAllCoroutines();
            DisableAll();
            GotoMist();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha6))
        {
            StopAllCoroutines();
            DisableAll();
            GotoOcean();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha7))
        {
            StopAllCoroutines();
            DisableAll();
            GotoPainting();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha8))
        {
            StopAllCoroutines();
            DisableAll();
            GotoStar();
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Alpha0))
        {
            StopAllCoroutines();
            DisableAll();
            fadeMat.SetVector("_Value", Vector4.zero);
            justStarted = true;
            masterRender = MasterRenderType.logo;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Q))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[0];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.W))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[1];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.E))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[2];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.F))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[3];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.T))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[4];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.Y))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[5];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.U))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[6];
            masterRender = MasterRenderType.emergency;
        }
        if (!justStarted && Input.GetKeyDown(KeyCode.I))
        {
            StopAllCoroutines();
            DisableAll();
            currentTex = showImages[7];
            masterRender = MasterRenderType.emergency;
        }
    }


    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (masterRender == MasterRenderType.logo)
        {
            Graphics.Blit(testPattern, dest);
        }
        else if (masterRender == MasterRenderType.fade)
        {
            Graphics.Blit(source, dest, fadeMat);
        }
        else
        {
            Graphics.Blit(currentTex, dest);
        }
    }

    static void DisableAll()
    {
        townScene.enabled = false;
        moonScene.enabled = false;
        rainScene.enabled = false;
        treeScene.enabled = false;
        mistScene.enabled = false;
        oceanScene.enabled = false;
        paintingScene.enabled = false;
        starScene.enabled = false;
    }

    IEnumerator FadeToTown()
    {
        float startTime = Time.time;
        float fadeDuration = 1f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        GotoTown();
    }

    public static void GotoTown()
    {
        townScene.enabled = true;
    }

    public static void GotoMoon()
    {
        moonScene.enabled = true;
    }

    public static void GotoRain()
    {
        rainScene.enabled = true;
    }

    public static void GotoTree()
    {
        treeScene.enabled = true;
    }

    public static void GotoMist()
    {
        mistScene.enabled = true;
    }

    public static void GotoOcean()
    {
        oceanScene.enabled = true;
    }

    public static void GotoPainting()
    {
        paintingScene.enabled = true;
    }

    public static void GotoStar()
    {
        starScene.enabled = true;
    }

   
}
