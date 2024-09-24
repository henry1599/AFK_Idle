using UnityEngine;

namespace AFK.Idle
{
    public class Controller : MonoBehaviour
    {
        UnitPrefab prefab;
        public void Setup(UnitPrefab prefab)
        {
            this.prefab = prefab;
            SetOrientation();
        }
        void SetOrientation()
        {
            // Hard code setting orientation for now
            transform.localScale = new Vector3(-1, 1, 1);
        }
        public void PlayAttackAnim(float attackSpeed) => this.prefab?.PlayAttack(attackSpeed);
        public void PlayDeathAnim() => this.prefab?.PlayDeath();
        public void PlayMoveAnim() => this.prefab?.PlayMove();
        public void PlayIdleAnim() => this.prefab?.PlayIdle();

        public void FaceDirection(Vector3 direction)
        {
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public void MoveTo(Vector3 position)
        {
            var direction = position - this.prefab.transform.position;
            direction.Normalize();
            this.transform.position += direction * Time.deltaTime * 1;
        }   
    }
}