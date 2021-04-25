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
        public float impactTime;
        public float impactTimeStart;
        public TMPro.TextMeshProUGUI impactProgressText;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            float drillProgressPct = Mathf.Floor(drillProgress / drillGoal * 100);
            drillProgressBar.rectTransform.sizeDelta = new Vector2(15f, drillProgressPct);
            drillProgressText.text = string.Format("{0,2:00}m", drillProgressPct);

            float tntProgressPct = Mathf.Floor(tntProgress / tntGoal * 100);
            tntProgressBar.rectTransform.sizeDelta = new Vector2(15f, tntProgressPct);
            tntProgressText.text = string.Format("{0,2:00}%", tntProgressPct);

            float impactTimePct = Mathf.Floor(impactTime / impactTimeStart * 100);
            impactBar.rectTransform.sizeDelta = new Vector2(15f, 100 - impactTimePct);
            int impactSecondsRemaining = Mathf.FloorToInt(impactTimeStart - impactTime);
            impactProgressText.text = string.Format("{0,2:00}:{1,2:00}", impactSecondsRemaining / 60, impactSecondsRemaining % 60);
        }
    }
}
