using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD48
{
    public class MainMenu : MonoBehaviour
    {
        public Text StartText;
        public Animator animator;
        public Animator animatorCredits;
        public StoryProgressor story;

        private void Start()
        {
            //earth.position = new Vector3(-347, -264, 0);
        }

        public void StartGame()
        {
            StartText.text = "Save us!";
            
            animator.SetTrigger("startAnimation");
            animatorCredits.SetTrigger("startAnimation");
            StartCoroutine(WaitAndStartStory());

        }

        IEnumerator WaitAndStartStory()
        {
            yield return new WaitForSeconds(2f);

            story.SetMainMenu(this);
            story.TriggerStartIntro();
        }

        public void IntroIsDone()
        {
            animator.SetBool("isLanding", true);

            StartCoroutine(WaitAndStartGame());
        }

        IEnumerator WaitAndStartGame()
        {
            yield return new WaitForSeconds(6.5f);

            SceneManager.LoadScene(1);
        }

    }
}
