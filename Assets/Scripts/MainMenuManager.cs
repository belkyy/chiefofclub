using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FootballManager.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Üst Menü Butonları")]
        public Button homeTabBtn;
        public Button fixtureTabBtn; // YENİ: Fikstür Butonu
        public Button transferTabBtn;
        public Button squadTabBtn;
        public Button settingsTabBtn;

        [Header("Ekran Panelleri")]
        public GameObject homePanel;
        public GameObject fixturePanel; // YENİ: Fikstür Paneli
        public GameObject transferPanel;
        public GameObject squadPanel;
        public GameObject settingsPanel;

        [Header("Maça Geçiş")]
        public Button goToMatchBtn;

        void Start()
        {
            // Menü Sekme Dinleyicileri
            if (homeTabBtn != null) homeTabBtn.onClick.AddListener(() => OpenPanel(homePanel));
            if (fixtureTabBtn != null) fixtureTabBtn.onClick.AddListener(() => OpenPanel(fixturePanel)); // YENİ
            if (transferTabBtn != null) transferTabBtn.onClick.AddListener(() => OpenPanel(transferPanel));
            if (squadTabBtn != null) squadTabBtn.onClick.AddListener(() => OpenPanel(squadPanel));
            if (settingsTabBtn != null) settingsTabBtn.onClick.AddListener(() => OpenPanel(settingsPanel));

            // Maç Sahnesine Geçiş Dinleyicisi
            if (goToMatchBtn != null) goToMatchBtn.onClick.AddListener(LoadMatchScene);

            // Varsayılan olarak Ana Sayfayı Aç
            OpenPanel(homePanel);
        }

        public void OpenPanel(GameObject targetPanel)
        {
            if (homePanel != null) homePanel.SetActive(false);
            if (fixturePanel != null) fixturePanel.SetActive(false); // YENİ
            if (transferPanel != null) transferPanel.SetActive(false);
            if (squadPanel != null) squadPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            if (targetPanel != null) targetPanel.SetActive(true);
        }

        public void LoadMatchScene()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}