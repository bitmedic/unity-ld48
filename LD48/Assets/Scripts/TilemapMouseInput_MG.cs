using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMouseInput_MG : MonoBehaviour
{
    Tilemap map;

    [SerializeField]
    Vector2 mousePosition;
    [SerializeField]
    Vector3 worldPosition;
    [SerializeField]
    Vector3Int cell;
    [SerializeField]
    TileBase tile;

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        cell = map.WorldToCell(worldPosition);
        tile = map.GetTile(cell);
    }
}
