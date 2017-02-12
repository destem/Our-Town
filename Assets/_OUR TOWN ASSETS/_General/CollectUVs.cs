using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CollectUVs : MonoBehaviour
{

    List<float> coords;
    public Material RIMat;
    public Texture2D TextoView;

    // Use this for initialization
    void Start()
    {
        coords = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            coords.Add(mousePos.x / Screen.width);
            coords.Add(mousePos.y / Screen.height);
            //theString += string.Format("{0:0.000}, {1:0.000}", mousePos.x / Screen.width, mousePos.y / Screen.height);
            EditorGUIUtility.systemCopyBuffer = "{" + string.Join(", ", coords.ConvertAll(i => string.Format("{0:0.000}f", i)).ToArray()) + "}";

        }
    }
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(TextoView, dest, RIMat);
    }
}
