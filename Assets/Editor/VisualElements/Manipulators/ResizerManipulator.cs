using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ResizerManipulator : MouseManipulator
{
    private Vector2 _startSize;
    private float _increment;
    private float _multiplier;

    public event EventHandler<ResizeArgs> onResize;

    public ResizerManipulator(float multiplier = 1f)
    {
        _multiplier = multiplier;

        activators.Add(new ManipulatorActivationFilter { modifiers = EventModifiers.Control });
    }

    public ResizerManipulator(Vector2 initialSize, float multiplier = 1f)
    {
        _startSize = initialSize;

        if (multiplier == 0f)
            _multiplier = 1f;
        else
            _multiplier = multiplier;

        activators.Add(new ManipulatorActivationFilter { modifiers = EventModifiers.Control });
    }

    public void SetInitialSize(Vector2 initialSize)
    {
        _startSize = initialSize;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<WheelEvent>(OnWheelEvent);
    }

    private void OnWheelEvent(WheelEvent evt)
    {
        if (CanStartManipulation(evt))
        {
            evt.StopImmediatePropagation();

            _increment += evt.delta.y * _multiplier;

            Vector2 size = new Vector2(_startSize.x + _increment, _startSize.y + _increment);
            target.style.width = size.x;
            target.style.height = size.y;

            onResize?.Invoke(this, new ResizeArgs()
            {
                sizeDelta = new Vector2(_startSize.y + _increment, _startSize.x + _increment),
                increment = _increment,
                sizeMultiplier = (size / _startSize)
            });
        }
    }

    public void SetMultiplier(float multiplier)
    {
        _multiplier = multiplier;
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<WheelEvent>(OnWheelEvent);
    }
}

public class ResizeArgs
{
    public Vector2 sizeDelta { get; set; }
    public float increment { get; set; }
    public Vector2 sizeMultiplier { get; set; }
}