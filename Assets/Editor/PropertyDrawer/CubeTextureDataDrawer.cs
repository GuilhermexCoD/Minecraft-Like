using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(CubeTextureData))]
public class CubeTextureDataDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new CubeTextureDataUI(property);
    }
}