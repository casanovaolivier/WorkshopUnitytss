using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistInput : MonoBehaviour
{
    public enum ControlsType { Mouse, Gamepad, GamepadManager }
    public ControlsType currentControlsType;

    private AssistBehavior _assist;
    private GamepadInfo _state;

    private GameObject _player;

    private void Awake()
    {
        _assist = GetComponent<AssistBehavior>();
    }

    public void SetControlsType(int value)
    {
        currentControlsType = (ControlsType)value;
    }

    private void Update()
    {
        switch (currentControlsType)
        {
            default:
            case ControlsType.Mouse:            UpdateForMouse();            break;
            case ControlsType.Gamepad:          UpdateForGamepad();             break;
            case ControlsType.GamepadManager:   UpdateForGamepadManager();      break;
        }
    }

    private void UpdateForMouse()
    {
        if (_player != null)
        {
            Vector3 playerPos = Camera.main.WorldToScreenPoint(_player.transform.position);
            Vector3 relativePos = (Input.mousePosition - playerPos).normalized;
            _assist.SetMoveValue(relativePos.x, relativePos.y);
        }
        else
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        _assist.AlignWithPlayer(Input.GetMouseButton(1));
    }

    private void UpdateForGamepad()
    {
        float ah = Input.GetAxis("Joystick1Axis4");
        float av = Input.GetAxis("Joystick1Axis5");
        _assist.SetMoveValue(ah, -av);

        _assist.AlignWithPlayer(Input.GetKey(KeyCode.Joystick1Button4));
    }

    private void UpdateForGamepadManager()
    {
        _state = GamepadManager.Instance.GetAssignedGamepad(0);

        if (!_state.IsConnected())
            return;

        float ah = GamepadInput.GetAxis("AltHorizontal", _state.Type, _state.Id);
        float av = GamepadInput.GetAxis("AltVertical", _state.Type, _state.Id);
        _assist.SetMoveValue(ah, av);

        _assist.AlignWithPlayer(GamepadInput.GetButton("Fire5", _state.Type, _state.Id));
    }
}
