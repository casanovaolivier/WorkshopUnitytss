using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExampleInputScreen : MonoBehaviour {

    public ExampleInputPlayer[] playerPanels;
    public GameObject prevScreen;
    private GameObject _prevButton;
    private GamepadInfo[] _currentGamepads;

    public void SetPrevButton(GameObject go) { _prevButton = go; }

	void OnEnable ()
    {
        _currentGamepads = GamepadManager.Instance.GetAllAssignedGamepads();

        if (_currentGamepads != null)
        {
            for (int i = 0; i < _currentGamepads.Length; i++)
            {
                if (_currentGamepads[i].Player != Player.One)
                {
                    playerPanels[i].cancel.gameObject.SetActive(false);
                    playerPanels[i].submit.gameObject.SetActive(false);
                }
                playerPanels[i].title.text = "P" + (int)_currentGamepads[i].Player + " - Gamepad #" + _currentGamepads[i].Id + " (" + _currentGamepads[i].Type + ")";

                playerPanels[i].gameObject.SetActive(true);
            }
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < playerPanels.Length; i++)
            playerPanels[i].gameObject.SetActive(false);
    }

    void Update ()
    {
		if (_currentGamepads != null)
        {
            for (int i = 0; i < _currentGamepads.Length; i++)
            {
                float h = GamepadInput.GetAxis("Horizontal", _currentGamepads[i].Type, _currentGamepads[i].Id);
                float v = GamepadInput.GetAxis("Vertical", _currentGamepads[i].Type, _currentGamepads[i].Id);

                playerPanels[i].h.text = "H = " + h.ToString("0.00");
                playerPanels[i].h.color = (h == 0f ? Color.black : Color.red);

                playerPanels[i].v.text = "V = " + v.ToString("0.00");
                playerPanels[i].v.color = (v == 0f ? Color.black : Color.red);

                bool f1 = GamepadInput.GetButton("Fire1", _currentGamepads[i].Type, _currentGamepads[i].Id);
                bool f2 = GamepadInput.GetButton("Fire2", _currentGamepads[i].Type, _currentGamepads[i].Id);
                bool f3 = GamepadInput.GetButton("Fire3", _currentGamepads[i].Type, _currentGamepads[i].Id);
                bool f4 = GamepadInput.GetButton("Fire4", _currentGamepads[i].Type, _currentGamepads[i].Id);
                bool f5 = GamepadInput.GetButton("Fire5", _currentGamepads[i].Type, _currentGamepads[i].Id);
                bool f6 = GamepadInput.GetButton("Fire6", _currentGamepads[i].Type, _currentGamepads[i].Id);

                playerPanels[i].f1.color = (f1 ? Color.red : Color.black);
                playerPanels[i].f2.color = (f2 ? Color.red : Color.black);
                playerPanels[i].f3.color = (f3 ? Color.red : Color.black);
                playerPanels[i].f4.color = (f4 ? Color.red : Color.black);
                playerPanels[i].f5.color = (f5 ? Color.red : Color.black);
                playerPanels[i].f6.color = (f6 ? Color.red : Color.black);

                if (_currentGamepads[i].Player == Player.One)
                {
                    string submit = GamepadInput.GetButtonForUI("Submit", _currentGamepads[i].Type, _currentGamepads[i].Id);
                    string cancel = GamepadInput.GetButtonForUI("Cancel", _currentGamepads[i].Type, _currentGamepads[i].Id);

                    bool sValue = Input.GetButtonDown(submit);
                    bool cValue = Input.GetButtonDown(cancel);

                    playerPanels[i].submit.color = (sValue ? Color.red : Color.black);
                    playerPanels[i].cancel.color = (cValue ? Color.red : Color.black);

                    bool start = GamepadInput.GetButton("Start", _currentGamepads[i].Type, _currentGamepads[i].Id);

                    if (f5 && f6 && start)
                    {
                        gameObject.SetActive(false);
                        prevScreen.SetActive(true);

                        if (_prevButton != null) EventSystem.current.SetSelectedGameObject(_prevButton);
                    }
                }
            }
        }
	}
}
