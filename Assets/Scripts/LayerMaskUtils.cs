using UnityEngine;

public static class LayerMaskUtils
{
    public static bool Contains(this LayerMask layerMask, int layer)
    {
        int layerToBit = LayerToBit(layer);

        return (layerMask & layerToBit) == layerToBit;
    }

    public static int Add(this LayerMask layerMask, int layerToAdd)
    {
        return layerMask |= (1 << layerToAdd);
    }

    public static int Remove(this LayerMask layerMask, int layerToRemove)
    {
        return layerMask &= ~(1 << layerToRemove);
    }

    public static int LayerToBit(int layer)
    {
        return 1 << layer;
    }
}