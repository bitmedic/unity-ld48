using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD48
{
    public class SingleBuildButton : MonoBehaviour
    {
        Image image;
        bool isSelected = false;


        private void Start()
        {
            this.image = this.GetComponent<Image>();
        }

        public void ToggleSelected()
        {
            if (this.isSelected)
            {
                this.SetUnselected();
            }
            else
            {
                // select
                image.color = new Color(59f/255, 221f/255, 39f/255);
                this.isSelected = true;
            }
        }

        public void SetUnselected()
        {
            float colorGrey = 183f / 255;
            image.color = new Color(colorGrey, colorGrey, colorGrey);
            this.isSelected = false;
        }
    }
}
