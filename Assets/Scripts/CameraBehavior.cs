using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform targetA;
    public Transform targetB;
    [Range(0f, 1f)] public float targetRatio = 0.5f;
    [Range(0f, 1f)] public float smoothSpeed = 0.125f;

	private void FixedUpdate ()
    {
        Vector3 newPosition = Vector3.Lerp(targetA.position, targetB.position, targetRatio);
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);     
	}
}
