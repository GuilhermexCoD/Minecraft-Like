using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static CubeMeshData;

[CustomPropertyDrawer(typeof(CubeTextureData))]
public class CubeTextureDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new CubeTextureDataUI(property);
    }
}