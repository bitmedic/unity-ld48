using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD48
{
    public class StoryProgressor : MonoBehaviour
    {
        [Header("Text")]
        public List<string> storyTextIntro;
        
        public List<string> storyTextAfterLanding;
        public List<string> storyTextAfterTier1;
        public List<string> storyTextAfterTier2;
        public List<string> storyTextAfterTier3;
        public List<string> storyVictory;
        public List<string> storyDefeat;

        [Header("Refrences")]
        public AssemblyManager assemblyManager;
        public Image frameTextArea;
        public Text textArea;

        [HideInInspector]
        public Machine rocket;
        private MainMenu mainMenu;
        private StoryStep enumStoryStep;
        private int indexStoryText = 0;
        [HideInInspector]
        public bool isTextShown = false;

        private bool hasIntroTriggered = false;
        private bool hasLandingTriggered = false;
        private bool hasTier1Triggered = false;
        private bool hasTier2Triggered = false;
        private bool hasTier3Triggered = false;
        private bool hasVictoryTriggered = false;
        private bool hasDefeatTriggered = false;

        private void Start()
        {
        }

        private void Update()
        {
            if (rocket == null)
            {
                this.rocket = assemblyManager?.GetRocket();
                this.rocket.OnOutputProduced += (machine, packages) =>
                {
                    RocketOutputProduced(machine, packages);
                };
            }

            if (isTextShown && Input.anyKeyDown)
            {
                this.ShowNextText();
            }            
        }

        public void RocketOutputProduced(Machine machine, List<Package> packages)
        {
            if (packages != null && packages.Count > 0)
            {
                if (packages[0].material.Equals("depth"))
                {
                    this.TriggerAfterTier1();
                }
                else if (packages[0].material.Equals("stability"))
                {
                    this.TriggerAfterTier2();
                }
                else if (packages[0].material.Equals("explosiveness"))
                {
                    this.TriggerAfterTier3();
                }
            }
        }

        public void TriggerStartIntro()
        {
            if (!this.hasIntroTriggered)
            {
                this.hasIntroTriggered = true;
                this.enumStoryStep = StoryStep.Intro;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerAfterLanding()
        {
            if (!this.hasLandingTriggered)
            {
                this.hasLandingTriggered = true;
                this.enumStoryStep = StoryStep.AfterLanding;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerAfterTier1()
        {
            if (!this.hasTier1Triggered)
            {
                this.hasTier1Triggered = true;
                this.enumStoryStep = StoryStep.AfterTier1;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerAfterTier2()
            {
            if (!this.hasTier2Triggered)
            {
                this.hasTier2Triggered = true;
                this.enumStoryStep = StoryStep.AfterTier2;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerAfterTier3()
        {
            if (!this.hasTier3Triggered)
            {
                this.hasTier3Triggered = true;
                this.enumStoryStep = StoryStep.AfterTier3;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerVictory()
        {
            if (!this.hasVictoryTriggered)
            {
                this.hasVictoryTriggered = true;
                this.enumStoryStep = StoryStep.Victory;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void TriggerDefeat()
        {
            if (!this.hasDefeatTriggered)
            {
                this.hasDefeatTriggered = true;
                this.enumStoryStep = StoryStep.Defeat;
                indexStoryText = -1;
                this.ShowNextText();
            }
        }

        public void SetMainMenu(MainMenu mm)
        {
            this.mainMenu = mm;
        }
        
        private void ShowNextText()
        {
            this.indexStoryText++;

            this.isTextShown = true;
            this.frameTextArea.gameObject.SetActive(true);

            List<string> currentStoryText = this.GetCurrentStory();

            if (currentStoryText != null)
            {
                if (currentStoryText.Count > indexStoryText)
                {
                    this.textArea.text = currentStoryText[indexStoryText];
                }
                else
                {
                    if (this.hasIntroTriggered == true && this.hasLandingTriggered == false)
                    {
                        if (this.mainMenu != null)
                        {
                            this.mainMenu.IntroIsDone();
                        }
                    }
                    if (this.hasVictoryTriggered == true || this.hasDefeatTriggered == true)
                    {
                        SceneManager.LoadScene(0);
                    }

                    this.isTextShown = false;
                    this.frameTextArea.gameObject.SetActive(false);
                }
            }
        }

        private List<string> GetCurrentStory()
        {
            if (enumStoryStep.Equals(StoryStep.Intro))
            {
                return this.storyTextIntro;
            }
            if (enumStoryStep.Equals(StoryStep.AfterLanding))
            {
                return this.storyTextAfterLanding;
            }
            if (enumStoryStep.Equals(StoryStep.AfterTier1))
            {
                return this.storyTextAfterTier1;
            }
            if (enumStoryStep.Equals(StoryStep.AfterTier2))
            {
                return this.storyTextAfterTier2;
            }
            if (enumStoryStep.Equals(StoryStep.AfterTier3))
            {
                return this.storyTextAfterTier3;
            }
            if (enumStoryStep.Equals(StoryStep.Victory))
            {
                return this.storyVictory;
            }
            if (enumStoryStep.Equals(StoryStep.Defeat))
            {
                return this.storyDefeat;
            }
            return null;
        }
    }

    public enum StoryStep
    {
        Intro,
        AfterLanding,
        AfterTier1,
        AfterTier2,
        AfterTier3,
        Victory,
        Defeat
}
}
