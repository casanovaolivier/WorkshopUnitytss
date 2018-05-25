using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltPlayerInput : MonoBehaviour
{
    public enum ControlsType { Keyboard, Gamepad, GamepadManager }
    public ControlsType currentControlsType;

    public enum ShootType { WithButtons, WithStick }
    public ShootType currentShootType;

    private AltPlayerBehavior _player;
    private GamepadInfo _state;

    private float _prevLT;
    private float _prevRT;

    private void Awake()
    {
        _player = GetComponent<AltPlayerBehavior>();
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
            case ControlsType.Keyboard: UpdateForKeyboard(); break;
            case ControlsType.Gamepad: UpdateForGamepad(); break;
            case ControlsType.GamepadManager: UpdateForGamepadManager(); break;
        }
    }

    private void UpdateForKeyboard()
    {
        float h = (Input.GetKey(KeyCode.Q) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = (Input.GetKey(KeyCode.Z) ? 1f : 0f) + (Input.GetKey(KeyCode.S) ? -1f : 0f);
        _player.SetMoveValue(h, v);

        _player.SetRunValue(Input.GetKey(KeyCode.LeftShift));

        switch (currentShootType)
        {
            case ShootType.WithButtons:

                if (Input.GetKeyDown(KeyCode.Space))
                    _player.InitShootAttack();

                if (Input.GetKey(KeyCode.Space))
                    _player.UpdateShootAttack();

                if (Input.GetKeyUp(KeyCode.Space))
                    _player.FinalizeShootAttack();
                break;
            case ShootType.WithStick:
                break;
        }
    }

    private void UpdateForGamepad()
    {
        float h = Input.GetAxis("Joystick1Axis1");
        float v = Input.GetAxis("Joystick1Axis2");
        _player.SetMoveValue(h, v);

        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            _player.SetRunValue(true);

        if (Input.GetKeyUp(KeyCode.Joystick1Button0))
            _player.SetRunValue(true);

        switch (currentShootType)
        {
            case ShootType.WithButtons:

                if (Input.GetKeyDown(KeyCode.Joystick1Button3))
                    _player.InitShootAttack();

                if (Input.GetKey(KeyCode.Joystick1Button3))
                    _player.UpdateShootAttack();

                if (Input.GetKeyUp(KeyCode.Joystick1Button3))
                    _player.FinalizeShootAttack();
                break;
        } 
    }

    private void UpdateForGamepadManager()
    {
        _state = GamepadManager.Instance.GetAssignedGamepad(0);

        if (!_state.IsConnected())
            return;

        float h = GamepadInput.GetAxis("Horizontal", _state.Type, _state.Id);
        float v = GamepadInput.GetAxis("Vertical", _state.Type, _state.Id);
        _player.SetMoveValue(h, v);

        if (GamepadInput.GetButtonDown("Fire1", _state.Type, _state.Id))
            _player.SetRunValue(true);

        if (GamepadInput.GetButtonUp("Fire1", _state.Type, _state.Id))
            _player.SetRunValue(true);

        switch (currentShootType)
        {
            case ShootType.WithButtons:
               

                if (GamepadInput.GetButtonDown("Fire4", _state.Type, _state.Id))
                    _player.InitShootAttack();

                if (GamepadInput.GetButton("Fire4", _state.Type, _state.Id))
                    _player.UpdateShootAttack();

                if (GamepadInput.GetButtonUp("Fire4", _state.Type, _state.Id))
                    _player.FinalizeShootAttack();
                break;
            
        }
    }
}
