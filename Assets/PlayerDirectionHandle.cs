using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionHandle : MonoBehaviour {

    public static PlayerDirectionHandle instance = null;

    public SpriteRenderer sr;

    [Header("Radius Drawing")]
    public float radiusDrawScale = 0.01f;   // the amount of verticies used to draw the radius. Lower number = more
    public float radiusFadeDuration = 0.25f;
    public float radiusFadeOutDuration = 0.1f;
    public float maxRadius = 5;             // default max radius

    [HideInInspector]
    public float radius = 0;              // current radius, gets interpolated to and from maxRadius via fading coroutine
    [HideInInspector]
    public float radiusOpacity = 0f;       // current opacity, gets interpolated to and from 1 via fading coroutine
    private Vector2 handleCenter;

    private bool fading = false;

    private bool fadedIn = false;

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;

        LevelTransition.onLevelTransition.AddListener(OnLevelTransition);
	}

    void OnLevelTransition(LevelTransition.Action action)
    {
        switch (action)
        {
            case LevelTransition.Action.FadeOutRadiusHandle:
                FadeRadius(false);
                break;
        }
    }

    public void UpdateLookDirection()
    {
        Player p = Player.instance;

        // get the world mouse position 
        Vector2 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // calculate the destination position
        Vector2 activePosition = Vector2.zero;

        // check which dot is "alone", and if one is alone, get its position to display the radius handle
        if (Player.dot0 != null && Player.dot1 == null)
        {
            activePosition = Player.dot0.transform.position;
            handleCenter = activePosition;
        }
        else if (Player.dot1 != null && Player.dot0 == null)
        {
            activePosition = Player.dot1.transform.position;
            handleCenter = activePosition;
        }
        else if (Player.dot0 != null && Player.dot1 != null)
        {
            if (!fading)
            {
                // both dots are alive, don't draw a radius
                if (Player.instance.newestDot == Player.DotType.Dot0)
                    activePosition = Player.dot0.transform.position;
                else
                    activePosition = Player.dot1.transform.position;
            }
            else
                activePosition = handleCenter;
        }
        else if(Player.dot0 == null && Player.dot1 == null)
        {
            activePosition = PlayerSpawn.instance.transform.position;
            handleCenter = activePosition;
        }

        p.lookDirection = mousePos - activePosition;
        p.lookDirection = p.lookDirection.normalized;

        // in case we use a controller
        Vector2 controllerInput = new Vector2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y"));
        if (InputDeviceDetector.inputType == InputDeviceDetector.InputType.Controler)
        {
            p.lookDirection = controllerInput;
            p.lookDirection = p.lookDirection.normalized;
        }

        UpdateHandle(activePosition);
    }

    /// Updates the Handle position and opacity
    void UpdateHandle(Vector2 activePos)
    {
        // set opacity value of the radius handle
        Color c = sr.color;
        c.a = radiusOpacity;
        sr.color = c;

        sr.gameObject.transform.position = activePos + (Player.instance.lookDirection + Player.instance.lookDirection / 2);
    }


    public void FadeRadius(bool fadeIn, Action onStart = null, Action onComplete = null)
    {
        if((fadeIn && !fadedIn) || (!fadeIn && fadedIn))
        {
            StopCoroutine("FadeRadiusCoroutine");
            StartCoroutine(C_FadeRadius(fadeIn, onStart, onComplete));
        }
    }


    // fade in the radius delayed when there are two dots present at once
    public void DelayedDoubleDotFadeIn()
    {
        Debug.Log("DelayedDoubleDotFadeIn");
        float playerMoveTime = RythmManager.playerBPM.ToSecs() / 3;
        if (Player.isDashing)
            playerMoveTime = RythmManager.playerDashBPM.ToSecs() / 3;

        if (!fadedIn)
        {
            StopCoroutine("C_FadeRadius");
            StartCoroutine(C_FadeRadius(true, null, null, playerMoveTime));
        }
            
    }


    /// fade the radius in or out by interpolating an opacity value that is used while drawing radius/handle
    IEnumerator C_FadeRadius(bool fadeIn, Action onStart = null, Action onComplete = null, float fadeDelay = 0f)
    {
        fading = true;
        //Debug.Log("Radius FadeIn " + fadeIn);

        if (fadeDelay != 0f)
            yield return new WaitForSeconds(fadeDelay);

        if (onStart != null)
        {
            onStart();
        }

        float startRadius = radius;
        float duration = 0f;
        if (fadeIn)
            duration = radiusFadeDuration;
        else
            duration = radiusFadeOutDuration;

        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn && radius != maxRadius)
            {
                radius = Mathf.SmoothStep(startRadius, maxRadius, (elapsedTime / duration));
                radiusOpacity = Mathf.SmoothStep(startRadius, 1, (elapsedTime / duration));
            }

            else if (!fadeIn && radius != 0)
            {
                radius = Mathf.SmoothStep(startRadius, 0, (elapsedTime / duration));
                radiusOpacity = Mathf.SmoothStep(startRadius, 0, (elapsedTime / duration));
            }
            yield return null;
        }

        fadedIn = fadeIn;

        if (onComplete != null)
        {
            onComplete();
        }

        

        fading = false;
        yield return null;
    }
}
