//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace bbbb
{
    public class CharacterSelect : MonoBehaviour
    {
        #region public variable
        public GameObject LookTarget;
        #endregion
        #region private variable
        private GameObject[] prefabs;
        private int _currentIndex = 0;
        private GameObject _obj;
        private Hashtable _hashtable;

        private string[] animations;
        #endregion


        // Start is called before the first frame update
        void Start()
        {
            prefabs = Resources.LoadAll<GameObject>("bbbb/Characters");
            animations = new string[5];
            animations[0] = "Jump";
            animations[1] = "Hello";
            animations[2] = "Hiding";
            animations[3] = "Dance";
            animations[4] = "Attack";

            spawnCharacter();
        }

        // Update is called once per frame
        void Update()
        {
            if (_obj != null) _obj.transform.LookAt(LookTarget.transform);
        }

        #region public function
        public void BtnPrev()
        {
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = prefabs.Length - 1;
            spawnCharacter();
        }
        public void BtnNext()
        {
            _currentIndex++; _currentIndex %= prefabs.Length;
            spawnCharacter();
        }
        #endregion

        #region private function
        private void spawnCharacter()
        {
            if (_obj != null) Destroy(_obj);
            Vector3 pos = transform.position;
            _obj = Instantiate(prefabs[_currentIndex], pos + new Vector3(0, -15.0f, 0), Quaternion.identity);
            _obj.transform.localScale = new Vector3(15, 15, 15);

            int randNum = Random.Range(0, 5);
            string prefabName = prefabs[_currentIndex].name;
            string characterType = "";
            for (int i = 0; i < prefabName.Length; i++)
            {
                if (prefabName[i] == ' ' || prefabName[i] == '\0')
                    break;
                characterType += prefabName[i];
            }
            string animationName = characterType + "_" + animations[randNum];
            _obj.transform.GetComponent<Animator>().Play(animationName);
            
            PlayerPrefs.SetInt("Character", _currentIndex);
            PlayerPrefs.SetString("CharacterName", prefabs[_currentIndex].name); // for loading player prefab
            PlayerPrefs.Save();
            //_hashtable = PhotonNetwork.LocalPlayer.CustomProperties; // PlayerPrefs 사용에 따른 Photon 제거
            //_hashtable["Character"] = _currentIndex;
            //PhotonNetwork.LocalPlayer.SetCustomProperties(_hashtable);
        }
        #endregion

    }
}