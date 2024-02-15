using System;

[Flags]
public enum FaceOrientation
{
    Front = 1,
    Top = 2,
    Back = 4,
    Bottom = 8,
    Right = 16,
    Left = 32,
}
