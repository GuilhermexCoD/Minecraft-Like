using UnityEngine;
using UnityEngine.UIElements;

public class SelectionBox : Box
{
    private Vector2Int _coordinate;

    private Vector2 _cellSize;
    public Vector2 CellSize
    {
        get => _cellSize;
        set
        {
            _cellSize = value;
            UpdateCellSize();
        }
    }

    private Color _borderColor;
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            _borderColor = value;
            UpdateBorderColor();
        }
    }

    private float _borderSize;
    public float BorderSize
    {
        get => _borderSize;
        set
        {
            _borderSize = value;
            UpdateBorderSize();
        }
    }

    public SelectionBox(Vector2Int coordinate, Color borderColor, Vector2 cellSize, float borderSize = 0.1f)
    {
        _coordinate = coordinate;
        _cellSize = cellSize;

        BorderColor = borderColor;
        BorderSize = borderSize;

        UpdateCellSize();
    }

    private void UpdateCellSize()
    {
        Vector2 pos = new();
        pos.x = _coordinate.x * _cellSize.x;
        pos.y = _coordinate.y * _cellSize.y;

        transform.position = pos;
        style.width = _cellSize.x;
        style.height = _cellSize.y;
        style.position = Position.Absolute;
    }

    private void UpdateBorderColor()
    {
        style.borderBottomColor = _borderColor;
        style.borderLeftColor = _borderColor;
        style.borderRightColor = _borderColor;
        style.borderTopColor = _borderColor;
    }

    private void UpdateBorderSize()
    {
        style.borderBottomWidth = _borderSize;
        style.borderLeftWidth = _borderSize;
        style.borderRightWidth = _borderSize;
        style.borderTopWidth = _borderSize;
    }
}
