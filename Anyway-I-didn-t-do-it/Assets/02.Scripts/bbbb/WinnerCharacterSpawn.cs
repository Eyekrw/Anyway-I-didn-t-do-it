using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace SourGrape.bbbb
{
    public class WinnerCharacterSpawn : MonoBehaviourPun
    {
        bool DEBUG = true;

        public Canvas canvas;

        private int _winnerCharacter;
        private GameObject[] _charObjs;
        private Camera _cam;
        private GameObject _obj;
        private Animator _animator;

        // Start is called before the first frame update
        void Start()
        {
            _cam = FindObjectOfType<Camera>();
            _charObjs = Resources.LoadAll<GameObject>("bbbb/Characters");
            Debug.Log(_charObjs.Length);

            Vector3 pos = _cam.transform.position + new Vector3(0f, -1.2f, 2.8f);
            Quaternion rot = Quaternion.Euler(0f, 160f, 0f);
            Vector3 scale = new Vector3(1.2f, 1.2f, 1.2f);

            canvas.worldCamera = _cam;
            if (DEBUG)
            {
                _obj = Instantiate(_charObjs[0], pos, rot);
            }
            else
            {
                _winnerCharacter = (int)PhotonNetwork.CurrentRoom.CustomProperties["Winner"];
                _obj = Instantiate(_charObjs[_winnerCharacter], pos, rot);
            }
            _obj.transform.localScale = scale;
            _animator = _obj.GetComponent<Animator>();
            _animator.Play("Bear_Dance");
        }

        // Update is called once per frame
    }
}
