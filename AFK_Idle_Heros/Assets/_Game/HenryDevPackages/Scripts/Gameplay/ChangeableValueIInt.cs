using System;
using HenryDev.Math;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HenryDev
{
    public class ChangeableValueIInt : MonoBehaviour, IChangeableValue<IInt>
    {
        [ReadOnly, SerializeField] IInt maxValue;
        [ReadOnly, SerializeField] IInt currentValue;
        [ReadOnly, SerializeField] bool isInitialized = false;

        // * Default value
        IInt defaultMaxValue = IInt.Zero;

        public event Action OnEmpty;
        public event Action<IInt> OnValueChanged;
        public void UpdateValue(IInt value)
        {
            ValueChecker();
            this.currentValue = this.currentValue + value;
            if (this.currentValue.IsLessThan(0) || this.currentValue.IsEqualTo(0))
            {
                this.currentValue = IInt.Zero;
                OnEmpty?.Invoke();
            }
            OnValueChanged?.Invoke(value);
        }

        public void InitValue(IInt maxHealth, bool startFrom0 = true)
        {
            if (this.isInitialized)
                return;
            ForceInitValue(maxHealth, startFrom0);
        }

        public void ForceInitValue(IInt maxHealth, bool startFrom0 = true)
        {
            this.isInitialized = true;
            this.maxValue = maxHealth;
            if (startFrom0)
            {
                this.currentValue.Assign(0);
            }
            else
            {
                this.currentValue.Assign(maxHealth);
            }
            this.Log(string.Format("Values are initialized, maxHealth: {0}, currentHealth: {1}", this.maxValue, this.currentValue), Color.green);
        }
        public void MakeEmpty()
        {
            ValueChecker();
            string abs = IInt.Absolute(this.currentValue);
            string diff = "-" + abs;
            this.currentValue.Assign(0);
            OnValueChanged?.Invoke(new IInt(diff));
            OnEmpty?.Invoke();
        }

        public IInt GetMaxValue()
        {
            ValueChecker();
            return this.maxValue;
        }

        public IInt GetValue()
        {
            ValueChecker();
            return this.currentValue;
        }

        public IInt GetValueNormalized()
        {
            ValueChecker();
            return IInt.Zero;
        }

        void ValueChecker()
        {
            if (!this.isInitialized)
            {
                this.Log(string.Format("Values are not initialized, use default value, maxHealth: {0}, currentHealth: {1}", this.defaultMaxValue, this.defaultMaxValue), Color.cyan);
                InitValue(this.defaultMaxValue);
            }
        }

        public void UpdateMaxValue(IInt maxValue)
        {
            var diff = this.maxValue - maxValue;
            this.maxValue = maxValue;
            UpdateValue(diff);
        }
    }
}
