using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridExtensions
{
    public static Vector3Int CubeToGrid(this Grid grid, Vector3Int cubeCoords)
    {
        int x = cubeCoords.x - (cubeCoords.y - (cubeCoords.y & 1)) / 2;
        int y = cubeCoords.y;
        return new Vector3Int(x, y, 0);
    }

    public static Vector3Int GridToCube (this Grid grid, Vector3Int gridCoords)
    {
        int q = gridCoords.x + (gridCoords.y - (gridCoords.y & 1)) / 2;
        int r = gridCoords.y;
        return new Vector3Int(q, r, -q - r);
    }
}
