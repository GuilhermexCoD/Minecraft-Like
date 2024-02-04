using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static CubeMeshData;

public class CubeTextureDataUI : VisualElement
{
    public event EventHandler<FaceArgs> OnFaceButtonPressed;

    private SerializedProperty _property;
    private CubeTextureData target => (CubeTextureData)_property.boxedValue;

    public CubeTextureDataUI(SerializedProperty property)
    {
        _property = property;
        
        var foldoutTextureData = VisualElementsExt.CreateFoldout(_property.displayName);

        Add(foldoutTextureData);

        var textureAtlasId = new PropertyField(_property.FindPropertyRelative(nameof(target.textureAtlasId)));
        var cellSize = new PropertyField(_property.FindPropertyRelative(nameof(target.cellSize)));

        var frontFace = CreateFaceUV(_property, FaceOrientation.Front);
        var topFace = CreateFaceUV(_property, FaceOrientation.Top);
        var backFace = CreateFaceUV(_property, FaceOrientation.Back);
        var bottomFace = CreateFaceUV(_property, FaceOrientation.Bottom);
        var rightFace = CreateFaceUV(_property, FaceOrientation.Right);
        var leftFace = CreateFaceUV(_property, FaceOrientation.Left);

        foldoutTextureData.Add(textureAtlasId);
        foldoutTextureData.Add(cellSize);

        foldoutTextureData.Add(frontFace);
        foldoutTextureData.Add(topFace);
        foldoutTextureData.Add(backFace);
        foldoutTextureData.Add(bottomFace);
        foldoutTextureData.Add(rightFace);
        foldoutTextureData.Add(leftFace);
    }

    private VisualElement CreateFaceUV(SerializedProperty property, FaceOrientation faceOrientation)
    {
        var container = new VisualElement();
        string orientationName = "";

        orientationName = faceOrientation switch
        {
            FaceOrientation.Front => nameof(target.frontUV),
            FaceOrientation.Top => nameof(target.topUV),
            FaceOrientation.Back => nameof(target.backUV),
            FaceOrientation.Bottom => nameof(target.bottomUV),
            FaceOrientation.Right => nameof(target.rightUV),
            FaceOrientation.Left => nameof(target.leftUV),
            _ => "Invalid"
        };

        var faceUV = new PropertyField(property.FindPropertyRelative(orientationName));
        var editButton = VisualElementsExt.CreateButton("Edit", () =>
        {
            OnFaceButtonPressed?.Invoke(this, new FaceArgs() { faceOrientation = faceOrientation });
        });

        container.Add(faceUV);
        container.Add(editButton);

        return container;
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
