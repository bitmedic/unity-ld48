using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LD48
{
    public class ProgressUI : MonoBehaviour
    {

        public Image drillProgressBar;
        public float drillProgress;
        public float drillGoal;
        public TMPro.TextMeshProUGUI drillProgressText;

        public Image tntProgressBar;
        public float tntProgress;
        public float tntGoal;
        public TMPro.TextMeshProUGUI tntProgressText;

        public Image impactBar;
        public float impactTimeLeft;
        public float impactTimeTotal;
        public TMPro.TextMeshProUGUI impactProgressText;

        public GameObject launchButtonPanel;
        private AssemblyManager assMan;

        // Start is called before the first frame update
        void Start()
        {
            assMan = GameObject.FindObjectOfType<AssemblyManager>();
        }

        // Update is called once per frame
        void Update()
        {
            impactTimeLeft = impactTimeTotal - Time.time; //TODO this must be moved to main game loop

            drillProgress = assMan.GetRocket().GetMaterialQuantity("depth");

            float drillProgressPct = Mathf.Floor(drillProgress / drillGoal * 100);
            drillProgressBar.rectTransform.sizeDelta = new Vector2(15f, drillProgressPct);
            drillProgressText.text = string.Format("{0,2:00}m", drillProgressPct);

            tntProgress = assMan.GetRocket().GetMaterialQuantity("explosiveness");
            float tntProgressPct = Mathf.Floor(tntProgress / tntGoal * 100);
            tntProgressBar.rectTransform.sizeDelta = new Vector2(15f, tntProgressPct);
            tntProgressText.text = string.Format("{0,2:00}%", tntProgressPct);

            float impactTimePct = Mathf.Floor(impactTimeLeft / impactTimeTotal * 100);
            impactBar.rectTransform.sizeDelta = new Vector2(15f, impactTimePct);
            impactProgressText.text = string.Format("{0,2:00}:{1,2:00}", Mathf.FloorToInt(impactTimeLeft / 60f), impactTimeLeft % 60);

            if (drillProgressPct > 100 && tntProgressPct > 100)
            {
                launchButtonPanel.SetActive(true);
            }
        }

        public void WinTheGame()
        {
            // Do Something
        }
    }
}
