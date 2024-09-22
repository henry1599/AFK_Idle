using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFK.Idle
{
    public class HeroController : MonoBehaviour
    {
        UnitPrefab heroPrefab;
        public void Setup()
        {
            this.heroPrefab = GetComponent<UnitPrefab>();
            if (this.heroPrefab == null)
            {
                Debug.LogError("Hero prefab not found");
                return;
            }
            SetOrientation();
        }
        void SetOrientation()
        {
            // Hard code setting orientation for now
            transform.localScale = new Vector3(-1, 1, 1);
        }
        public void PlayAttackAnim(float attackSpeed) => this.heroPrefab?.PlayAttack(attackSpeed);
        public void PlayDeathAnim() => this.heroPrefab?.PlayDeath();
        public void PlayMoveAnim() => this.heroPrefab?.PlayMove();
    }
}
