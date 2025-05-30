using System;
using UnityEngine;
using VContainer.Unity;

namespace Match3d.Core.Utils
{
    [UnityEngine.Scripting.Preserve]
    public class Timer : ITickable
    {
        public event Action TimerEnd;
        public event Action<int> TimerTick;
        public event Action<bool> TimerStateChanged;

        private bool _isRunning;
        private float _seconds;
        private int _lastWholeSeconds;

        public int RemainingSeconds => _lastWholeSeconds;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                TimerStateChanged?.Invoke(_isRunning);
            }
        }

        public void SetTimer(float seconds)
        {
            _seconds = seconds;
            _lastWholeSeconds = Mathf.CeilToInt(_seconds);
        }

        public void Tick()
        {
            if (_seconds <= 0 || !_isRunning)
            {
                return;
            }

            _seconds -= Time.deltaTime;

            var currentWholeSeconds = Mathf.CeilToInt(_seconds);
            if (currentWholeSeconds < _lastWholeSeconds)
            {
                _lastWholeSeconds = currentWholeSeconds;
                TimerTick?.Invoke(currentWholeSeconds);
            }

            if (_seconds <= 0)
            {
                _isRunning = false;
                _seconds = 0;
                TimerEnd?.Invoke();
            }
        }
    }
}