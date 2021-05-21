using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class BuildingTooltip : MonoBehaviour
    {
        public Text titleText;
        public Text titleInput;
        public Text input1Text;
        public Text input2Text;
        public Text titleOutput;
        public Text outputText;
        public Text productionTime;
        public Text titleTime;

        public void ShowOrHideTooltip(BuildingSizeSO buildingSO)
        {
            if (buildingSO == null)
            {
                this.Hide();
            }
            else if (buildingSO.machineInfo != null)
            {
                if (buildingSO.machineInfo.Count == 1)
                {
                    if (buildingSO.machineInfo[0].production.Count > 0)
                    {
                        this.ShowTooltip(buildingSO.machineInfo[0]);
                    }
                }
                else if (buildingSO.machineInfo.Count > 1)
                {
                    this.ShowTooltip(buildingSO.machineInfo);
                }
                else if (buildingSO.rotations.Count > 0)
                {
                    // conveyers
                    this.ShowTooltipConveyer();
                }
                else
                {
                    this.Hide();
                }
            }
            else
            {
                if (buildingSO.tile == null)
                {
                    this.ShowTooltip();
                }
                else
                {
                    this.Hide();
                }
            }
        }



        private void ShowTooltip()
        {
            this.gameObject.SetActive(true);

            this.titleText.text = "Demolish";
            this.titleInput.text = "";
            this.titleOutput.text = "";

            this.input1Text.text = ""; 
            this.input2Text.text = "";
            this.outputText.text = "";

            this.productionTime.text = string.Format("");
            this.titleTime.text = string.Format("");
        }

        private void ShowTooltip(List<MachineInfo> machineInfo)
        {
            string buildingName = machineInfo[0].name;

            this.gameObject.SetActive(true);

            this.titleText.text = buildingName;
            this.titleInput.text = "Output:";
            this.titleOutput.text = "";


            if (machineInfo[0].production[0] != null)
            {
                this.input1Text.text = string.Format("{0} {1}", machineInfo[0].production[0].amount, machineInfo[0].production[0].material);
            }
            else
            {
                this.input1Text.text = "";
            }

            if (machineInfo[1].production[0] != null)
            {
                this.input2Text.text = string.Format("{0} {1}", machineInfo[1].production[0].amount, machineInfo[1].production[0].material);
            }
            else
            {
                this.input2Text.text = "";
            }

            if (machineInfo[1].production[0] != null)
            {
                this.outputText.text = string.Format("{0} {1}", machineInfo[2].production[0].amount, machineInfo[2].production[0].material);
            }
            else
            {
                this.outputText.text = "";
            }

            this.productionTime.text = string.Format("");
            this.productionTime.text = string.Format("{0} Ticks", machineInfo[0].production[0].tickCost);
        }

        private void ShowTooltip(MachineInfo machineInfo)
        {
            string buildingName = machineInfo.name;
            Production production = machineInfo.production[0];

            this.titleInput.text = "Input:";
            this.titleOutput.text = "Output:";

            this.gameObject.SetActive(true);

            this.titleText.text = buildingName;

            if (production.formula.Count >= 1)
            {
                int index = 0;
                foreach (var f in production.formula)
                {
                    if (index == 0)
                    {
                        this.input1Text.text = string.Format("{0} {1}", f.Value, f.Key);
                    }
                    index++;
                }
            }
            else
            {
                this.input1Text.text = "";
            }

            if (production.formula.Count >= 2)
            {
                int index = 0;
                foreach (var f in production.formula)
                {
                    if (index == 1)
                    {
                        this.input2Text.text = string.Format("{0} {1}", f.Value, f.Key);
                    }
                    index++;
                }
            }
            else
            {
                this.input2Text.text = "";
            }

            this.outputText.text = string.Format("{0} {1}", production.amount, production.material);

            this.titleTime.text = "Time:";
            this.productionTime.text = string.Format("{0} Ticks", production.tickCost);            
        }

        private void ShowTooltipConveyer()
        {
            this.gameObject.SetActive(true);

            this.titleText.text = "Conveyer";
            this.titleInput.text = "Use \"R\" to rotate";
            this.titleOutput.text = "Blue Arrow shows";
            this.titleTime.text = "direction";

            this.input1Text.text = "";
            this.input2Text.text = "";
            this.outputText.text = "";

            this.productionTime.text = string.Format("");
        }

        private void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
