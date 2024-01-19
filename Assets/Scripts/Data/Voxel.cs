using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel
{
    public BlockType blockType;

    public bool isSolid
    {
        get
        {
            return blockType != BlockType.air;
        }
    }
}
