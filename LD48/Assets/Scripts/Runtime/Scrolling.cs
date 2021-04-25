using System;
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
        [SerializeField]
        float maxX = 10;
        [SerializeField]
        float minY = -2;
        [SerializeField]
        float maxY = 8;
        [SerializeField]
        float minZoom = 0.5f;
        [SerializeField]
        float maxZoom = 3;

        private void Start()
        {
            this.gridTransform = this.GetComponent<Transform>();
        }

        private void Update()
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

            Vector2 mouseZoom = Input.mouseScrollDelta;
            if (mouseZoom.y != 0f)
            {
                float newScale = gridTransform.localScale.y + mouseZoom.y * 10 * Time.deltaTime;

                newScale = Math.Min(newScale, maxZoom);
                newScale = Math.Max(newScale, minZoom);

                gridTransform.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }
}
