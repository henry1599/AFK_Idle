using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HenryDev
{
    public class ChangeableValue : MonoBehaviour, IChangeableValue
    {
        [ReadOnly, SerializeField] float maxValue;
        [ReadOnly, SerializeField] float currentValue;
        [ReadOnly, SerializeField] bool isInitialized = false;

        // * Default value
        float defaultMaxValue = 100;

        public event Action OnEmpty;
        public event Action<float> OnValueChanged;
        public void UpdateValue(float value)
        {
            ValueChecker();
            this.currentValue += value;
            if (this.currentValue <= 0)
            {
                this.currentValue = 0;
                OnEmpty?.Invoke();
            }
            OnValueChanged?.Invoke(value);
        }

        public void InitValue(float maxHealth, bool startFrom0 = true)
        {
            if (this.isInitialized)
                return;
            ForceInitValue(maxHealth, startFrom0);
        }

        public void ForceInitValue(float maxHealth, bool startFrom0 = true)
        {
            this.isInitialized = true;
            this.maxValue = maxHealth;
            this.currentValue = startFrom0 ? 0 : maxHealth;
            this.Log(string.Format("Values are initialized, maxHealth: {0}, currentHealth: {1}", this.maxValue, this.currentValue), Color.green);
        }
        public void MakeEmpty()
        {
            ValueChecker();
            float diff = -Mathf.Abs(this.currentValue);
            this.currentValue = 0;
            OnValueChanged?.Invoke(diff);
            OnEmpty?.Invoke();
        }

        public float GetMaxValue()
        {
            ValueChecker();
            return this.maxValue;
        }

        public float GetValue()
        {
            ValueChecker();
            return this.currentValue;
        }

        public float GetValueNormalized()
        {
            ValueChecker();
            return this.currentValue / this.maxValue;
        }

        void ValueChecker()
        {
            if (!this.isInitialized)
            {
                this.Log(string.Format("Values are not initialized, use default value, maxHealth: {0}, currentHealth: {1}", this.defaultMaxValue, this.defaultMaxValue), Color.cyan);
                InitValue(this.defaultMaxValue);
            }
        }

        public void UpdateMaxValue(float maxValue)
        {
            var diff = this.maxValue - maxValue;
            this.maxValue = maxValue;
            UpdateValue(diff);
        }
    }
}
