using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.yourName
{
    public class LobbyBack : MonoBehaviour
    {
        void Start()
        {
            int randNum = Random.Range(0, 7);
            transform.GetChild(randNum).gameObject.SetActive(true);
        }
    }
}
