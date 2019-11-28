using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CaveErosion
{
    public static LevelTile[,] map;

    public static float kd = 1, kp = .1f;
    public static int r = 2, sr = 1;

    static Dictionary<Coord, float> weights;

    static LevelTile GetTileAt(Coord coord)
    {
        if (coord.tileX >= 0 && coord.tileX < map.GetLength(0) && coord.tileY >= 0 && coord.tileY < map.GetLength(1))
            return map[coord.tileX, coord.tileY];
        return new LevelTile(LevelTileType.Nothing);
    }

    public static Coord Raycast(Coord start, Coord end, LevelTileType mask)
    {
        int deltaX = end.tileX - start.tileX;

        if (deltaX == 0) throw new CaveErosionException("Raycasting encountered deltaX 0.");

        for (int x = start.tileX; x != end.tileX; x += (int)Mathf.Sign(deltaX))
        {
            // TODO: Diagonal bias, try out different raycasting approaches
            int y = Mathf.FloorToInt(MathExtensions.Lerp(start.tileX, end.tileX, start.tileY, end.tileY, x));
            if (map[x, y].type == mask) return new Coord(x, y);
        }

        if (map[end.tileX, end.tileY].type == mask) return new Coord(end.tileX, end.tileY);

        throw new CaveErosionException();
    }

    static Coord PickRandomErosionCoord(List<Coord> coords)
    {
        float totalWeight = coords.Sum(e => weights[e]);

        float randomNum = (float)GameManager.Instance.rng.NextDouble() * totalWeight;

        Coord selectedCoord = new Coord(-1, -1);
        foreach (Coord coord in coords)
        {
            if (randomNum < weights[coord])
            {
                selectedCoord = coord;
                break;
            }

            randomNum -= weights[coord];
        }

        return selectedCoord;
    }

    static HashSet<Coord> GetPotentialCoords(Coord end, Coord erosionCoord)
    {
        HashSet<Coord> result = new HashSet<Coord>();

        for (int x = -r + erosionCoord.tileX; x < r + erosionCoord.tileX; x++)
            for (int y = -r + erosionCoord.tileY; y < r + erosionCoord.tileY; y++)
            {
                if (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1))
                {
                    if (x != end.tileX && y != end.tileY)
                    {
                        Coord c;
                        try
                        {
                            c = Raycast(end, new Coord(x, y), LevelTileType.Nothing);
                        }
                        catch (CaveErosionException)
                        {
                            continue;
                        }

                        if (GetTileAt(new Coord(c.tileX - 1, c.tileY)).type == LevelTileType.Wall) result.Add(new Coord(c.tileX - 1, c.tileY));
                        if (GetTileAt(new Coord(c.tileX + 1, c.tileY)).type == LevelTileType.Wall) result.Add(new Coord(c.tileX + 1, c.tileY));
                        if (GetTileAt(new Coord(c.tileX, c.tileY - 1)).type == LevelTileType.Wall) result.Add(new Coord(c.tileX, c.tileY - 1));
                        if (GetTileAt(new Coord(c.tileX, c.tileY + 1)).type == LevelTileType.Wall) result.Add(new Coord(c.tileX, c.tileY + 1));
                    }
                }
                    
            }
        return result;
    }

    public static void Erode(LevelTile[,] _map, Coord start, Coord end)
    {
        map = _map;

        weights = new Dictionary<Coord, float>();

        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new LevelTile(LevelTileType.Wall);

        Coord currentCoord = start;
        map[currentCoord.tileX, currentCoord.tileY].type = LevelTileType.Nothing;

        int iterations = 0;
        bool complete;

        do
        {
            if (currentCoord == end) return;
            if (GetTileAt(end).type == LevelTileType.Nothing) return;

            List<Coord> potentialCoords = GetPotentialCoords(end, currentCoord).ToList();

            complete = false;
            for (int i = 0; i < potentialCoords.Count; i++)
            {
                if (potentialCoords[i] == end)
                {
                    complete = true;
                    break;
                }
                weights[potentialCoords[i]] = Mathf.Pow(kd / MathExtensions.SqrDistance(end, potentialCoords[i]), kp);
            }

            if (!complete)
            {
                Coord toDelete = PickRandomErosionCoord(potentialCoords);
                if (toDelete.tileX == -1) return;

                for (int x = -sr; x < sr; x++)
                    for (int y = -sr; y < sr; y++)
                        if (x * x + y * y <= sr * sr)
                            if (x + toDelete.tileX >= 0 && x + toDelete.tileX < map.GetLength(0) && y + toDelete.tileY >= 0 && y + toDelete.tileY < map.GetLength(1))
                                map[x + toDelete.tileX, y + toDelete.tileY].type = LevelTileType.Nothing;

                currentCoord = toDelete;
            }
            else
                map[end.tileX, end.tileY].type = LevelTileType.Nothing;
            iterations++;

        } while (!complete);
    }
}
