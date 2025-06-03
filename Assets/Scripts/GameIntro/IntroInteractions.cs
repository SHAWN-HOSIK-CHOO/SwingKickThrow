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
            UISoundManager._instance.Click.Play();
            SceneManager.LoadScene("LobbyMenu");
        }

        private void OnCreditsButtonClicked()
        {
            bgGameObject.gameObject.SetActive(false);
            UISoundManager._instance.Click.Play();
        }

        private void OnReturnToBgButtonClicked()
        {
            bgGameObject.gameObject.SetActive(true);
            UISoundManager._instance.Back.Play();
        }

        private void OnGameExit()
        {
            UISoundManager._instance.Notify.Play();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}
