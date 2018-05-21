using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Gamepad Manager/Gamepad Trigger")]
[RequireComponent(typeof(EventTrigger))]
public class GamepadTrigger : MonoBehaviour {

    EventTrigger _eventTrigger;
    List<EventTrigger.Entry> _eventEntry;

    public List<EventTrigger.Entry> Triggers { get { return _eventEntry; } }

    private void Awake ()
    {
        _eventTrigger = GetComponent<EventTrigger>();

        _eventEntry = new List<EventTrigger.Entry>();

        foreach(EventTrigger.Entry entry in _eventTrigger.triggers ?? Enumerable.Empty<EventTrigger.Entry>())
            _eventEntry.Add(entry);

        _eventTrigger.triggers.Clear();
        _eventTrigger.triggers.TrimExcess();
    }

}
