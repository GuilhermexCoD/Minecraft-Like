
using System;
using System.Reflection;

/// <summary>
/// This attribute creates a button on the inspector
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : Attribute
{
    public string Label { get; set; }

    public MethodInfo MethodInfo { get; set; }

    public object[] values { get; set; }

    public InspectorButtonAttribute(){ }

    public InspectorButtonAttribute(string name)
    {
        Label = name;
    }
}