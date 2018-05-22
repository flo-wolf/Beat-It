using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour {
    public Material lineMaterial;



    public Color mainColor = new Color(0f, 1f, 0f, 1f);

    // Use this for initialization
    void Start () {
        
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 3);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Less);
            //lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        }
        
    }

    void OnPostRender()
    {
        CreateLineMaterial();
        // set the current material
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        GL.Color(mainColor);

        int xCount = InputInterpreter.keybaordMap[0].Length;
        int yCount = InputInterpreter.keybaordMap.Length - 1;

        // lineRenderer.positionCount = InputInterpreter.keybaordMap.Length * InputInterpreter.keybaordMap[0].Length;

        //X axis lines (top to bottom)
        for (float i = 0; i < xCount; i ++)
        {
            Vector3 start = new Vector3((1+(i * 2)) / 16, 1f - (3f / 18f), 1);
            Vector3 end = new Vector3((1+((i * 2) + yCount * 0.5f)) / 16, (1f - ((3f + (yCount * 4f)) / 18f)), 1);

            start = Camera.main.ViewportToWorldPoint(start);
            end = Camera.main.ViewportToWorldPoint(end);

            start.z = 1;
            end.z = 1;

            //GL.Vertex3((m * (1 + (i * 2))) / 16, m * (1f - (3f / 18f)), 1);
            //GL.Vertex3(m * ((1 + (i * 2)) + yCount * 0.5f) / 16, m * (1f - ((3f + (yCount * 4f)) / 18f)), 1);

            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
        }

        //Y axis lines (left to right)
        for (float i = 0; i <= yCount; i++)
        {
            Vector3 start = new Vector3((1f + i * 0.5f) / 16, (1f - ((3f + (i * 4f)) / 18f)), 1);
            Vector3 end = new Vector3(((1f + i * 0.5f) + (xCount*2f - 2)) / 16,
                (1f - ((3f + (i * 4f)) / 18f)),
                1);

            start = Camera.main.ViewportToWorldPoint(start);
            end = Camera.main.ViewportToWorldPoint(end);

            start.z = 1;
            end.z = 1;

            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
        }

        /*
        for (int k = 0; k < InputInterpreter.keybaordMap.Length; k++)
        {
            for (int j = 0; j < InputInterpreter.keybaordMap[k].Length; j++)
            {
                
                Vector2 worldPos = Camera.main.ViewportToWorldPoint(new Vector2(((1 + (j * 2)) + k * 0.5f) / 16, 1f - ((3f + (k * 4f)) / 18f)));
                lineRenderer.SetPosition(j + (InputInterpreter.keybaordMap[0].Length * k), worldPos);
            }
        }
        */
        GL.End();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
