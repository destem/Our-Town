using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistScene : MonoBehaviour {

    //public GameObject screen;
    public Material scrollMat;
    public float scrollSpeed;
    public Material fadeMat;
    float offset = 0f;
    OurTownGestureListener gesture;
    bool next = false;
    bool usingFade = false;

    // Use this for initialization
    void Start () {
        //screen.GetComponent<Renderer>().material = scrollMat;
        gesture = OurTownGestureListener.Instance;
        Reset();
    }

    void Reset()
    {
        StopAllCoroutines();
        usingFade = false;
        fadeMat.SetVector("_Value", Vector4.zero);

        StartCoroutine("RunMistScene");
    }
	
	// Update is called once per frame
	void Update () {
        //offset += Time.deltaTime * scrollSpeed;
        //scrollMat.mainTextureOffset = new Vector2(offset, 0f);
        //scrollMat.SetFloat("_offset", offset);
        next = false;

        if (Input.GetButtonDown("Jump"))
        {
            next = true;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (usingFade)
        {
            Graphics.Blit(source, dest, fadeMat);
        }
        else
        {
            Graphics.Blit(scrollMat.mainTexture, dest, scrollMat);
        }
    }

    IEnumerator RunMistScene()
    {
        print("Started mist");
        yield return new WaitForSeconds(1f); //gesture not getting initialized fast enough??
        print("Waiting to activate The More You Know gesture to start Act III");
        while (!next)
        {
            yield return null;
        }
        next = false;
        print("Waiting for The More You Know");
        gesture.SetCurrentGesture(KinectGestures.Gestures.TheMoreYouKnow);
        while (!next && !gesture.IsCurrentGesture())
        {
            yield return null;
        }
        next = false;
        usingFade = true;
        float startTime = Time.time;
        float fadeDuration = 0.5f;
        while (Time.time - startTime < fadeDuration)
        {
            fadeMat.SetVector("_Value", new Vector4((Time.time - startTime) / fadeDuration, 0f, 0f, 0f));
            yield return null;
        }
        fadeMat.SetVector("_Value", Vector4.one);
        print("Going to ocean scene");
        OurTownManager.GotoOcean();
    }

    void OnDisable()
    {
        Reset();
        StopAllCoroutines();
    }
}
