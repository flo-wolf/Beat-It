using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : LevelObject {

    public static List<Rotator> rotators = new List<Rotator>();
    public static int currentRotator = 0;

    public float pullPercentage = 0.2f;
    public bool rotateRight = true;

    private List<GridDot> surroundingDots = new List<GridDot>();
    private bool waitForRotation = false;
    private bool nextDot = false;

    private void OnEnable()
    {
        rotators = new List<Rotator>();
    }

    void Start ()
    {
        rotators.Add(this);
        RythmManager.onBPM.AddListener(OnRythm);
	}

    private void OnRythm(BPMinfo bpm)
    {
        if (bpm.Equals(RythmManager.rotatorBPM) && Game.state == Game.State.Playing)
        {
            Debug.Log("Ratoator bpm");
            // if are there dots that can be rotated around this rotator, rotate them
            if (rotators[currentRotator].Equals(this))
            {
                nextDot = true;
                if (FindSourroundingDots())
                {
                    waitForRotation = true;
                }
                else Debug.LogError("Not able to rotate, because some sourrounding dots are not available!");
            }
        }
        if(bpm.Equals(RythmManager.playerBPM) && nextDot)
        {
            nextDot = false;

            if (waitForRotation)
            {
                waitForRotation = false;
                RotateSurroundingDots(RythmManager.playerBPM.ToSecs());
            }

            // play rotators in sequence if there are multiple than one in one level
            if (currentRotator + 1 < rotators.Count)
                currentRotator++;
            else
                currentRotator = 0;
        }
    }

    // find the dots sourrounding this rotator
    bool FindSourroundingDots()
    {
        surroundingDots = null;
        surroundingDots = Grid.GetSurroundingDots(gridDot);

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

        // divide the duration into two parts. suck in, suck out. During both phases the rotator rotates
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
        Vector3 betweenDotsPos = new Vector3();

        // suck in
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation;
        endRotation *= Quaternion.Euler(0, 0, -30); // this adds a 90 degrees Y rotation


        float elapsedTime = 0f;
        float elapsedRotationTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            elapsedRotationTime += Time.deltaTime;

            for (int i = 0; i < dotArray.Length; i++)
            {
                // j = nextDot indicator
                int j = i;
                if (rotateRight)
                {
                    if (i + 1 < dotArray.Length)
                        j++;
                    else
                        j = 0;
                }
                else
                {
                    if (i - 1 >= 0)
                        j--;
                    else
                        j = dotArray.Length - 1;
                }

                betweenDotsPos = Vector3.Lerp((Vector3)originalPositions[dotArray[i]], (Vector3)originalPositions[dotArray[j]], 0.5f);
                endPos = Vector3.Lerp(betweenDotsPos, transform.position, pullPercentage); // "sucked in" position, 80% of the way from the dotPos towards the rotator center
                slerpPos = Vector3.Slerp((Vector3)originalPositions[dotArray[i]], endPos, (elapsedTime / duration));

                //Debug.Log("i: " + i + "  --- originalpos: " + (Vector3)originalPositions[dotArray[i]] + "  --- endPos: " + endPos + "  ---- SlerpPos: " + slerpPos  + "  ---- currentPos: " + dotArray[i].transform.position);
                dotArray[i].transform.transform.position = slerpPos;


            }

            // rotate the roator dot
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime / duration));

            yield return null;
        }


        // store all the position data prior to the shoot out
        Hashtable suckedInPositions = new Hashtable();
        foreach (GridDot dot in surroundingDots)
        {
            suckedInPositions.Add(dot, new Vector3(dot.transform.position.x, dot.transform.position.y, 0));
        }

        // shoot out
        startRotation = transform.rotation;
        endRotation = transform.rotation;
        endRotation *= Quaternion.Euler(0, 0, -30); // this adds a 90 degrees Y rotation

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            elapsedRotationTime += Time.deltaTime;

            for (int i = 0; i < dotArray.Length; i++)
            {
                int j = i;
                if (rotateRight)
                {
                    if (i + 1 < dotArray.Length)
                        j++;
                    else
                        j = 0;
                }
                else
                {
                    if (i - 1 >= 0)
                        j--;
                    else
                        j = dotArray.Length - 1;
                }

                endPos = (Vector3)originalPositions[dotArray[j]];
                slerpPos = Vector3.Slerp((Vector3)suckedInPositions[dotArray[i]], endPos, (elapsedTime / duration));
                //Debug.Log("i: " + i + "  --- originalpos: " + (Vector3)originalPositions[dotArray[i]] + "  --- endPos: " + endPos + "  ---- SlerpPos: " + slerpPos  + "  ---- currentPos: " + dotArray[i].transform.position);
                dotArray[i].transform.transform.position = slerpPos;

            }


            // rotate the roator dot
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime/duration));
            yield return null;
        }

        FixGridDots();
        yield return null;
    }

    void FixGridDots()
    {
        GridDot[] dotArray = surroundingDots.ToArray();
        GridDot memoryDot = null;

        Hashtable originalRowsColumns = new Hashtable();

        // store the original row/column information
        for (int i = 0; i < dotArray.Length; i++)
        {
            int[] rc = new int[] { dotArray[i].row, dotArray[i].column };
            originalRowsColumns.Add(dotArray[i], rc);
        }

        for (int i = 0; i < dotArray.Length; i++)
        {
            // i = original dot
            // j = next dot in rotation that has the correct row and column information for our rotated dot.
            int j = i;
            if (rotateRight)
            {
                if (i + 1 < dotArray.Length)
                    j++;
                else
                    j = 0;
            }
            else
            {
                if (i - 1 >= 0)
                    j--;
                else
                    j = dotArray.Length - 1;
            }

            int[] newRowsColumns = (int[])originalRowsColumns[dotArray[j]];

            int newRow = newRowsColumns[0];
            int newColumn = newRowsColumns[1];

            dotArray[i].row = newRow;
            dotArray[i].column = newColumn;

            Grid.gridDots[newRow, newColumn] = dotArray[i];
        }
    }
}
