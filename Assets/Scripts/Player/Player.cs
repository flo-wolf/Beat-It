using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Color Info:
/// BG Dark Blue: 09131B
/// Yellow: F7C95D
/// </summary>

public class Player : MonoBehaviour
{
    //Dash bools
    public bool dashOnBeat = false;

    public bool enableJump = false;

    // structure
    public static Player instance;           // self reference
    public static PlayerDot dot0 = null;  // mouse1 dot
    public static PlayerDot dot1 = null; // mouse2 dot

    // player was killed by a loop segment
    public static bool deathBySegment = false;
    public static bool deathByMovingKillDot = false;

    // keep track of which dot is the newest
    public enum DotType { None, Dot0, Dot1 }

    [HideInInspector]
    public DotType newestDot = DotType.None;

    /*
    [Header("Additional Player Controls")]
    public float maxIncreasedRadius = 10;
    public float radiusIncreaseSpeed;
    */

    [HideInInspector]
    public bool tempoUp = false;
    [HideInInspector]
    public bool tempoDown = false;

    public bool enableJump = false;


    [Header("Components")]
    public GameObject playerDotPrefab;      // needed for creating new playerDots
    public PlayerSegment playerSegment;     // the line drawn between two playerDots, collision detectable
    public LineRenderer radiusLineRenderer;

    // aiming
    [HideInInspector]
    public Vector2 lookDirection = new Vector2(); //
    private bool lookingRight = false;
    private GridDot aimedGridDot;
    private float thumbstickTreshhold = 0.03f;

    private GridDot spawnDot = null;
    private GridDot lastTeleportParentDot = null;

    public static bool allowMove = true;


    /// initialization
    void Start()
    {
        instance = this;

        spawnDot = Grid.FindPlayerSpawn();

        Game.onGameStateChange.AddListener(GameStateChanged);

        // move the player on the beat
        RythmManager.onBPM.AddListener(OnRythmMove);


        // math tests
        Vector3 a = new Vector3(2,2);
        Vector3 b = new Vector3(1, 1);
        float x = Vector3.Dot(a, b);

        Vector3 c = (b-a) * Mathf.Cos(90);

        // PlayerSpawnDataHolder (PlayerSpawnData) pos: (-20.0, 0.0, 0.0) level: 1
        // cross: a,b tests-------- x: (0.0, 0.0, 0.0)

    }

    /// Input Handling and Radius Drawing
    void Update()
    {
        PlayerDirectionHandle.instance.UpdateLookDirection();
        CheckIfSingleDot();
        //Debug.Log("LASTPARENTDOT == " + lastTeleportParentDot);
    }

    private void GameStateChanged(Game.State newState)
    {
        switch (newState)
        {
            case Game.State.Playing:
                allowMove = true;
                break;
            case Game.State.Death:
                Death();

                break;
            default: break;
        }
    }

    public bool CheckIfSingleDot()
    {
        if (dot0 != null && dot1 != null)
        {
            return false;
        }

        if (dot0 == null && dot1 == null)
        {
            return false;
        }

        return true;
    }

    // the clock has reached its end, move the player
    void OnRythmMove(BPMinfo bpm)
    {
        if (Game.state == Game.State.Playing && allowMove)
        {

            // check if the player is dashing
            bool playerIsDashing = false;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Joystick1Button5))
                playerIsDashing = true;

            // should dash happen on or offbeat?
            if (dashOnBeat == false)
            {
                if (bpm.Equals(RythmManager.playerBPM))
                {
                        HandleDotRemoval(false);
                }

                if (bpm.Equals(RythmManager.playerDashBPM) && playerIsDashing)
                {
                        HandleDotRemoval(true);
                }
            }

