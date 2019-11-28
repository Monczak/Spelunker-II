using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Room
{
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Coord> innerTiles;

    public int roomSize;

    public int mineralAbundance;

    public Room() { }

    public Room(List<Coord> roomTiles, LevelTile[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;

        edgeTiles = new List<Coord>();
        innerTiles = new List<Coord>();
        foreach (Coord tile in tiles)
        {
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x == tile.tileX || y == tile.tileY)
                        if (GameManager.Instance.mapGenerator.IsInMapRange(x, y))
                            if (map[x, y].type == LevelTileType.Wall)
                                edgeTiles.Add(tile);
                            else
                                innerTiles.Add(tile);
                }
            }
        }
    }
}
