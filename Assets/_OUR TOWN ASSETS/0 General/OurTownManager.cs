using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon.Chat;


public class OurTownManager : MonoBehaviour, IChatClientListener
{

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
    static UnityStandardAssets.ImageEffects.ColorCorrectionCurves ccCurves;
    enum MasterRenderType { logo, fade, emergency }
    MasterRenderType masterRender = MasterRenderType.logo;
    Texture2D currentTex;
    string output = "";

    public string[] ChannelsToJoinOnConnect ;
    int HistoryLengthToFetch = 0;

    string UserName = "OurTownServer";

    public ChatClient chatClient;

    public Texture2D[] paintingProgression;
    public static Texture2DArray paintingArray;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        this.chatClient = new ChatClient(this);

        string chatAppId = ChatSettings.Instance.AppId;
        this.chatClient.Connect(chatAppId, "1.0", new AuthenticationValues(UserName));
        chatClient.Subscribe(new string[] { "OurTown", "All" });

        paintingArray = new Texture2DArray(8192, 1024, 18, TextureFormat.RGB24, false);
        for (int i = 0; i < 18; i++)
        {
            paintingArray.SetPixels(paintingProgression[i].GetPixels(0), i);
        }
        paintingArray.Apply();

        Debug.Log("Connecting as: " + UserName);
        townScene = GetComponent<TownScene>();
        moonScene = GetComponent<MoonScene>();
        rainScene = GetComponent<RainScene>();
        treeScene = GetComponent<TreeScene>();
        mistScene = GetComponent<MistScene>();
        oceanScene = GetComponent<OceanScene>();
        paintingScene = GetComponent<PaintingScene>();
        starScene = GetComponent<StarScene>();
        ccCurves = GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>();
        DisableAll();
        fadeMat.SetVector("_Value", Vector4.zero);
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
        //chatClient.PublishMessage("OurTown", Time.time.ToString());
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
            Graphics.Blit(testPattern, dest, fadeMat);
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
        ccCurves.enabled = false;
        //townScene.Stop();
        //moonScene.Stop();
        //rainScene.Stop();
        //treeScene.Stop();
        //mistScene.Stop();
        //oceanScene.Stop();
        //paintingScene.Stop();
        //starScene.Stop();
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

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        // Remove callback when object goes out of scope
        Application.logMessageReceived -= HandleLog;
        chatClient.Disconnect();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        //output += logString + "\n";
        //chatClient.PublishMessage("All", logString);
        //stack = stackTrace;
        chatClient.SendPrivateMessage("marc", logString);
    }
    public void OnConnected()
    {
        if (ChannelsToJoinOnConnect != null && ChannelsToJoinOnConnect.Length > 0)
        {
            chatClient.Subscribe(ChannelsToJoinOnConnect, HistoryLengthToFetch);
        }

        chatClient.AddFriends(new string[] { "tobi", "ilya" }); // Add some users to the server-list to get their status updates
        chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    }

    public void OnDisconnected()
    {
    }

    public void OnChatStateChange(ChatState state)
    {

    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // in this demo, we simply send a message into each channel. This is NOT a must have!
        foreach (string channel in channels)
        {
            chatClient.PublishMessage(channel, "says 'hi'."); // you don't HAVE to send a msg on join but you could.

        }
    }
    public void OnUnsubscribed(string[] channels)
    {
       
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
       
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
       
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        // this is how you get status updates of friends.
        // this demo simply adds status updates to the currently shown chat.
        // you could buffer them or use them any other way, too.

        // TODO: add status updates
        //if (activeChannel != null)
        //{
        //    activeChannel.Add("info", string.Format("{0} is {1}. Msg:{2}", user, status, message));
        //}

        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
    }

    public void AddMessageToSelectedChannel(string msg)
    {
       
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            UnityEngine.Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        else
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
