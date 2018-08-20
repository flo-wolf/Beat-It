using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressIndicator : MonoBehaviour {

    [HideInInspector]
    public bool highlighted = false;

    private Vector3 originalScale;
    private SpriteRenderer sr = null;

    // Use this for initialization
    void Start()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        sr = GetComponent<SpriteRenderer>();
    }

    public void FadeIn(float duration, bool highlight)
    {
        StartCoroutine(C_Fade(true, duration, highlight));
    }

    public void FadeOut(float duration, bool highlight)
    {
        StartCoroutine(C_Fade(false, duration, highlight));
    }

    public void Highlight(bool highlight, float duration)
    {
        StartCoroutine(C_Highlight(highlight, duration));
    }

    IEnumerator C_Highlight(bool highlight, float duration)
    {
        //Debug.Log("LevelObject FadeIn: " + fadeIn + " -- name: " + gameObject.name);
        float elapsedTime = 0f;
        Vector3 size = originalScale;
        Color color;
        Color startColor = LevelProgress.instance.defaultColor;
        startColor = sr.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if (highlight)
            {
                if(elapsedTime < duration/2)
                    size = Vector3.Slerp(originalScale, originalScale * 3, (elapsedTime / (duration / 2)));
                else
                    size = Vector3.Slerp(originalScale * 3, originalScale, (elapsedTime / ((duration / 2) + (duration / 2))));

                color = Color.Lerp(startColor, LevelProgress.instance.highlightColor, (elapsedTime / duration));
            }
            else
            {
                color = Color.Lerp(startColor, LevelProgress.instance.defaultColor, (elapsedTime / duration));
            }

            transform.localScale = size;
            sr.color = color;
            yield return null;
        }

        yield return null;
    }


    IEnumerator C_Fade(bool fadeIn, float duration, bool highlight)
    {
        //Debug.Log("LevelObject FadeIn: " + fadeIn + " -- name: " + gameObject.name);
        float elapsedTime = 0f;
        Vector3 size = Vector3.zero;
        Color color;
        Color startColor = LevelProgress.instance.defaultColor;

        if (highlight && sr != null)
        {
            startColor = sr.color;
        }
        else highlight = false;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if (fadeIn)
            {
                size = Vector3.Lerp(originalScale/2, originalScale, (elapsedTime / duration));
                
                if (highlight)
                {
                    if (elapsedTime < duration / 2)
                        size = Vector3.Slerp(originalScale, originalScale * 3, (elapsedTime / (duration / 2)));
                    else
                        size = Vector3.Slerp(originalScale * 3, originalScale, (elapsedTime / ((duration / 2) + (duration / 2))));
                    color = Color.Lerp(startColor, LevelProgress.instance.highlightColor, (elapsedTime / duration));
                }
                else
                {
                    color = Color.Lerp(startColor, LevelProgress.instance.defaultColor, (elapsedTime / duration));
                }
                color.a = Mathf.SmoothStep(0, 1, (elapsedTime / duration));
            }
            else
            {
                size = Vector3.Lerp(originalScale, originalScale / 2, (elapsedTime / duration));
                if (highlight)
                {
                    color = Color.Lerp(startColor, LevelProgress.instance.highlightColor, (elapsedTime / duration));
                }
                else
                {
                    color = Color.Lerp(startColor, LevelProgress.instance.defaultColor, (elapsedTime / duration));
                }
                color.a = Mathf.SmoothStep(1, 0, (elapsedTime / duration));
            }
                

            transform.localScale = size;
            sr.color = color;
            yield return null;
        }

        if (fadeIn)
            transform.localScale = originalScale;
        else
            transform.localScale = originalScale / 2;

        yield return null;
    }
}
