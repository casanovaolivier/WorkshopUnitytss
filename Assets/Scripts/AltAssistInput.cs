using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltAssistInput : MonoBehaviour
{
    public enum ControlsType { Mouse, Gamepad, GamepadManager }
    public ControlsType currentControlsType;

    private AltAssistBehavior _assist;
    private GamepadInfo _state;

    private GameObject _player;

    private float _prevLT;
    private float _prevRT;

    private void Awake()
    {
        _assist = GetComponent<AltAssistBehavior>();
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

        _assist.SetState(Input.mouseScrollDelta.y);

        if (Input.GetKeyDown(KeyCode.E))
            _assist.SetHead(true);

        if (Input.GetKeyDown(KeyCode.R))
            _assist.SetHead(false);
    }

    private void UpdateForGamepad()
    {
        float ah = Input.GetAxis("Joystick1Axis4");
        float av = Input.GetAxis("Joystick1Axis5");
        _assist.SetMoveValue(ah, -av);

        if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            _assist.SetState(-1f);

        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            _assist.SetState(1f);

        float rt = Input.GetAxis("Joystick1Axis10");
        if (rt > 0f && _prevRT == 0f)  // RT Down
        {
            _assist.SetHead(true);
        }
        _prevRT = rt;

        float lt = Input.GetAxis("Joystick1Axis9");
        if (lt > 0f && _prevLT == 0f)  // LT Down
        {
            _assist.SetHead(false);
        }
        _prevLT = lt;
    }

    private void UpdateForGamepadManager()
    {
        _state = GamepadManager.Instance.GetAssignedGamepad(0);

        if (!_state.IsConnected())
            return;

        float ah = GamepadInput.GetAxis("AltHorizontal", _state.Type, _state.Id);
        float av = GamepadInput.GetAxis("AltVertical", _state.Type, _state.Id);
        _assist.SetMoveValue(ah, av);

        if (GamepadInput.GetButton("Fire5", _state.Type, _state.Id))
            _assist.SetState(-1f);

        if (GamepadInput.GetButton("Fire6", _state.Type, _state.Id))
            _assist.SetState(1f);

        float rt = GamepadInput.GetAxis("RightTrigger", _state.Type, _state.Id);
        if (rt > 0f && _prevRT == 0f)  // RT Down
        {
            _assist.SetHead(true);
        }
        _prevRT = rt;

        float lt = GamepadInput.GetAxis("LeftTrigger", _state.Type, _state.Id);
        if (lt > 0f && _prevLT == 0f)  // LT Down
        {
            _assist.SetHead(false);
        }
        _prevLT = lt;
    }
}
