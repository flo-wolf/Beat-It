using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : LevelObject
{
    public static PlayerGoal instance;

    public Player player;

    ParticleSystem goalFeedback;

    public static bool respawnRemoveDot;

    private bool nextLevelLoads = false;

    private float startSize = 0;

    int soundCount = 0;
    //public static bool respawn = false;


    public override void Awake()
    {
        // don't delete this, it overrides the levelobjects awake function which would otherwise set this levelobjects scale to 0.
    }

    private void Start()
    {
        instance = this;

        startSize = transform.localScale.x;
        transform.localScale = Vector3.zero;

        RythmManager.onBPM.AddListener(OnRythmRespawn);
        goalFeedback = GetComponent<ParticleSystem>();

        player = FindObjectOfType<Player>();
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        if (collision.gameObject.CompareTag("Player") && Game.state == Game.State.Playing && Player.allowMove)
        {
            Player.allowMove = false;
            respawnRemoveDot = true;

            if(soundCount < 1)
            {
                AudioManager.instance.Play("ReachedGoal");
                soundCount++;
            }
        }
    }
   
    void OnRythmRespawn(BPMinfo bpm)
    {
        if ((bpm.Equals(RythmManager.playerBPM) || bpm.Equals(RythmManager.playerDashBPM)) && Game.state == Game.State.Playing && KillDot.jumpTouchedKilldot == false)
        {
            if(!respawnRemoveDot)
            {
                if (Player.dot0 != null && Player.dot0.transform.position == gameObject.transform.parent.position)
                {
                    //goalFeedback.Play();
                    

                    if(nextLevelLoads != true)
                    {
                        AudioManager.instance.Play("Goal");
                        Game.SetState(Game.State.NextLevelFade);
                    }
                    nextLevelLoads = true;
                    soundCount = 0;
                }

                else if (Player.dot1 != null && Player.dot1.transform.position == gameObject.transform.parent.position)
                {
                    //goalFeedback.Play();
                    if (nextLevelLoads != true)
                    {
                        AudioManager.instance.Play("Goal");
                        Game.SetState(Game.State.NextLevelFade);
                    }
                    nextLevelLoads = true;
                    soundCount = 0;
                }
            }

            else if(respawnRemoveDot)
            {
                if (Player.dot0 != null && Player.dot0.transform.position == gameObject.transform.parent.position)
                {
                    player.RemoveDot(false);
                    respawnRemoveDot = false;
                }

                else if (Player.dot1 != null && Player.dot1.transform.position == gameObject.transform.parent.position)
                {
                    player.RemoveDot(true);
                    respawnRemoveDot = false;
                }
            }
        }
    }


    public void Fade(float duration, bool fadeIn)
    {
        if (fadeIn)
        {
            StartCoroutine(C_FadeSize(duration, true));
            //anim["Spawn_FadeIn"].speed = 1;
            //anim["Spawn_FadeIn"].time = 0;
            //anim.Play("Spawn_FadeIn");
        }
        else
        {
            StartCoroutine(C_FadeSize(duration, false));
            //anim["Spawn_FadeIn"].speed = -1;
            //anim["Spawn_FadeIn"].time = anim["Spawn_FadeIn"].length;
            //anim.Play("Spawn_FadeIn");
        }
    }

    IEnumerator C_FadeSize(float duration, bool fadeIn)
    {
        float size = 0;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (fadeIn)
            {
                size = Mathfx.BerpExtreme(0, startSize, (elapsedTime / duration));
            }

            else
            {
                size = Mathfx.BerpExtreme(startSize, 0, (elapsedTime / duration));
            }
            transform.localScale = new Vector3(size, size, size);
            yield return null;
        }
        yield return null;
    }
}
