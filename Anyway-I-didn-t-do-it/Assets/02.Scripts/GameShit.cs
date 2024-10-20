using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.yourName
{
    public class GameShit : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
