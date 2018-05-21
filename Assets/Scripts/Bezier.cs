using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;
    [Range(0f, 1f)] public float lineProgress;
    [Range(-1f, 1f)] public float lineOrientation;

    private Vector3[] workingPoints;
    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 12;

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 3;
        workingPoints = new Vector3[controlPoints.Length];
    }

    void Update()
    {
        for (int i = 0; i < workingPoints.Length; i++)
        {
            if (i == (workingPoints.Length - 1))
                workingPoints[i] = (controlPoints[i].localPosition + new Vector3 (lineOrientation * 2f, 0f, -0.33f * Mathf.Abs(lineOrientation))) * lineProgress;
            else
                workingPoints[i] = controlPoints[i].localPosition * lineProgress;
        }
            

        DrawCurve();  
    }

    void DrawCurve()
    {
        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 3;
                Vector3 pixel = CalculateCubicBezierPoint(t, workingPoints[nodeIndex], workingPoints[nodeIndex + 1], workingPoints[nodeIndex + 2], workingPoints[nodeIndex + 3]);
                lineRenderer.positionCount = (((j * SEGMENT_COUNT) + i));
                lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }
        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
