using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HenryDev
{
    public interface IChangeableValue<CustomValue>
    {
        /// <summary>
        /// Value is decrease to 0
        /// </summary>
        event System.Action OnEmpty;
        /// <summary>
        /// Has value been changed and how much?
        /// </summary>
        event System.Action<CustomValue> OnValueChanged;

        void InitValue(CustomValue customValue, bool startFrom0 = true);
        void ForceInitValue(CustomValue customValue, bool startFrom0 = true);
        /// <summary>
        /// Plus "value" param to current value
        /// </summary>
        /// <param name="value"></param>
        void UpdateValue(CustomValue value);
        void MakeEmpty();
        /// <summary>
        /// Update max value, update current value in proportional to max value
        /// </summary>
        /// <param name="maxValue"></param>
        void UpdateMaxValue(CustomValue maxValue);

        CustomValue GetMaxValue();
        CustomValue GetValue();
        CustomValue GetValueNormalized();
    }
}
