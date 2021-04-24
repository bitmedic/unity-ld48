using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMouseInput_MG : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemapFactory;
    [SerializeField]
    Tilemap tilemapTerrain;

    [SerializeField]
    IsometricRuleTile tileToPlace;

    [SerializeField]
    Vector2 mousePosition;
    [SerializeField]
    Vector3 worldPosition;
    [SerializeField]
    Vector3Int cell;
    [SerializeField]
    TileBase terrainTile;
    [SerializeField]
    TileBase factoryTile;


    bool tryPlace = false;

    // Update is called once per frame
    void Update()
    {
        // go into placing mode as long as mouse button 0 is down
        if (Input.GetMouseButtonDown(0))
        {
            tryPlace = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            tryPlace = false;
        }

        mousePosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        cell = tilemapTerrain.WorldToCell(worldPosition);
        cell.x--;
        cell.y--;
        cell.z = 0;

        tilemapFactory.ClearAllEditorPreviewTiles();

        if (tilemapTerrain.HasTile(cell))
        { 
            // if the tile exists on the asteroid
            terrainTile = tilemapTerrain.GetTile(cell);

            if (!tilemapFactory.HasTile(cell))
            {
                // if nothing is placed here on the factory layer
                if (tryPlace)
                {
                    tilemapFactory.SetTile(cell, this.tileToPlace);
                }
                else
                {
                    tilemapFactory.SetEditorPreviewTile(cell, this.tileToPlace);
                }
            }
        }
    }
}
