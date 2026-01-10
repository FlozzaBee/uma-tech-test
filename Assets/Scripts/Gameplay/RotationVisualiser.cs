using System;
using UnityEngine;

public class RotationVisualiser : Singleton<RotationVisualiser>
{
    [SerializeField] private Renderer xAxis;
    [SerializeField] private Renderer yAxis;
    [SerializeField] private Renderer zAxis;

    [SerializeField] private float _fadeDuration = 1f;

    [SerializeField] private Color xColor = Color.red;
    [SerializeField] private Color yColor = Color.green;
    [SerializeField] private Color zColor = Color.blue;

    private Color xCurrentColor;
    private Color yCurrentColor;
    private Color zCurrentColor;
    
    public enum Axis
    {
        X,
        Y,
        Z,
    }

    public void VisualiseRotation(Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                xCurrentColor = xColor;
                break;
            case Axis.Y:
                yCurrentColor = yColor;
                break;
            case Axis.Z:
                zCurrentColor = zColor;
                break;
        }
    }

    private void Start()
    {
        xAxis.material.color = Color.clear;
        yAxis.material.color = Color.clear;
        zAxis.material.color = Color.clear;
    }

    private void Update()
    {
        if (xCurrentColor.a > 0)
        {
            xCurrentColor.a -= Time.deltaTime / _fadeDuration;
            xAxis.material.color = xCurrentColor;
        }
        if (yCurrentColor.a > 0)
        {
            yCurrentColor.a -= Time.deltaTime / _fadeDuration;
            yAxis.material.color = yCurrentColor;
        }
        if (zCurrentColor.a > 0)
        {
            zCurrentColor.a -= Time.deltaTime / _fadeDuration;
            zAxis.material.color = zCurrentColor;
        }
    }
}
