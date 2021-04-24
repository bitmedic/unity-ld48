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
            //DebugConnect();
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


        private static string key_conveyer_SW_NE = "conveyors_swne";
        private static string key_conveyer_SW_NW = "conveyors_swnw";
        private static string key_conveyer_SW_SE = "conveyors_swse";
        private static string key_conveyer_SE_NW = "conveyors_senw";
        private static string key_conveyer_SE_NE = "conveyors_sene";
        private static string key_conveyer_SE_SW = "conveyors_sesw";
        private static string key_conveyer_NW_SE = "conveyors_nwse";
        private static string key_conveyer_NW_SW = "conveyors_nwsw";
        private static string key_conveyer_NW_NE = "conveyors_nwne";
        private static string key_conveyer_NE_SW = "conveyors_nesw";
        private static string key_conveyer_NE_SE = "conveyors_nese";
        private static string key_conveyer_NE_NW = "conveyors_nenw";

        private void MatchAllMachines(AssemblyLine assembly)
        {
            foreach (Machine m in assembly.machines)
            {
                // TODO get name form scriptableObject?
                if (m.info.key.Equals(key_conveyer_SE_NW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_SW_NE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NW_SE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NE_SW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get going to machine
                }

                if (m.info.key.Equals(key_conveyer_SE_NE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x +1 , m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_SE_SW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_SW_SE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_SW_NW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get going to machine
                }
                if (m.info.key.Equals(key_conveyer_NW_NE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NW_SW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x - 1, m.position.y)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NE_SE))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y - 1)); // get going to machine
                }
                else if (m.info.key.Equals(key_conveyer_NE_NW))
                {
                    this.SetBuildingInput(m, new Vector2Int(m.position.x + 1, m.position.y)); // get coming from machine
                    this.SetBuildingOutput(m, new Vector2Int(m.position.x, m.position.y + 1)); // get going to machine
                }
            }
        }

        private void SetBuildingInput(Machine parentMachine, Vector2Int searchPosition)
        {
            Machine neighbour = this.GetMachineAtPosition(searchPosition);

            if (neighbour != null)
            {
                parentMachine.outputPorts.Add(new Port(neighbour));
                this.MatchNeighbourInput(neighbour, parentMachine);
            }
            else
            {
                Vector2Int alternativeSearchPostion = new Vector2Int(searchPosition.x - 1, searchPosition.y);
                if (this.MatchNeighbourInput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }

                alternativeSearchPostion = new Vector2Int(searchPosition.x, searchPosition.y - 1);
                if (this.MatchNeighbourInput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }

                alternativeSearchPostion = new Vector2Int(searchPosition.x - 1, searchPosition.y - 1);
                if (this.MatchNeighbourInput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }
            }
        }

        private void SetBuildingOutput(Machine parentMachine, Vector2Int searchPosition)
        {
            Machine neighbour = this.GetMachineAtPosition(searchPosition);

            if (neighbour != null)
            {
                parentMachine.outputPorts.Add(new Port(neighbour));
                MatchNeighbourOutput(neighbour, parentMachine);
            }
            else 
            {
                Vector2Int alternativeSearchPostion = new Vector2Int(searchPosition.x - 1, searchPosition.y);
                if (this.MatchNeighbourOutput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }

                alternativeSearchPostion = new Vector2Int(searchPosition.x, searchPosition.y - 1);
                if (this.MatchNeighbourOutput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }

                alternativeSearchPostion = new Vector2Int(searchPosition.x - 1, searchPosition.y - 1);
                if (this.MatchNeighbourOutput(this.GetMachineAtPosition(alternativeSearchPostion), parentMachine))
                {
                    return;
                }
            }
        }

        private bool MatchNeighbourInput(Machine neighbour, Machine parentMachine)
        {
            if (neighbour != null)
            {
                if (neighbour.info.key.Equals(key_drill))
                {
                    parentMachine.inputPorts.Add(new Port(neighbour));
                    neighbour.outputPorts.Add(new Port(parentMachine));
                    return true;
                }
                else if (neighbour.info.key.Equals(key_refinery))
                {
                    parentMachine.inputPorts.Add(new Port(neighbour));
                    neighbour.outputPorts.Add(new Port(parentMachine));
                    return true;
                }
                else if (neighbour.info.key.Equals(key_refinery2))
                {
                    parentMachine.inputPorts.Add(new Port(neighbour));
                    neighbour.outputPorts.Add(new Port(parentMachine));
                    return true;
                }
            }
            return false;
        }

        private bool MatchNeighbourOutput(Machine neighbour, Machine parentMachine)
        {
            if (neighbour != null)
            {
                if (neighbour.info.key.Equals(key_drill))
                {
                    parentMachine.outputPorts.Add(new Port(neighbour));
                    neighbour.inputPorts.Add(new Port(parentMachine));
                    return true;
                }
                else if (neighbour.info.key.Equals(key_refinery))
                {
                    parentMachine.outputPorts.Add(new Port(neighbour));
                    neighbour.inputPorts.Add(new Port(parentMachine));
                    return true;
                }
                else if (neighbour.info.key.Equals(key_refinery2))
                {
                    parentMachine.outputPorts.Add(new Port(neighbour));
                    neighbour.inputPorts.Add(new Port(parentMachine));
                    return true;
                }
            }
            return false;
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