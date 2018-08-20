using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour {

    public static LevelProgress instance = null;
    public static int highlight = 0;
    public LevelProgressIndicator[] levelIndicators;

    public float fadeInDurationOffset = 0.1f;
    public float fadeInDuration = 0.5f;
    public Color defaultColor;
    public Color highlightColor;

    public static bool fadedIn = false;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);

        highlight = Game.level - 1;
    }

    // fades in all levelprogress indicators, but only highlights the one at the given index (levelnumber - 1)
    public void FadeInAll(float duration)
    {
        fadedIn = true;
        StartCoroutine(C_FadeAll(true, duration));
    }

    public void FadeOutAll(float duration)
    {
        StartCoroutine(C_FadeAll(false, duration));
    }

    public void Hightlight(float duration)
    {
        highlight = Game.level - 1;

        if (highlight + 1 < levelIndicators.Length)
            highlight++;

        if(highlight >  0)
            levelIndicators[highlight-1].Highlight(false, duration);

        levelIndicators[highlight].Highlight(true, duration);
    }

    IEnumerator C_FadeAll(bool fadeIn, float duration)
    {
        float singleDuration = duration;

        Debug.Log("FadeAll: " + fadeIn);
        for (int i = 0; i < levelIndicators.Length; i++)
        {
            if (fadeIn)
            {
                if (highlight != i)
                    levelIndicators[i].FadeIn(singleDuration, false);
                else
                    levelIndicators[i].FadeIn(singleDuration, true);
            }
            else
            {
                if (highlight != i)
                    levelIndicators[i].FadeOut(singleDuration, false);
                else
                    levelIndicators[i].FadeOut(singleDuration, true);
            }

            //yield return new WaitForSeconds(singleDuration);
        }
        yield return null;
    }
}
