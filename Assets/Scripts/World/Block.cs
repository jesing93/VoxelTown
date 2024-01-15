using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newBlock", menuName = "Custom/Block Class")]
public class Block : ScriptableObject
{
    public BlockType blockType;
    [Tooltip("Side, top, bottom (Missing materials will use the last material in the array)")]
    //public Texture2D[] blockTextures; //Side, top, bottom
    public Vector2[] atlasCoordinate; //Side, top, bottom
}

public enum BlockType 
{
    dirt,
    stone,
    bedrock
}
