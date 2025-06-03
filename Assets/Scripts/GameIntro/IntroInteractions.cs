using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameIntro
{
    public class IntroInteractions : MonoBehaviour
    {
        public GameObject bgGameObject;

        public Button creditsToBgReturnButton;
        public Button multiButton;
        public Button creditsButton;
        public Button exitButton;

        private void Awake()
        {
            creditsToBgReturnButton.onClick.AddListener(OnReturnToBgButtonClicked);
            multiButton.onClick.AddListener(OnMultiButtonClicked);
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);
            exitButton.onClick.AddListener(OnGameExit);
        }

        private void OnMultiButtonClicked()
        {
            SceneManager.LoadScene("LobbyMenu");
        }

        private void OnCreditsButtonClicked()
        {
            bgGameObject.gameObject.SetActive(false);
        }

        private void OnReturnToBgButtonClicked()
        {
            bgGameObject.gameObject.SetActive(true);
        }

        private void OnGameExit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}
