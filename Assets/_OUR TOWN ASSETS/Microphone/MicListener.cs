using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MicListener : MonoBehaviour {
	
	public float sensitivity = 100;
	static public float loudness = 0;
	AudioSource sound;
	bool micExists = false;
	string micName;
	int i; //counter

	float[] soundData = new float[256];
	float soundSum = 0;

	Text micText;

	void Start() {
        foreach (string device in Microphone.devices)
        {
            print("Name: " + device);
        }

        if (Microphone.devices.Length > 0)
        {
            micExists = true;
            //micName = Microphone.devices[0];
            //print("we have a mic and its name is " + micName);
            sound = GetComponent<AudioSource>();
            sound.clip = Microphone.Start(null, true, 10, 44100); // theoretically should loop over itself, so we don't need to flush RAM
            sound.loop = true; // Set the AudioClip to loop
            while (!(Microphone.GetPosition(null) > 0)) { } // Wait until recording starts
            sound.Play(); // Play the audio source so that we can check its volume
            print("Recording");
        }
        //micText = GameObject.Find ("MicText").GetComponent<Text> ();
    }

	void Update(){
		if (micExists) {
			loudness = GetAveragedVolume () * sensitivity;
			//if (micText) {
			//	micText.text = "Loudness: " + string.Format("{0:0.000}", loudness);

   //         }
		}
	}

	float GetAveragedVolume()
	{ 
		soundSum = 0f;
		sound.GetOutputData(soundData,0);
		for (i=0; i< soundData.Length; i++)
		{
			soundSum += Mathf.Abs(soundData[i]);
		}
		return soundSum/256;
	}
}