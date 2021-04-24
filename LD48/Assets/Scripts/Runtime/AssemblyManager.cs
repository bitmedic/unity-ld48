using System.Collections;
using System.Collections.Generic;
using LD48;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AssemblyManager : MonoBehaviour
{
    public Tilemap tilemap;

    private AssemblyLine ass;

    private void Start()
    {
        CreateModel();
    }

    public void CreateModel()
    {
        ass = new AssemblyLine();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
            }
        }
    }

    public void Tick()
    {
    }
}