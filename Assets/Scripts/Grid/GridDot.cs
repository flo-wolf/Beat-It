using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDot : MonoBehaviour
{

    public Material materialActive;
    public Material materialDeactive;
    private SpriteRenderer sr;
    private Animation anim;

    public int row = 0;
    public int column = 0;

    public bool active = true;

    [HideInInspector]
    public Vector3 defaultSize = Vector3.zero;

    [HideInInspector]
    public bool wasFadedIn = false;

    [HideInInspector]
    public bool isGettingMoved = false;

    [HideInInspector]
    // position to be checked when searching for suitable griddots (compensates for rotator rotation etc)
    private Vector3 m_checkPosition = Vector3.zero;
    public Vector3 checkPosition
    {
        get { if (isGettingMoved) return m_checkPosition; else return transform.position; }
        set { m_checkPosition = value; }
    }


    // the object attached to our dot, oqupying it (player or levelobject)
    private LevelObject m_levelObj;
    public LevelObject levelObject
    {
        get { return m_levelObj; }
        set { m_levelObj = value; if(value != null) value.gridDot = this; } // if levelobject gets set, also reference this gridDot on the levelobject
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Game.onGameStateChange.AddListener(GameStateChanged);
        defaultSize = transform.localScale;
    }

    void Start()
    {
        anim = GetComponent<Animation>();
        FindLevelObjectChildren();
        AdjustMaterial();
        
        transform.localScale = Vector3.zero;

        LevelTransition.onLevelTransition.AddListener(OnLevelTransition);
    }

    void OnLevelTransition(LevelTransition.Action a)
    {
        switch (a)
        {
            case LevelTransition.Action.FadeOutGridDots:
                Fade(false);
                break;
        }
    }

    void FindLevelObjectChildren()
    {
        levelObject = gameObject.GetComponentInChildren<LevelObject>();
        //levelObject = gameObject.FindComponentInChildWithTag<LevelObject>("LevelObject");
        if(levelObject != null)
            levelObject.gridDot = this;
    }

    // Adjust the 
    void OnValidate()
    {
        AdjustMaterial();
    }

    // Adjust the material depending on the state of the griddot (active => opaque / non active => half transparent)
    void AdjustMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && materialActive != null && materialDeactive != null)
        {
            if (active)
                sr.material = materialActive;
            else
                sr.material = materialDeactive;
        }
    }

    private void GameStateChanged(Game.State state)
    {
        switch (state)
        {
            case Game.State.LevelFadein:
                if (anim)
                    anim.Stop();
                //StartCoroutine(Fade(true));
                break;
            case Game.State.RestartFade:
                if (anim)
                    anim.Stop();
                    Fade(false);
                break;

            case Game.State.NextLevelFade:
                if (anim)
                    anim.Stop();
                //StartCoroutine(Fade(false));
                break;

            case Game.State.Death:
                if (anim)
                    anim.Stop();
                //StartCoroutine(Fade(false));
                break;
            case Game.State.Playing:
                if (anim)
                    anim.Play();
                break;
            default:
                break;
        }
    }

    public void Fade(bool fadeIn)
    {
        if(fadeIn)
            StartCoroutine(C_Fade(true));
        else
            StartCoroutine(C_Fade(false));
    }

    public void LevelTransitionFade(bool fadeIn)
    {
       if(!wasFadedIn)
            Fade(fadeIn);
     //   }
    }

    /// fade the radius in or out by interpolating an opacity value that is used while drawing radius/handle
    IEnumerator C_Fade(bool fadeIn)
    {
        //Debug.Log("Fade Dots: " + fadeIn);
        // size
        
        // color
        Color c = sr.color;
        float startSize = transform.localScale.x;
        float size = startSize;


        float elapsedTime = 0f;
        float dur = RythmManager.playerBPM.ToSecs()/2;
        while (elapsedTime <= dur)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                size = Mathf.SmoothStep(0, defaultSize.x, (elapsedTime / dur));
                transform.localScale = new Vector3(size, size, size);

                //c.a = Mathf.SmoothStep(0, 1, (elapsedTime / dur));
                //sr.color = c;

            }

            else
            {
                size = Mathf.SmoothStep(startSize, 0, (elapsedTime / dur));
                transform.localScale = new Vector3(size, size, size);

                //c.a = Mathf.SmoothStep(1, 0, (elapsedTime / dur));
                //sr.color = c;
            }
            yield return null;
        }
        yield return null;
    }

    public bool PlayerCanMoveOnto()
    {
        if (levelObject != null && !levelObject.playerCanMoveOnto)
            return false;
        return true;
    }
}
