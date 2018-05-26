using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RythmManager : MonoBehaviour {

    public static RythmEvent onRythm = new RythmEvent();
    public static float clock = 0f;
    public float clockDuration = 0.5f;

	
	// Update is called once per frame
	void Update () {
        UpdateClock();
	}

    private void UpdateClock()
    {
        if (clock + Time.deltaTime < clockDuration)
        {
            clock += Time.deltaTime;

            // rythm events go here (offbeat, half clock etc for loop spawning)
        }

        // the clock has reached its end => set back to 0
        else if (clock + Time.deltaTime >= clockDuration)
        {
            clock = clockDuration;
            
            onRythm.Invoke(clock);
            clock = 0f;
        }
    }

    // timeSlider.value = clock.Remap(0, clockDuration, 0, 1);

    // events
    public class RythmEvent : UnityEvent<float> { }
}
