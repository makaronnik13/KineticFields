using System;
using com.armatur.common.events;
using com.armatur.common.logic;
using com.armatur.common.serialization;
using UnityEngine;

namespace com.armatur.common.flags
{
    [System.Serializable]
    public class ChangingValueComparable<T> : ChangingValue<T>, IFlagComparable<T> where T : IComparable
    {
        public IFlag<bool> GetFlag(LogicOperation operation, IFlag<T> compareCounter, string name = null)
        {
            return new ChangingValueFlag<T>(this, operation, compareCounter, name);
        }

        public IFlag<bool> GetFixedFlag(LogicOperation operation, T compareValue, string name = null)
        {
            return ChangingValueFlag<T>.CreateFixed(this, operation, compareValue, name);
        }

        public ChangingValueComparable(string name, T value) : base(name, value)
        {
        }
    }

    [System.Serializable]
    public class ChangingValue<T> : IFlag<T>
    {
        [NonSerialized]
        protected PriorityEvent<T> _event;
        private T _state;

        public ChangingValue()
        {
            Debug.Log("Deserialize");
            _event = new PriorityEvent<T>(Name + " changed");
        }

        protected ChangingValue(string name, T value)
        {
            Name = name;
            _event = new PriorityEvent<T>(name + " changed");
            _state = value;
        }

        public string Name { get; }

        [Savable]
        [SerializeData(FieldOmitName.True, FieldRequired.False)]
        public T Value
        {
            get { return _state; }
            protected set { SetInnerState(value); }
        }

        public IListenerController AddListener(Action<T> listener, bool run = true, int priority = 0)
        {
            if (run)
                listener(Value);
            if (_event == null)
            {
                _event = new PriorityEvent<T>(Name + " changed");
            }

            return _event.AddListener(listener, priority);
        }

        public void RemoveListener(Action<T> listener)
        {
            _event?.RemoveListener(listener);
        }

        public IListenerController AddListener(Action listener, bool run = true)
        {
            if (run)
                listener();

            if (_event == null)
            {
                _event = new PriorityEvent<T>(Name + " changed");
            }

            return _event.AddListener(listener);
        }

        public void RemoveListener(Action listener)
        {
            _event.RemoveListener(listener);
        }

        protected void SetInnerState(T value, bool fire = true)
        {
            // Set _state only if the new value different from the current (check for case value == _state == null as well).
            if (value?.Equals(_state) ?? _state == null) return;
            _state = value;
            if (fire)
            {
                if (_event == null)
                {
                    _event = new PriorityEvent<T>(Name + " changed");
                }
                _event.Fire(_state);
            }
        }

        public void RaiseEvent()
        {
            if (_event == null)
            {
                _event = new PriorityEvent<T>(Name + " changed");
            }
            _event.Fire(_state);
        }
    }
}