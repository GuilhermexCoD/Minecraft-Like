using UnityEngine;

[CreateAssetMenu(fileName = "blockData", menuName = "Block Data")]
public class BlockData : ScriptableObject
{

    public string blockName;

    public CubeTextureData textureData;
}
