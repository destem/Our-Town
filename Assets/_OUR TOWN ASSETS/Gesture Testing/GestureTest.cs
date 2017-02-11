using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureTest : MonoBehaviour {

    TreeGestureListener gesture;
    public Text t;
    KinectGestures.Gestures currentGesture;

    // Use this for initialization
    void Start () {
        gesture = TreeGestureListener.Instance;
        //currentGesture = KinectGestures.Gestures.Clap;
        //gesture.SetCurrentGesture(currentGesture);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Success());
        }
        switch (currentGesture)
        {
            case KinectGestures.Gestures.Clap:
                if (gesture.IsClap())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.PointSidetoSide:
                if (gesture.IsPointSidetoSide())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.Here:
                if (gesture.IsHere())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.BrushHair:
                if (gesture.IsBrushHair())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.SwirlyArms:
                if (gesture.IsSwirlyArms())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.HandSweep:
                if (gesture.IsHandSweep())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.Behold:
                if (gesture.IsBehold())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.Clench:
                if (gesture.IsClench())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.MicDrop:
                if (gesture.IsMicDrop())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.Sweep:
                if (gesture.IsSweep())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.YogaTree:
                if (gesture.IsYogaTree())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.StepAndSweep:
                if (gesture.IsStepAndSweep())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.HeadTilt:
                if (gesture.IsHeadTilt())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.ForearmWave:
                if (gesture.IsForearmWave())
                {
                    StartCoroutine(Success());
                }
                break;
            case KinectGestures.Gestures.ForearmPivot:
                if (gesture.IsForearmPivot())
                {
                    StartCoroutine(Success());
                }
                break;
               
        }
    }

    IEnumerator Success()
    {
        Camera.main.backgroundColor = Color.green;
        yield return new WaitForSeconds(1f);
        Camera.main.backgroundColor = Color.red;
    }

    public void SetGesture(string g)
    {
        switch (g)
        {
            case "Clap":
                t.text = "CLAP";
                gesture.SetCurrentGesture(KinectGestures.Gestures.Clap);
                break;
            case "Here":
                t.text = "HERE";
                gesture.SetCurrentGesture(KinectGestures.Gestures.Here);
                break;
            case "Point":
                t.text = "POINT SIDE TO SIDE";
                gesture.SetCurrentGesture(KinectGestures.Gestures.PointSidetoSide);
                break;
            case "Brush":
                t.text = "BRUSH HAIR";
                gesture.SetCurrentGesture(KinectGestures.Gestures.BrushHair);
                break;
            case "Swirl":
                t.text = "SWIRLY ARMS";
                gesture.SetCurrentGesture(KinectGestures.Gestures.SwirlyArms);
                break;
            case "HandSweep":        
                t.text = "HAND SWEEP";
                gesture.SetCurrentGesture(KinectGestures.Gestures.HandSweep);
                break;
            case "Behold":
                t.text = "BEHOLD";
                gesture.SetCurrentGesture(KinectGestures.Gestures.Behold);
                break;
            case "Clench":
                t.text = "CLENCH";
                gesture.SetCurrentGesture(KinectGestures.Gestures.Clench);
                break;
            case "MicDrop":
                t.text = "DROP THE MIC";
                gesture.SetCurrentGesture(KinectGestures.Gestures.MicDrop);
                break;
            case "Sweep":
                t.text = "FOOT SWEEP";
                gesture.SetCurrentGesture(KinectGestures.Gestures.Sweep);
                break;
            case "YogaTree":
                t.text = "YOGA TREE";
                gesture.SetCurrentGesture(KinectGestures.Gestures.YogaTree);
                break;
            case "StepSweep":
                t.text = "STEP FORWARD AND SWIPE UP";
                gesture.SetCurrentGesture(KinectGestures.Gestures.StepAndSweep);
                break;
            case "HeadTilt":
                t.text = "WHO'S A GOOD BOY??";
                gesture.SetCurrentGesture(KinectGestures.Gestures.HeadTilt);
                break;
            case "FWave":
                t.text = "WAVE FOREARMS";
                gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmWave);
                break;
            case "FPivot":
                t.text = "FOREARM PIVOT";
                gesture.SetCurrentGesture(KinectGestures.Gestures.ForearmPivot);
                break;
        }

    }
}
