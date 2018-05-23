using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForDoor : MonoBehaviour
{
    public Animator doorToTriggered;
    public float doorClosingTime;
    private bool _doorIsOpen = false;
    private float _doorOpeningTime;
    private MeshRenderer _doorTrigger;
    public void Awake()
    {
        _doorTrigger = GetComponentInChildren<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_doorIsOpen && collision.gameObject.tag == "PlayerBall")
        {
            doorToTriggered.SetBool("isOpen", true);
            _doorTrigger.material.color = Color.green;
            _doorOpeningTime = Time.time;
            _doorIsOpen = true;
        }
    }

    private void Update()
    {
        if (_doorIsOpen && doorClosingTime > 0f && Time.time - _doorOpeningTime > doorClosingTime)
        {
            doorToTriggered.SetBool("isOpen", false);
            _doorTrigger.material.color = Color.red;
            _doorIsOpen = false;
        }
    }
}
