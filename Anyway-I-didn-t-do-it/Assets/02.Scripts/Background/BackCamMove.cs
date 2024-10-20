using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.yewon
{
    public class BackCamMove : MonoBehaviour
    {
        public float Speed;

        private Vector3 _destPos;
        private GameObject _camera;
        private float _target;

        private void Awake()
        {
            _camera = GameObject.Find("Main Camera");
            _target = Time.time + 4f;
        }
        void Start()
        {
            _destPos = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 20f);
        }

        void Update()
        {
            if (_target < Time.time)
            {
                _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, _destPos, Speed * Time.deltaTime);
            }
        }
    }
}
