using UnityEngine;
// Rooms are a 2D array of Width and Height Tiles. These will be stored in another array called Levels. 
public class Room
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2 Position { get; set; }
    public float TileSize { get; set; }
    private Tile[,] tiles;

    // tileSize might look at a little weird here, but it is the scale that Unity should build tiles at. So, a tile at address 3,1 will be represented in Unity units at 3,1.
    // Set tileSize to 2f, and that tile will be at 6,2. 
    public Room(int width, int height, Vector2 position, float tileSize = 1f)
    {
        Width = width;
        Height = height;
        Position = position;
        TileSize = tileSize;
        tiles = new Tile[width, height];
        InitializeTiles();
    }
    // Function that creates tiles at each coordinate for world generation.
    // Has been edited to use local coordinates.
    private void InitializeTiles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(x, y);
                tiles[x, y].parentRoom = this;
            }
        }
    }
    // Getter that returns the x and y of valid tiles.
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return null;
        return tiles[x, y];
    }
    // Feature setter for world generation.
    public void SetFeature(int x, int y, int featureID)
    {
        Tile tile = GetTile(x, y);
        if (tile != null)
        {
            tile.FeatureID = featureID;
        }
    }

    // Marks tiles as walls for world generation.
    public void SetWall(int x, int y)
    {
        Tile tile = GetTile(x, y);
        if (tile != null)
        {
            tile.IsWall = true;
            tile.IsFloor = false;
        }
    }

    // Sample Room generator.
    public void GenerateBasicRoom()
    {
        // Sets perimeter tiles to be walls, not floors.
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                {
                    SetWall(x, y);
                    SetFeature(x, y, FeatureID.WALL);
                }
                else
                {
                    Tile t = GetTile(x, y);
                    t.IsWall = false;
                    t.IsFloor = true;
                }
            }
        }
    }

    // Retrieves Tiles with Features (Tiles where FeatureID != 0).
    public Tile[] GetTilesWithFeatures()
    {
        System.Collections.Generic.List<Tile> result = new System.Collections.Generic.List<Tile>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y].FeatureID != 0)
                {
                    result.Add(tiles[x, y]);
                }
            }
        }
        return result.ToArray();
    }
    // Not sure how necessary these are, but included functions to convert tiles to World Position based off scale (TileSize) with Vector2 inputs and specific coordinates.
    public Vector2 TileToWorldPosition(int x, int y)
    {
        return new Vector2(
            (Position.x + x) * TileSize,
            (Position.y + y) * TileSize
        );
    }

    public Vector2 TileToWorldPosition(Vector2Int tilePos)
    {
        return new Vector2(
            tilePos.x * TileSize,
            tilePos.y * TileSize
        );
    }
}
