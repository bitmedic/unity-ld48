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
        Tilemap tilemapDecoration;

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

        bool doBulldoze = false;
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
                doBulldoze = false;
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

                if (doBulldoze)
                {
                    if (clickedToPlace)
                    {
                        this.BulldozeBuilding(this.tileToPlace, cell);
                    }
                }
                else if (this.CheckCanBuild(cell))
                {
                    // if nothing is placed here on the factory layer
                    if (clickedToPlace)
                    {
                        this.BuildBuilding(this.tileToPlace, cell);
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

            if (this.tileToPlace == null)
            {
                doBulldoze = true;
            }
            else
            {
                doBulldoze = false;
            }

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
                }
            }

            return true; // if no other object was found
        }

        private void BulldozeBuilding(TileBase tileToPlace, Vector3Int cellLocation)
        {
            // TODO: How to find Building if an emptyFiller was clicked??
            TileBase clickedTile = tilemapFactory.GetTile(cellLocation);

            if (this.emptyFillerTile.Equals(clickedTile))
            {
                // find building and demolish
                if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z))) &&
                    this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x + 1, cellLocation.y - 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
                }
                else if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z))) &&
                         this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x - 1, cellLocation.y - 1, cellLocation.z);
                }
                else if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x - 1, cellLocation.y + 1, cellLocation.z))) &&
                         this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
                }
            }


            // if the building is larger than 1x1, then also remove the empty fillers
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (x == 0 && y == 0) // if not the base spawn cell
                    {
                        //bulldoze
                        tilemapFactory.SetTile(cellLocation, null);
                    }
                    else
                    { 
                        Vector3Int currentCell = new Vector3Int(cellLocation.x + x, cellLocation.y + y, cellLocation.z);

                        if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(currentCell)))
                        {
                            tilemapFactory.SetTile(currentCell, null);
                        }
                    }
                }
            }
        }

        private void BuildBuilding(TileBase tileToPlace, Vector3Int cellLocation)
        {
            // build
            tilemapFactory.SetTile(cellLocation, tileToPlace);
            tilemapDecoration.SetTile(cellLocation, this.emptyFillerTile);

            // if the building is larger than 1x1, then place something at the other positions
            for (int x = 0; x < this.buildingWidth; x++)
            {
                for (int y = 0; y < this.buildingHeight; y++)
                {
                    if (x != 0 || y != 0) // if not the base spawn cell
                    {
                        Vector3Int currentCell = new Vector3Int(cellLocation.x + x, cellLocation.y + y, cellLocation.z);

                        // place something here
                        tilemapFactory.SetTile(currentCell, this.emptyFillerTile);
                        tilemapDecoration.SetTile(currentCell, this.emptyFillerTile);

                        this.clickedToPlace = false; // only every build 1 large building
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
