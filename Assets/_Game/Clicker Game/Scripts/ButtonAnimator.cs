using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker_Game.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ButtonAnimator : MonoBehaviour
    {
        [SerializeField] private float idleScaleMax;
        [SerializeField] private float idleAnimationTime;

        [SerializeField] private float pressedScaleMin;
        [SerializeField] private float pressedAnimationTime;

        private Coroutine _currentAnimation;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(HandleClickButton);
            
            _currentAnimation =  StartCoroutine(IdleAnimation());
        }

        private void HandleClickButton()
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            StartCoroutine(ClickAnimation());
        }

        private IEnumerator ClickAnimation()
        {
            transform.localScale = Vector3.one;
            var halfTime = pressedAnimationTime / 2;
            float timer = 0;
            while (timer < halfTime)
            {
                var progress = timer / halfTime;
                var currentScale = Mathf.Lerp(1, pressedScaleMin, progress);

                transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                yield return null;
                timer += Time.deltaTime;
            }

            timer = 0;

            while (timer < halfTime)
            {
                var progress = timer / halfTime;
                var currentScale = Mathf.Lerp(pressedScaleMin, 1, progress);

                transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                yield return null;
                timer += Time.deltaTime;
            }

            transform.localScale = Vector3.one;
            
            _currentAnimation =  StartCoroutine(IdleAnimation());
        }

        private IEnumerator IdleAnimation()
        {
            while (true)
            {
                transform.localScale = Vector3.one;
                var halfTime = idleAnimationTime / 2;
                float timer = 0;
                while (timer < halfTime)
                {
                    var progress = timer / halfTime;
                    var currentScale = Mathf.Lerp(1, idleScaleMax, progress);

                    transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                    yield return null;
                    timer += Time.deltaTime;
                }

                timer = 0;

                while (timer < halfTime)
                {
                    var progress = timer / halfTime;
                    var currentScale = Mathf.Lerp(idleScaleMax, 1, progress);

                    transform.localScale = new Vector3(currentScale, currentScale, currentScale);

                    yield return null;
                    timer += Time.deltaTime;
                }
            }
        }
    }
}