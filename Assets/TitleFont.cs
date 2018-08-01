using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFont : MonoBehaviour {

    private SpriteRenderer sr;
    public float fadeDuration = 2f;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        Game.onGameStateChange.AddListener(OnGameStateChange);
        StartCoroutine(Fade(true));
    }

    public void OnGameStateChange(Game.State state)
    {
        if(state == Game.State.NextLevelFade)
        {
            StartCoroutine(Fade(false));
        }
    }
	
	// Update is called once per frame
	void Update () {
	}


    /// interpolates the dots opacity as well as its size
    IEnumerator Fade(bool fadeIn)
    {
        float startAlpha = sr.color.a;
        float elapsedTime = 0f;

        while (elapsedTime <= fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color c = sr.color;

            if (fadeIn)
            {
                c.a = Mathf.SmoothStep(0, 1, (elapsedTime / fadeDuration));
            }
            else
            {
                c.a = Mathf.SmoothStep(1, 0, (elapsedTime / fadeDuration));
            }
            sr.color = c;

            yield return null;
        }
        yield return null;
    }
}
