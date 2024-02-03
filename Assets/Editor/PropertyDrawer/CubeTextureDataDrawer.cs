using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static CubeMeshData;
using static UnityEngine.GraphicsBuffer;

[CustomPropertyDrawer(typeof(CubeTextureData))]
public class CubeTextureDataDrawer : PropertyDrawer
{
    public event EventHandler<FaceUVArgs> OnFaceUVChanged;
    public event EventHandler<FaceArgs> OnFaceButtonPressed;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var foldoutTextureData = VisualElementsExt.CreateFoldout(property.displayName);

        container.Add(foldoutTextureData);

        var target = (CubeTextureData)property.boxedValue;

        var textureAtlasId = new PropertyField(property.FindPropertyRelative(nameof(target.textureAtlasId)));
        var cellSize = new PropertyField(property.FindPropertyRelative(nameof(target.cellSize)));

        var frontFace = CreateFaceUV(property, FaceOrientation.Front);
        var topFace = CreateFaceUV(property, FaceOrientation.Top);
        var backFace = CreateFaceUV(property, FaceOrientation.Back);
        var bottomFace = CreateFaceUV(property, FaceOrientation.Bottom);
        var rightFace = CreateFaceUV(property, FaceOrientation.Right);
        var leftFace = CreateFaceUV(property, FaceOrientation.Left);

        foldoutTextureData.Add(textureAtlasId);
        foldoutTextureData.Add(cellSize);
        
        foldoutTextureData.Add(frontFace);
        foldoutTextureData.Add(topFace);
        foldoutTextureData.Add(backFace);
        foldoutTextureData.Add(bottomFace);
        foldoutTextureData.Add(rightFace);
        foldoutTextureData.Add(leftFace);

        return container;
    }

    private VisualElement CreateFaceUV(SerializedProperty property, FaceOrientation faceOrientation)
    {
        var target = (CubeTextureData)property.boxedValue;

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