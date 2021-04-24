using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace LD48
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField]
        Tilemap tilemapFactory;
        [SerializeField]
        Tilemap tilemapTerrain;

        [SerializeField]
        List<Button> buildButtons;

        [SerializeField]
        TileBase emptyFillerTile;

        TileBase tileToPlace;
        int buildingWidth;
        int buildingHeight;


        Vector2 mousePosition;
        Vector3 worldPosition;
        Vector3Int cell;
        
        bool clickedToPlace = false;

        // Update is called once per frame
        void Update()
        {
            // go into placing mode as long as mouse button 0 is down
            if (Input.GetMouseButtonDown(0))
            {
                clickedToPlace = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                clickedToPlace = false;
            }

            // if right click -> end building mode
            if (Input.GetMouseButtonDown(1))
            {
                tileToPlace = null;
                clickedToPlace = false;
                this.ResetAllBuildButtons();
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
                //terrainTile = tilemapTerrain.GetTile(cell);

                if (this.CheckCanBuild(cell))
                {
                    // if nothing is placed here on the factory layer
                    if (clickedToPlace)
                    {
                        this.BuildBuildting(this.tileToPlace, cell);
                    }
                    else if (tileToPlace != null)
                    {
                        tilemapFactory.SetEditorPreviewTile(cell, this.tileToPlace);
                    }
                    // else do nothing for now
                }
            }
        }

        public void SetBuildingSelected(BuildingSizeSO buildingSO)
        {
            this.ResetAllBuildButtons(); // new button was selected
            this.tileToPlace = buildingSO.tile;
            this.buildingWidth = buildingSO.width;
            this.buildingHeight = buildingSO.height;
        }

        private bool CheckCanBuild(Vector3Int cell)
        {
            for (int x = 0; x < this.buildingWidth; x++)
            {
                for (int y = 0; y < this.buildingHeight; y++)
                {
                    Vector3Int currentCell = new Vector3Int(cell.x + x, cell.y + y, cell.z);


                    // check if cell does has a terrain field
                    if (!tilemapTerrain.HasTile(currentCell))
                    {
                        return false;
                    }

                    // check if cell has no factory on it
                    if (tilemapFactory.HasTile(currentCell))
                    {
                        return false;
                    }
                }
            }

            return true; // if no other object was found
        }

        private void BuildBuildting(TileBase tileToPlace, Vector3Int cellLocation)
        {
            tilemapFactory.SetTile(cellLocation, this.tileToPlace);

            // if the building is larger than 1x1, then place something at the other positions
            for (int x = 0; x < this.buildingWidth; x++)
            {
                for (int y = 0; y < this.buildingHeight; y++)
                {
                    if (x != 0 || y != 0) // if not the base spawn cell
                    {
                        Vector3Int currentCell = new Vector3Int(cell.x + x, cell.y + y, cell.z);

                        // place something here
                        tilemapFactory.SetTile(currentCell, this.emptyFillerTile);
                    }
                }
            }
        }


        private void ResetAllBuildButtons()
        {
            foreach(Button btn in this.buildButtons)
            {
                btn.GetComponent<SingleBuildButton>().SetUnselected();
            }
        }
    }
}
