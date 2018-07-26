using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMask : MonoBehaviour
{
    public static ScreenMask instance;

    void Awake()
    {
        //We dont want that there can be multiple AudioManagers, whenever a new scene starts, so we use a Singleton pattern,
        //to check if there is already an instance of our AudioManager, and if yes, we just destroy it.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
            Destroy(this.gameObject);
    }
}
