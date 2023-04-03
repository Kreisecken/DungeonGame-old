using DungeonGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Dungeon
{
    // rough prototyping

    public class Dungeon : MonoBehaviour
    {
        public DungeonAsset asset;

        public List<DungeonRoom> rooms;
    
        public void GenerateDungeon(DungeonAsset asset, Seed seed, GameObject parent)
        {
            // get rooms from asset
            // -> keep in mind that some rooms are going to be locked
            // -> theirfor some key elements could be out of reach
            // -> after the following step, so this part needs to be rethought
            // distribute them evenly
            // create delaunay triangulation
            // find minimum spanning tree
            // choose random edges
            // pathfind hallways
            // finally place tiles and do final touches 
        }
    }

    public class DungeonRoom : MonoBehaviour
    {
        // this class could handle DungeonRoom events
        // like OnPlayerEnter() or OnPlayerMove()
        // this could be usefull for example closing the doors
        // when the player enters and opening them again
        // after all monsters die
    }
}