using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    public class RandomizeRessources : MonoBehaviour
    {
        public float ratioWater ;
        public float ratioIron;
        public float ratioPhosphor;

        public Tilemap groundTilemap;

        public TileBase tileWaterResourceNode;
        public TileBase tileIronResourceNode;
        public TileBase tilePhosphorResourceNode;

        List<Vector3Int> coordsRessourceNodes = new List<Vector3Int>()
        {
            new Vector3Int( 4,11,0),
            new Vector3Int(-2,10,0),
            new Vector3Int( 1,9,0),
            new Vector3Int( 9,7,0),
            new Vector3Int( 8,3,0),
            new Vector3Int( 1,3,0),
            new Vector3Int(10,-1,0),
            new Vector3Int( 5,-7,0),
            new Vector3Int(-1,-5,0),
            new Vector3Int(-5,2,0),
            new Vector3Int(-8,4,0),
            new Vector3Int(-8,-10,0),
            new Vector3Int(-9,-3,0),
            new Vector3Int( -12,-3,0)
        };


        // Start is called before the first frame update
        void Start()
        {
            RandomDistributionFixedAmoung();
        }


        private void RandomDistributionFixedAmoung()
        {
            int summeRessource = (int)(ratioWater + ratioPhosphor + ratioIron);
            for (int i = 0; i < summeRessource; i++)
            {
                if (coordsRessourceNodes.Count > 0)
                {
                    int randomIndex = Random.Range(0, coordsRessourceNodes.Count - 1);

                    TileBase resTile = tileWaterResourceNode;
                    if (i < (int)(ratioWater))
                    {
                        resTile = tileWaterResourceNode;
                    }
                    else if (i < (int)(ratioWater + ratioIron))
                    {
                        resTile = tileIronResourceNode;

                    }
                    else //if (i < (int)(ratioWater + ratioIron + ratioPhosphor))
                    {
                        resTile = tilePhosphorResourceNode;
                    }

                    Vector3Int coord = coordsRessourceNodes[randomIndex];
                    if (groundTilemap.HasTile(coord))
                    {
                        groundTilemap.SetTile(coord, resTile);
                        groundTilemap.SetTile(new Vector3Int(coord.x - 1, coord.y, coord.z), resTile);
                        groundTilemap.SetTile(new Vector3Int(coord.x - 1, coord.y - 1, coord.z), resTile);
                        groundTilemap.SetTile(new Vector3Int(coord.x, coord.y - 1, coord.z), resTile);
                    }

                    coordsRessourceNodes.RemoveAt(randomIndex);
                }
                }
        }


        private void TotalRondomRatio()
        { 

            foreach(Vector3Int coord in  coordsRessourceNodes)
            {
                int summeRessource = (int)(ratioWater + ratioPhosphor + ratioIron);
                float randomRessource = Random.Range(0, summeRessource);
                TileBase resTile = tileWaterResourceNode;

                if (randomRessource <= ratioWater)
                {
                    resTile = tileWaterResourceNode;
                }
                else if (randomRessource <= ratioWater + ratioIron)
                {
                    resTile = tileIronResourceNode;
                }
                else if (randomRessource <= ratioWater + ratioIron + ratioPhosphor)
                {
                    resTile = tilePhosphorResourceNode;
                }

                if (groundTilemap.HasTile(coord))
                {
                    groundTilemap.SetTile(coord, resTile);
                    groundTilemap.SetTile(new Vector3Int(coord.x - 1, coord.y, coord.z), resTile);
                    groundTilemap.SetTile(new Vector3Int(coord.x - 1, coord.y - 1, coord.z), resTile);
                    groundTilemap.SetTile(new Vector3Int(coord.x, coord.y - 1, coord.z), resTile);
                }
            }
        }
    }
}
