using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Enemy
{
    public class EnemySightSensor : MonoBehaviour
    {
        public Transform Player { get; private set; }

        [SerializeField] private LayerMask _ignoreMask;

        private Ray _ray;

        private Animator anim;

        private void Awake()
        {
            Player = GameObject.Find("Player").transform;
            anim = GetComponentInChildren<Animator>();
        }

        public bool Ping()
        {
            if (Player == null)
                return false;

            _ray = new Ray(this.transform.position, Player.position - this.transform.position);

            var dir = new Vector3(_ray.direction.x, 0, _ray.direction.z);

            var angle = Vector3.Angle(dir, this.transform.forward);

            if (angle > 60)
                return false;

            if (!Physics.Raycast(_ray, out var hit, 100, ~_ignoreMask))
            {
                return false;
            }

            if (hit.collider.tag == "Player")
            {
                // NOTE: probably would be cleaner to move this to EnemyAnimation.cs,
                //       but don't currently have time to figure out how to obtain reference
                //       to state from there
                //
                // Set trigger for chase animation
                anim.SetTrigger("Walking2");

                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * 100);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 100);
        }
    }
}