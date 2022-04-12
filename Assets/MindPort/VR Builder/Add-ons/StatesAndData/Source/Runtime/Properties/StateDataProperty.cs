using System;
using UnityEngine;

namespace VRBuilder.StatesAndData.Properties
{
    /// <summary>
    /// Template class for state data properties. Override to create usable state data properties.
    /// </summary>    

    public class StateDataProperty<T> : StateDataPropertyBase where T : Enum
    {
        [SerializeField]
        private T defaultValue;

        /// <inheritdoc/>
        public override int DefaultValue => Convert.ToInt32(defaultValue);

        /// <inheritdoc/>
        public override Type StateType => typeof(T);

        /// <summary>
        /// Sets the current state to the specified value.
        /// </summary>
        public void SetState(T state)
        {
            SetValue(Convert.ToInt32(state));
        }

        /// <summary>
        /// Returns the current state.
        /// </summary>       
        public T GetState()
        {
            return (T)Enum.ToObject(StateType, GetValue());
        }

        protected override string ValueToString(int value)
        {
            return ((T)Enum.ToObject(StateType,value)).ToString();
        }
    }
}