using System.Collections.Generic;
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
            DebugConnect();
        }

        private void DebugConnect()
        {
            assembly.machines[6].outputPorts[0].connectedMachine = assembly.machines[5];
            assembly.machines[5].inputPorts[0].connectedMachine = assembly.machines[6];
            assembly.machines[5].outputPorts[0].connectedMachine = assembly.machines[4];
            assembly.machines[4].inputPorts[0].connectedMachine = assembly.machines[5];
            assembly.machines[4].outputPorts[0].connectedMachine = assembly.machines[3];
            assembly.machines[3].inputPorts[0].connectedMachine = assembly.machines[4];
            assembly.machines[3].outputPorts[0].connectedMachine = assembly.machines[2];
            assembly.machines[2].inputPorts[0].connectedMachine = assembly.machines[3];
            assembly.machines[2].outputPorts[0].connectedMachine = assembly.machines[1];
            assembly.machines[1].inputPorts[0].connectedMachine = assembly.machines[2];
            assembly.machines[1].outputPorts[0].connectedMachine = assembly.machines[0];
            assembly.machines[0].inputPorts[0].connectedMachine = assembly.machines[1];
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

            this.MatchAllMachines(assembly);

            Debug.Log("Detected the following tile types: " + string.Join(", ", machineTypes));
            if (unmappedMachineTypes.Count > 0) Debug.LogError("Failed to map the following tile types: " + string.Join(", ", unmappedMachineTypes));
        }

        public void Tick()
        {
            assembly.Tick();
        }

        private static string key_drill = "drill_0";
        private static string key_refinery = "factory_0";
        private static string key_refinery2 = "_0"; //TODO umbennen

        private static string key_conveyer_SO_NW = "";
        private static string key_conveyer_SW_NO = "";
        private static string key_conveyer_NW_SO = "";
        private static string key_conveyer_NO_SW = "";

        private void MatchAllMachines(AssemblyLine assembly)
        {
            foreach (Machine m in assembly.machines)
            {
                // TODO get name form scriptableObject?
                if (m.info.key.Equals(key_conveyer_SO_NW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_SW_NO))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NW_SO))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NO_SW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get going to machine
                }
            }
        }

        private void SetBuildingInput(Machine parentMachine, Vector2Int searchPosition)
        {
            Machine neighbour = this.GetMachineAtPosition(searchPosition);

            if (neighbour != null)
            {
                parentMachine.inputPorts.Add(new Port(neighbour));

                if (neighbour.info.key.Equals(key_drill))
                {
                    neighbour.outputPorts.Add(new Port(parentMachine));
                }
                else if (neighbour.info.key.Equals(key_refinery))
                {
                    neighbour.outputPorts.Add(new Port(parentMachine));
                }
                else if (neighbour.info.key.Equals(key_refinery2))
                {
                    neighbour.outputPorts.Add(new Port(parentMachine));
                }
            }
        }

        private void SetBuildingOutput(Machine parentMachine, Vector2Int searchPosition)
        {
            Machine neighbour = this.GetMachineAtPosition(searchPosition);

            if (neighbour != null)
            {
                parentMachine.outputPorts.Add(new Port(neighbour));

                if (neighbour.info.key.Equals(key_drill))
                {
                    neighbour.inputPorts.Add(new Port(parentMachine));
                }
                else if (neighbour.info.key.Equals(key_refinery))
                {
                    neighbour.inputPorts.Add(new Port(parentMachine));
                }
                else if (neighbour.info.key.Equals(key_refinery2))
                {
                    neighbour.inputPorts.Add(new Port(parentMachine));
                }
            }
        }

        private Machine GetMachineAtPosition(Vector2Int position)
        {
            foreach (Machine m in assembly.machines)
            {
                if (m.position.Equals(position))
                {
                    return m;
                }

            }

            return null;
        }
    }
}