using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TextureUvSelector : Image
{
    private Texture2D _texture;
    private float _cellSize;
    private Action<Vector2Int> _coordinatesCallback;

    private Dictionary<Vector2Int, Box> _coordinateBoxes = new();

    public TextureUvSelector(Texture2D texture, float cellSize = 16f)
    {
        _texture = texture;
        _cellSize = cellSize;

        style.alignSelf = Align.Center;
        style.height = _texture.height;
        style.width = _texture.width;
        image = _texture;
    }

    public void SetCellSize(float cellSize)
    {
        _cellSize = cellSize;

        //TODO Redraw CoordinateBoxes
    }

    public void GetCoordinatesOnMouseClick(Action<Vector2Int> coordinatesCallback)
    {
        Debug.Log("Waiting for input");

        _coordinatesCallback = coordinatesCallback;

        RegisterCallback<PointerDownEvent>(OnSelectedCoordinate);
    }

    private void OnSelectedCoordinate(PointerDownEvent evt)
    {
        var coordinate = CalculateImageCoordinates(evt.localPosition);
        _coordinatesCallback?.Invoke(coordinate);

        UnregisterCallback<PointerDownEvent>(OnSelectedCoordinate);
    }

    /// <summary>
    /// Adds a CoordnateBox based on local Mouse position with given color and border size
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="color"></param>
    /// <param name="borderSize"></param>
    /// <returns></returns>
    public bool TryAddCoordinateBox(Vector2 mousePos, Color color, float borderSize = 0.1f)
    {
        var coordinate = CalculateImageCoordinates(mousePos);
        return TryAddCoordinateBox(coordinate, color, borderSize);
    }

    /// <summary>
    /// Adds a CoordnateBox based on uv coordinates with given color and border size
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="color"></param>
    /// <param name="borderSize"></param>
    /// <returns></returns>
    public bool TryAddCoordinateBox(Vector2Int coordinate, Color color, float borderSize = 0.1f)
    {
        var box = CreateBox(coordinate, color, borderSize);
        if (_coordinateBoxes.TryAdd(coordinate, box))
        {
            Add(box);
            return true;
        }

        return false;
    }

    public bool RemoveCoordinateBox(Vector2Int coordinate)
    {
        bool success = _coordinateBoxes.Remove(coordinate, out Box box);

        if (success)
            Remove(box);

        return success;
    }

    public void UpdateCoordinateBox(Vector2Int coordinate, Color color, float borderSize = 0.1f)
    {
        var box = _coordinateBoxes[coordinate];
        UpdateCoordinateBox(box, color, borderSize);
    }

    public void ClearAllCoordinateBoxes()
    {
        for (int i = _coordinateBoxes.Count - 1; i >= 0; i--)
            RemoveCoordinateBox(_coordinateBoxes.ElementAt(i).Key);
    }

    private void UpdateCoordinateBox(Box box, Color color, float borderSize = 0.1f)
    {
        box.style.borderBottomWidth = borderSize;
        box.style.borderLeftWidth = borderSize;
        box.style.borderRightWidth = borderSize;
        box.style.borderTopWidth = borderSize;

        box.style.borderBottomColor = color;
        box.style.borderLeftColor = color;
        box.style.borderRightColor = color;
        box.style.borderTopColor = color;
    }

    private Box CreateBox(Vector2Int coordinate, Color borderColor, float borderSize = 0.1f)
    {
        Box box = new();
        Vector2 pos = new();

        pos.x = coordinate.x * _cellSize;
        pos.y = coordinate.y * _cellSize;

        box.transform.position = pos;
        box.style.height = _cellSize;
        box.style.width = _cellSize;
        box.style.position = Position.Absolute;

        UpdateCoordinateBox(box, borderColor, borderSize);

        return box;
    }

    public Vector2Int CalculateImageCoordinates(Vector2 pos)
    {
        Vector2Int uvCoordinates = new();
        uvCoordinates.x = Mathf.FloorToInt(pos.x / _cellSize);
        uvCoordinates.y = Mathf.FloorToInt(pos.y / _cellSize);

        Debug.Log($"Column: {uvCoordinates.x} Row: {uvCoordinates.y}");

        return uvCoordinates;
    }

    public int GetMaxRows()
    {
        return Mathf.FloorToInt(_texture.height / _cellSize);
    }

    public int GetMaxColumns()
    {
        return Mathf.FloorToInt(_texture.width / _cellSize);
    }


}