            else if (dashOnBeat == true)
            {
                if (bpm.Equals(RythmManager.playerBPM) && playerIsDashing)
                {
                        HandleDotRemoval(true);
                }

                if (bpm.Equals(RythmManager.playerDashBPM) && !playerIsDashing)
                {
                        HandleDotRemoval(false);
                }
            }
        }

    }



    // remove the correct dot, considering aimposition, teleporters, dashing...
    public void HandleDotRemoval(bool dashing)
    {
        Vector3 oldPos = Vector3.zero;
        Vector3 newPos = Vector3.zero;

        // both dots alive
        if (dot1 != null && dot0 != null)
        {
            if (!Teleporter.teleporterTouched)
            {
                if (newestDot == DotType.Dot0)
                {
                    newPos = dot0.transform.position;
                    oldPos = dot1.transform.position;

                    bool isAimingAtOldDot = IsAimingAtOldDot(oldPos, newPos);
                    if (isAimingAtOldDot)
                        RemoveDot(true);
                    else
                        RemoveDot(false);

                }
                else
                {
                    oldPos = dot0.transform.position;
                    newPos = dot1.transform.position;

                    bool isAimingAtOldDot = IsAimingAtOldDot(oldPos, newPos);
                    if (isAimingAtOldDot)
                        RemoveDot(false);
                    else
                        RemoveDot(true);
                }
                AudioManager.instance.Play("HiHat");
                AudioManager.instance.Play("Segment");

            }
            else // teleporter touched
            {
                if (Teleporter.dot0OnTeleportPosition)
                {
                    RemoveDot(false);
                    AudioManager.instance.Play("Snare");
                    SpawnDot();
                    RemoveDot(true);
                    Teleporter.teleportEnabled = false;
                }

                else if (!Teleporter.dot0OnTeleportPosition)
                {
                    RemoveDot(true);
                    AudioManager.instance.Play("Snare");
                    SpawnDot();
                    RemoveDot(false);
                    Teleporter.teleportEnabled = false;
                }
            }


        }

        // if there is only one dot
        else
        {
            if (lastTeleportParentDot != null)
            {
                if (Player.dot0 != null && Player.dot0.transform.position != lastTeleportParentDot.transform.position)
                {
                    Teleporter.teleportEnabled = true;
                }

                else if (Player.dot1 != null && Player.dot1.transform.position != lastTeleportParentDot.transform.position)
                {
                    Teleporter.teleportEnabled = true;
                }
            }

            SpawnDot();
            AudioManager.instance.Play("Kick");
            
        }
    }

    // we want to know if our aimposition from the NewestDot is closer to OldestDot or to another hexadot.
    public bool IsAimingAtOldDot(Vector3 oldPos, Vector3 newPos)
    { 
        Vector2 aimPos = Vector2.zero;

        // calculate the aimposition based on the input type
        // mouse input
        if (InputDeviceDetector.inputType == InputDeviceDetector.InputType.MouseKeyboard)
        {
            Vector2 mousePos = Input.mousePosition;
            aimPos = Camera.main.ScreenToWorldPoint(mousePos);
            aimPos = (Vector2)newPos - aimPos;

        }

        // controller input
        else
        {
            Vector2 controllerInput = new Vector2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y"));

            aimPos = controllerInput;
            aimPos = (Vector2)newPos - aimPos;
        }


        // calculate the position of the relative next point in the hexagon
        Vector3 oldToNewDir = newPos - oldPos; // 1,1

        Vector3 nextHexaDotPos = Vector3.zero;
        float hexaDegrees = 30f;
        float x = newPos.x + oldToNewDir.magnitude * Mathf.Cos((2f * Mathf.PI) / (360 / hexaDegrees));
        float y = newPos.y + oldToNewDir.magnitude * Mathf.Sin((2f * Mathf.PI) / (360 / hexaDegrees));
        nextHexaDotPos.x = x;
        nextHexaDotPos.y = y;

        Vector3 nextToNewDir = newPos - nextHexaDotPos;

        // calculate the angle between our old-to-newpoint-line and the nextdot
        float nextOldAngle = Vector3.Angle(oldToNewDir, nextToNewDir);
        nextOldAngle = 30f;

        // calculate the angle between our old-to-newpoint-line and the aimposition
        float aimPosAngle = Vector3.Angle(oldToNewDir, aimPos);


        Debug.Log("nextOldAngle: " + nextOldAngle + " aimPosAngle: " + aimPosAngle + " nextHexaDotPos: " + nextHexaDotPos);

        if (Mathf.Abs(nextOldAngle) > Mathf.Abs(aimPosAngle))
            return true;
        return false;
    }

    /// Creates a new PlayerDot Object at the lookDestination position and draws the connecting segment in between
    void SpawnDot()
    {
        if (Game.state == Game.State.Playing)
        {
            // only spawn dots of one of the dot slots is free => dont spawn more than the two conencted to the input triggers
            if (dot0 != null && dot1 != null)
                return;

            //Debug.Log("Dot0: " + dot0 + " ---- Dot1: " + dot1);
            int dotWasSpawned = -1; // -1 = no dot spawned, 0 = dot0 spawndd, 1 = dot1 spawned

            Vector2 controllerInput = new Vector2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y"));

            // the closest dot that the next dot should be spawned on
            GridDot parentDot = null;

            if (!Teleporter.teleporterTouched)
            {
                // dot1 exists, dot0 doesnt => spawn dot0
                if (dot1 != null && dot0 == null)
                {
                    parentDot = Grid.GetNearestActiveDot(dot1.gridDot, lookDirection);

                    if (parentDot == null)
                        return;

                    dotWasSpawned = 0;
                    PlayerDirectionHandle.instance.FadeRadius(false);
                }
                // dot0 exists, dot1 doesnt => spawn dot1
                else if (dot0 != null && dot1 == null)
                {
                    parentDot = Grid.GetNearestActiveDot(dot0.gridDot, lookDirection);

                    if (parentDot == null)
                        return;

                    dotWasSpawned = 1;
                    PlayerDirectionHandle.instance.FadeRadius(false);
                }
                // there are no dots, spawn the first dot
                else
                {
                    if (spawnDot == null)
                        parentDot = Grid.GetRandomActiveDot();
                    else
                        parentDot = spawnDot;
                    dotWasSpawned = 0;

                    PlayerDirectionHandle.instance.FadeRadius(true);
                }
            }

            else if (Teleporter.teleporterTouched)
            {
                // dot1 exists, dot0 doesnt => spawn dot0
                if (dot1 != null && dot0 == null)
                {
                    if (!Teleporter.instance.pairMode)
                    {
                        foreach (GridDot dot in Teleporter.instance.gridDotList)
                        {
                            if (dot1.transform.position == dot.transform.position)
                            {
                                Teleporter.instance.indexOfElement = Teleporter.instance.gridDotList.IndexOf(dot);
                                Debug.Log("TELEPORTER INDEX OF ELEMENT" + Teleporter.instance.indexOfElement);

                                if (Teleporter.instance.indexOfElement < (Teleporter.instance.lenghtofList - 1))
                                {
                                    int newDot = Teleporter.instance.indexOfElement + 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }

                                else if (Teleporter.instance.indexOfElement == (Teleporter.instance.lenghtofList - 1))
                                {
                                    parentDot = Teleporter.instance.gridDotList[0];
                                    lastTeleportParentDot = parentDot;
                                }
                            }
                        }
                    }

                    if (Teleporter.instance.pairMode)
                    {
                        foreach (GridDot dot in Teleporter.instance.gridDotList)
                        {
                            if (dot1.transform.position == dot.transform.position)
                            {
                                Teleporter.instance.indexOfElement = Teleporter.instance.gridDotList.IndexOf(dot);
                                Debug.Log("TELEPORTER INDEX OF ELEMENT" + Teleporter.instance.indexOfElement);

                                if (Teleporter.instance.indexOfElement % 2 == 0)
                                {
                                    int newDot = Teleporter.instance.indexOfElement + 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }

                                else if (Teleporter.instance.indexOfElement % 2 != 0)
                                {
                                    int newDot = Teleporter.instance.indexOfElement - 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }
                            }
                        }
                    }

                    if (parentDot == null)
                        return;

                    dotWasSpawned = 0;
                    PlayerDirectionHandle.instance.FadeRadius(false);

                }

                // dot0 exists, dot1 doesnt => spawn dot1
                else if (dot0 != null && dot1 == null)
                {
                    if (!Teleporter.instance.pairMode)
                    {
                        foreach (GridDot dot in Teleporter.instance.gridDotList)
                        {
                            if (dot0.transform.position == dot.transform.position)
                            {
                                Teleporter.instance.indexOfElement = Teleporter.instance.gridDotList.IndexOf(dot);
                                Debug.Log("TELEPORTER LISTPOSITION" + Teleporter.instance.indexOfElement);

                                if (Teleporter.instance.indexOfElement < (Teleporter.instance.lenghtofList - 1))
                                {
                                    int newDot = Teleporter.instance.indexOfElement + 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }

                                else if (Teleporter.instance.indexOfElement == (Teleporter.instance.lenghtofList - 1))
                                {
                                    parentDot = Teleporter.instance.gridDotList[0];
                                    lastTeleportParentDot = parentDot;
                                }
                            }
                        }
                    }

                    if (Teleporter.instance.pairMode)
                    {
                        foreach (GridDot dot in Teleporter.instance.gridDotList)
                        {
                            if (dot0.transform.position == dot.transform.position)
                            {
                                Teleporter.instance.indexOfElement = Teleporter.instance.gridDotList.IndexOf(dot);
                                Debug.Log("TELEPORTER INDEX OF ELEMENT" + Teleporter.instance.indexOfElement);

                                if (Teleporter.instance.indexOfElement % 2 == 0)
                                {
                                    int newDot = Teleporter.instance.indexOfElement + 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }

                                else if (Teleporter.instance.indexOfElement % 2 != 0)
                                {
                                    int newDot = Teleporter.instance.indexOfElement - 1;
                                    Debug.Log("INDEX NEW DOT" + newDot);
                                    parentDot = Teleporter.instance.gridDotList[newDot];
                                    lastTeleportParentDot = parentDot;
                                }
                            }
                        }
                    }

                    if (parentDot == null)
                        return;

                    dotWasSpawned = 1;
                    PlayerDirectionHandle.instance.FadeRadius(false);
                }

                // there are no dots, spawn the first dot
                else
                {
                    if (spawnDot == null)
                        parentDot = Grid.GetRandomActiveDot();
                    else
                        parentDot = spawnDot;
                    dotWasSpawned = 0;

                    //move the moving kill dot again

                    PlayerDirectionHandle.instance.FadeRadius(true);
                }
            }

            if (parentDot != null)
            {
                // spawn the new dot
                GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, Vector2.zero, Quaternion.identity);
                newDotGo.transform.parent = parentDot.transform;
                newDotGo.transform.localPosition = Vector3.zero;
                PlayerDot newPlayerDot = newDotGo.GetComponent<PlayerDot>();
                parentDot.levelObject = newPlayerDot;

                // set the newest dot information
                if (dotWasSpawned == 0)
                {
                    newDotGo.name = "dot0";
                    dot0 = newPlayerDot;
                    newestDot = DotType.Dot0;
                }
                else if (dotWasSpawned == 1)
                {
                    newDotGo.name = "dot1";
                    dot1 = newPlayerDot;
                    newestDot = DotType.Dot1;
                }
            }

            if(Teleporter.teleporterTouched)
            {
                Teleporter.teleporterTouched = false;
                Debug.Log("TELEPORTER TOUCHED" + Teleporter.teleporterTouched);
            }

            //Debug.Log("activePosition: " + activePosition + " spawnPosition: " + spawnPosition);

            // fill the segment in between two dots
            if (dot0 != null && dot1 != null)
            {
                if (newestDot == DotType.Dot0)
                    playerSegment.FillSegment(dot1.gridDot, dot0.gridDot, playerSegment.fillTime);
                else if (newestDot == DotType.Dot1)
                    playerSegment.FillSegment(dot0.gridDot, dot1.gridDot, playerSegment.fillTime);
            }
            //Instantiate(spawnEffect, newPlayerDot.transform.position, Quaternion.identity);
        }

    }


    /// Retracts the segment towards the new player dot and removes the old one
    public void RemoveDot(bool removeDot0)
    {
        //StartCoroutine(RadiusFade(true));

        bool switchSegmentDirection = false;

        if (removeDot0 && dot0 != null)
        {
            dot0.Remove();
            dot0 = null;

            if (dot1 != null)
            {
                if (newestDot == DotType.Dot1)
                    switchSegmentDirection = true;
                newestDot = DotType.Dot1;
                PlayerDirectionHandle.instance.FadeRadius(true);
            }
            else
                PlayerDirectionHandle.instance.FadeRadius(false);
        }
        else if (!removeDot0 && dot1 != null)
        {
            dot1.Remove();
            dot1 = null;

            if (dot0 != null)
            {
                if (newestDot == DotType.Dot0)
                    switchSegmentDirection = true;
                newestDot = DotType.Dot0;
                PlayerDirectionHandle.instance.FadeRadius(true);
            }
            else
                PlayerDirectionHandle.instance.FadeRadius(false);
        }

        // both dots are gone => Death/Restart
        if (dot1 == null && dot0 == null)
        {
            PlayerDirectionHandle.instance.radius = 0;
            PlayerDirectionHandle.instance.radiusOpacity = 0;
            newestDot = DotType.None;
        }

        // empty the segment in between those two dots
        playerSegment.EmptySegment(switchSegmentDirection, playerSegment.emptyTime);
    }

    public void Death()
    {
        allowMove = false;

        if (newestDot == DotType.Dot0)
        {
            RemoveDot(false);
            RemoveDot(true);
        }
        else if (newestDot == DotType.Dot1)
        {
            RemoveDot(true);
            RemoveDot(false);
        }

    }
}

