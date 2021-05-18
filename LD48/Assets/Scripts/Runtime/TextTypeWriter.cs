using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class TextTypeWriter : MonoBehaviour
    {
        private static TextTypeWriter instance;
        private List<TextTypeWriterSingle> textTypeWriterSingleList;


        private void Awake()
        {
            instance = this;
            textTypeWriterSingleList = new List<TextTypeWriterSingle>();
        }

        public static TextTypeWriterSingle AddWriter_Static(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            instance.RemoveWriter(uiText);
            return instance.AddWriter(uiText, textToWrite, timePerCharacter, audioSource, onComplete);
        }

        private TextTypeWriterSingle AddWriter(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            TextTypeWriterSingle ttws = new TextTypeWriterSingle(uiText, textToWrite, timePerCharacter, audioSource, onComplete);
            textTypeWriterSingleList.Add(ttws);
            return ttws;
        }

        public static void RemoveWriter_Static(Text uiText)
        {
            instance.RemoveWriter(uiText);
        }

        private void RemoveWriter(Text uiText)
        {
            for(int i = 0; i < textTypeWriterSingleList.Count; i++)
            {
                if (textTypeWriterSingleList[i].GetText().Equals(uiText))
                {
                    textTypeWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < textTypeWriterSingleList.Count; i++)
            {
                bool destroyInstance = textTypeWriterSingleList[i].Update();
                if (destroyInstance) 
                {
                    textTypeWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public class TextTypeWriterSingle
    {
        private AudioSource audiosource;
        private Action onComplete;
        private Text uiText;
        private string textToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;

        public TextTypeWriterSingle(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            this.onComplete = onComplete;
            this.audiosource = audioSource;
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            this.timePerCharacter = timePerCharacter;
            this.characterIndex = 0;
        }

        public bool Update()
        {
            if (uiText != null)
            {
                timer -= Time.deltaTime;
                while(timer <= 0f)
                {
                    timer += timePerCharacter;
                    characterIndex++;
                    string text = textToWrite.Substring(0, characterIndex);
                    text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";

                    if (characterIndex % 2 == 0)
                    {
                        audiosource.Play(); // play soudn every 2nd character
                    }

                    uiText.text = text;

                    if (characterIndex >= textToWrite.Length)
                    {
                        if (onComplete != null) onComplete();

                        uiText = null;
                        return true;
                    }
                }
            }

            return false;
        }

        public Text GetText()
        {
            return this.uiText;
        }

        public bool IsActive()
        {
            return characterIndex < textToWrite.Length;
        }

        public void WriteAllAndDestroy()
        {
            uiText.text = textToWrite;
            characterIndex = textToWrite.Length;
            if (onComplete != null) onComplete();
            TextTypeWriter.RemoveWriter_Static(uiText);
        }
    }
}
