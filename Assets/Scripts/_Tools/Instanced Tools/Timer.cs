using System;
using System.Collections;
using UnityEngine;


namespace JiRO
{
    public class Timer : MonoBehaviour
    {
        public bool oneShot = true;

        public float waitTime = 1f;

        private float _timeLeft = 0f;

        public bool finished = true;

        public Action OnFinished;

        public float timeLeft
        {
            get => _timeLeft;
        }


        public static Timer CreateNew(GameObject attached_to, float wait_time = 1, bool one_shot = true)
        {
            Timer timer = attached_to.AddComponent<Timer>();

            timer.waitTime = wait_time;
            timer.oneShot = one_shot;

            return timer;
        }

        public void StartTimer()
        {
            finished = false;
            _timeLeft = waitTime;

            StopAllCoroutines();
            StartCoroutine(Tick());
        }

        public void StartTimer(float wait_time)
        {
            finished = false;
            _timeLeft = wait_time;

            StopAllCoroutines();
            StartCoroutine(Tick());
        }

        public void StopTimer()
        {
            StopAllCoroutines();
            _timeLeft = 0;
            finished = true;

            OnFinished?.Invoke();
        }


        private IEnumerator Tick()
        {
            while (_timeLeft > 0)
            {
                _timeLeft -= Time.unscaledDeltaTime;

                yield return new WaitForSeconds(0);
            }

            _timeLeft = 0;
            finished = true;

            OnFinished?.Invoke();

            if (!oneShot)
            {
                StartTimer();
            }

        }
    }
}
