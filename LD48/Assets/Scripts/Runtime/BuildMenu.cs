using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        List<ResourceNodeSO> resourceNodeTiles;

        [SerializeField]
        List<Button> buildButtons;

        [SerializeField]
        TileBase rocketTile;

        [SerializeField]
        TileBase drillTile;

        [SerializeField]
        TileBase emptyFillerTile;

        [SerializeField]
        List<BuildingSizeSO> numberKeysToBuild;
        [SerializeField]
        List<SingleBuildButton> numberKeysToBuildButtonScript;

        [SerializeField]
        AssemblyManager assemblyManager;

        [SerializeField]
        BuidlingToolTip buidlingToolTip;

        BuildingSizeSO tileToPlace;
        int buildingWidth;
        int buildingHeight;


        Vector2 mousePosition;
        Vector3 worldPosition;
        Vector3Int cell;

        bool doBulldoze = false;
        bool clickedToPlace = false;

        private void Start()
        {
            // place rocket at 0,0
            tilemapFactory.SetTile(new Vector3Int(0, 0, 0), rocketTile);
        }

        // Update is called once per frame
        void Update()
        {
            // go into placing mode as long as mouse button 0 is down
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    // a UI Button was clicked, so don't build anything
                }
                else
                {
                    clickedToPlace = true;
                }
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
                tileToPlace?.ResetRotate();
                this.ResetAllBuildButtons();
            }

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.R))
            {
                tileToPlace?.DoRotate();
            }

            this.checkNumberKeysToBuild();

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
                        this.BulldozeBuilding(null, cell);
                    }
                }
                else if (this.buildingHeight > 0 && this.buildingWidth > 0 && this.CheckCanBuild(cell))
                {
                    // if nothing is placed here on the factory layer
                    if (clickedToPlace)
                    {
                        this.BuildBuilding(this.tileToPlace?.GetTileRotation(), cell);
                    }
                    else if (tileToPlace != null)
                    {
                        tilemapFactory.SetEditorPreviewTile(cell, this.tileToPlace.GetTileRotation());
                    }
                    // else do nothing for now
                }
            }
        }

        private bool CheckCanBuild(Vector3Int cell)
        {
            for (int x = 0; x < this.buildingWidth; x++)
            {
                for (int y = 0; y < this.buildingHeight; y++)
                {
                    Vector3Int currentCell = new Vector3Int(cell.x + x, cell.y + y, cell.z);

                    if (currentCell.x >= -1 && currentCell.x <= 1 && currentCell.y >= -1 && currentCell.y <= 1)
                    {
                        // do not allow to build at spawn, because that is where the rocket is
                        return false;
                    }

                    // check if cell does has a terrain field
                    if (!tilemapTerrain.HasTile(currentCell))
                    {
                        return false;
                    }

                    // check if cell has no factory field
                    if (tilemapFactory.HasTile(currentCell))
                    {
                        return false;
                    }

                    if (this.tileToPlace != null && this.tileToPlace.Equals(this.drillTile))
                    {
                        bool foundResourceNode = false;

                        // if building a drill check if all cells are also resource cells
                        foreach (ResourceNodeSO resNode in this.resourceNodeTiles)
                        {
                            if (resNode.resourceNodeTiles.Contains(tilemapTerrain.GetTile(currentCell)))
                            {
                                foundResourceNode = true;
                            }
                        }

                        if (!foundResourceNode)
                        {
                            return false;
                        }
                    }
                }
            }

            return true; // if no other object was found
        }

        private void BulldozeBuilding(TileBase tileToPlace, Vector3Int cellLocation)
        {
            if (cellLocation.x >= -1 && cellLocation.x <= 1 && cellLocation.y >= -1 && cellLocation.y <= 1)
            {
                // do not allow to bulldoze at spawn, because that is where the rocket is
                return;
            }

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

            this.assemblyManager?.CreateModel();
        }

        private void BuildBuilding(TileBase tileToPlace, Vector3Int cellLocation)
        {
            // build
            tilemapFactory.SetTile(cellLocation, tileToPlace);
            tilemapDecoration.SetTile(cellLocation, this.emptyFillerTile);

            this.assemblyManager?.CreateModel();

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

        public void SetBuildingSelected(BuildingSizeSO buildingSO)
        {
            this.ResetAllBuildButtons(); // new button was selected
            this.tileToPlace = buildingSO;

            if (buildingSO.machineInfo != null && buildingSO.machineInfo.production.Count > 0)
            {
                this.buidlingToolTip.ShowToolTipp(buildingSO.machineInfo.name, buildingSO.machineInfo.production[0]);
            }

            if (this.tileToPlace.tile == null)
            {
                doBulldoze = true;
            }
            else
            {
                doBulldoze = false;
            }

            this.tileToPlace?.ResetRotate();

            this.buildingWidth = buildingSO.width;
            this.buildingHeight = buildingSO.height;
        }


        private void ResetAllBuildButtons()
        {
            foreach(Button btn in this.buildButtons)
            {
                btn.GetComponent<SingleBuildButton>().SetUnselected();
            }
        }

        private void checkNumberKeysToBuild()
        {
            int index = -1;

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                index = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                index = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                index = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                index = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                index = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                index = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                index = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                index = 7;
            }

            if (index > 0)
            {
                if (numberKeysToBuild.Count > index)
                {
                    this.SetBuildingSelected(numberKeysToBuild[index]);

                    if (this.numberKeysToBuildButtonScript.Count > index)
                    {
                        this.numberKeysToBuildButtonScript[index].ToggleSelected();
                    }
                    else
                    {
                        Debug.Log("Not enough Buttons defined in the List for the number selection");
                    }
                }
                else
                {
                    Debug.Log("Not enough Numbers to ButtonsizeSO defined in the List for the number selection");
                }
            }
        }
    }
}
