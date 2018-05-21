using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GamepadManager : MonoBehaviour {

#region Variables

    static public GamepadManager Instance;
    public bool dontDestroyOnLoad;
    public GamepadProfile activeProfile;

    public GamepadManagerState startState;

    public UnityEvent whenWaitForFirstJoystickSuccess;
    public UnityEvent whenJoystickIsDisconnected;
    public UnityEvent whenJoystickIsReconnected;

    public Dictionary<string, Gamepad> joystickNames;

    public Dictionary<string, List<PS4Axis>> inputAxisPS4;
    public Dictionary<string, List<XboxAxis>> inputAxisXbox;
    public Dictionary<string, JoyConAxis> inputAxisJoyCon;

    public Dictionary<string, List<PS4_Button>> inputButtonsPS4;
    public Dictionary<string, List<Xbox_Button>> inputButtonsXbox;
    public Dictionary<string, List<JoyCon_Button>> inputButtonsJoyCon;

    public Dictionary<string, PS4_ButtonForUI> inputUiButtonsPS4;
    public Dictionary<string, Xbox_ButtonForUI> inputUiButtonsXbox;
    public Dictionary<string, JoyCon_ButtonForUI> inputUiButtonsJoyCon;

    private GamepadPanel[] _panelCheckFor;
    private GamepadPanel _currentPanel;

    private GameObject _currentSelectedGameObject;

    private bool _disconnectedGamepad;
    private GamepadStatut[] _detectedGamepads = new GamepadStatut[8];
    private GamepadInfo[] _assignedGamepads;
    private GamepadInfo _mainGamepad;

    private GamepadManagerState _currentManagerState;

    private EventSystem _eventSystem;
    private StandaloneInputModule _standaloneInputModule;
    private EventTrigger _eventTrigger;

    public bool panelIsActive { get; private set; }

    public GamepadInfo GetMainGamepad() { return _mainGamepad; }
    public GamepadInfo GetAssignedGamepad(int playerIndex) { return _assignedGamepads[playerIndex]; }
    public GamepadInfo[] GetAllAssignedGamepads() { return _assignedGamepads; }

    public bool GamepadAreConfigured(int nb)
    {
        if (_assignedGamepads == null)
            return false;

        if (_assignedGamepads.Length != nb)
            return false;

        for (int i = 0; i < _assignedGamepads.Length; i++)
        {
            if ((int)_assignedGamepads[i].Player == 0 || _assignedGamepads[i].Id == 0 || (int)_assignedGamepads[i].Type == 0)
                return false;
        }

        return true;
    }

    #endregion

#region Init Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        InitDictionnaries();
        InitPanels();

        _eventSystem = EventSystem.current;
        _standaloneInputModule = _eventSystem.gameObject.GetComponent<StandaloneInputModule>();

        switch(startState)
        {
            case GamepadManagerState.WaitForFirstJoystick:
                WaitForFirstJoystick();
                break;
            case GamepadManagerState.CheckForOneJoystick:
                CheckForOnePlayer(true);
                break;
            case GamepadManagerState.CheckForTwoJoystick:
                CheckForTwoPlayer(true);
                break;
            case GamepadManagerState.CheckForThreeJoystick:
                CheckForThreePlayer(true);
                break;
            case GamepadManagerState.CheckForFourJoystick:
                CheckForOnePlayer(true);
                break;
            case GamepadManagerState.DoNothing:
                DoNothing();
                break;
        }
    }

    void Start ()
    {
        /*Debug.Log(Input.GetJoystickNames().Length);
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            Debug.Log(Input.GetJoystickNames()[i]);
        }*/
    }

    private void InitDictionnaries()
    {
        joystickNames = new Dictionary<string, Gamepad>
        {
            { "controller (xbox one for windows)", Gamepad.Xbox },
            { "controller (xbox 360 for windows)", Gamepad.Xbox },
            { "controller (xbox 360 wireless receiver for windows)", Gamepad.Xbox },
            { "dualshock®4 usb wireless adaptor", Gamepad.PS4 },
            { "wireless controller", Gamepad.PS4 },
            { "wireless gamepad", Gamepad.JoyCon },
            { "", Gamepad.None }
        };

        inputAxisPS4 = new Dictionary<string, List<PS4Axis>>();
        inputAxisXbox = new Dictionary<string, List<XboxAxis>>();
        inputAxisJoyCon = new Dictionary<string, JoyConAxis>();

        for (int i = 0; i < activeProfile.axis.Length; i++)
        {
            inputAxisPS4.Add(activeProfile.axis[i].name, activeProfile.axis[i].GetPS4AxisList());
            inputAxisXbox.Add(activeProfile.axis[i].name, activeProfile.axis[i].GetXboxAxisList());
            inputAxisJoyCon.Add(activeProfile.axis[i].name, activeProfile.axis[i].JoyCon);
        }

        inputButtonsPS4 = new Dictionary<string, List<PS4_Button>>();
        inputButtonsXbox = new Dictionary<string, List<Xbox_Button>>();
        inputButtonsJoyCon = new Dictionary<string, List<JoyCon_Button>>();

        for (int j = 0; j < activeProfile.buttons.Length; j++)
        {
            inputButtonsPS4.Add(activeProfile.buttons[j].name, activeProfile.buttons[j].GetPS4ButtonsList());
            inputButtonsXbox.Add(activeProfile.buttons[j].name, activeProfile.buttons[j].GetXboxButtonsList());
            inputButtonsJoyCon.Add(activeProfile.buttons[j].name, activeProfile.buttons[j].GetJoyConButtonsList());
        }

        inputUiButtonsPS4 = new Dictionary<string, PS4_ButtonForUI>();
        inputUiButtonsXbox = new Dictionary<string, Xbox_ButtonForUI>();
        inputUiButtonsJoyCon = new Dictionary<string, JoyCon_ButtonForUI>();

        for (int k = 0; k < activeProfile.buttonsForUI.Length; k++)
        {
            inputUiButtonsPS4.Add(activeProfile.buttonsForUI[k].name, activeProfile.buttonsForUI[k].PS4);
            inputUiButtonsXbox.Add(activeProfile.buttonsForUI[k].name, activeProfile.buttonsForUI[k].Xbox);
            inputUiButtonsJoyCon.Add(activeProfile.buttonsForUI[k].name, activeProfile.buttonsForUI[k].JoyCon);
        }
    }

    private void InitPanels()
    {
        _panelCheckFor = new GamepadPanel[4]
        {
            transform.Find("Canvas/Panel_ControllerSelection_1P").GetComponent<GamepadPanel>(),
            transform.Find("Canvas/Panel_ControllerSelection_2P").GetComponent<GamepadPanel>(),
            transform.Find("Canvas/Panel_ControllerSelection_3P").GetComponent<GamepadPanel>(),
            transform.Find("Canvas/Panel_ControllerSelection_4P").GetComponent<GamepadPanel>()
        };

        foreach (GamepadPanel gp in _panelCheckFor)
        {
            gp.Init();
            gp.gameObject.SetActive(false);
        }

        panelIsActive = false;
    }

