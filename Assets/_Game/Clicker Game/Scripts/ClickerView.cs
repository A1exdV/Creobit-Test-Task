using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker_Game.Scripts
{
    public class ClickerView : MonoBehaviour
    {
        [SerializeField] private Button clickButton;
        [SerializeField] private TextMeshProUGUI scoreText;

        public event Action OnClickButton;

        private void Start()
        {
            clickButton.onClick.AddListener(HandleClickButton);
        }

        private void HandleClickButton()
        {
            OnClickButton?.Invoke();
        }

        public void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}