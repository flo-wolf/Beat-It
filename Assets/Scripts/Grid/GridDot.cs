using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDot : MonoBehaviour
{

    public Material materialActive;
    public Material materialDeactive;
    private SpriteRenderer sr;

    public int row = 0;
    public int column = 0;

    public bool active = true;

    private float defaultSize;

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
    }

    void Start()
    {
        defaultSize = transform.localScale.x;
        FindLevelObjectChildren();
        AdjustMaterial();
    }

    void FindLevelObjectChildren()
    {
        levelObject = gameObject.FindComponentInChildWithTag<LevelObject>("LevelObject");
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
                StartCoroutine(Fade(true));
                break;
            case Game.State.LevelFadeout:
                StartCoroutine(Fade(false));
                break;
            default:
                break;
        }
    }

    /// fade the radius in or out by interpolating an opacity value that is used while drawing radius/handle
    IEnumerator Fade(bool fadeIn)
    {

        // size
        float size = 0f;

        // color
        Color c = sr.color;

        float elapsedTime = 0f;
        float dur = Game.instance.levelSwitchDuration;
        while (elapsedTime <= dur)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn && size != defaultSize)
            {
                size = Mathf.SmoothStep(0, defaultSize, (elapsedTime / dur));
                transform.localScale = new Vector3(size, size, size);

                c.a = Mathf.SmoothStep(0, 1, (elapsedTime / dur));
                sr.color = c;

            }

            else if (!fadeIn && size != 0)
            {
                size = Mathf.SmoothStep(defaultSize, 0, (elapsedTime / dur));
                transform.localScale = new Vector3(size, size, size);

                c.a = Mathf.SmoothStep(1, 0, (elapsedTime / dur));
                sr.color = c;
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
