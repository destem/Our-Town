using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScene : MonoBehaviour {

    public Texture2D staticBackground;
    public Texture2D star;
    Color32[] starBlock;
    Texture2D dynamicBackground;

	// Use this for initialization
	void Start () {
        starBlock = star.GetPixels32();
        dynamicBackground = Instantiate(staticBackground);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            int mousex = (int)Input.mousePosition.x;
            int mousey = (int)Input.mousePosition.y;
            Color32[] blendedBlock = new Color32[1024];
            //loop through 32 x 32, get bg color, combine with corresponding star color
            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 32; i++)
                {
                    Color bgColor = dynamicBackground.GetPixel(mousex + i, mousey - j);
                    Color32 starColor = starBlock[j * 32 + i];
                    //print(starColor.a.ToString());
                    Color blendedColor = new Color(starColor.r / 255f * starColor.a / 255f + bgColor.r * (1 - starColor.a / 255f), starColor.g / 255f * starColor.a / 255f + bgColor.g * (1 - starColor.a / 255f), starColor.b / 255f * starColor.a / 255f + bgColor.b * (1 - starColor.a / 255f));
                    //pre-multiplied alph?a (PNG)
                    //Color blendedColor = new Color(starColor.r/255f + bgColor.r * (1 - starColor.a / 255f), starColor.g / 255f + bgColor.g * (1 - starColor.a / 255f), starColor.b / 255f + bgColor.b * (1 - starColor.a / 255f));


                    blendedBlock[j * 32 + i] = (Color32)blendedColor;
                }
            }
            
            dynamicBackground.SetPixels32(mousex, mousey, 32, 32, blendedBlock);
            dynamicBackground.Apply();
        }
	}

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(dynamicBackground, dest);
    }
}
