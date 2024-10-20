using UnityEngine;
using System.Collections;

namespace SourGrape.hongyeop
{
    public class ArrowEffects : MonoBehaviour
    {
        public GameObject ImpactParticle; // Effect spawned when projectile hits a collider
        public GameObject ProjectileParticle; // Effect attached to the gameobject as child
        public GameObject MuzzleParticle; // Effect instantly spawned when gameobject is spawned
        [Header("IF not Parent Sphere Collider")]
        public float ColliderRadius = 1f;
        [Range(0f, 1f)] // This is an offset that moves the impact effect slightly away from the point of impact to reduce clipping of the impact effect
        public float CollideOffset = 0.15f;

        private Rigidbody _parentRigidbody;
        private SphereCollider _parentSphereCollider;

        public void Boom()
        {
            ImpactParticle = Instantiate(ImpactParticle, transform.position, transform.rotation);
            Destroy(ImpactParticle, 3.5f);
        }

        void Start()
        {
            // 부모의 Rigidbody와 SphereCollider 참조
            _parentRigidbody = GetComponentInParent<Rigidbody>();
            _parentSphereCollider = GetComponentInParent<SphereCollider>();

            ProjectileParticle = Instantiate(ProjectileParticle, transform.position, transform.rotation) as GameObject;
            ProjectileParticle.transform.parent = transform;
            if (MuzzleParticle) // 화살 발사 될 때 effect 
            {
                MuzzleParticle = Instantiate(MuzzleParticle, transform.position, transform.rotation) as GameObject;
                Destroy(MuzzleParticle, 1.5f); // 2nd parameter is lifetime of effect in seconds
            }
        }
    }
}