////// OUTTAKES

/*
    /// enemy collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            GameObject.Destroy(this.gameObject);

    }

    IEnumerator travel()
    {
        float elapsedTime = 0f;
        Vector2 startPos = (Vector2)transform.position;
        Vector2 endPos = startPos + lookDirection;
        while (elapsedTime <= travelDuration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.SmoothStep(startPos.x, endPos.x, (elapsedTime / travelDuration));
            float newY = Mathf.SmoothStep(startPos.y, endPos.y, (elapsedTime / travelDuration));
            transform.position = new Vector2(newX, newY);

            yield return null;
        }
        _player.StartCoroutine(radiusFadeIn());

        yield return null;
    }


    /// rotate towards the mouse and calculate the look direction
    void LookAtMouse()
    {
        // face the player towards the destination point
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    
    void SpawnDot()
    {
        // calculate the spawn position
        Vector2 activePosition = _playerDots[_activeDotIndex].transform.position;
        Vector3 spawnPosition = activePosition + lookDirection;

        // spawn the new dot
        GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, spawnPosition, Quaternion.identity);
        PlayerDot newPlayerDot = newDotGo.GetComponent<PlayerDot>();

        // add the new dot to the list
        _playerDots.Add(newPlayerDot);


        _activeDotIndex = _playerDots.IndexOf(newPlayerDot);

        // remove the old dots from the list
        if (_activeDotIndex - 2 >= 0)
        {
            PlayerDot removeDot = _playerDots[_activeDotIndex - 2];
            removeDot.remove();
            _playerDots.RemoveAt(_activeDotIndex - 2);
        }

        // update the list
        _activeDotIndex = _playerDots.IndexOf(newPlayerDot);
    }

*/



