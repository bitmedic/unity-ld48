using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public StoryProgressor story;

        public GameObject launchButtonPanel;
        private AssemblyManager assMan;

        // Start is called before the first frame update
        void Start()
        {
            assMan = GameObject.FindObjectOfType<AssemblyManager>();
            impactTimeLeft = impactTimeTotal;
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: pausing if text is shown only makes sense if also no ticks are generated
            //if (!story.isTextShown) 
            //{
                impactTimeLeft = impactTimeLeft - Time.deltaTime; //TODO this must be moved to main game loop
                impactTimeLeft = Mathf.Max(impactTimeLeft, 0);
            //}

            int depth = assMan.GetRocket().GetMaterialQuantity("depth") + 1;
            int stability = assMan.GetRocket().GetMaterialQuantity("stability") + 1;

            drillProgress = depth * stability;
            float drillProgressPct = Mathf.Floor(drillProgress / drillGoal * 100);

            drillProgressPct = Mathf.Min(drillProgressPct, 100); // max 100%
            drillProgressBar.rectTransform.sizeDelta = new Vector2(15f, drillProgressPct);
            drillProgressText.text = FormatDrillProgress(drillProgress);

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

            if (impactTimeLeft <= 0)
            {
                this.LoseTheGame();
            }
        }



        public void WinTheGame()
        {
            SceneManager.LoadScene(2);
        }

        public void LoseTheGame()
        {
            // Do Something
            story.TriggerDefeat();
        }

        private string FormatDrillProgress(float progress)
        {
            if (progress < 1000) 
            {
                return string.Format("{0:F0}m", progress);
            }
            else
            {
                return string.Format("{0:F1}km", progress / 1000);
            }
        }
    }
}
