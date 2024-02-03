using System;
using UnityEngine.UIElements;

public static class VisualElementsExt
{
    public static Button CreateButton(string text, Action onClick)
    {
        return new Button(onClick)
        {
            text = text,
        };
    }

    public static Foldout CreateFoldout(string text)
    {
        return new Foldout()
        {
            text = text,
        };
    }
}
