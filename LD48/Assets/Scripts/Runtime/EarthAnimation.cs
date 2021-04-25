using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class EarthAnimation : MonoBehaviour
    {
        public Animator animator;
        public float secondsDuration;

        // Start is called before the first frame update
        void Start()
        {
            // 4,5 sekunden animation
            float speed = (4.5f / secondsDuration);
            animator.SetFloat("earthSpeedMultiplier", speed);
        }
    }
}
