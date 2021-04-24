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
        float maxX = 10;
        float minY = -2;
        float maxY = 8;
        float minZoom = 2;
        float maxZoom = 10;

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
                Camera.main.orthographicSize -= mouseZoom.y * 100 * Time.deltaTime;

                Camera.main.orthographicSize = Math.Min(Camera.main.orthographicSize, maxZoom);
                Camera.main.orthographicSize = Math.Max(Camera.main.orthographicSize, minZoom);
            }
        }
    }
}
