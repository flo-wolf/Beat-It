using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDot : LevelObject{

    [Header("Settings")]
    public float fadeDuration = 0.25f;

    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public SpriteRenderer emptyFillSr;
    public GameObject fillCircleGo;

    private Material defaultMaterial;
    public Material killMaterial;

    private float fillAmount = 0; // 0 = no fill, empty black;  1 = full fill, white

    public enum State { Filling, Full, Emptying, Empty }
    public State state = State.Filling;

    private float defaultLocalScale;            // default player dot size
    private float defaultFillCircleSize;        // default fill indicator size (inside player dot)
    private float scale = 0f;
    private float opacity = 0f;

    public override void Awake()
    {
        // don't delete this, it overrides the levelobjects awake function which would otherwise set this levelobjects scale to 0.
    }

    void Start()
    {
        defaultMaterial = sr.material;
        defaultLocalScale = transform.localScale.x;
        defaultFillCircleSize = fillCircleGo.transform.localScale.x;
        StartCoroutine(Fade(true));

        Game.onGameStateChange.AddListener(GameStateChanged);
    }

    private void GameStateChanged(Game.State state)
    {
        switch (state)
        {
            case Game.State.DeathOnNextBeat:
                sr.material = killMaterial;
                break;

            case Game.State.Death:
                sr.material = killMaterial;
                break;

            default:
                
                break;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("LevelObject") )
        {
            Color otherColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            killMaterial.color = otherColor;

            PlayerSegment.instance.killMaterial.color = otherColor;
        }
    }

    /// interpolates the dots opacity as well as its size
    IEnumerator Fade(bool fadeIn)
    {
        Debug.Log("PlayerDot fadeIn: " + fadeIn + " -- name: " + gameObject.name);

        if (!Player.isDashing)
            fadeDuration = RythmManager.playerBPM.ToSecs() / 2;
        else
            fadeDuration = RythmManager.playerDashBPM.ToSecs() / 2;

        if (fadeIn)
            state = State.Filling;
        else
            state = State.Emptying;

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
            state = State.Empty;
            gridDot.levelObject = null;
            GameObject.Destroy(gameObject);
        }
        else
            state = State.Full;
        yield return null;
    }
}
