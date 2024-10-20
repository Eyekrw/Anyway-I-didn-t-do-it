using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace SourGrape.hongyeop
{
    public class Weapon_dummy : MonoBehaviourPun
    {
        public enum EWeaponType { Melee, Range };

        public EWeaponType WeaponType;
        public int Damage;
        public float AttackRate;

        [SerializeField]
        private BoxCollider _meleeArea;
        [SerializeField]
        private TrailRenderer _trailEffect;
        [SerializeField]
        //private Animator _anim;  // 엔진에서 가져옴


        void Start()
        {
            //_meleeArea = GetComponent<BoxCollider>();
            _trailEffect = GetComponentInChildren<TrailRenderer>();
            //_meleeArea.enabled = false;
        }

        public void Use()
        {
            if (WeaponType == EWeaponType.Melee)
            {
                StopCoroutine("Swing");
                StartCoroutine("Swing");
            }
        }

        private IEnumerator Swing()
        {
            yield return new WaitForSeconds(0.32f);
            //_meleeArea.enabled = true;
            //_anim.SetBool("isMeleeEnabled", true);
            _trailEffect.enabled = true;

            yield return new WaitForSeconds(0.12f);
            //_anim.SetBool("isMeleeEnabled", false);
            //_meleeArea.enabled = false;

            yield return new WaitForSeconds(0.3f);
            _trailEffect.enabled = false;
        }

        //private void OnDrawGizmos()
        //{
        //    //bool isMeleeEnabled = _anim.GetBool("isMeleeEnabled");
        //    if (isMeleeEnabled)
        //    {
        //        Gizmos.color = Color.red;
        //        Gizmos.DrawWireCube(_meleeArea.bounds.center, _meleeArea.bounds.size);
        //    }
        //    //if (_meleeArea != null && _meleeArea.enabled)  // Collider가 활성화되었을 때만
        //    //{
        //    //    Gizmos.color = Color.red;  // Gizmo 색상 설정
        //    //    Gizmos.DrawWireCube(_meleeArea.bounds.center, _meleeArea.bounds.size);  // BoxCollider의 크기와 위치에 맞춰 와이어 박스 그리기
        //    //}
        //}
    }
}

