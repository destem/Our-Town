using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UVHandCoordinates : MonoBehaviour {
    public Transform lWrist;
    public Transform rWrist;
    public Text t;
    RaycastHit rHit;
    RaycastHit lHit;
    public TreeScene scene;
    bool flag = false;

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
        flag = false;
        if (Physics.Raycast(lWrist.position, lWrist.up, out lHit)) {
            if (lHit.collider.gameObject.name == "360screen_stretchedUVs")
            {
                flag = true;
                Vector2 coords = new Vector2(lHit.textureCoord.x, lHit.textureCoord.y);
                //scene.SetLeftHand(coords);
                //t.text = string.Format("{0:0.000}, {1:0.000}", coords.x, coords.y);
            }
        }
        if (Physics.Raycast(rWrist.position, rWrist.up, out rHit))
        {
            if (rHit.collider.gameObject.name == "360screen_stretchedUVs")
            {
                flag = true;
                Vector2 coords = new Vector2(rHit.textureCoord.x, rHit.textureCoord.y);
                //scene.SetRightHand(coords);
                //t.text = string.Format("{0:0.000}, {1:0.000}", coords.x, coords.y);
            }
        }
        if (!flag)
        {
            scene.ResetUVs();
        }
        //Debug.DrawRay(lWrist.position, lWrist.up, Color.red);
    }
}
