using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using HenryDev;

namespace AFK.Idle
{
    public class HeroAttack : MonoBehaviour
    {
        [BoxGroup("Attack Zone"), SerializeField] Transform attackBoxPoint;
        [BoxGroup("Attack Zone"), SerializeField] Vector2 boxSize;
        [BoxGroup("Data"), ReadOnly, SerializeField] float attackSpeed;
        [BoxGroup("Data"), ReadOnly, SerializeField] float attackDamage;
        private float attackTimer;
        private HeroController controller;
        private bool isSetup = false;
        public void Setup(HeroController controller, UnitData data)
        {
            this.controller = controller;
            // this.attackSpeed = data.
            this.isSetup = true;
            ResetTimer();
        }
        void Update()
        {
            if (this.attackTimer > 0)
            {
                this.attackTimer -= Time.deltaTime;
                return;
            }
            GameObject nearestEnemy = GetNearestEnemyInZone();
            if (nearestEnemy == null)
                return;
            Attack(nearestEnemy);
            ResetTimer();
        }
        void ResetTimer()
        {
            this.attackTimer = 1f / this.attackSpeed;
        }
        void Attack(GameObject enemy)
        {
            this.controller.PlayAttackAnim(this.attackSpeed);
            Health health = enemy.GetComponent<Health>();
            if (health == null)
                return;
            health.UpdateValue(-this.attackDamage);
        }
        List<GameObject> GatherEnemyInZone()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(attackBoxPoint.position, boxSize, 0);
            List<GameObject> enemies = new List<GameObject>();
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    enemies.Add(collider.gameObject);
                }
            }
            return enemies;
        }
        GameObject GetNearestEnemyInZone()
        {
            List<GameObject> enemies = GatherEnemyInZone();
            if (enemies.Count == 0)
            {
                return null;
            }
            GameObject nearestEnemy = enemies[0];
            float minDistance = Vector2.SqrMagnitude(transform.position - nearestEnemy.transform.position);
            foreach (var enemy in enemies)
            {
                float distance = Vector2.SqrMagnitude(transform.position - enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }
            return nearestEnemy;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackBoxPoint.position, boxSize);
        }
    }
}
