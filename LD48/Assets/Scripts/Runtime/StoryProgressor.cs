using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        [Header("Refrences")]
        public AssemblyManager assemblyManager;
        public Image frameTextArea;
        public Text textArea;
        public Machine Rocket;

        private Machine rocket;
        private StoryStep enumStoryStep;
        private int indexStoryText = 0;
        private bool isTextShown = false;


        private void Start()
        {
            this.rocket = assemblyManager.GetRocket();
            this.rocket.OnOutputProduced += new OutputProduced(RocketOutputProduced);
        }

        private void Update()
        {
            if (rocket == null)
            {
                this.rocket = assemblyManager.GetRocket();
                this.rocket.OnOutputProduced += new OutputProduced(RocketOutputProduced);
            }

            if (isTextShown && Input.anyKeyDown)
            {
                this.ShowNextText();
            }            
        }

        public void RocketOutputProduced(Machine machine, List<Package> packages)
        {
            Debug.Log("Output prodúced! " + machine + ", " + packages);
        }

        public void TriggerStartIntro()
        {
            this.enumStoryStep = StoryStep.Intro;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerAfterLanding()
        {
            this.enumStoryStep = StoryStep.AfterLanding;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerAfterTier1()
        {
            this.enumStoryStep = StoryStep.AfterTier1;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerAfterTier2()
        {
            this.enumStoryStep = StoryStep.AfterTier2;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerAfterTier3()
        {
            this.enumStoryStep = StoryStep.AfterTier3;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerVictory()
        {
            this.enumStoryStep = StoryStep.Victory;
            indexStoryText = -1;
            this.ShowNextText();
        }

        public void TriggerDefeat()
        {
            this.enumStoryStep = StoryStep.Defeat;
            indexStoryText = -1;
            this.ShowNextText();
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
