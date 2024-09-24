using System;
using UnityEngine;

namespace HenryDev
{
    public class Health : ChangeableValueIInt
    {
        void Awake()
        {
            OnEmpty += OnDeath;
        }
        void OnDestroy()
        {
            OnEmpty -= OnDeath;
        }

        private void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}