using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSizer : MonoBehaviour {

    [Header("Change these values to update the camera to fit the grid")]
    public float paddingTop;
    public float paddingRight;

    private BoxCollider2D gridBoxCollider = null;

    private void OnEnable()
    {
        if(gridBoxCollider == null)
            gridBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnValidate()
    {
        if (gridBoxCollider == null)
            gridBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnGUI()
    {
        if (gridBoxCollider == null)
            gridBoxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if(gridBoxCollider != null)
        FitCameraToGridBounds();
#endif
    }

    void FitCameraToGridBounds()
    {
        float boxH = gridBoxCollider.size.y / 2;
        float boxW = gridBoxCollider.size.x / 2;

        float aspect = Camera.main.aspect;

        // is the box heigehr than wide? => pad the height
        if (boxH * aspect >= boxW)
            Camera.main.orthographicSize = boxH + paddingTop;
        else
            Camera.main.orthographicSize = (boxW/aspect) + paddingRight;
    }
}