#endregion

#region Update Methods

    private void Update()
    {
        UpdateManagerState();
    }

    private void UpdateJoystickState()
    {
        string[] inputJoystickNames =  Input.GetJoystickNames();
        if (inputJoystickNames.Length > 0)
        {
            for(int i = 0; i < Mathf.Min(inputJoystickNames.Length, _detectedGamepads.Length); i++)
            {
                Gamepad value;
                if (joystickNames.TryGetValue(inputJoystickNames[i].ToLower(), out value))              // Key was in dictionary; "value" contains corresponding value
                {
                    if (_detectedGamepads[i].Type != value)
                    {
                        if (value == Gamepad.None)                          //Joystick Disconnected
                        {
                            OnJoystickDisconnect(i + 1);
                            _detectedGamepads[i].Selected = false;
                        }
                        //else { }                                          //Joystick Connected

                        //Debug.Log(value + " " + (i + 1).ToString());

                        _detectedGamepads[i].Type = value;
                    }
                }
                else                                                                                    // Key wasn't in dictionary; "value" is now 0
                {
                    Debug.Log("Unknow joystickName from Input.GetJoystickNames() :" + inputJoystickNames[i]);

                    if (_detectedGamepads[i].Type != Gamepad.Unknown)    
                    {
                        if (_detectedGamepads[i].Type != Gamepad.None)      //Joystick Disconnected -> Replace by Unknow ?     
                        {
                            OnJoystickDisconnect(i + 1);
                            _detectedGamepads[i].Selected = false;
                        }
                        //else { }                                          //Unknown Joystick Connected

                        //Debug.Log(Gamepad.Unknown + " " + (i + 1).ToString());

                        _detectedGamepads[i].Type = Gamepad.Unknown;
                        
                    }
                }
            }
        }
    }

    private void UpdateManagerState()
    {
        switch (_currentManagerState)
        {
            case GamepadManagerState.WaitForFirstJoystick:
                UpdateJoystickState();

                for (int i = 0; i < _detectedGamepads.Length; i++)
                {
                    if (_detectedGamepads[i].Type != Gamepad.None && _detectedGamepads[i].Type != Gamepad.Unknown)
                    {
                        bool pressL = GamepadInput.GetButton("Fire5", _detectedGamepads[i].Type, i + 1);
                        bool pressR = GamepadInput.GetButton("Fire6", _detectedGamepads[i].Type, i + 1);

                        string submit = GamepadInput.GetButtonForUI("Submit", _detectedGamepads[i].Type, i + 1);

                        if ((pressL && pressR) || Input.GetButtonDown(submit))
                        {
                            int emptySlot = GetNextJoystickEmptySlot();

                            if (emptySlot != -1)
                            {
                                _mainGamepad.Player = _assignedGamepads[emptySlot].Player = Player.One;
                                _mainGamepad.Id = _assignedGamepads[emptySlot].Id = i + 1;
                                _mainGamepad.Type = _assignedGamepads[emptySlot].Type = _detectedGamepads[i].Type;

                                _detectedGamepads[i].Selected = true;

                                SetStandaloneInputModule(_mainGamepad);

                                //Debug.Log("Joystick #" + _assignedGamepeds[0].Id);

                                whenWaitForFirstJoystickSuccess.Invoke();

                                _currentManagerState = GamepadManagerState.WaitForDisconnection;
                            }   
                        }
                    }
                }
                break;
            case GamepadManagerState.CheckForOneJoystick:
            case GamepadManagerState.CheckForTwoJoystick:
            case GamepadManagerState.CheckForThreeJoystick:
            case GamepadManagerState.CheckForFourJoystick:
                UpdateJoystickState();

                if (!GamepadAreConfigured((int)_currentManagerState - 1))
                {
                    _currentPanel.SetOnSubmitActive(false);

                    for (int i = 0; i < _detectedGamepads.Length; i++)
                    {
                        if (_detectedGamepads[i].Type != Gamepad.None && _detectedGamepads[i].Type != Gamepad.Unknown && !_detectedGamepads[i].Selected)
                        {
                            bool pressL = GamepadInput.GetButton("Fire5", _detectedGamepads[i].Type, i + 1);
                            bool pressR = GamepadInput.GetButton("Fire6", _detectedGamepads[i].Type, i + 1);

                            if (pressL && pressR)
                            {
                                int emptySlotIndex = GetNextJoystickEmptySlot();

                                if (emptySlotIndex != -1)
                                {
                                    _assignedGamepads[emptySlotIndex].Player = (Player)(emptySlotIndex + 1);
                                    _assignedGamepads[emptySlotIndex].Id = i + 1;
                                    _assignedGamepads[emptySlotIndex].Type = _detectedGamepads[i].Type;

                                    _detectedGamepads[i].Selected = true;

                                    if (emptySlotIndex == 0)
                                    {
                                        _mainGamepad = _assignedGamepads[emptySlotIndex];
                                        SetStandaloneInputModule(_mainGamepad);
                                    }

                                    //Debug.Log("Joystick #" + _assignedGamepeds[emptySlotIndex].Id);

                                    _currentPanel.AddJoystickSlot(emptySlotIndex, _assignedGamepads[emptySlotIndex]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _currentPanel.SetOnSubmitActive(true);
                }
                break;
            case GamepadManagerState.WaitForDisconnection:
                UpdateJoystickState();
                //TODO
                break;
        }
    }

#endregion

#region Private Methods

    private void OnJoystickDisconnect(int id)
    {
        switch (_currentManagerState)
        {
            case GamepadManagerState.CheckForOneJoystick:
            case GamepadManagerState.CheckForTwoJoystick:
            case GamepadManagerState.CheckForThreeJoystick:
            case GamepadManagerState.CheckForFourJoystick:
                for(int i = 0; i < _assignedGamepads.Length; i++)
                {
                    if (_assignedGamepads[i].Id == id)
                    {
                        _currentPanel.RemoveJoystickSlot((int)_assignedGamepads[i].Player - 1);

                        _assignedGamepads[i].Id = 0;
                        _assignedGamepads[i].Player = Player.Unknown;
                        _assignedGamepads[i].Type = Gamepad.None;
                    }
                }
                break;
            case GamepadManagerState.WaitForDisconnection:
                whenJoystickIsDisconnected.Invoke();
                _disconnectedGamepad = true;

                for (int i = 0; i < _assignedGamepads.Length; i++)
                {
                    if (_assignedGamepads[i].Id == id)
                    {
                        _currentPanel.RemoveJoystickSlot((int)_assignedGamepads[i].Player - 1);

                        _assignedGamepads[i].Id = 0;
                        _assignedGamepads[i].Player = Player.Unknown;
                        _assignedGamepads[i].Type = Gamepad.None;
                    }
                }

                switch (_assignedGamepads.Length)
                {
                    case 1:
                        CheckForOnePlayer(false);
                        break;
                    case 2:
                        CheckForTwoPlayer(false);
                        break;
                    case 3:
                        CheckForThreePlayer(false);
                        break;
                    case 4:
                        CheckForFourPlayer(false);
                        break;
                }
                break;
        }
    }

    private int GetNextJoystickEmptySlot()
    {
        for (int i = 0; i < _assignedGamepads.Length; i++)
        {
            if ((int)_assignedGamepads[i].Player == 0 || (int)_assignedGamepads[i].Id == 0 || (int)_assignedGamepads[i].Type == 0)
                return i;
        }

        return -1;
    }

    private void SetStandaloneInputModule(GamepadInfo gi)
    {
        string h = GamepadInput.GetAxisName("Horizontal", gi.Type, gi.Id);
        _standaloneInputModule.horizontalAxis = h.Equals("") ? "Joystick" + gi.Id + "Axis1" : h; 
        string v = GamepadInput.GetAxisName("Vertical", gi.Type, gi.Id);
        _standaloneInputModule.verticalAxis = v.Equals("") ? "Joystick" + gi.Id + "Axis2" : v;
        _standaloneInputModule.submitButton = GamepadInput.GetButtonForUI("Submit", gi.Type, gi.Id);
        _standaloneInputModule.cancelButton = GamepadInput.GetButtonForUI("Cancel", gi.Type, gi.Id);

    }

    #endregion

#region Public Methods (for UnityEvent UI) 

    public void AddGamepadTrigger(GamepadTrigger gt)
    {
        if(_currentPanel != null)
            _currentPanel.AddGamepadTrigger(gt);
    }

    public void DoNothing()
    {
        _currentManagerState = GamepadManagerState.DoNothing;
    }

    public void WaitForFirstJoystick()
    {
        _assignedGamepads = new GamepadInfo[1];

        _currentManagerState = GamepadManagerState.WaitForFirstJoystick;
    }

    public void CheckForOnePlayer(bool isObligatory)
    {
        if (isObligatory)
        {
            _assignedGamepads = new GamepadInfo[1];
            for (int i = 0; i < _detectedGamepads.Length; i++)
            {
                _detectedGamepads[i].Selected = false;
            }

            _currentPanel = _panelCheckFor[0];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForOneJoystick;
        }
        else if (!GamepadAreConfigured(1))
        {
            if (_assignedGamepads == null || _assignedGamepads.Length != 1)
            {
                _assignedGamepads = new GamepadInfo[1];
                for (int i = 0; i < _detectedGamepads.Length; i++)
                {
                    _detectedGamepads[i].Selected = false;
                }
            }

            _currentPanel = _panelCheckFor[0];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForOneJoystick;
        }
        else
        {
            _currentManagerState = GamepadManagerState.WaitForDisconnection;  
        }
    }

    public void CheckForTwoPlayer(bool isObligatory)
    {
        if (isObligatory)
        {
            _assignedGamepads = new GamepadInfo[2];
            for (int i = 0; i < _detectedGamepads.Length; i++)
            {
                _detectedGamepads[i].Selected = false;
            }

            _currentPanel = _panelCheckFor[1];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForTwoJoystick;
        }
        else if (!GamepadAreConfigured(2))
        {
            if (_assignedGamepads == null || _assignedGamepads.Length != 2)
            {
                _assignedGamepads = new GamepadInfo[2];
                for (int i = 0; i < _detectedGamepads.Length; i++)
                {
                    _detectedGamepads[i].Selected = false;
                }
            }

            _currentPanel = _panelCheckFor[1];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForTwoJoystick;
        }
        else
        {
            _currentManagerState = GamepadManagerState.WaitForDisconnection;
        }
    }

    public void CheckForThreePlayer(bool isObligatory)
    {
        if (isObligatory)
        {
            _assignedGamepads = new GamepadInfo[3];
            for (int i = 0; i < _detectedGamepads.Length; i++)
            {
                _detectedGamepads[i].Selected = false;
            }

            _currentPanel = _panelCheckFor[2];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForThreeJoystick;
        }
        else if(!GamepadAreConfigured(3))
        {
            if (_assignedGamepads == null || _assignedGamepads.Length != 3)
            {
                _assignedGamepads = new GamepadInfo[3];
                for (int i = 0; i < _detectedGamepads.Length; i++)
                {
                    _detectedGamepads[i].Selected = false;
                }
            }

            _currentPanel = _panelCheckFor[2];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForThreeJoystick;
        }
        else
        {
            _currentManagerState = GamepadManagerState.WaitForDisconnection;
        }
    }

    public void CheckForFourPlayer(bool isObligatory)
    {
        if (isObligatory)
        {
            _assignedGamepads = new GamepadInfo[4];
            for (int i = 0; i < _detectedGamepads.Length; i++)
            {
                _detectedGamepads[i].Selected = false;
            }

            _currentPanel = _panelCheckFor[3];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForFourJoystick;
        }
        else if (!GamepadAreConfigured(4))
        {
            if (_assignedGamepads == null || _assignedGamepads.Length != 4)
            {
                _assignedGamepads = new GamepadInfo[4];
                for (int i = 0; i < _detectedGamepads.Length; i++)
                {
                    _detectedGamepads[i].Selected = false;
                }
            }

            _currentPanel = _panelCheckFor[3];
            _currentPanel.gameObject.SetActive(true);
            panelIsActive = true;

            _eventSystem.SetSelectedGameObject(_currentPanel.gameObject);

            _currentManagerState = GamepadManagerState.CheckForFourJoystick;
        }
        else
        {
            _currentManagerState = GamepadManagerState.WaitForDisconnection;
        }
    }

    public void ClosePanel(bool isSubmit)
    {
        if (isSubmit)
        {
            if (_disconnectedGamepad)
            {
                whenJoystickIsReconnected.Invoke();
                _disconnectedGamepad = false;
            }

            _currentManagerState = GamepadManagerState.WaitForDisconnection;
            
            _currentPanel.gameObject.SetActive(false);
            Invoke("DelayPanelDesactivate", 0.2f);

        }
        else
        {
            _assignedGamepads = new GamepadInfo[1];
            _assignedGamepads[0] = _mainGamepad;

            _currentManagerState = GamepadManagerState.DoNothing;
            
            _currentPanel.gameObject.SetActive(false);
            Invoke("DelayPanelDesactivate", 0.2f);
        }
    }

    public void DelayPanelDesactivate()
    {
        panelIsActive = false;
    }

    public void SaveCurrentSelectedGameObject()
    {
        _currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    }

    public void LoadSavedSelectedGameObject()
    {
        if (_currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(_currentSelectedGameObject);
    }

    #endregion
}
