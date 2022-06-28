using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MyCat
{
    public class Dongle : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            MoveMouse();
        }

        void MoveMouse()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0.0f;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.01f);
        }
    }
}
