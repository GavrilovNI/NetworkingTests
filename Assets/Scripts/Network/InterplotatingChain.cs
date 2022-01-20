using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class InterplotatingChain<T>
    {
        public event Action<T> ValueChanged;

        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                _values.Clear();
                _times.Clear();
                _values.Add(value);

                LastTimeChainChanged = Time.realtimeSinceStartup;
                ValueChanged?.Invoke(_value);
            }
        }

        public float LastTimeChainChanged { get; private set; }

        private float _time = 0;

        private List<T> _values = new List<T> {};
        private List<float> _times = new List<float>();

        private Func<T, T, float, T> _interpolator;
        private Func<T, T, float> _sqrDistance;
        public int Length => _values.Count;

        public InterplotatingChain(Func<T, T, float, T> interpolator, Func<T, T, float> sqrDistance)
        {
            _interpolator = interpolator;
            _sqrDistance = sqrDistance;
            Value = default(T);
        }

        public T GetValue(int index)
        {
            return _values[index];
        }

        public void AddToChain(T value, float time)
        {
            _values.Add(value);
            _times.Add(time);

            LastTimeChainChanged = Time.realtimeSinceStartup;
        }

        public void Update(float deltaTime)
        {
            if (_values.Count < 2 || deltaTime <= 0)
                return;

            _time += deltaTime;
            InterpolateValue();
        }

        private void InterpolateValue()
        {
            if (_values.Count < 2)
            {
                _time = 0;
                return;
            }

            if(_time >= _times[0])
            {
                _time -= _times[0];
                _value = _values[0];
                _times.RemoveAt(0);
                _values.RemoveAt(0);

                ValueChanged?.Invoke(_value);
                InterpolateValue();

                return;
            }

            if(_time > 0)
            {
                _value = _interpolator(_values[0], _values[1], _time / _times[0]);
                ValueChanged?.Invoke(_value);
            }
        }

        public void Normalize()
        {
            if (_values.Count < 2)
                return;

            float time = _times.Sum();
            T prev = _values[0];
            float sqrDistanceTotal = _values.Sum(value =>
            {
                float sqrDistance = _sqrDistance(value, prev);
                prev = value;
                return sqrDistance;
            });

            if(sqrDistanceTotal == 0)
            {
                return;
            }

            for (int i = 0; i < _times.Count; i++)
            {
                _times[i] = time * _sqrDistance(_values[i], _values[i + 1]) / sqrDistanceTotal;
            }
        }
    }
}
