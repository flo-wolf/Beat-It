using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInterpreter : MonoBehaviour {

    public static char[][] keybaordMap = new char[4][];
    public static Vector2 playerPos = new Vector2();



	void Start ()
    {
        FillKeybaordMap();
    }


    void FillKeybaordMap()
    {
        keybaordMap[0] = new char[] { '1', '2', '3', '4', '5', '6', '7' };
        keybaordMap[1] = new char[] { 'Q', 'W', 'E', 'R', 'T', 'Z', 'U' };
        keybaordMap[2] = new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J' };
        keybaordMap[3] = new char[] { 'Y', 'X', 'C', 'V', 'B', 'N', 'M' };
    }

    Vector2 KeybaordToWorldPos(char c, KeyCode keyCode)
    {
        Debug.Log(c);

        Vector2 worldPos = new Vector2();
        Vector2 keyboardPos = new Vector2();
        // find indicies on keymap
        for(int i = 0; i < keybaordMap.Length; i++)
        {
            for(int j = 0; j < keybaordMap[i].Length; j++)
            {
                if(keybaordMap[i][j] == c)
                {
                    keyboardPos.x = i;
                    keyboardPos.y = j;
                    goto LoopBreak;
                }
                else if("Alpha" + keybaordMap[i][j].ToString() == keyCode.ToString())
                {
                    keyboardPos.x = i;
                    keyboardPos.y = j;
                    goto LoopBreak;
                }
            }
        }

        LoopBreak:

        // grad paddings
        worldPos = Camera.main.ViewportToWorldPoint(new Vector2((1 + (keyboardPos.y * 2)) / 16, 1- ((1 + (keyboardPos.x * 4)) / 18)));
        Debug.Log(worldPos);

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
                        Debug.Log(code + " was pressed");

                        // spawn player
                        char key = code.ToString().ToCharArray()[0];
                        Player._player.SpawnDot(KeybaordToWorldPos(key, code), key);
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
                Debug.Log(code + " was released");
                //Debug.Log("Removed Key Index: " + pressedInput.IndexOf(code));

                char key = code.ToString().ToCharArray()[0];
                Player._player.RemoveDot(key);

                releasedInput.Remove(code);
            }
        }

        m_activeInputs = releasedInput;
    }
}
