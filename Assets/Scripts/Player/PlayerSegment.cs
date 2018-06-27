using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSegment : Segment {

    public static PlayerSegment instance;

    public Material movingKillDotMaterial;

    private Color defaultColor;
    private LineRenderer lr;
    private Color killColor;

    public static bool touchedKillDot = false;
    

    private void Start()
    {
        instance = this;
        Game.onGameStateChange.AddListener(GameStateChanged);

        lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            defaultColor = lr.startColor;
            killColor = movingKillDotMaterial.color;

            lr.startColor = defaultColor;
            lr.endColor = defaultColor;
        }
    }

    private void GameStateChanged(Game.State state)
    {
        if(state == Game.State.Playing)
        {
            touchedKillDot = false;
            

            lr.endColor = defaultColor;
            lr.startColor = defaultColor;
        }
        else if(state == Game.State.Death)
        {

        }
    }

    public void AdaptKillColor()
    {
        Debug.Log("kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
        touchedKillDot = true;
        lr.endColor = killColor;
        lr.startColor = killColor;
        touchedKillDot = false;
    }
}
