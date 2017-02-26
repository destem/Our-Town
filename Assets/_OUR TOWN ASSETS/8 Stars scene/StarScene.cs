using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScene : MonoBehaviour {

    public Texture2D staticBackground;
    public Texture2D[] stars;
    public Texture2D maskOne;
    public Texture2D maskTwo;
    public Material displayMat;
    Color32[] starBlock;
    Texture2D dynamicBackground;
    bool usingGrowth = true;
    Vector2[] coordArray;
    List<Vector2> coordList;
    System.Random rng = new System.Random();

    List<Vector2> Shuffle(List<Vector2> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector2 value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    // Use this for initialization
    void Start () {
        rng = new System.Random();
        coordArray = new Vector2[8192 / 16 * 1024 / 16];
        for (int i = 0; i< coordArray.Length; i++)
        {
            coordArray[i] = new Vector2((i % 512) * 16, (i / 512) * 16);
        }
        coordList = new List<Vector2>(coordArray);
        coordList = Shuffle(coordList);
        
        dynamicBackground = Instantiate(staticBackground);
        StartCoroutine("MakeStars");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MakeStars()
    {
        int n = coordList.Count - 1;
        while (n > 0)
        {
            int cluster = n > 20 ? 20 : n;
            for (int i = 0; i < cluster; i++)
            {
                BlitStar((int)coordList[n].x, (int)coordList[n].y);
                n--;
            }
            dynamicBackground.Apply();

            yield return null;
        }
    }

    void BlitStar(int x, int y)
    {
        starBlock = stars[Random.Range(0, 3)].GetPixels32();
        Color32[] blendedBlock = new Color32[1024];
        //loop through 32 x 32, get bg color, combine with corresponding star color
        for (int j = 0; j < 32; j++)
        {
            for (int i = 0; i < 32; i++)
            {
                Color bgColor = dynamicBackground.GetPixel(x + i, y + j);
                Color32 starColor = starBlock[j * 32 + i];
                Color blendedColor = new Color(starColor.r / 255f * starColor.a / 255f + bgColor.r * (1 - starColor.a / 255f), starColor.g / 255f * starColor.a / 255f + bgColor.g * (1 - starColor.a / 255f), starColor.b / 255f * starColor.a / 255f + bgColor.b * (1 - starColor.a / 255f));


                blendedBlock[j * 32 + i] = (Color32)blendedColor;
            }
        }
        
            dynamicBackground.SetPixels32(x, y, 32, 32, blendedBlock);
       
    }

void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(dynamicBackground, dest);
    }
}
