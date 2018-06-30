using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : LevelObject {

    public float pullPercentage = 0.2f;

    private List<GridDot> surroundingDots = new List<GridDot>();
    private bool waitForRotation = false;

	void Start ()
    {
        RythmManager.onBPM.AddListener(OnRythm);
	}

    private void OnRythm(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.rotatorBPM))
        {
            Debug.Log("Ratoator bpm");
            // if are there dots that can be rotated around this rotator, rotate them
            if (FindSourroundingDots())
            {
                waitForRotation = true;
            }
            else Debug.LogError("Not able to rotate, because some sourrounding dots are not available!");
        }
        if(bpm.Equals(RythmManager.playerBPM) && waitForRotation)
        {
            waitForRotation = false;
            RotateSurroundingDots(RythmManager.playerBPM.ToSecs());
        }
    }

    // find the dots sourrounding this rotator
    bool FindSourroundingDots()
    {
        surroundingDots = null;
        surroundingDots = Grid.GetSourroundingDots(gridDot);

        foreach (GridDot d in surroundingDots)
        {
            if (d != null && d.active && d.gameObject.activeSelf)
            {
            }
            else
                return false;
        }
        return true;
    }

    // rotate 'em
    void RotateSurroundingDots(float duration)
    {
        StartCoroutine(C_Rotate(duration));
    }


    IEnumerator C_Rotate(float totalDuration)
    {

        // divide the duration into three parts. suck in, rotation, suck out.
        float duration = totalDuration / 2;

        GridDot[] dotArray = surroundingDots.ToArray();

        // store all the position data prior to the suck in
        Hashtable originalPositions = new Hashtable();
        foreach(GridDot dot in surroundingDots)
        {
            originalPositions.Add(dot, new Vector3(dot.transform.position.x, dot.transform.position.y, 0));
        }


        Vector3 slerpPos = new Vector3();
        Vector3 endPos = new Vector3();

        // suck in
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            for(int i = 0; i<dotArray.Length; i++)
            {
                endPos = Vector3.Lerp((Vector3)originalPositions[dotArray[i]], transform.position, pullPercentage);
                slerpPos = Vector3.Slerp((Vector3)originalPositions[dotArray[i]], endPos, (elapsedTime / duration));
                //Debug.Log("i: " + i + "  --- originalpos: " + (Vector3)originalPositions[dotArray[i]] + "  --- endPos: " + endPos + "  ---- SlerpPos: " + slerpPos  + "  ---- currentPos: " + dotArray[i].transform.position);
                dotArray[i].transform.transform.position = slerpPos;

            }
            yield return null;
        }


        // store all the position data prior to the shoot out
        Hashtable suckedInPositions = new Hashtable();
        foreach (GridDot dot in surroundingDots)
        {
            suckedInPositions.Add(dot, new Vector3(dot.transform.position.x, dot.transform.position.y, 0));
        }

        // shoot out
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < dotArray.Length; i++)
            {
                endPos = (Vector3)originalPositions[dotArray[i]];
                slerpPos = Vector3.Slerp((Vector3)suckedInPositions[dotArray[i]], endPos, (elapsedTime / duration));
                //Debug.Log("i: " + i + "  --- originalpos: " + (Vector3)originalPositions[dotArray[i]] + "  --- endPos: " + endPos + "  ---- SlerpPos: " + slerpPos  + "  ---- currentPos: " + dotArray[i].transform.position);
                dotArray[i].transform.transform.position = slerpPos;

            }
            yield return null;
        }

        FixGridDots();
        yield return null;
    }

    void FixGridDots()
    {

    }
}
