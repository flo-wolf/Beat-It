using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDot : LevelObject{

    [Header("Settings")]
    public float fadeDuration = 0.5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public SpriteRenderer emptyFillSr;
    public GameObject fillCircleGo;

    private float fillAmount = 0; // 0 = no fill, empty black;  1 = full fill, white

    private enum State { Spawn, Idle, Extend, Despawn }
    private State state = State.Spawn;

    private float defaultLocalScale;            // default player dot size
    private float defaultFillCircleSize;        // default fill indicator size (inside player dot)
    private float scale = 0f;
    private float opacity = 0f;

    void Start()
    {
        defaultLocalScale = transform.localScale.x;
        defaultFillCircleSize = fillCircleGo.transform.localScale.x;
        StartCoroutine(Fade(true));
    }

    public void Remove()
    {
        StopCoroutine("Fade");
        StartCoroutine(Fade(false));
    }

    void Update()
    {
        // update fill amount
        float fillScale = Mathf.Lerp(0, defaultFillCircleSize, PlayerSegment.instance.fillProgress);
        fillCircleGo.transform.localScale = new Vector3(fillScale, fillScale, 1);
    }

    /// interpolates the dots opacity as well as its size
    IEnumerator Fade(bool fadeIn)
    {
        float elapsedTime = 0f;
        float startScale = scale;
        float startOpacity = opacity;

        while (elapsedTime <= fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color c = sr.color;
            if (fadeIn)
            {
                scale = Mathf.SmoothStep(startScale, defaultLocalScale, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale,scale,1);

                opacity = Mathf.SmoothStep(startOpacity, 1, (elapsedTime / fadeDuration));
            }
            else
            {
                scale = Mathf.SmoothStep(startScale, 0, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale,1);
                opacity = Mathf.SmoothStep(startOpacity, 0, (elapsedTime / fadeDuration));
            }

            c.a = opacity;
            sr.color = c;

            yield return null;
        }
        if (!fadeIn)
        {
            gridDot.levelObject = null;
            GameObject.Destroy(gameObject);
        }
        yield return null;
    }
}
