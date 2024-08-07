using UnityEngine;
using UnityEngine.UI;

namespace Main_Menu.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ExitGame : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Application.Quit);
        }
    }
}
