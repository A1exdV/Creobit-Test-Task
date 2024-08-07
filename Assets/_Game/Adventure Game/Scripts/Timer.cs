using System;
using System.Collections;
using R3;
using UnityEngine;

namespace Adventure_Game.Scripts
{
    public class Timer: MonoBehaviour
    {
        public Observable<float> Time => _time;
        private readonly ReactiveProperty<float> _time = new ();
        private Coroutine _timerRoutine;

        private void Awake()
        {
            gameObject.name = "[TIMER]";
        }

        public void StartTimer()
        {
            _time.Value = 0;
            if(_timerRoutine!= null) 
                StopCoroutine(_timerRoutine);
            _timerRoutine = StartCoroutine(TimerRoutine());
        }

        public float StopTimer()
        {
            if(_timerRoutine!= null) 
                StopCoroutine(_timerRoutine);
            return _time.Value;
        }

        public float GetTime()
        {
            return _time.Value;
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                _time.Value += UnityEngine.Time.deltaTime;
                yield return null;
            }
        }
    }
}
