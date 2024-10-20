using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SourGrape.hongyeop
{
    public class PlayerGhostStatusController : MonoBehaviour
    {
        #region public properties
        public bool IsDead = false; // 죽음 여부
        #endregion

        #region private properties
        private Animator _anim;
        #endregion

        void Start()
        {
            _anim = GetComponent<Animator>();
            _anim.applyRootMotion = false;  // Root Motion을 비활성화하여 애니메이션이 회전에 영향을 주지 않도록 함
            GameObject summonParticle = PhotonNetwork.Instantiate("Effect/GrenadeExplosionBlue", transform.position, transform.rotation);
            StartCoroutine(DestroyAfterDelay(summonParticle, 2f));
        }

        void Update()
        {
            if (IsDead)
            {
                Death();
            }
        }

        private void Death()
        {
            IsDead = false;
            StartCoroutine(Dying());
        }

        private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            PhotonNetwork.Destroy(obj);
        }

        private IEnumerator Dying()
        {
            _anim.SetTrigger("Die");
            // 지연 시간 동안 대기
            yield return new WaitForSeconds(2);

            // 오브젝트 자체를 비활성화 (렌더링과 상호작용을 비활성화)
            GameObject summonParticle = PhotonNetwork.Instantiate("Effect/GrenadeExplosionBlue", transform.position, transform.rotation);
            StartCoroutine(DestroyAfterDelay(summonParticle, 2f));
            PhotonNetwork.Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}
