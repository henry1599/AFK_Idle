using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using HenryDev;
using System;
using HenryDev.Gameplay;
using HenryDev.Math;

namespace AFK.Idle
{
    public class UnitMonoBehaviour : MonoBehaviour
    {
        public enum eState
        {
            Idle,
            Attack,
            Move
        }
        [BoxGroup("Setup"), SerializeField] Transform attackPoint;
        [BoxGroup("Setup"), SerializeField] float attackRange;
        [BoxGroup("Data"), ReadOnly, SerializeField] float attackSpeed;
        [BoxGroup("Data"), ReadOnly, SerializeField] string attackDamage;
        [SerializeField] eState state = default;
        private float attackTimer;
        private Controller controller;
        private bool isSetup = false;
        private Transform sightPoint;
        private Vector2 boxSightSize;
        private string enemyTag;
        public void Setup(GlobalStatsSaveData statData, Controller controller, Transform sightPoint, Vector2 boxSightSize, string enemyTag = "Player")
        {
            this.controller = controller;
            this.sightPoint = sightPoint;
            this.boxSightSize = boxSightSize;
            this.enemyTag = enemyTag;
            this.isSetup = true;

            this.attackDamage = statData.Attack;
            this.attackSpeed = statData.AttackSpeed;
        }

        private void ResetTimer()
        {
            this.attackTimer = 1 / this.attackSpeed;
        }

        void Update()
        {
            var enemyInSight = CastEnemyInSightZone();
            var enemyInAttack = CastEnemyInAttackZone();
            if (enemyInAttack != null)
            {
                state = eState.Attack;
                AttackState(enemyInAttack);
                return;
            }
            if (enemyInSight != null)
            {
                state = eState.Move;
                MoveState(enemyInSight);
                return;
            }
            state = eState.Idle;
            IdleState();
        }

        private void IdleState()
        {
            this.controller?.PlayIdleAnim();
        }

        private void AttackState(GameObject target)
        {
            if (target == null) return;
            if (this.attackTimer > 0)
            {
                this.attackTimer -= Time.deltaTime;
                return;
            }
            ResetTimer();
            Vector3 direction = (target.transform.position - transform.position).normalized;
            controller.FaceDirection(direction);
            controller.PlayAttackAnim(this.attackSpeed);
            Health health = target.GetComponent<Health>();
            if (health == null)
            {
                state = eState.Idle;
                return;
            }
            health.UpdateValue(new IInt("-" + this.attackDamage));
        }

        private void MoveState(GameObject target)
        {
            if (target == null) return;
            Vector3 direction = (target.transform.position - transform.position).normalized;
            controller.FaceDirection(direction);
            controller.PlayMoveAnim();
            controller.MoveTo(target.transform.position);
        }

        GameObject CastEnemyInSightZone()
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapBoxAll(sightPoint.position, boxSightSize, 0);
            GameObject target = null;
            float nearestDistance = float.MaxValue;

            foreach (var enemy in enemiesInRange)
            {
                if (!enemy.gameObject.CompareTag(this.enemyTag))
                    continue;
                float distance = Vector2.SqrMagnitude(sightPoint.position - enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    target = enemy.gameObject;
                }
            }
            return target;
        }
        GameObject CastEnemyInAttackZone()
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(this.attackPoint.position, this.attackRange);
            GameObject target = null;
            float nearestDistance = float.MaxValue;

            foreach (var enemy in enemiesInRange)
            {
                if (!enemy.gameObject.CompareTag(this.enemyTag))
                    continue;
                float distance = Vector2.SqrMagnitude(sightPoint.position - enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    target = enemy.gameObject;
                }
            }
            return target;
        }
        void OnDrawGizmos()
        {
            if (this.attackPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(this.attackPoint.position, attackRange);
            }
        }
    }
}