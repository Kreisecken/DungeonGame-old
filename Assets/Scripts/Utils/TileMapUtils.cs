using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGame.Utils
{
    public static class TileMapUtils
    {
        public static IEnumerable<(Vector3Int position, TileBase tileBase)> GetTiles(this Tilemap tileMap)
        {
            foreach (var pos in tileMap.cellBounds.allPositionsWithin)
            {
                if (tileMap.HasTile(pos))
                
                yield return (pos, tileMap.GetTile(pos));
            }
        }
    }
}