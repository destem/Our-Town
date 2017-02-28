using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurTownManager : MonoBehaviour {

    public Texture2D testPattern;
    bool justStarted = true;
    static TownScene townScene;
    static MoonScene moonScene;
    static RainScene rainScene;
    static TreeScene treeScene;
    static MistScene mistScene;
    static OceanScene oceanScene;
    static PaintingScene paintingScene;
    static StarScene starScene;

	// Use this for initialization
	void Start () {
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
        if (justStarted && Input.anyKeyDown)
        {
            justStarted = false;
            GotoTown();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DisableAll();
            GotoTown();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DisableAll();
            GotoMoon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DisableAll();
            GotoRain();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DisableAll();
            GotoTree();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DisableAll();
            GotoMist();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            DisableAll();
            GotoOcean();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DisableAll();
            GotoPainting();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DisableAll();
            GotoStar();
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (justStarted)
        {
            Graphics.Blit(testPattern, dest);
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
