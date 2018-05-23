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
    public static PlayerDot dot0 = null;  // mouse1 dot
    public static PlayerDot dot1 = null; // mouse2 dot

    // keep track of which dot is the newest
    public enum DotType { None, Dot0, Dot1 }
    public DotType newestDot = DotType.None;

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
    private bool lookingRight = false;

    /// initialization
    void Start()
    {
        _player = this;
        //GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, transform.position, Quaternion.identity);
        //_playerDots.Add(newDotGo.GetComponent<PlayerDot>());
        //StartCoroutine(RadiusFade(true));

        Game.onTimeStepChange.AddListener(TimeStepMove);
    }

    /// Input Handling and Radius Drawing
    void Update()
    {
        /*
        // Key Down
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnDot(true);                                   
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SpawnDot(false);
        }

        // Key Up
        if (Input.GetKeyUp(KeyCode.A))
        {
            RemoveDot(true);
        }
        // Key Up
        else if (Input.GetKeyUp(KeyCode.D))
        {
            RemoveDot(false);
        }
        */

        UpdateRadius();
        UpdateRadiusHandle();
    }

    // the timestep has reached its end, move the player
    void TimeStepMove(float timestep)
    {
        

        // if there are two dots
        if (dot1 != null && dot0 != null)
        {
            
            // get the world mouse position 
            Vector2 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            // check which point is closest to the lookdirection
            float dot0LookLength = (mousePos - (Vector2)dot0.transform.position).magnitude;
            float dot1LookLength = (mousePos - (Vector2)dot1.transform.position).magnitude;

            Debug.Log("dot0LookLength: " + dot0LookLength + "   dot1LookLength: " + dot1LookLength);

            // dot 0 is closer to the direction we are aiming at => remove dot1
            if (dot0LookLength <= dot1LookLength)
            {
                RemoveDot(false);
            }
            // dot 1 is closer to the direction we are aiming at => remove dot0
            else
            {
                RemoveDot(true);
            }
        }

        // if there is only one dot
        else
        {
            SpawnDot();
        }
            // spawn the new dot at the aimed at position
    }

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
        Vector2 activePosition = Vector2.zero;
        


        // check which dot is "alone", and if one is alone, get its position to display the radius handle
        if (dot0 != null && dot1 == null)
            activePosition = dot0.transform.position;
        else if (dot1 != null && dot0 == null)
            activePosition = dot1.transform.position;
        else if(dot0 != null && dot1 != null){
            // both dots are alive, don't draw a radius
            activePosition = Vector2.Lerp(dot0.transform.position, dot1.transform.position, 1 / 2);

            /*
            Vector2 dotDistance = dot1.transform.position - dot0.transform.position;
            Vector2 dotDistancePerp = -dotDistance;
            Vector2 dotMiddle = dotDistance / 2;
            Vector2 diagonalStart = new Vector2(dotMiddle.x, );
            Vector2 diagonalEnd = Vector2.zero;
            */
        }

        lookDirection = mousePos - activePosition;
        lookDirection = lookDirection.normalized;
        lookDirection = lookDirection * maxRadius;

        lookHandleSr.gameObject.transform.position = activePosition + lookDirection;
    }

    void FadeRadius(bool fadeIn, Action onStart = null, Action onComplete = null)
    {
        StopCoroutine("FadeRadiusCoroutine");
        StartCoroutine(FadeRadiusCoroutine(fadeIn, onStart, onComplete));
    }

    /// fade the radius in or out by interpolating an opacity value that is used while drawing radius/handle
    IEnumerator FadeRadiusCoroutine(bool fadeIn, Action onStart = null, Action onComplete = null)
    {
        //Debug.Log("Radius FadeIn " + fadeIn);

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
    void SpawnDot()
    {
        // only spawn dots of one of the dot slots is free => dont spawn more than the two conencted to the input triggers
        if(dot0 != null && dot1 != null)
            return;

        //Debug.Log("Dot0: " + dot0 + " ---- Dot1: " + dot1);
        int dotWasSpawned = -1; // -1 = no dot spawned, 0 = dot0 spawndd, 1 = dot1 spawned

        Vector2 activePosition = new Vector2();
        Vector2 spawnPosition;

        // dot1 exists, dot0 doesnt => spawn dot0
        if (dot1 != null && dot0 == null)
        {
            activePosition = dot1.transform.position;
            spawnPosition = activePosition + lookDirection;

            dotWasSpawned = 0;

            FadeRadius(false);
        }
        // dot0 exists, dot1 doesnt => spawn dot1
        else if (dot0 != null && dot1 == null)
        {
            activePosition = dot0.transform.position;
            spawnPosition = activePosition + lookDirection;

            dotWasSpawned = 1;

            FadeRadius(false);
        }
        // there are no dots, spawn the first dot
        else
        {
            // get the world mouse position and spawn the dot there
            Vector2 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            spawnPosition = mousePos;

            dotWasSpawned = 0;

            FadeRadius(true);
        }

        // spawn the new dot
        GameObject newDotGo = GameObject.Instantiate(playerDotPrefab, spawnPosition, Quaternion.identity);
        PlayerDot newPlayerDot = newDotGo.GetComponent<PlayerDot>();

        // set the newest dot information
        if (dotWasSpawned == 0)
        {
            newDotGo.name = "dot0";
            dot0 = newPlayerDot;
            newestDot = DotType.Dot0;
        }
        else if(dotWasSpawned == 1)
        {
            newDotGo.name = "dot1";
            dot1 = newPlayerDot;
            newestDot = DotType.Dot1;
        }
            

        //Debug.Log("activePosition: " + activePosition + " spawnPosition: " + spawnPosition);

        // fill the segment in between two dots
        if (dot0 != null && dot1 != null)
        {
            if(newestDot == DotType.Dot0)
                playerSegment.FillSegment(dot1.transform.position, dot0.transform.position);
            else if(newestDot == DotType.Dot1)
                playerSegment.FillSegment(dot0.transform.position, dot1.transform.position);
        }
    }

    /// Retracts the segment towards the new player dot and removes the old one
    void RemoveDot(bool removeDot0)
    {
        //StartCoroutine(RadiusFade(true));

        bool switchSegmentDirection = false;

        if (removeDot0 && dot0 != null)
        {
            dot0.Remove();
            dot0 = null;

            if(dot1 != null)
            {
                if(newestDot == DotType.Dot1)
                    switchSegmentDirection = true;
                newestDot = DotType.Dot1;
                FadeRadius(true);
            }
            else
                FadeRadius(false);
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
                FadeRadius(true);
            }
            else
                FadeRadius(false);
        }

        // both dots are gone => Death/Restart
        if (dot1 == null && dot0 == null)
        {
            Game.SetState(Game.State.Death);
            radius = 0;
            radiusOpacity = 0;
            newestDot = DotType.None;
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