/*
void IncreaseRadius()
{
   bool holdKey;
   float originalSize = 5;

   if (Input.GetKey(KeyCode.Space))
   {
       holdKey = true;
   }

   else
   {
       holdKey = false;
   }

   if (holdKey)
   {
       maxRadius += radiusIncreaseSpeed * Time.deltaTime;
       if (maxRadius >= maxIncreasedRadius)
       {
           maxRadius = maxIncreasedRadius;
       }
   }

   else if (!holdKey)
   {
       maxRadius -= radiusIncreaseSpeed * Time.deltaTime;
       if (maxRadius <= originalSize)
       {
           maxRadius = originalSize;
       }
   }
}

void TempoControls()
{
   if (Input.GetMouseButtonDown(0))
   {
       tempoUp = true;
       Debug.Log("Left M.button pressed");
   }

   else if (Input.GetMouseButtonDown(1))
   {
       tempoDown = true;
       Debug.Log("Right M.button pressed");
   }
}
*/


/*
/// Updates the radius size, opacity and Position
void UpdateRadius()
{
Vector2 activePosition;

// check which dot is "alone", and if one is alone, get its position to display the radius circle
if (dot0 != null && dot1 == null)
{
    activePosition = dot0.transform.position;
    radiusCenter = dot0.transform.position;
}
else if (dot1 != null && dot0 == null)
{
    activePosition = dot1.transform.position;
    radiusCenter = dot1.transform.position;
}
else
    activePosition = radiusCenter;


// set opacity value of the line renderer - opacity gets modiefied in radiusFade Corouine
Color startC = radiusLineRenderer.startColor;
startC.a = radiusOpacity;
Color endC = radiusLineRenderer.endColor;
endC.a = radiusOpacity;
radiusLineRenderer.SetColors(startC, endC);

// draw the radius with the line renderer
float theta = 0f;
int size = (int)((1f / radiusDrawScale) + 1f);
radiusLineRenderer.SetVertexCount(size);
radiusLineRenderer.numCornerVertices = 500;
for (int i = 0; i < size; i++)
{
    theta += (2.0f * Mathf.PI * radiusDrawScale);
    float x = activePosition.x + radius * Mathf.Cos(theta);
    float y = activePosition.y + radius * Mathf.Sin(theta);
    radiusLineRenderer.SetPosition(i, new Vector3(x, y, 0));
}
}
*/
