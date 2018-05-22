using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInterpreter : MonoBehaviour {

    public static KeyCode[][] keybaordMap = new KeyCode[4][];
    public static Vector2 playerPos = new Vector2();


	void Start ()
    {
        FillKeybaordMap();
    }


    void FillKeybaordMap()
    {
        keybaordMap[0] = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7 };
        keybaordMap[1] = new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.U };
        keybaordMap[2] = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J };
        keybaordMap[3] = new KeyCode[] { KeyCode.Y, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M };
    }

    Vector2 KeybaordToWorldPos(KeyCode keyCode)
    {
        Debug.Log(keyCode);

        Vector2 worldPos = new Vector2();
        Vector2 keyboardPos = new Vector2();

        // find indicies on keymap
        for(int i = 0; i < keybaordMap.Length; i++)
        {
            for(int j = 0; j < keybaordMap[i].Length; j++)
            {
                if(keybaordMap[i][j] == keyCode)
                {
                    keyboardPos.x = i;
                    keyboardPos.y = j;
                    goto LoopBreak;
                }
            }
        }
        return Vector2.zero;

        LoopBreak:

        // grad paddings
        worldPos = Camera.main.ViewportToWorldPoint(new Vector2(((1 + (keyboardPos.y * 2)) + keyboardPos.x * 0.5f) / 16, 1- ((3 + (keyboardPos.x * 4)) / 18)));
        //Debug.Log(worldPos);

        return worldPos;
    }


    protected List<KeyCode> m_activeInputs = new List<KeyCode>();

    public void Update()
    {
        List<KeyCode> pressedInput = new List<KeyCode>();

        if (Input.anyKeyDown || Input.anyKey)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    if (!m_activeInputs.Contains(code))
                    {
                        //Debug.Log(code + " was pressed");

                        // spawn player
                        //char key = code.ToString().ToCharArray()[0];
                        Vector2 spawnPos = KeybaordToWorldPos(code);
                        if (spawnPos != Vector2.zero)
                            Player._player.SpawnDot(spawnPos, code);
                    }

                    m_activeInputs.Remove(code);
                    m_activeInputs.Add(code);
                    pressedInput.Add(code);
                }
            }
        }

        List<KeyCode> releasedInput = new List<KeyCode>();

        foreach (KeyCode code in m_activeInputs)
        {
            releasedInput.Add(code);

            if (!pressedInput.Contains(code))
            {
                //Debug.Log(code + " was released");
                //Debug.Log("Removed Key Index: " + pressedInput.IndexOf(code));

                //char key = code.ToString().ToCharArray()[0];
                Player._player.RemoveDot(code);

                releasedInput.Remove(code);
            }
        }

        m_activeInputs = releasedInput;
    }
}
