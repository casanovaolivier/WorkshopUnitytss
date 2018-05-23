using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New Gamepad Profile", menuName = "Gamepad Profile", order = 202)]
public class GamepadProfile : ScriptableObject
{
    public Axis[] axis;
    public Buttons[] buttons;
    public ButtonsForUI[] buttonsForUI;
}

[System.Serializable]
public struct Axis
{
    public string name;
    public PS4Axis[] PS4;
    public XboxAxis[] Xbox;
    public JoyConAxis JoyCon;

    public List<PS4Axis> GetPS4AxisList() { return PS4.ToList(); }
    public List<XboxAxis> GetXboxAxisList() { return Xbox.ToList(); }
}

[System.Serializable]
public struct Buttons
{
    public string name;
    public PS4_Button[] PS4;
    public Xbox_Button[] Xbox;
    public JoyCon_Button[] JoyCon;

    public List<PS4_Button> GetPS4ButtonsList() { return PS4.ToList(); }
    public List<Xbox_Button> GetXboxButtonsList() { return Xbox.ToList(); }
    public List<JoyCon_Button> GetJoyConButtonsList() { return JoyCon.ToList(); }
}

[System.Serializable]
public struct ButtonsForUI
{
    public string name;
    public PS4_ButtonForUI PS4;
    public Xbox_ButtonForUI Xbox;
    public JoyCon_ButtonForUI JoyCon;
}

[System.Serializable]
public struct PS4Axis
{
    public PS4_Axis PS4_Axis;
    public bool Invert;
}

[System.Serializable]
public struct XboxAxis
{
    public Xbox_Axis Xbox_Axis;
    public bool Invert;
}

[System.Serializable]
public struct JoyConAxis
{
    public JoyCon_Axis JoyCon_Axis;
    public bool Invert;
}

[System.Serializable]
public struct GamepadStatut
{
    public Gamepad Type;
    public bool Selected;
}

[System.Serializable]
public struct GamepadInfo
{
    public Player Player;
    public int Id;
    public Gamepad Type;

    public bool IsConnected() { return (Player > 0 && Id > 0 && Type > 0); }
}