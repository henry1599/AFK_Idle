using System.Collections;
using System.Collections.Generic;
using HenryDev;
using HenryDev.Math;
using UnityEngine;

namespace AFK.Idle
{
    public class Enemy : MonoBehaviour
    {
        public int MaxHealth;
        private Health health;
        public void Start()
        {
            this.health = GetComponent<Health>();
            this.health?.InitValue(new IInt(this.MaxHealth));
        }
    }
}
