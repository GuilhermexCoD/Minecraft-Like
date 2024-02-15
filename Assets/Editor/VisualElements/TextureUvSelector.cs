using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TextureUvSelector : Image
{
    private Texture2D _texture;
    private float _cellSize;
    private Action<Vector2Int> _coordinatesCallback;

    private Dictionary<Vector2Int, SelectionBox> _coordinateBoxes = new();
    private ResizerManipulator _resizerManipulator;
    private Vector2 _sizeMultiplier = Vector2.one;

    public TextureUvSelector() : this(null , 16f)
    {

    }

    public TextureUvSelector(Texture2D texture, float cellSize = 16f)
    {
        _texture = texture;
        _cellSize = cellSize;

        style.alignSelf = Align.Center;

        SetTexture(texture);
        _sizeMultiplier = Vector2.one;

        _resizerManipulator = new ResizerManipulator(4f);
        _resizerManipulator.onResize += OnResizeChange;
        this.AddManipulator(_resizerManipulator);
    }

    private void OnResizeChange(object sender, ResizeArgs e)
    {
        _sizeMultiplier = e.sizeMultiplier;

        foreach (var keyValue in _coordinateBoxes)
            keyValue.Value.CellSize = GetZoomCellSize();

    }

    public void SetTexture(Texture2D texture)
    {
        if (texture == null)
            return;

        _texture = texture;

        style.height = _texture.height;
        style.width = _texture.width;

        _resizerManipulator.SetInitialSize(new Vector2(_texture.width, _texture.height));

        image = _texture;
        
        MarkDirtyRepaint();
        //TODO Redraw CoordinateBoxes
    }

    public void SetCellSize(float cellSize)
    {
        _cellSize = cellSize;
        MarkDirtyRepaint();
        //TODO Redraw CoordinateBoxes
    }

    public void GetCoordinatesOnMouseClick(Action<Vector2Int> coordinatesCallback)
    {
        CancelGetCoordinatesOnMouseClick();

        Debug.Log("Waiting for input");

        _coordinatesCallback = coordinatesCallback;

        RegisterCallback<PointerDownEvent>(OnSelectedCoordinate);
    }

    public void CancelGetCoordinatesOnMouseClick()
    {
        if (_coordinatesCallback != null)
        {
            UnregisterCallback<PointerDownEvent>(OnSelectedCoordinate);
            _coordinatesCallback = null;
        }
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
        if (_texture == null)
            return false;

        SelectionBox box = new(coordinate, color, GetZoomCellSize(), borderSize);

        if (_coordinateBoxes.TryAdd(coordinate, box))
        {
            Add(box);
            return true;
        }

        return false;
    }

    public bool RemoveCoordinateBox(Vector2Int coordinate)
    {
        bool success = _coordinateBoxes.Remove(coordinate, out SelectionBox box);

        if (success)
            Remove(box);

        return success;
    }

    public void UpdateCoordinateBox(Vector2Int coordinate, Color color, float borderSize = 0.1f)
    {
        var box = _coordinateBoxes[coordinate];
        box.BorderColor = color;
        box.BorderSize = borderSize;
    }

    public void ClearAllCoordinateBoxes()
    {
        for (int i = _coordinateBoxes.Count - 1; i >= 0; i--)
            RemoveCoordinateBox(_coordinateBoxes.ElementAt(i).Key);
    }

    private Vector2 GetZoomCellSize()
    {
        return _sizeMultiplier * _cellSize;
    }

    public Vector2Int CalculateImageCoordinates(Vector2 pos)
    {
        Vector2Int uvCoordinates = new();
        var zoomCellSize = GetZoomCellSize();
        uvCoordinates.x = Mathf.FloorToInt(pos.x / zoomCellSize.x);
        uvCoordinates.y = Mathf.FloorToInt(pos.y / zoomCellSize.y);

        return uvCoordinates;
    }

    public int GetMaxRows()
    {
        if (_texture == null)
            return 0;

        return Mathf.FloorToInt(_texture.height / _cellSize);
    }

    public int GetMaxColumns()
    {
        if (_texture == null)
            return 0;

        return Mathf.FloorToInt(_texture.width / _cellSize);
    }


}
