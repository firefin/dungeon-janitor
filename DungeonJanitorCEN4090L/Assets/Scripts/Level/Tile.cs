using System.Security.Cryptography.X509Certificates;
using UnityEngine;

// Tile is a class that is used in level generation. Tiles are effectively chunks used for level generation.
// They may or may not contain a feature. 
public class Tile
{
    public Vector2Int Position { get; set; }

    // Logic mostly for walls
    public bool IsFloor { get; set; }
    public bool IsWall { get; set; }

    // Retrieves ID of the Feature on the tile, returns 0 on no feature.
    public int FeatureID { get; set; }
    public Room parentRoom;
    public Tile(int x, int y)
    {
        Position = new Vector2Int(x, y);
        IsFloor = true;
        IsWall = false;
        FeatureID = 0;
    }

}