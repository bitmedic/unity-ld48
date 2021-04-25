using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class BuidlingToolTip : MonoBehaviour
    {
        public Text titleText;
        public Text titleInput;
        public Text input1Text;
        public Text input2Text;
        public Text titleOutput;
        public Text outputText;
        public Text productionTime;
        public Text titleTime;


        public void ShowToolTipp()
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

        public void ShowToolTipp(List<MachineInfo> machineInfo)
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
            this.titleTime.text = string.Format("");
        }

        public void ShowToolTipp(MachineInfo machineInfo)
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

            this.productionTime.text = string.Format("{0}", production.tickCost);            
        }

        public void HideToolTipp()
        {
            this.gameObject.SetActive(false);
        }
    }
}
