using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class MainMenu : MonoBehaviour
    {
        public Text StartText;
        public Animator animator;
        public StoryProgressor story;

        // Start is called before the first frame update
        void Start()
        {
            story.SetMainMenu(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void StartGame()
        {
            StartText.text = "Launch";
            

            animator.SetTrigger("startAnimation");

            story.TriggerStartIntro();
        }

        public void IntroIsDone()
        {
            animator.SetTrigger("startLanding");
        }
    }
}
