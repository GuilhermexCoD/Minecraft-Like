using UnityEngine;
using UnityEngine.UIElements;

public class PanManipulator : MouseManipulator
{
    private bool _active;
    private Vector2 _startPosition;
    private Vector2 _lastPosition;

    public PanManipulator()
    {
        _active = false;
        _startPosition = Vector2.zero;
        _lastPosition = Vector2.zero;
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.MiddleMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (!_active || !target.HasMouseCapture())
            return;

        Vector2 delta = evt.localMousePosition - _startPosition;


        if (target.layout.x + delta.x <= -target.layout.width * 0.9f || target.layout.x + delta.x >= target.parent.layout.xMax * 0.9f)
            delta.x = 0;

        if (target.layout.y + delta.y <= 0 || target.layout.y + delta.y >= target.parent.layout.yMax * 0.9f)
            delta.y = 0;

        _lastPosition += delta;

        target.style.left = _lastPosition.x;
        target.style.top = _lastPosition.y;

        evt.StopPropagation();
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (!_active || !target.HasMouseCapture() || !CanStopManipulation(evt))
            return;

        _active = false;
        target.ReleaseMouse();
        evt.StopPropagation();
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (_active)
        {
            evt.StopImmediatePropagation();
            return;
        }

        if (CanStartManipulation(evt))
        {
            _startPosition = evt.localMousePosition;
            _active = true;
            target.CaptureMouse();
            evt.StopImmediatePropagation();
        }
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
    }

}
