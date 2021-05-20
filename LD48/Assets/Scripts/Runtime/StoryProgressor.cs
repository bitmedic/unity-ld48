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
        [Header("Text")] [TextArea] 
        public List<string> storyTextIntro;
        [TextArea] public List<string> storyTextAfterLanding;
        [TextArea] public List<string> storyTextAfterTier1;
        [TextArea] public List<string> storyTextAfterTier2;
        [TextArea] public List<string> storyTextAfterTier3;
        [TextArea] public List<string> storyPressButtonNow;
        [TextArea] public List<string> storyVictory;
        [TextArea] public List<string> storyDefeat;

        [Header("References")] 
        public AssemblyManager assemblyManager;
        public Animator defeatAnimation;
        public Image frameTextArea;
        public Text textArea;
        public Text textPressSpace;
        public AudioSource audioSource;

        [Header("Parameters")] 
        public float characterSpeed;

        [HideInInspector] public Machine rocket;
        private MainMenu mainMenu;
        private StoryStep enumStoryStep;
        private int indexStoryText = 0;
        [HideInInspector] public bool isTextShown = false;

        [HideInInspector]
        public Action onTier1Complete;
        [HideInInspector]
        public Action onTier2Complete;

        private bool hasIntroTriggered = false;
        private bool hasLandingTriggered = false;
        private bool hasTier1Triggered = false;
        private bool hasTier2Triggered = false;
        private bool hasTier3Triggered = false;
        private bool hasPressButtonNowTriggered = false;
        private bool hasVictoryTriggered = false;
        private bool hasDefeatTriggered = false;

        private TextTypeWriterSingle textTypeWriterInstance;
        private void Start()
        {
        }

        private void Update()
        {
            if (rocket == null)
            {
                this.rocket = assemblyManager?.GetRocket();
                this.rocket.OnOutputProduced += (machine, packages) => { RocketOutputProduced(machine, packages); };
            }

            if (isTextShown && Input.GetKeyUp(KeyCode.Space))
            {
                audioSource.Play();

                // check if texttypewriter is still active
                if (textTypeWriterInstance != null && textTypeWriterInstance.IsActive())
                {
                    // show all text
                    textTypeWriterInstance.WriteAllAndDestroy();
                }
                else
                {
                    this.ShowNextText();
                }
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

                if (onTier1Complete != null)
                {
                    onTier1Complete();
                }

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

                if (onTier2Complete != null)
                {
                    onTier2Complete();
                }

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
        
        public void TriggerPressButtonNow()
        {
            if (!this.hasPressButtonNowTriggered)
            {
                this.hasPressButtonNowTriggered = true;
                this.enumStoryStep = StoryStep.PressButtonNow;
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
                this.defeatAnimation.SetBool("isDefeat", true);

                this.ShowNextText();
            }
        }

        public void RegisterTierCompleteActions(Action onTier1Complete, Action onTier2Complete)
        {
            this.onTier1Complete = onTier1Complete;
            this.onTier2Complete = onTier2Complete;
        }

        public void SetMainMenu(MainMenu mm)
        {
            this.mainMenu = mm;
        }

        public void ShowNextText()
        {
            this.indexStoryText++;
            
            List<string> currentStoryText = this.GetCurrentStory();

            this.isTextShown = true;
            this.frameTextArea.gameObject.SetActive(true);


            if (currentStoryText != null)
            {
                if (currentStoryText.Count > indexStoryText)
                {
                    // textPressSpace.gameObject.SetActive(false);
                    textTypeWriterInstance = TextTypeWriter.AddWriter_Static(this.textArea, currentStoryText[indexStoryText], characterSpeed, audioSource, TextAnimationComplete);
                    
                    //this.textArea.text = currentStoryText[indexStoryText];
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

                    textTypeWriterInstance.WriteAllAndDestroy(); // this also stops the sound
                    this.isTextShown = false;
                    this.frameTextArea.gameObject.SetActive(false);
                }
            }
        }

        private void TextAnimationComplete()
        {
            textPressSpace.gameObject.SetActive(true);
        }

        private List<string> GetCurrentStory()
        {
            switch(enumStoryStep)
            {
                case StoryStep.Intro:
                    return this.storyTextIntro;
                case StoryStep.AfterLanding:
                    return this.storyTextAfterLanding;
                case StoryStep.AfterTier1:
                    return this.storyTextAfterTier1;
                case StoryStep.AfterTier2:
                    return this.storyTextAfterTier2;
                case StoryStep.AfterTier3:
                    return this.storyTextAfterTier3;
                case StoryStep.PressButtonNow:
                    return this.storyPressButtonNow;
                case StoryStep.Victory:
                    return this.storyVictory;
                case StoryStep.Defeat:
                    return this.storyDefeat;
            }
            throw new ArgumentOutOfRangeException();
        }
    }

    public enum StoryStep
    {
        Intro,
        AfterLanding,
        AfterTier1,
        AfterTier2,
        AfterTier3,
        PressButtonNow,
        Victory,
        Defeat
    }
}