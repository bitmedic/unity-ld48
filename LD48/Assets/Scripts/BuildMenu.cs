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

        TileBase tileToPlace;

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

                if (!tilemapFactory.HasTile(cell))
                {
                    // if nothing is placed here on the factory layer
                    if (clickedToPlace)
                    {
                        tilemapFactory.SetTile(cell, this.tileToPlace);
                    }
                    else if (tileToPlace != null)
                    {
                        tilemapFactory.SetEditorPreviewTile(cell, this.tileToPlace);
                    }
                    // else do nothing for now
                }
            }
        }

        public void SetBuildingSelected(TileBase buildingTile)
        {
            this.ResetAllBuildButtons(); // new button was selected
            this.tileToPlace = buildingTile;
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
