using System.Collections;
using UnityEngine;

namespace SourGrape.minwu
{
    public class Palette : MonoBehaviour
    {
        public float speed = 5f;
        public GameObject PlayerObject; 
        public GameObject OtherPaletteObject; 

        private Vector3 _startPosition;
        private Vector3 _leftPosition;
        private Vector3 _rightPosition;


        private void Start()
        {
            _startPosition = new Vector3(0f, -2.5f, 10f);
            _leftPosition = new Vector3(-10f, -2.5f, 10f);
            _rightPosition = new Vector3(10f, -2.5f, 10f);

            transform.position = _startPosition;

            StartCoroutine(MovePalette());
        }

        private IEnumerator MovePalette()
        {
            while (true)
            {
                yield return StartCoroutine(MoveToPosition(_leftPosition));

 
                yield return StartCoroutine(MoveToPosition(_rightPosition));
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == PlayerObject)
            {
                PlayerObject.transform.SetParent(transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == PlayerObject)
            {
                PlayerObject.transform.SetParent(null);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == OtherPaletteObject)
            {
                return;
            }
        }
    }
}