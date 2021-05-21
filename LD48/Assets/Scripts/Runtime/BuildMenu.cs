using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LD48
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] Tilemap tilemapBuildPreview;
        [SerializeField] Tilemap tilemapFactory;
        [SerializeField] Tilemap tilemapTerrain;
        [SerializeField] Tilemap tilemapDecoration;

        [SerializeField] UndirectionalConveyors undirectionalConveyors;

        [SerializeField] List<ResourceNodeSO> resourceNodeTiles;

        [SerializeField] List<Button> buildButtons;

        [SerializeField] TileBase rocketTile;

        [SerializeField] TileBase drillTile;

        [SerializeField] TileBase emptyFillerTile;

        [SerializeField] List<BuildingSizeSO> numberKeysToBuild;
        [SerializeField] List<SingleBuildButton> numberKeysToBuildButtonScript;

        [SerializeField] AssemblyManager assemblyManager;

        [SerializeField] StoryProgressor storyProgressor;

        [SerializeField]
        BuildingTooltip buildingTooltip;

        [SerializeField]
        ClickSounds clickSoundAudioSource;

        [SerializeField]
        List<Button> tier2Buttons;
        [SerializeField]
        List<Button> tier3Buttons;

        private bool isTier2BuildingsActive;
        private bool isTier3BuildingsActive;

        BuildingSizeSO tileToPlace;

        Vector2 mousePosition;
        Vector3 worldPosition;
        Vector3Int cell;
        Vector3Int lastPreview;

        bool doBulldoze = false;
        bool clickedToPlace = false;



        [SerializeField] GameObject helpPanel;
        [SerializeField] GameObject confirmExit;

        private void Start()
        {
            // place rocket at 0,0
            tilemapFactory.SetTile(new Vector3Int(0, 0, 0), rocketTile);

            if (this.storyProgressor != null)
            {
                storyProgressor.RegisterTierCompleteActions(ActiveTier2Buildings, ActiveTier3Buildings);
                storyProgressor.TriggerAfterLanding();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscKey();
            }

            ShowHideUnlockedBuildings();

            // go into placing mode as long as mouse button 0 is down
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    // a UI Button was clicked, so don't build anything
                }
                else if (this.storyProgressor.isTextShown)
                {
                    // if story text is shown, don't try to build
                }
                else
                {
                    clickedToPlace = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                clickedToPlace = false;
            }

            // if right click -> end building mode
            if (Input.GetMouseButtonDown(1))
            {
                CancelBuildMode();
            }

            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.R))
            {
                tileToPlace?.DoRotate(1);
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
            {
                tileToPlace?.DoRotate(-1);
            }

            this.CheckNumberKeysToBuild();

            mousePosition = Input.mousePosition;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;
            cell = tilemapTerrain.WorldToCell(worldPosition);
            cell.x--;
            cell.y--;
            cell.z = 0;

            if (tilemapTerrain.HasTile(cell))
            {
                // if the tile exists on the asteroid
                //terrainTile = tilemapTerrain.GetTile(cell);

                if (doBulldoze)
                {
                    if (clickedToPlace)
                    {
                        bool bulldozingConveyor = this.undirectionalConveyors.IsTileConveyor(tilemapFactory.GetTile(cell));
                        if (bulldozingConveyor)
                        {
                            this.undirectionalConveyors.RemoveConveyor(cell);
                        }
                        else
                        {
                            this.BulldozeBuilding(cell);
                        }
                    }
                }
                else if (tileToPlace != null && tileToPlace.height > 0 && tileToPlace.width > 0 && this.CheckCanBuild(cell))
                {
                    // if nothing is placed here on the factory layer
                    if (clickedToPlace)
                    {
                        tilemapBuildPreview.ClearAllTiles();
                        this.BuildBuilding(this.tileToPlace.GetRotatedTile(), cell);
                    }
                    else
                    {
                        if (lastPreview != cell)
                        {
                            tilemapBuildPreview.ClearAllTiles();
                        }
                        tilemapBuildPreview.SetTile(cell, this.tileToPlace.GetRotatedTile());
                        lastPreview = cell;
                    }
                    // else do nothing for now
                }
            }
        }

        private void CancelBuildMode()
        {
            tileToPlace = null;
            clickedToPlace = false;
            doBulldoze = false;
            tileToPlace?.ResetRotate();
            this.ResetAllBuildButtons();
            buildingTooltip.ShowOrHideTooltip(null);
            tilemapBuildPreview.ClearAllTiles();
        }

        private void HandleEscKey()
        {
            if (helpPanel.activeSelf)
            {
                helpPanel.SetActive(false);
            }
            else if (storyProgressor.isTextShown)
            {
                storyProgressor.ShowNextText();
            }
            else if (tileToPlace != null)
            {
                CancelBuildMode();
            }
            else if (confirmExit.activeSelf)
            {
                confirmExit.SetActive(false);
            }
            else
            {
                confirmExit.SetActive(true);
            }
        }

        private void ShowHideUnlockedBuildings()
        {
            if (tier2Buttons.Count > 0)
            {
                foreach (Button btn in tier2Buttons)
                {
                    if (isTier2BuildingsActive)
                    {
                        btn.GetComponent<Transform>().localScale = new Vector3(0.8f, 0.8f, 0);
                    }
                    else
                    {
                        btn.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
                    }
                }
            }

            if (tier3Buttons.Count > 0)
            {
                foreach (Button btn in tier3Buttons)
                {
                    if (isTier3BuildingsActive)
                    {
                        btn.GetComponent<Transform>().localScale = new Vector3(0.8f, 0.8f, 0);
                    }
                    else
                    {
                        btn.GetComponent<Transform>().localScale = new Vector3(0, 0, 0);
                    }
                }
            }
        }

        private bool CheckCanBuild(Vector3Int cell)
        {
            for (int x = 0; x < tileToPlace.width; x++)
            {
                for (int y = 0; y < tileToPlace.height; y++)
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
                    if (tilemapFactory.HasTile(currentCell) && 
                        !this.undirectionalConveyors.IsTileConveyor(tilemapFactory.GetTile(currentCell)))
                    {
                        return false;
                    }

                    if (this.tileToPlace != null && this.tileToPlace.tile.Equals(this.drillTile))
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

        private void BulldozeBuilding(Vector3Int cellLocation)
        {
            if (cellLocation.x >= -1 && cellLocation.x <= 1 && cellLocation.y >= -1 && cellLocation.y <= 1)
            {
                // do not allow to bulldoze at spawn, because that is where the rocket is
                return;
            }

            // TODO: How to find Building if an emptyFiller was clicked??
            TileBase clickedTile = tilemapFactory.GetTile(cellLocation);
            bool needToDeleteEmptys = false;

            if (this.emptyFillerTile.Equals(clickedTile))
            {
                // find building and demolish
                if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z))) &&
                    this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x + 1, cellLocation.y - 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
                    needToDeleteEmptys = true;
                }
                else if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z))) &&
                         this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x - 1, cellLocation.y - 1, cellLocation.z);
                    needToDeleteEmptys = true;
                }
                else if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x - 1, cellLocation.y + 1, cellLocation.z))) &&
                         this.emptyFillerTile.Equals(tilemapFactory.GetTile(new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z))))
                {
                    cellLocation = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
                    needToDeleteEmptys = true;
                }
            }

            // if the main tile of a building was selected it should still delete the empty tiles
            if (this.drillTile.Equals(clickedTile) ||
                this.undirectionalConveyors.refinery.Equals(clickedTile) ||
                this.undirectionalConveyors.smelter.Equals(clickedTile) ||
                this.undirectionalConveyors.armory.Equals(clickedTile))
            {
                needToDeleteEmptys = true;
            }

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (x == 0 && y == 0) // if not the base spawn cell
                    {
                        //bulldoze
                        clickSoundAudioSource.PlayBulldozeSound();
                        tilemapFactory.SetTile(cellLocation, null);
                    }
                    // if the building is larger than 1x1, then also remove the empty fillers
                    else if (needToDeleteEmptys)
                    {
                        Vector3Int currentCell = new Vector3Int(cellLocation.x + x, cellLocation.y + y, cellLocation.z);

                        if (this.emptyFillerTile.Equals(tilemapFactory.GetTile(currentCell)))
                        {
                            tilemapFactory.SetTile(currentCell, null);
                        }
                    }
                }
            }

            this.UpdateMachines();
        }

        private void BuildBuilding(TileBase tileToPlace, Vector3Int cellLocation)
        {
            if (tileToPlace == null)
            {
                return;
            }

            if (this.undirectionalConveyors.IsTileConveyor(tileToPlace))
            {
                TileBase oldTile = tilemapFactory.GetTile(cellLocation);
                this.undirectionalConveyors.BuildNewConveyor(cellLocation, tileToPlace);
                TileBase newTile = tilemapFactory.GetTile(cellLocation);

                if (oldTile == null || !oldTile.Equals(newTile))
                {
                    clickSoundAudioSource.PlayBuildSound();
                }
            }
            else
            {
                clickSoundAudioSource.PlayBuildSound();
                tilemapFactory.SetTile(cellLocation, tileToPlace);
            }

            tilemapDecoration.SetTile(cellLocation, this.emptyFillerTile);
            this.UpdateMachines();

            // if the building is larger than 1x1, then place something at the other positions
            for (int x = 0; x < this.tileToPlace.width; x++)
            {
                for (int y = 0; y < this.tileToPlace.height; y++)
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
            clickSoundAudioSource.PlaySelectSound();

            this.ResetAllBuildButtons(); // new button was selected
            this.tileToPlace = buildingSO;

            buildingTooltip.ShowOrHideTooltip(buildingSO);

            if (this.tileToPlace.tile == null)
            {
                doBulldoze = true;
            }
            else
            {
                doBulldoze = false;
            }

            this.tileToPlace?.ResetRotate();

        }

        public void ActiveTier2Buildings()
        {
            this.isTier2BuildingsActive = true;
        }

        public void ActiveTier3Buildings()
        {
            this.isTier3BuildingsActive = true;
        }

        private void UpdateMachines()
        {
            if (this.assemblyManager != null)
            {
                this.assemblyManager.CreateModel();
                storyProgressor.rocket = null;
            }
        }

        private void ResetAllBuildButtons()
        {
            foreach (Button btn in this.buildButtons)
            {
                btn.GetComponent<SingleBuildButton>().SetUnselected();
            }
        }

        private void CheckNumberKeysToBuild()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                HandleBuildKeyInput(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                HandleBuildKeyInput(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HandleBuildKeyInput(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                HandleBuildKeyInput(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                HandleBuildKeyInput(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                HandleBuildKeyInput(5);
            }
        }

        public void HandleBuildKeyInput(int index)
        {
            tilemapBuildPreview.ClearAllTiles();
            if (numberKeysToBuild.Count > index)
            {
                if (this.numberKeysToBuildButtonScript.Count > index)
                {
                    if (!isTier2BuildingsActive)
                    {
                        foreach (Button btn in tier2Buttons)
                        {
                            if (btn.GetComponent<SingleBuildButton>().Equals(numberKeysToBuildButtonScript[index]))
                            {
                                // button is in still unlocked tier 2 => prevent
                                return;
                            }
                        }
                    }

                    if (!isTier3BuildingsActive)
                    {
                        foreach (Button btn in tier3Buttons)
                        {
                            if (btn.GetComponent<SingleBuildButton>().Equals(numberKeysToBuildButtonScript[index]))
                            {
                                // button is in still unlocked tier 2 => prevent
                                return;
                            }
                        }
                    }

                    this.SetBuildingSelected(numberKeysToBuild[index]);
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

        public void ReturnToTitleScreen()
        {
            SceneManager.LoadScene(0);
        }
    }
}