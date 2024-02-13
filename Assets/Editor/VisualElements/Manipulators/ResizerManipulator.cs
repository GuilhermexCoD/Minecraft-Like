using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ResizerManipulator : MouseManipulator
{
    private Vector2 m_StartSize;
    private float m_Increment;
    private float m_Multiplier;

    public event EventHandler<ResizeArgs> onResize;

    public ResizerManipulator(Vector2 initialSize, float multiplier = 1f)
    {
        m_StartSize = initialSize;

        if (multiplier == 0f)
            m_Multiplier = 1f;
        else
            m_Multiplier = multiplier;

        activators.Add(new ManipulatorActivationFilter { modifiers = EventModifiers.Control });
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
            Debug.Log($"On Wheel {nameof(ResizerManipulator)} : Delta: {evt.delta}");

            m_Increment += evt.delta.y * m_Multiplier;

            Vector2 size = new Vector2(m_StartSize.x + m_Increment, m_StartSize.y + m_Increment);
            target.style.width = size.x;
            target.style.height = size.y;

            onResize?.Invoke(this, new ResizeArgs()
            {
                sizeDelta = new Vector2(m_StartSize.y + m_Increment, m_StartSize.x + m_Increment),
                increment = m_Increment,
                sizeMultiplier = (size / m_StartSize)
            });
        }
    }

    public void SetMultiplier(float multiplier)
    {
        m_Multiplier = multiplier;
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