using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class InputDeviceDetector : MonoBehaviour
{
    public enum InputType
    {
        MouseKeyboard,
        Controler
    };
    public static InputType inputType = InputType.MouseKeyboard;

    //*************************//
    // Unity member methods    //
    //*************************//

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

    //***************************//
    // Public member methods     //
    //***************************//

    public InputType GetInputState()
    {
        return inputType;
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

        return false;
    }
}
