using SourGrape.hongyeop;
using System.Collections;
using UnityEngine;

namespace SourGrape.kiyoung
{
    public class Arrow : MonoBehaviour
    {

        [SerializeField]
        private float Speed;
        [SerializeField]
        private float _lifetime = 20f;
        private Rigidbody _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // 자식 오브젝트의 ArrowEffects 컴포넌트의 Boom 메서드 호출
                ArrowEffects arrowEffects = GetComponentInChildren<ArrowEffects>();
                if (arrowEffects != null)
                {
                    arrowEffects.Boom(); // 폭발 이펙트 생성
                }
                gameObject.SetActive(false); // 화살 제거
            }
        }

        public void Shoot(Vector3 spawnPosition, Vector3 direction, float speed)
        {
            transform.position = spawnPosition;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
            StartCoroutine(DisableArrowAfterTime());
        }

        private IEnumerator DisableArrowAfterTime()
        {
            yield return new WaitForSeconds(_lifetime);
            gameObject.SetActive(false);
        }
    }
}