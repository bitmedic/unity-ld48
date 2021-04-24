using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LD48
{
    public class AssemblyManager : MonoBehaviour
    {
        [Header("Static References")] public Tilemap tilemap;
        public StringMachineInfoDictionary machinery;
        public List<string> ignoreTileTypes;

        [Header("Runtime Info")] public AssemblyLine assembly;

        private void Start()
        {
            CreateModel();
        }

        public void CreateModel()
        {
            assembly = new AssemblyLine();
            HashSet<string> machineTypes = new HashSet<string>();
            HashSet<string> unmappedMachineTypes = new HashSet<string>();

            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile == null) continue;

                    string key = tile.name.ToLowerInvariant();
                    machineTypes.Add(key);
                    if (!machinery.ContainsKey(key))
                    {
                        if (!ignoreTileTypes.Contains(key)) unmappedMachineTypes.Add(key);
                        continue;
                    }

                    Machine m = new Machine(machinery[key]);
                    m.position = new Vector2Int(x, y);
                    assembly.WithMachine(m);
                }
            }

            Debug.Log("Detected the following tile types: " + string.Join(", ", machineTypes));
            if (unmappedMachineTypes.Count > 0) Debug.LogError("Failed to map the following tile types: " + string.Join(", ", unmappedMachineTypes));
        }

        public void Tick()
        {
        }
    }
}