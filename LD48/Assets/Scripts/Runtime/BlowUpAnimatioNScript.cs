using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class BlowUpAnimatioNScript : MonoBehaviour
    {
        public StoryProgressor story;

        void Start()
        {

            StartCoroutine(ShowEndText());

        }

        IEnumerator ShowEndText()
        {
            yield return new WaitForSeconds(5f);

            story.TriggerVictory();
        }
    }
}
