using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour {

    public static LevelTransition instance = null;
    public float dotFadeDuration = 2f;
    public bool smoothLerp = true;

	// Use this for initialization
	void Start ()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(C_LevelIntro());
	}
	
	public void FadeOutLevel()
    {
        Grid.FadeGridDotsGradually(Grid.FindPlayerGoal().transform.position, dotFadeDuration, 80f, false, Game.LoadNextLevel);
    }

    IEnumerator C_LevelIntro()
    {
        yield return null;
    }
}
