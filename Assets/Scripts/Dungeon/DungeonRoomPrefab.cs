using DungeonGame.Utils;
using DungeonGame.Utils.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace DungeonGame.Dungeons
{
    public class DungeonRoomPrefab : MonoBehaviour
    {
        public static Dictionary<string, (Vector3Int[] positions, TileBase[] tilebases)> TileMapData = new();

        public Collider2D collider2d;

        public Tilemap tileMap;

        public void Init(string identifier)
        {
            if (tileMap == null) return;

            //if (DungeonRoomPrefab.TileMapData.ContainsKey(identifier)) return;

            var tiles = tileMap.GetTiles();
            
            //Debug.Log($"Adding {tiles.Count()} tiles to the TileMapData dictionary");

            DungeonRoomPrefab.TileMapData[identifier] = (tiles.Select((tile) => tile.position).ToArray(), tiles.Select((tile) => tile.tileBase).ToArray());
        }
    }
}