using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class EarthAnimation : MonoBehaviour
    {
        public Animator animator;
        public ProgressUI progressui;

        // Start is called before the first frame update
        void Start()
        {
            // 4,5 sekunden animation
            float speed = (4.5f / progressui.impactTimeTotal);
            animator.SetFloat("earthSpeedMultiplier", speed);
        }
    }
}
