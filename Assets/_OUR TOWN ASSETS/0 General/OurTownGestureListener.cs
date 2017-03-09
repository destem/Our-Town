using UnityEngine;
using System.Collections;
using System;
//using Windows.Kinect;

public class OurTownGestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("GUI-Text to display gesture-listener messages and gesture information.")]
	 GUIText gestureInfo;

	// singleton instance of the class
	private static OurTownGestureListener instance = null;

	// internal variables to track if progress message has been displayed
	private bool progressDisplayed;
	private float progressGestureTime;

	// whether the needed gesture has been detected or not
	private bool swipeLeft;
	private bool swipeRight;
	private bool swipeUp;
    private bool swipeDown;
    private bool leanRight;
    private bool psi;
    private bool clap;
    private bool here;
    private bool pointSidetoSide;
    private bool brushHair;
    private bool swirlyArms;
    private bool handSweep;
    private bool behold;
    private bool clench;
    private bool micDrop;
    private bool sweep;
    private bool yogaTree;
    private bool stepAndSweep;
    private bool headTilt;
    private bool forearmWave;
    private bool forearmPivot;
    private bool currentGestureComplete;


    KinectGestures.Gestures currentGesture = KinectGestures.Gestures.LeanRight; //should never actually get used
    KinectManager manager = KinectManager.Instance;
    long currentID;

    public long getCurrentID()
    {
        return currentID;
    }

    public bool SetCurrentGesture(KinectGestures.Gestures g)
    {
        //print(KinectManager.Instance);
        //print(currentGesture);
        //if (KinectManager.Instance.GetGesturesCount(KinectManager.Instance.GetUserIdByIndex(0)) > 0)
        //{
        //    KinectManager.Instance.DeleteGesture(KinectManager.Instance.GetUserIdByIndex(0), currentGesture);
        //}

        //KinectManager.Instance.ClearGestures(KinectManager.Instance.GetUserIdByIndex(0));
        //currentGesture = g;
        //KinectManager.Instance.DetectGesture(KinectManager.Instance.GetUserIdByIndex(0), g);

        KinectManager.Instance.ClearGestures(currentID);
        currentGesture = g;
        KinectManager.Instance.DetectGesture(currentID, g);

        return true;
    }

    public KinectGestures.Gestures GetCurrentGesture()
    {
        return KinectManager.Instance.GetGesturesList(KinectManager.Instance.GetUserIdByIndex(0))[0];
    }
    /// <summary>
    /// Gets the singleton CubeGestureListener instance.
    /// </summary>
    /// <value>The CubeGestureListener instance.</value>
    public static OurTownGestureListener Instance
	{
		get
		{
			return instance;
		}
	}
	
	/// <summary>
	/// Determines whether swipe left is detected.
	/// </summary>
	/// <returns><c>true</c> if swipe left is detected; otherwise, <c>false</c>.</returns>
	public bool IsSwipeLeft()
	{
		if(swipeLeft)
		{
			swipeLeft = false;
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether swipe right is detected.
	/// </summary>
	/// <returns><c>true</c> if swipe right is detected; otherwise, <c>false</c>.</returns>
	public bool IsSwipeRight()
	{
		if(swipeRight)
		{
			swipeRight = false;
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether swipe up is detected.
	/// </summary>
	/// <returns><c>true</c> if swipe up is detected; otherwise, <c>false</c>.</returns>
	public bool IsSwipeUp()
	{
		if(swipeUp)
		{
			swipeUp = false;
			return true;
		}
		
		return false;
	}

    public bool IsSwipeDown()
    {
        if (swipeDown)
        {
            swipeDown = false;
            return true;
        }

        return false;
    }

    public bool IsPsi()
    {
        if (psi)
        {
            psi = false;
            return true;
        }

        return false;
    }

    public bool IsClap()
    {
        if (clap)
        {
            clap = false;
            return true;
        }

        return false;
    }

    public bool IsHere()
    {
        if (here)
        {
            here = false;
            return true;
        }

        return false;
    }

    public bool IsPointSidetoSide()
    {
        if (pointSidetoSide)
        {
            pointSidetoSide = false;
            return true;
        }

        return false;
    }

    public bool IsBrushHair()
    {
        if (brushHair)
        {
            brushHair = false;
            return true;
        }

        return false;
    }

    public bool IsSwirlyArms()
    {
        if (swirlyArms)
        {
            swirlyArms = false;
            return true;
        }

        return false;
    }

    public bool IsHandSweep()
    {
        if (handSweep)
        {
            handSweep = false;
            return true;
        }

        return false;
    }

    public bool IsBehold()
    {
        if (behold)
        {
            behold = false;
            return true;
        }

        return false;
    }

    public bool IsClench()
    {
        if (clench)
        {
            clench = false;
            return true;
        }

        return false;
    }

    public bool IsMicDrop()
    {
        if (micDrop)
        {
            micDrop = false;
            return true;
        }

        return false;
    }

    public bool IsSweep()
    {
        if (sweep)
        {
            sweep = false;
            return true;
        }

        return false;
    }

    public bool IsYogaTree()
    {
        if (yogaTree)
        {
            yogaTree = false;
            return true;
        }

        return false;
    }

    public bool IsStepAndSweep()
    {
        if (stepAndSweep)
        {
            stepAndSweep = false;
            return true;
        }

        return false;
    }

    public bool IsHeadTilt()
    {
        if (headTilt)
        {
            headTilt = false;
            return true;
        }

        return false;
    }

    public bool IsForearmWave()
    {
        if (forearmWave)
        {
            forearmWave = false;
            return true;
        }

        return false;
    }

    public bool IsForearmPivot()
    {
        if (forearmPivot)
        {
            forearmPivot = false;
            return true;
        }

        return false;
    }
    public bool IsLeanRight()
    {
        if (leanRight)
        {
            leanRight = false;
            return true;
        }

        return false;
    }

    public bool IsCurrentGesture()
    {
        if (currentGestureComplete)
        {
            currentGestureComplete = false;
            return true;
        }
        return false;
    }

    public void ClearGestureSuccess()
    {
        currentGestureComplete = false;
    }


    /// <summary>
    /// Invoked when a new user is detected. Here you can start gesture tracking by invoking KinectManager.DetectGesture()-function.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="userIndex">User index</param>
    public void UserDetected(long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		//KinectManager manager = KinectManager.Instance;
		//if(!manager || (userIndex != playerIndex))
		//	return;

        // detect these user specific gestures
        print("Setting dectected");

        currentID = userId;
        SetCurrentGesture(currentGesture);
		//manager.DetectGesture(userId, currentGesture);
		//manager.DetectGesture(userId, KinectGestures.Gestures.SwipeRight);
	 //   manager.DetectGesture(userId, KinectGestures.Gestures.SwipeUp);
  //      manager.DetectGesture(userId, KinectGestures.Gestures.Psi);
  //      manager.DetectGesture(userId, KinectGestures.Gestures.SwipeDown);
        // manager.DetectGesture(userId, KinectGestures.Gestures.Clap);


    }

	/// <summary>
	/// Invoked when a user gets lost. All tracked gestures for this user are cleared automatically.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	public void UserLost(long userId, int userIndex)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return;
		
		if(gestureInfo != null)
		{
			gestureInfo.text = string.Empty;
		}
	}

	/// <summary>
	/// Invoked when a gesture is in progress.
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="progress">Gesture progress [0..1]</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public void GestureInProgress(long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectInterop.JointType joint, Vector3 screenPos)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return;

		if((gesture == KinectGestures.Gestures.ZoomOut || gesture == KinectGestures.Gestures.ZoomIn) && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - {1:F0}%", gesture, screenPos.z * 100f);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
		else if((gesture == KinectGestures.Gestures.Wheel || gesture == KinectGestures.Gestures.LeanLeft || 
		         gesture == KinectGestures.Gestures.LeanRight) && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - {1:F0} degrees", gesture, screenPos.z);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
		else if(gesture == KinectGestures.Gestures.Run && progress > 0.5f)
		{
			if(gestureInfo != null)
			{
				string sGestureText = string.Format ("{0} - progress: {1:F0}%", gesture, progress * 100);
				gestureInfo.text = sGestureText;
				
				progressDisplayed = true;
				progressGestureTime = Time.realtimeSinceStartup;
			}
		}
	}

	/// <summary>
	/// Invoked if a gesture is completed.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	/// <param name="screenPos">Normalized viewport position</param>
	public bool GestureCompleted (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint, Vector3 screenPos)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return false;
		
		if(gestureInfo != null)
		{
			string sGestureText = gesture + " detected";
			gestureInfo.text = sGestureText;
		}

        currentGestureComplete = true;

        if (gesture == KinectGestures.Gestures.SwipeLeft)
			swipeLeft = true;
		else if(gesture == KinectGestures.Gestures.SwipeRight)
			swipeRight = true;
		else if(gesture == KinectGestures.Gestures.SwipeUp)
			swipeUp = true;
        else if (gesture == KinectGestures.Gestures.SwipeDown)
            swipeDown = true;
        else if (gesture == KinectGestures.Gestures.Psi)
            psi = true;
        else if (gesture == KinectGestures.Gestures.Clap)
            clap = true;
        else if (gesture == KinectGestures.Gestures.Here)
            here = true;
        else if (gesture == KinectGestures.Gestures.PointSidetoSide)
            pointSidetoSide = true;
        else if (gesture == KinectGestures.Gestures.BrushHair)
            brushHair = true;
        else if (gesture == KinectGestures.Gestures.TheMoreYouKnow)
            swirlyArms = true;
        else if (gesture == KinectGestures.Gestures.Behold)
            behold = true;
        else if (gesture == KinectGestures.Gestures.Clench)
            clench = true;
        else if (gesture == KinectGestures.Gestures.MicDrop)
            micDrop = true;
        else if (gesture == KinectGestures.Gestures.Sweep)
            sweep = true;
        else if (gesture == KinectGestures.Gestures.YogaTree)
            yogaTree = true;
        else if (gesture == KinectGestures.Gestures.StepAndSweep)
            stepAndSweep = true;
        else if (gesture == KinectGestures.Gestures.HeadTilt)
            headTilt = true;
        else if (gesture == KinectGestures.Gestures.HandSweep)
            handSweep = true;
        else if (gesture == KinectGestures.Gestures.ForearmWave)
            forearmWave = true;
        else if (gesture == KinectGestures.Gestures.ForearmPivot)
            forearmPivot = true;
        else if (gesture == KinectGestures.Gestures.LeanRight)
            leanRight = true;

        return true;
	}

	/// <summary>
	/// Invoked if a gesture is cancelled.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture type</param>
	/// <param name="joint">Joint type</param>
	public bool GestureCancelled (long userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectInterop.JointType joint)
	{
		// the gestures are allowed for the primary user only
		if(userIndex != playerIndex)
			return false;
		
		if(progressDisplayed)
		{
			progressDisplayed = false;
			
			if(gestureInfo != null)
			{
				gestureInfo.text = String.Empty;
			}
		}
		
		return true;
	}

	
	void Awake()
	{
		instance = this;
        manager = KinectManager.Instance;
	}

	void Update()
	{
		if(progressDisplayed && ((Time.realtimeSinceStartup - progressGestureTime) > 2f))
		{
			progressDisplayed = false;
			gestureInfo.text = String.Empty;

			Debug.Log("Forced progress to end.");
		}
	}

}
