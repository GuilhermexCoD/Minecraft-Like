using System;
using UnityEngine;

public static class BitmaskUtil
{
    public static bool Contains(this uint bitmask, int layer)
    {
        int layerToBit = LayerToBit(layer);

        return (bitmask & layerToBit) == layerToBit;
    }

    public static uint Add(this uint bitmask, int layerToAdd)
    {
        return bitmask |= ((uint)1 << layerToAdd);
    }

    public static uint Remove(this uint bitmask, int layerToRemove)
    {
        return bitmask &= ~((uint)1 << layerToRemove);
    }

    public static int LayerToBit(int layer)
    {
        return 1 << layer;
    }

    public static void DebugConsole(this uint bitmask)
    {
        Debug.Log(Convert.ToString(bitmask, 2).PadLeft(8, '0'));
    }
}
