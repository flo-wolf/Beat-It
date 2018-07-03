using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingKillDot : LevelObject {

    [Header("Settings")]
    public float fadeDuration = 0.5f;

    [Header("Components")]
    public SpriteRenderer sr;

    private float defaultLocalScale;            // default player dot size
    private float scale = 0f;
    private float opacity = 0f;

    ParticleSystem killFeedback;

    private Animation deathAnim;

    private void Start()
    {
        defaultLocalScale = transform.localScale.x;
        StartCoroutine(Fade(true));

        killFeedback = GetComponent<ParticleSystem>();

        deathAnim = GetComponentInChildren<Animation>();
    }

    public void Remove()
    {
        StopCoroutine("Fade");
        StartCoroutine(Fade(false));
    }

    public void PlayParticleSystem()
    {
        killFeedback.Play();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (deathAnim != null)
                deathAnim.Play("MovingKillDotDeathFast");
        }
    }

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
                transform.localScale = new Vector3(scale, scale, 1);

                opacity = Mathf.SmoothStep(startOpacity, 1, (elapsedTime / fadeDuration));
            }
            else
            {
                scale = Mathf.SmoothStep(startScale, 0, (elapsedTime / fadeDuration));
                transform.localScale = new Vector3(scale, scale, 1);
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
