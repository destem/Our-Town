using UnityEngine;
using System.Collections;

public class PingPongDistance : MonoBehaviour {

    Transform[] children;
    float count;
    float t;
    public float min = .005f;
    public float max = .2f;
    public float pingPongtime = 10f;

	// Use this for initialization
	void Start () {
        children = GetComponentsInChildren<Transform> ();
        count = transform.childCount;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            t = Input.mousePosition.x / 2000f;
            t = Mathf.Clamp(t, min, max);
            for (int i = 0; i <= count; i++)
            {
                children[i].localPosition = new Vector3(0f, 0f, -i * t + count * t / 2f);
            }
        }
        //t = Mathf.Lerp (min, max, Mathf.PingPong (Time.time / pingPongtime, 1f));
        
	}
}
    