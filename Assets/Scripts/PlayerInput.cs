using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public enum ControlsType { Keyboard, Gamepad, GamepadManager }
    public ControlsType currentControlsType;

    private PlayerBehavior _player;
    private GamepadInfo _state;

    private float _prevLT;
    private float _prevRT;

    private void Awake()
    {
        _player = GetComponent<PlayerBehavior>();
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
            case ControlsType.Keyboard:             UpdateForKeyboard();            break;
            case ControlsType.Gamepad:              UpdateForGamepad();             break;
            case ControlsType.GamepadManager:       UpdateForGamepadManager();      break;
        }
    }

    private void UpdateForKeyboard()
    {
        float h = (Input.GetKey(KeyCode.Q) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = (Input.GetKey(KeyCode.Z) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
        _player.SetMoveValue(h, v);

        _player.SetRunValue(Input.GetKey(KeyCode.LeftShift));

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            _player.InstantShootAttack();

        if (Input.GetKeyDown(KeyCode.Space))
            _player.InitShootAttack();

        if (Input.GetKey(KeyCode.Space))
            _player.UpdateShootAttack();

        if (Input.GetKeyUp(KeyCode.Space))
            _player.FinalizeShootAttack();
    }

    private void UpdateForGamepad()
    {
        float h = Input.GetAxis("Joystick1Axis1");
        float v = Input.GetAxis("Joystick1Axis2");
        _player.SetMoveValue(h, v);

        float lt = Input.GetAxis("Joystick1Axis9");
        if (lt > 0f && _prevLT == 0f)  // LT Down
        {
            _player.SetRunValue(true);
        }
        else if (lt == 0f && _prevLT > 0f) // LT Up
        {
            _player.SetRunValue(false);
        }
        _prevLT = lt;

        float rt = Input.GetAxis("Joystick1Axis10");
        if (rt > 0f && _prevRT == 0f)  // RT Down
        {
            _player.InstantShootAttack();
        }
        else if (rt > 0f && _prevRT > 0f) // RT Stay
        {
            _player.InstantShootAttack();
        }
        _prevRT = rt;

        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            _player.InitShootAttack();

        if (Input.GetKey(KeyCode.Joystick1Button5))
            _player.UpdateShootAttack();

        if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            _player.FinalizeShootAttack();
    }

    private void UpdateForGamepadManager()
    {
        _state = GamepadManager.Instance.GetAssignedGamepad(0);

        if (!_state.IsConnected())
            return;

        float h = GamepadInput.GetAxis("Horizontal", _state.Type, _state.Id);
        float v = GamepadInput.GetAxis("Vertical", _state.Type, _state.Id);
        _player.SetMoveValue(h, v);

        float lt = GamepadInput.GetAxis("LeftTrigger", _state.Type, _state.Id);
        if (lt > 0f && _prevLT == 0f)  // LT Down
        {
            _player.SetRunValue(true);
        }
        else if (lt == 0f && _prevLT > 0f) // LT Up
        {
            _player.SetRunValue(false);
        }
        _prevLT = lt;

        float rt = GamepadInput.GetAxis("RightTrigger", _state.Type, _state.Id);
        if (rt > 0f && _prevRT == 0f)  // RT Down
        {
            _player.InstantShootAttack();
        }
        else if (rt > 0f && _prevRT > 0f) // RT Stay
        {
            _player.InstantShootAttack();
        }
        _prevRT = rt;

        if (GamepadInput.GetButtonDown("Fire6", _state.Type, _state.Id))
            _player.InitShootAttack();

        if (GamepadInput.GetButton("Fire6", _state.Type, _state.Id))
            _player.UpdateShootAttack();

        if (GamepadInput.GetButtonUp("Fire6", _state.Type, _state.Id))
            _player.FinalizeShootAttack();
    }
}
