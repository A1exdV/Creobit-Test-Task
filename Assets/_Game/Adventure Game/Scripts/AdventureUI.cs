using System;
using R3;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Adventure_Game.Scripts
{
    public class AdventureUI : MonoBehaviour
    {
        [SerializeField] private GameObject beginUI;
        [SerializeField] private GameObject runningUI;
        [SerializeField] private GameObject finishedUI;
        [SerializeField] private TextMeshProUGUI[] timersText;
        [SerializeField] private TextMeshProUGUI recordText;

        [SerializeField] private Button restart;
        public event Action OnRestartPressed;
        
        [SerializeField] private Button[] exits;
        public event Action OnExitPressed;
        
        [Inject] private Timer _timer;

        public void Enable(UI value)
        {
            switch (value)
            {
                case UI.Await:
                    beginUI.SetActive(true);
                    runningUI.SetActive(false);
                    finishedUI.SetActive(false);
                    break;
                case UI.Running:
                    beginUI.SetActive(false);
                    runningUI.SetActive(true);
                    finishedUI.SetActive(false);
                    break;
                case UI.Finished:
                    beginUI.SetActive(false);
                    runningUI.SetActive(false);
                    finishedUI.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public void ShowRecord(string output)
        {
            recordText.text = output;
        }
        
        private void Awake()
        {
            beginUI.SetActive(false);
            runningUI.SetActive(false);
            finishedUI.SetActive(false);
            restart.onClick.AddListener(()=>OnRestartPressed?.Invoke());

            foreach (var exit in exits)
            {
                exit.onClick.AddListener(()=>OnExitPressed?.Invoke());
            }
            
            _timer.Time.Subscribe(OnTimerUpdated);

        }

        private void OnTimerUpdated(float value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            foreach (var timerText in timersText)
            {
                timerText.text = string.Format($"{time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds:D3}");
            }
        }
    }

    public enum UI
    {
        Await,
        Running,
        Finished
    }
}