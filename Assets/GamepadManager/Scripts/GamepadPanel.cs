using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadPanel : UIBehaviour
{
    public int nbJoystick;

    public GameObject[] joystickSlots;
    public Text[] joystickSlotsText;
    public Image[] joystickSlotsIcon;

    public Sprite[] joystickIcons;

    private EventTrigger _eventTrigger;
    private GamepadTrigger _gamepadTrigger;

    private List<EventTrigger.Entry> _entryOnCancel;
    private List<EventTrigger.Entry> _entryOnSubmit;
    private bool _onSubmitIsActive;

    public void Init()
    {
        _eventTrigger = GetComponent<EventTrigger>();

        _entryOnCancel = new List<EventTrigger.Entry>();
        _entryOnSubmit = new List<EventTrigger.Entry>();

        foreach (EventTrigger.Entry entry in _eventTrigger.triggers ?? Enumerable.Empty<EventTrigger.Entry>())
        {
            if (entry.eventID == EventTriggerType.Submit)
                _entryOnSubmit.Add(entry);
            else if (entry.eventID == EventTriggerType.Cancel)
                _entryOnCancel.Add(entry);
        }

        _eventTrigger.triggers.Clear();
        _eventTrigger.triggers.TrimExcess();

        _onSubmitIsActive = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _eventTrigger.triggers.Clear();
        _eventTrigger.triggers.TrimExcess();

        foreach (EventTrigger.Entry entry in _entryOnCancel ?? Enumerable.Empty<EventTrigger.Entry>())
        {
            _eventTrigger.triggers.Add(entry);
        }

        for (int i = 0; i < nbJoystick; i++)
        {
            GamepadInfo padInfo = GamepadManager.Instance.GetAssignedGamepad(i);
            
            if ((int)padInfo.Player > 0 && (int)padInfo.Id > 0 && (int)padInfo.Type > 0)
            {
                joystickSlotsText[(int)padInfo.Player - 1].text = "#" + padInfo.Id;
                joystickSlotsIcon[(int)padInfo.Player - 1].sprite = joystickIcons[(int)padInfo.Type - 1];
                joystickSlots[(int)padInfo.Player - 1].SetActive(true);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        for (int i = 0; i < nbJoystick; i++)
            joystickSlots[i].SetActive(false);

        _gamepadTrigger = null;
    }

    private static bool SubmitEvent(EventTrigger.Entry e)
    {
        return e.eventID == EventTriggerType.Submit && e != null;
    }

    private static bool CancelEvent(EventTrigger.Entry e)
    {
        return e.eventID == EventTriggerType.Cancel && e != null;
    }

    public void AddJoystickSlot(int slot, GamepadInfo joyInfo)
    {
        joystickSlotsText[slot].text = "#" + joyInfo.Id;
        joystickSlotsIcon[slot].sprite = joystickIcons[((int)joyInfo.Type) - 1];
        joystickSlots[slot].SetActive(true);
    }

    public void RemoveJoystickSlot(int slot)
    {
        joystickSlots[slot].SetActive(false);
    }

    public void SetOnSubmitActive(bool enable)
    {
        if (enable !=_onSubmitIsActive)
        {
            if (enable)
            {
                foreach (EventTrigger.Entry entry in _entryOnSubmit ?? Enumerable.Empty<EventTrigger.Entry>())
                    _eventTrigger.triggers.Add(entry);

                if (_gamepadTrigger != null)
                {
                    foreach (EventTrigger.Entry entry in _gamepadTrigger.Triggers ?? Enumerable.Empty<EventTrigger.Entry>())
                        if (entry.eventID == EventTriggerType.Submit) _eventTrigger.triggers.Add(entry);
                }
            }
            else
            {
                _eventTrigger.triggers.RemoveAll(SubmitEvent);
            }

            _onSubmitIsActive = enable;
        }
    }

    public void AddGamepadTrigger(GamepadTrigger et)
    {
        if (_gamepadTrigger == null)
        {
            _gamepadTrigger = et;

            foreach (EventTrigger.Entry entry in _gamepadTrigger.Triggers ?? Enumerable.Empty<EventTrigger.Entry>())
            {
                if (!_onSubmitIsActive)
                {
                    if (entry.eventID != EventTriggerType.Submit)
                        _eventTrigger.triggers.Add(entry);
                }
                else
                {
                    _eventTrigger.triggers.Add(entry);
                }
            }
        } 
    }

}
