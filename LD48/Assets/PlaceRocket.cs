using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    public class PlaceRocket : MonoBehaviour
    {
        public Tilemap tilemap;
        public TileBase rocketTile;

        // Start is called before the first frame update
        void Start()
        {
            tilemap.SetTile(new Vector3Int(0, 0, 0), rocketTile);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
