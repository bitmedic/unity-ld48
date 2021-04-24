using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class Scrolling : MonoBehaviour
    {
        Transform gridTransform;

        [SerializeField]
        float minX = -7;
        float maxX = 10;
        float minY = -2;
        float maxY = 8;

        private void Start()
        {
            this.gridTransform = this.GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            float scrollX = Input.GetAxisRaw("Horizontal");
            float scrollY = Input.GetAxisRaw("Vertical");
            if (scrollX != 0f || scrollY != 0f)
            {
                Vector3 asteroidPosition = this.gridTransform.position;
                asteroidPosition.x -= scrollX * 10 * Time.deltaTime;
                asteroidPosition.y -= scrollY * 10 * Time.deltaTime;

                asteroidPosition.x = Mathf.Min(asteroidPosition.x, maxX);
                asteroidPosition.x = Mathf.Max(asteroidPosition.x, minX);
                asteroidPosition.y = Mathf.Min(asteroidPosition.y, maxY);
                asteroidPosition.y = Mathf.Max(asteroidPosition.y, minY);

                this.gridTransform.position = asteroidPosition;
            }
        }
    }
}
