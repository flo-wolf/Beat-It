using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class InputDeviceDetector : MonoBehaviour
{
    public static InputDeviceDetector instance = null;

    public enum InputType
    {
        MouseKeyboard,
        Controler
    };
    public static InputType inputType = InputType.MouseKeyboard;

    //*************************//
    // Unity member methods    //
    //*************************//

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DetectController();
    }


    private int Xbox_One_Controller = 0;
    private int PS4_Controller = 0;
    void DetectController()
    {
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            print(names[x].Length);
            if (names[x].Length == 19)
            {
                print("PS4 CONTROLLER IS CONNECTED");
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            if (names[x].Length == 33)
            {
                print("XBOX ONE CONTROLLER IS CONNECTED");
                //set a controller bool to true
                PS4_Controller = 0;
                Xbox_One_Controller = 1;
            }
        }
        if (Xbox_One_Controller == 1)
        {
            //do something
        }
        else if (PS4_Controller == 1)
        {
            //do something
        }
        else
        {
            inputType = InputType.MouseKeyboard;
            return;
        }
        inputType = InputType.Controler;
    }

    /*
    void OnGUI()
    {
        switch (inputType)
        {
            case InputType.MouseKeyboard:
                if (isControlerInput())
                {
                    inputType = InputType.Controler;
                    Debug.Log("DREAM - JoyStick being used");
                }
                break;
            case InputType.Controler:
                if (isMouseKeyboard())
                {
                    inputType = InputType.MouseKeyboard;
                    Debug.Log("DREAM - Mouse & Keyboard being used");
                }
                break;
        }
    }
    */

    //***************************//
    // Public member methods     //
    //***************************//

    public InputType GetInputState()
    {
        return inputType;
    }

    // used to check for input before the player moves (see game class)
    public bool RecievingStartInput()
    {
        if(inputType == InputType.Controler)
        {
            return (Input.GetAxis("Joystick X") != 0.0f ||
           Input.GetAxis("Joystick Y") != 0.0f);
        }
        else
        {
            return Input.GetKey(KeyCode.Mouse0);
        }
    }

    //****************************//
    // Private member methods     //
    //****************************//

    private bool isMouseKeyboard()
    {
        // mouse & keyboard buttons
        if (Event.current.isKey ||
            Event.current.isMouse)
        {
            return true;
        }
        // mouse movement
        if (Input.GetAxis("Mouse X") != 0.0f ||
            Input.GetAxis("Mouse Y") != 0.0f)
        {
            return true;
        }
        return false;
    }

    private bool isControlerInput()
    {
        // joystick buttons
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19))
        {
            return true;
        }

        // joystick axis
        if (Input.GetAxis("Joystick X") != 0.0f ||
           Input.GetAxis("Joystick Y") != 0.0f)
        {
            return true;
        }

        // set this to return false in order to allow mouse input!!!
        return false;
    }
}
