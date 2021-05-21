using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD48
{
    public class MainMenu : MonoBehaviour
    {
        public Animator animator;
        public Animator animatorCredits;
        public StoryProgressor story;
        public GameObject helpPanel;

        private void Start()
        {
            //earth.position = new Vector3(-347, -264, 0);

#if UNITY_WEBGL
            transform.Find("Buttons/ButtonExit").gameObject.SetActive(false);
#endif

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (helpPanel.activeSelf)
                {
                    this.HideHelp();
                }
            }
        }

        public void StartGame()
        {
            transform.Find("Buttons")?.gameObject.SetActive(false);
            
            animator.SetTrigger("startAnimation");
            animatorCredits.SetTrigger("startAnimation");
            StartCoroutine(WaitAndStartStory());

        }

        public void ExitGame()
        {
            Application.Quit(); // ignored in editor
        }

        public void ShowHelp()
        {
            helpPanel.SetActive(true);
        }

        public void HideHelp()
        {
            helpPanel.SetActive(false);
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
