using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD48
{
    public class ProgressUI : MonoBehaviour
    {

        public float drillProgress;
        public float drillGoal;
        public TMPro.TextMeshProUGUI drillProgressText;
        
        public Image steelProgressBar;
        public float steelProgress;
        public float steelGoal;

        public Image hydroProgressBar; 
        public float hydroProgress;
        public float hydroGoal;

        public Image tntProgressBar;
        public float tntProgress;
        public float tntGoal;
        public TMPro.TextMeshProUGUI tntProgressText;

        public Image impactBar;
        public float impactTimeLeft;
        public float impactTimeTotal;
        public TMPro.TextMeshProUGUI impactProgressText;

        public StoryProgressor story;
        public Animator oneMinuteWarningAnimator;
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

            steelProgress = assMan.GetRocket().GetMaterialQuantity("stability");
            hydroProgress = assMan.GetRocket().GetMaterialQuantity("depth");

            float steelProgressPct = steelProgress / steelGoal * 100;
            steelProgressPct = Mathf.Clamp(steelProgressPct, 0, 100); // max 100%
            steelProgressBar.rectTransform.sizeDelta = new Vector2(15f, steelProgressPct);

            float hydroProgressPct = hydroProgress / hydroGoal * 100;
            hydroProgressPct = Mathf.Clamp(hydroProgressPct, 0, 100); // max 100%
            hydroProgressBar.rectTransform.sizeDelta = new Vector2(15f, hydroProgressPct);

            drillProgress = (hydroProgressPct + steelProgressPct) / 2;

            float drillProgressPct = Mathf.Floor(drillProgress / drillGoal * 100);
            drillProgressPct = Mathf.Clamp(drillProgressPct, 0, 100); // max 100%
            drillProgressText.text = string.Format("{0,2}%", drillProgressPct);

            tntProgress = assMan.GetRocket().GetMaterialQuantity("explosiveness");
            float tntProgressPct = Mathf.Floor(tntProgress / tntGoal * 100);
            tntProgressPct = Mathf.Clamp(tntProgressPct, 0, 100); // max 100%
            tntProgressBar.rectTransform.sizeDelta = new Vector2(15f, tntProgressPct);
            tntProgressText.text = string.Format("{0,2}%", tntProgressPct);

            float impactTimePct = Mathf.Floor(impactTimeLeft / impactTimeTotal * 100);
            impactBar.rectTransform.sizeDelta = new Vector2(15f, impactTimePct);
            impactProgressText.text = string.Format("{0,2:00}:{1,2:00}", Mathf.FloorToInt(impactTimeLeft / 60f), Mathf.FloorToInt(impactTimeLeft % 60));

            if (drillProgressPct >= 100 && tntProgressPct >= 100)
            {
                launchButtonPanel.SetActive(true);
                story.TriggerPressButtonNow();
            }

            if (impactTimeLeft <= 0)
            {
                this.LoseTheGame();
            }

            if (impactTimeLeft <= 60)
            {
                oneMinuteWarningAnimator.SetBool("StartOneMinuteWarning", true);
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

    }
}
