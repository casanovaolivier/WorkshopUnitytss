using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamepadInput {
    
    static public float GetAxis(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return CheckAxisPS4(name, id);
            case Gamepad.Xbox: return CheckAxisXbox(name, id);
            case Gamepad.JoyCon: return CheckAxisJoyCon(name, id);
            default: return 0f;
        }
    }

    static public float GetAxisRaw(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return CheckAxisRawPS4(name, id);
            case Gamepad.Xbox: return CheckAxisRawXbox(name, id);
            case Gamepad.JoyCon: return CheckAxisRawJoyCon(name, id);
            default: return 0f;
        }
    }

    static public string GetAxisName(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return CheckAxisNamePS4(name, id);
            case Gamepad.Xbox: return CheckAxisNameXbox(name, id);
            case Gamepad.JoyCon: return CheckAxisNameJoyCon(name, id);
            default: return "";
        }
    }

    static public bool GetButtonDown(string name, Gamepad type, int id)
    {
        switch(type)
        {
            case Gamepad.PS4: return CheckButtonDownPS4(name, id);
            case Gamepad.Xbox: return CheckButtonDownXbox(name, id);
            case Gamepad.JoyCon: return CheckButtonDownJoyCon(name, id);
            default: return false;
        }
    }

    static public bool GetButton(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return CheckButtonPS4(name, id);
            case Gamepad.Xbox: return CheckButtonXbox(name, id);
            case Gamepad.JoyCon: return CheckButtonJoyCon(name, id);
            default: return false;
        }
    }

    static public bool GetButtonUp(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return CheckButtonUpPS4(name, id);
            case Gamepad.Xbox: return CheckButtonUpXbox(name, id);
            case Gamepad.JoyCon: return CheckButtonUpJoyCon(name, id);
            default: return false;
        }
    }

    static public string GetButtonForUI(string name, Gamepad type, int id)
    {
        switch (type)
        {
            case Gamepad.PS4: return "Joystick" + id + "Button" + (int)GamepadManager.Instance.inputUiButtonsPS4[name];
            case Gamepad.Xbox: return "Joystick" + id + "Button" + (int)GamepadManager.Instance.inputUiButtonsXbox[name];
            case Gamepad.JoyCon: return "Joystick" + id + "Button" + (int)GamepadManager.Instance.inputUiButtonsJoyCon[name];
            default: return "";
        }
    }

    static private float CheckAxisPS4(string name, int id)
    {
        List<PS4Axis> list = GamepadManager.Instance.inputAxisPS4[name];

        for (int i = 0; i < list.Count; i++)
        {
            float value = Input.GetAxis("Joystick" + id + "Axis" + (int)list[i].PS4_Axis);
            if ((int)list[i].PS4_Axis == 2)
            {
                if (value != 0f) return (value * (list[i].Invert ? 1f : -1f));
            }
            else
            {
                if (value != 0f) return (value * (list[i].Invert ? -1f : 1f));
            }
        }

        return 0f;
    }

    static private float CheckAxisXbox(string name, int id)
    {
        List<XboxAxis> list = GamepadManager.Instance.inputAxisXbox[name];

        for (int i = 0; i < list.Count; i++)
        {
            float value = Input.GetAxis("Joystick" + id + "Axis" + (int)list[i].Xbox_Axis);
            if ((int)list[i].Xbox_Axis == 2)
            {
                if (value != 0f) return (value * (list[i].Invert ? 1f : -1f));
            }           
            else
            {
                if (value != 0f) return (value * (list[i].Invert ? -1f : 1f));
            }  
        }

        return 0f;
    }

    static private float CheckAxisJoyCon(string name, int id)
    {
        JoyConAxis axis = GamepadManager.Instance.inputAxisJoyCon[name];

        float value = Input.GetAxis("Joystick" + id + "Axis" + (int)axis.JoyCon_Axis);
        if (value != 0f) return (value * (axis.Invert ? -1f : 1f));

        return 0f;
    }

    static private float CheckAxisRawPS4(string name, int id)
    {
        List<PS4Axis> list = GamepadManager.Instance.inputAxisPS4[name];

        for (int i = 0; i < list.Count; i++)
        {
            float value = Input.GetAxisRaw("Joystick" + id + "Axis" + (int)list[i].PS4_Axis);
            if ((int)list[i].PS4_Axis == 2)
                if (value != 0f)
                    return (value * (list[i].Invert ? 1f : -1f));
            else
                if (value != 0f)
                    return (value * (list[i].Invert ? -1f : 1f));
        }

        return 0f;
    }

    static private float CheckAxisRawXbox(string name, int id)
    {
        List<XboxAxis> list = GamepadManager.Instance.inputAxisXbox[name];

        for (int i = 0; i < list.Count; i++)
        {
            float value = Input.GetAxisRaw("Joystick" + id + "Axis" + (int)list[i].Xbox_Axis);
            if ((int)list[i].Xbox_Axis == 2)
                if (value != 0f)
                    return (value * (list[i].Invert ? 1f : -1f));
            else
                if (value != 0f)
                    return (value * (list[i].Invert ? -1f : 1f));
        }

        return 0f;
    }

    static private float CheckAxisRawJoyCon(string name, int id)
    {
        JoyConAxis axis = GamepadManager.Instance.inputAxisJoyCon[name];

        float value = Input.GetAxisRaw("Joystick" + id + "Axis" + (int)axis.JoyCon_Axis);
        if (value != 0f) return (value * (axis.Invert ? -1f : 1f));

        return 0f;
    }

    static private string CheckAxisNamePS4(string name, int id)
    {
        List<PS4Axis> list = GamepadManager.Instance.inputAxisPS4[name];

        if (list.Count > 0)
        {
            return "Joystick" + id + "Axis" + (int)list[0].PS4_Axis;
        }

        return "";
    }

    static private string CheckAxisNameXbox(string name, int id)
    {
        List<XboxAxis> list = GamepadManager.Instance.inputAxisXbox[name];

        if (list.Count > 0)
        {
            return  "Joystick" + id + "Axis" + (int)list[0].Xbox_Axis;
        }

        return "";
    }

    static private string CheckAxisNameJoyCon(string name, int id)
    {
        JoyConAxis axis = GamepadManager.Instance.inputAxisJoyCon[name];

        return "Joystick" + id + "Axis" + (int)axis.JoyCon_Axis;
    }

    static private bool CheckButtonDownPS4(string name, int id)
    {
        List<PS4_Button> list = GamepadManager.Instance.inputButtonsPS4[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyDown(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonDownXbox(string name, int id)
    {
        List<Xbox_Button> list = GamepadManager.Instance.inputButtonsXbox[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyDown(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonDownJoyCon(string name, int id)
    {
        List<JoyCon_Button> list = GamepadManager.Instance.inputButtonsJoyCon[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyDown(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonPS4(string name, int id)
    {
        List<PS4_Button> list = GamepadManager.Instance.inputButtonsPS4[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKey(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonXbox(string name, int id)
    {
        List<Xbox_Button> list = GamepadManager.Instance.inputButtonsXbox[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKey(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonJoyCon(string name, int id)
    {
        List<JoyCon_Button> list = GamepadManager.Instance.inputButtonsJoyCon[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKey(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonUpPS4(string name, int id)
    {
        List<PS4_Button> list = GamepadManager.Instance.inputButtonsPS4[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyUp(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonUpXbox(string name, int id)
    {
        List<Xbox_Button> list = GamepadManager.Instance.inputButtonsXbox[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyUp(key);
            if (value) return value;
        }

        return false;
    }

    static private bool CheckButtonUpJoyCon(string name, int id)
    {
        List<JoyCon_Button> list = GamepadManager.Instance.inputButtonsJoyCon[name];

        for (int i = 0; i < list.Count; i++)
        {
            KeyCode key = (KeyCode)330 + (id * 20) + (int)list[i];
            bool value = Input.GetKeyUp(key);
            if (value) return value;
        }

        return false;
    }

}
