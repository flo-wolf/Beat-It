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

public class Player : MonoBehaviour {

    // structure
    public static Player _player;           // self reference
    public static PlayerDot oldDot = null;  // mouse1 dot
    public static PlayerDot newDot = null; // mouse2 dot

    // keep track of which dot is the newest
    public enum DotType { None, OldDot, NewDot }

    [Header("Radius Drawing")]
    public float radiusDrawScale = 0.01f;   // the amount of verticies used to draw the radius. Lower number = more
    public float radiusFadeDuration = 1f;
    public float maxRadius = 10;            // default max radius
    private float radius = 0;              // current radius, gets interpolated to and from maxRadius via fading coroutine
    private float radiusOpacity = 0f;       // current opacity, gets interpolated to and from 1 via fading coroutine
    private Vector2 radiusCenter;

    [Header("Components")]
    public GameObject playerDotPrefab;      // needed for creating new playerDots
    public PlayerSegment playerSegment;     // the line drawn between two playerDots, collision detectable
    public LineRenderer radiusLineRenderer;
    public SpriteRenderer lookHandleSr;
    public SpriteRenderer lookHandleInnerSr;

    // aiming
    private Vector2 lookDirection = new Vector2(); //

    /// initialization
    void Start()
    {
        _player = this;
        //GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, transform.position, Quaternion.identity);
        //_playerDots.Add(newDotGo.GetComponent<PlayerDot>());
        //StartCoroutine(RadiusFade(true));
    }

    /// Input Handling and Radius Drawing
    void Update()
    {
    }

    void FadeRadius(bool fadeIn, Action onStart = null, Action onComplete = null)
    {
        StopCoroutine("FadeRadiusCoroutine");
        StartCoroutine(FadeRadiusCoroutine(fadeIn, onStart, onComplete));
    }

    /// fade the radius in or out by interpolating an opacity value that is used while drawing radius/handle
    IEnumerator FadeRadiusCoroutine(bool fadeIn, Action onStart = null, Action onComplete = null)
    {
        Debug.Log("Radius FadeIn " + fadeIn);

        if (onStart != null)
        {
            onStart();
        }

        float startRadius = radius;

        float elapsedTime = 0f;
        while (elapsedTime <= radiusFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn && radius != maxRadius)
            {
                radius = Mathf.SmoothStep(startRadius, maxRadius, (elapsedTime / radiusFadeDuration));
                radiusOpacity = Mathf.SmoothStep(startRadius, 1, (elapsedTime / radiusFadeDuration));
            }
            else if(!fadeIn && radius != 0)
            {
                radius = Mathf.SmoothStep(startRadius, 0, (elapsedTime / radiusFadeDuration));
                radiusOpacity = Mathf.SmoothStep(startRadius, 0, (elapsedTime / radiusFadeDuration));
            }
            yield return null;
        }

        if (onComplete != null)
        {
            onComplete();
        }

        yield return null;
    }

    /// Creates a new PlayerDot Object at the lookDestination position and draws the connecting segment in between
    public void SpawnDot(Vector2 spawnPos, char pressedChar)
    {
        // only spawn dots of one of the dot slots is free => dont spawn more than the two conencted to the input triggers
        if(oldDot != null && newDot != null)
            return;


        // spawn the new dot
        GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, spawnPos, Quaternion.identity);
        PlayerDot newPlayerDot = newDotGo.GetComponent<PlayerDot>();
        newPlayerDot.associatedKey = pressedChar;

        // there are now two dots remove the radius
        if (newDot != null && oldDot == null)
        {
            FadeRadius(false);
            oldDot = newDot;
            newDot = newPlayerDot;
        }
        // there are no dots, spawn the first dot and show the radius
        else if(oldDot == null && newDot == null)
        {
            FadeRadius(true);
            oldDot = null;
            newDot = newPlayerDot;
        }

        

        // fill the segment in between two dots
        if (oldDot != null && newDot != null)
        {
            playerSegment.FillSegment(oldDot.transform.position, newDot.transform.position);
        }
    }

    /// Retracts the segment towards the new player dot and removes the old one
    public void RemoveDot(char removeKey)
    {

        
        //StartCoroutine(RadiusFade(true));

        bool switchSegmentDirection = false;

        // which dot gets removed? Find the corresponding PlayerDot based on the key that was released
        DotType removeDot = DotType.None;
        if (oldDot != null && oldDot.associatedKey == removeKey)
            removeDot = DotType.OldDot;
        else if (newDot != null && newDot.associatedKey == removeKey)
            removeDot = DotType.NewDot;

        //Debug.Log("oldDot.associatedKey: " + oldDot.associatedKey);
        //Debug.Log("newDot.associatedKey: " + newDot.associatedKey);
        Debug.Log("removeDot: " + removeDot);

        // oldDot gets removed
        if (removeDot == DotType.OldDot && oldDot != null)
        {
            oldDot.Remove();
            oldDot = null;

            if(newDot != null)
            {
                switchSegmentDirection = true;
                FadeRadius(true);
            }
            else
                FadeRadius(false);
        }
        // newDot gets removed
        else if (removeDot == DotType.NewDot && newDot != null)
        {
            newDot.Remove();
            newDot = null;

            if (oldDot != null)
            {
                switchSegmentDirection = true;
                FadeRadius(true);
            }
            else
                FadeRadius(false);
        }

        // both dots are gone => Death/Restart
        if (oldDot == null && newDot == null)
        {
            Game.SetState(Game.State.Death);
            radius = 0;
            radiusOpacity = 0;
        }

        // empty the segment in between those two dots
        playerSegment.EmptySegment(switchSegmentDirection);
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

/// Updates the Handle position and opacity
void UpdateRadiusHandle()
{
    // set opacity value of the radius handle
    Color c = lookHandleSr.color;
    c.a = radiusOpacity;
    lookHandleSr.color = c;

    // get the world mouse position 
    Vector2 mousePos = Input.mousePosition;
    mousePos = Camera.main.ScreenToWorldPoint(mousePos);

    // calculate the destination position
    Vector2 activePosition;

    // check which dot is "alone", and if one is alone, get its position to display the radius handle
    if (oldDot != null && newDot == null)
        activePosition = dot0.transform.position;
    else if (dot1 != null && dot0 == null)
        activePosition = dot1.transform.position;
    else // both dots are alive, don't draw a radius
        return;

    lookDirection = mousePos - activePosition;
    lookDirection = lookDirection.normalized;
    lookDirection = lookDirection * maxRadius;

    lookHandleSr.gameObject.transform.position = activePosition + lookDirection;
}
*/
