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
        private List<TextTypeWriterSingle> textTypeWriterSinlgeList;


        private void Awake()
        {
            instance = this;
            textTypeWriterSinlgeList = new List<TextTypeWriterSingle>();
        }

        public static TextTypeWriterSingle AddWriter_Static(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            instance.RemoveWriter(uiText);
            return instance.AddWriter(uiText, textToWrite, timePerCharacter, audioSource, onComplete);
        }

        private TextTypeWriterSingle AddWriter(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            TextTypeWriterSingle ttws = new TextTypeWriterSingle(uiText, textToWrite, timePerCharacter, audioSource, onComplete);
            textTypeWriterSinlgeList.Add(ttws);
            return ttws;
        }

        public static void RemoveWriter_Static(Text uiText)
        {
            instance.RemoveWriter(uiText);
        }

        private void RemoveWriter(Text uiText)
        {
            for(int i = 0; i < textTypeWriterSinlgeList.Count; i++)
            {
                if (textTypeWriterSinlgeList[i].GetText().Equals(uiText))
                {
                    textTypeWriterSinlgeList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < textTypeWriterSinlgeList.Count; i++)
            {
                bool destroyInstance = textTypeWriterSinlgeList[i].Update();
                if (destroyInstance) 
                {
                    textTypeWriterSinlgeList.RemoveAt(i);
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
        //private string rawTextToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;

        private bool isTagOpen = false;

        public TextTypeWriterSingle(Text uiText, string textToWrite, float timePerCharacter, AudioSource audioSource, Action onComplete)
        {
            this.onComplete = onComplete;
            this.audiosource = audioSource;
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            //this.rawTextToWrite = GetRawText(textToWrite);
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

                    if (textToWrite.Substring(characterIndex-1, 1).Equals("<"))
                    {
                        int indextEndTag = textToWrite.IndexOf('>', characterIndex);
                        if (indextEndTag > 0)
                        {
                            if (textToWrite.Substring(characterIndex, 1).Equals("/"))
                            {
                                isTagOpen = false;
                            }
                            else
                            {
                                isTagOpen = true;
                            }

                            characterIndex = indextEndTag + 2;
                        }
                    }

                    string text = textToWrite.Substring(0, characterIndex);
                    if (isTagOpen) text += "</color>";
                    
                    text += "<color=#00000000>" + GetRawText(textToWrite.Substring(characterIndex)) + "</color>";

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

        private String GetRawText(string text)
        {
            string remainingText = text;
            string rawText = string.Empty;

            int nextTagEnd = 0;
            int nextTagStart = remainingText.IndexOf('<');

            while (nextTagStart >= 0)
            {
                rawText += remainingText.Substring(0, nextTagStart);

                nextTagEnd = remainingText.IndexOf('>') + 1;
                remainingText = remainingText.Substring(nextTagEnd);

                nextTagStart = remainingText.IndexOf('<');
            }

            rawText += remainingText;

            return rawText;
        }
    }
}
