using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CubeTextureDataUI : VisualElement
{
    public event EventHandler<FaceArgs> OnFaceButtonPressed;

    private const float kBorderSize = 3f;
    private SerializedProperty _property;
    private TextureUvSelector _textureUvSelector;

    private CubeTextureData _target;

    public CubeTextureDataUI(SerializedProperty property)
    {
        _property = property;

        _target = (CubeTextureData)_property.boxedValue;

        var foldoutTextureData = VisualElementsExt.CreateFoldout(_property.displayName);

        var propertyTextureAtlas = _property.FindPropertyRelative(nameof(_target.textureAtlasData));
        var textureAtlasData = new PropertyField(propertyTextureAtlas);

        textureAtlasData.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnTextureChanged);
        textureAtlasData.RegisterCallback<ChangeEvent<float>>(OnCellSizeChanged);

        var frontFace = CreateFaceUV(_property, FaceOrientation.Front);
        var topFace = CreateFaceUV(_property, FaceOrientation.Top);
        var backFace = CreateFaceUV(_property, FaceOrientation.Back);
        var bottomFace = CreateFaceUV(_property, FaceOrientation.Bottom);
        var rightFace = CreateFaceUV(_property, FaceOrientation.Right);
        var leftFace = CreateFaceUV(_property, FaceOrientation.Left);

        foldoutTextureData.Add(textureAtlasData);

        foldoutTextureData.Add(frontFace);
        foldoutTextureData.Add(topFace);
        foldoutTextureData.Add(backFace);
        foldoutTextureData.Add(bottomFace);
        foldoutTextureData.Add(rightFace);
        foldoutTextureData.Add(leftFace);
        Add(foldoutTextureData);

        VisualElement container = new();

        _textureUvSelector = new();
        UpdateTexture(_target.textureAtlasData.texture);

        _textureUvSelector.AddManipulator(new PanManipulator());
        //_textureUvSelector.RegisterCallback<PointerDownEvent>(OnPointerClickTextureAtlas);
        DrawUVCoordiantes();
        container.Add(_textureUvSelector);
        Add(container);

    }

    private void OnPointerClickTextureAtlas(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        var coordinate = _textureUvSelector.CalculateImageCoordinates(evt.localPosition);

        bool success = _textureUvSelector.TryAddCoordinateBox(coordinate, Color.green, kBorderSize);
        
        if (!success)
            Debug.Log($"Failed to create Coordinate Box at: {coordinate}", _property.serializedObject.targetObject);
    }

    private void DrawUVCoordiantes()
    {
        if (_textureUvSelector == null)
            return;

        _textureUvSelector.ClearAllCoordinateBoxes();

        foreach (var uv in _target.faceUVs)
        {
            var maxRows = _textureUvSelector.GetMaxRows() - 1;
            var coordinate = new Vector2Int(uv.Value.x, maxRows - uv.Value.y);

            bool success = _textureUvSelector.TryAddCoordinateBox(coordinate, Color.blue, kBorderSize);

            if (!success)
                Debug.Log($"Failed to create Coordinate Box at: {uv.Value}", _property.serializedObject.targetObject);
        }
    }

    private void OnCellSizeChanged(ChangeEvent<float> evt)
    {
        _textureUvSelector.SetCellSize(evt.newValue);
        _target.textureAtlasData.cellSize = evt.newValue;
        _property.boxedValue = _target;
        _property.serializedObject.ApplyModifiedProperties();
        DrawUVCoordiantes();
    }

    private void OnTextureChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        var newValue = (Texture2D)evt.newValue;

        UpdateTexture(newValue);
    }

    private void UpdateTexture(Texture2D texture)
    {
        if (texture != null)
        {
            _textureUvSelector.SetTexture(texture);
            _target.textureAtlasData.texture = texture;
            _property.boxedValue = _target;
            _property.serializedObject.ApplyModifiedProperties();
            DrawUVCoordiantes();
        }
    }

    private VisualElement CreateFaceUV(SerializedProperty property, FaceOrientation faceOrientation)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;

        string orientationName = "";

        orientationName = faceOrientation switch
        {
            FaceOrientation.Front => nameof(_target.frontUV),
            FaceOrientation.Top => nameof(_target.topUV),
            FaceOrientation.Back => nameof(_target.backUV),
            FaceOrientation.Bottom => nameof(_target.bottomUV),
            FaceOrientation.Right => nameof(_target.rightUV),
            FaceOrientation.Left => nameof(_target.leftUV),
            _ => "Invalid"
        };

        var faceUV = new PropertyField(property.FindPropertyRelative(orientationName));
        faceUV.style.flexGrow = 1;

        var editButton = VisualElementsExt.CreateButton("Edit", () =>
        {
            OnFaceButtonPressed?.Invoke(this, new FaceArgs() { faceOrientation = faceOrientation });

            UpdateFaceCoordinateOnMouseClick(faceOrientation);
        });
        editButton.style.flexGrow = 1;

        container.Add(faceUV);
        container.Add(editButton);

        return container;
    }

    private void UpdateFaceCoordinateOnMouseClick(FaceOrientation faceOrientation)
    {
        _textureUvSelector.GetCoordinatesOnMouseClick((coordinate) =>
        {
            var maxRows = _textureUvSelector.GetMaxRows() - 1;
            var uv = new Vector2Int(coordinate.x, maxRows - coordinate.y);

            switch (faceOrientation)
            {
                case FaceOrientation.Front:
                    _target.frontUV = uv;
                    break;
                case FaceOrientation.Top:
                    _target.topUV = uv;
                    break;
                case FaceOrientation.Back:
                    _target.backUV = uv;
                    break;
                case FaceOrientation.Bottom:
                    _target.bottomUV = uv;
                    break;
                case FaceOrientation.Right:
                    _target.rightUV = uv;
                    break;
                case FaceOrientation.Left:
                    _target.leftUV = uv;
                    break;
                default:
                    break;
            }

            _property.boxedValue = _target;
            _property.serializedObject.ApplyModifiedProperties();
            DrawUVCoordiantes();
        });
    }
}

public class FaceUVArgs : EventArgs
{
    public FaceOrientation faceOrientation { get; set; }
    public Vector2IntField uv { get; set; }
}

public class FaceArgs : EventArgs
{
    public FaceOrientation faceOrientation { get; set; }
}
