
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true)]
public class BaseEditor : Editor
{
    private InspectorButtonAttribute[] _inspectorButtonAttributes;
    private bool _showMethods;

    private void OnEnable()
    {
        if (_inspectorButtonAttributes == null)
            _inspectorButtonAttributes = GetButtonAttributes(target).ToArray();

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CreateButtons(_inspectorButtonAttributes, targets, ref _showMethods);
    }

    public static void CreateButtons(InspectorButtonAttribute[] buttonsAttributes, UnityEngine.Object[] targets, ref bool showMethods)
    {
        if (buttonsAttributes != null)
        {
            GUILayout.Space(10);

            showMethods = EditorGUILayout.BeginFoldoutHeaderGroup(showMethods, " METHODS ");

            if (showMethods)
            {
                for (int i = 0; i < buttonsAttributes.Count(); i++)
                {
                    var buttonAtt = buttonsAttributes[i];

                    var methodInfo = buttonAtt.MethodInfo;

                    GUILayout.Space(10);
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();

                    if (buttonAtt.values == null)
                        buttonAtt.values = new object[methodInfo.GetParameters().Length];

                    if (GUILayout.Button(buttonAtt.Label))
                    {
                        object result = null;

                        foreach (var target in targets)
                            result = methodInfo.Invoke(target, buttonAtt.values);

                        if (result != null)
                            Debug.Log($"Button {buttonAtt.Label}: {result}");

                    }

                    for (int j = 0; j < methodInfo.GetParameters().Length; j++)
                    {
                        var param = methodInfo.GetParameters()[j];
                        var type = param.ParameterType;
                        if (type == typeof(Single))
                        {
                            if (buttonAtt.values[j] == null)
                                buttonAtt.values[j] = 0f;

                            buttonAtt.values[j] = EditorGUILayout.FloatField(param.Name, (float)buttonAtt.values[j]);
                        }

                        if (type.BaseType == typeof(Enum))
                        {
                            if (buttonAtt.values[j] == null)
                                buttonAtt.values[j] = Enum.GetValues(type).GetValue(0);

                            buttonAtt.values[j] = EditorGUILayout.EnumPopup(param.Name, (Enum)buttonAtt.values[j]);
                        }
                    }

                    GUILayout.EndVertical();

                    buttonAtt.GetType();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

    }

    public static IEnumerable<InspectorButtonAttribute> GetButtonAttributes(UnityEngine.Object obj)
    {

        var attributes = obj.GetType().GetMethods().Select(m =>
        {
            var att = m.GetCustomAttribute<InspectorButtonAttribute>();
            if (att != null)
            {
                att.MethodInfo = m;

                if (string.IsNullOrWhiteSpace(att.Label))
                    att.Label = m.Name;
            }

            return att;
        }).Where(a => a != null);

        return attributes;
    }

}