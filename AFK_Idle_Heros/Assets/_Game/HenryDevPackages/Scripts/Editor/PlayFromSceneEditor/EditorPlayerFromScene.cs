using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HenryDev.EditorTools
{
    public class EditorPlayerFromScene : EditorWindow
    {
        [SerializeField] VisualTreeAsset tree;
        [SerializeField] ListView sceneButtonListView;
        [SerializeField] ToolbarSearchField sceneSearchField;
        Button openButton;
        Button playButton;
        Button playMainButton;
        Button playMainRoomButton;
        Button openAdditiveButton;
        Toggle keepPrevSceneToggle;
        Button reloadButton;
        private static EditorPlayerFromScene thisWindow;
        private Dictionary<string, string> originalSceneNames;
        private Dictionary<string, string> filteredSceneNames;
        private Button chosenSceneButton = null;
        private string cacheSceneName = string.Empty;
        [MenuItem("Tools/Scene Utils")]
        public static void ShowEditor()
        {
            var window = GetWindow<EditorPlayerFromScene>();
            window.titleContent = new GUIContent("Scene Utils");

            thisWindow = window;
        }
        void CreateGUI()
        {
            this.tree.CloneTree(rootVisualElement);
            rootVisualElement.Bind(new SerializedObject(this));

            this.sceneButtonListView = rootVisualElement.Q<ListView>("_sceneButtonListView");
            this.sceneSearchField = rootVisualElement.Q<ToolbarSearchField>("_sceneSearchField");
            this.openButton = rootVisualElement.Q<Button>("_btnOpen");
            this.playButton = rootVisualElement.Q<Button>("_btnPlay");
            this.playMainButton = rootVisualElement.Q<Button>("_btnPlayMain");
            this.playMainRoomButton = rootVisualElement.Q<Button>("_btnPlayMainRoom");
            this.openAdditiveButton = rootVisualElement.Q<Button>("_btnOpenAdditive");
            this.keepPrevSceneToggle = rootVisualElement.Q<Toggle>("_keepPrevSceneToggle");
            this.reloadButton = rootVisualElement.Q<Button>("_btnReload");

            this.openButton.clicked += OnOpenButtonClicked;
            this.playButton.clicked += OnPlayButtonClicked;
            this.openAdditiveButton.clicked += OnOpenAdditiveButtonClicked;
            this.reloadButton.clicked += OnReloadButtonClicked;

            this.playMainButton.clicked += OnPlayMainButtonClicked;
            this.playMainRoomButton.clicked += OnPlayMainRoomButtonClicked;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;


            SetupSceneButtons();
            SetupSearchField();
        }
        void OnDestroy() 
        {
            this.openButton.clicked -= OnOpenButtonClicked;
            this.playButton.clicked -= OnPlayButtonClicked;
            this.openAdditiveButton.clicked -= OnOpenAdditiveButtonClicked;

            this.playMainButton.clicked -= OnPlayMainButtonClicked;
            this.playMainRoomButton.clicked -= OnPlayMainRoomButtonClicked;
            this.reloadButton.clicked -= OnReloadButtonClicked;
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnReloadButtonClicked()
        {
            SetupSceneButtons();
            SetupSearchField();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (!this.keepPrevSceneToggle.value)
                return;
            if (change != PlayModeStateChange.EnteredEditMode)
                return;
            if (string.IsNullOrEmpty(this.cacheSceneName))
                return;
            OpenScene(this.cacheSceneName);
            ClearCache();
            ResetChosenButton();
        }

        private void OnPlayMainRoomButtonClicked()
        {
            CacheScene();
            PlayScene(sceneName: "MainMenu");
            ResetChosenButton();
        }

        private void OnPlayMainButtonClicked()
        {
            CacheScene();
            PlayScene(sceneName: "Main");
            ResetChosenButton();
        }

        private void OnOpenAdditiveButtonClicked()
        {
            OpenScene(mode: OpenSceneMode.Additive);
            ResetChosenButton();
        }

        private void OnPlayButtonClicked()
        {
            CacheScene();
            PlayScene();
            ResetChosenButton();
        }

        private void OnOpenButtonClicked()
        {
            OpenScene();
            ResetChosenButton();
        }
        string GetChosenScenePath(string customSceneName = "")
        {
            string text = string.Empty;
            if (string.IsNullOrEmpty(customSceneName))
                text = this.chosenSceneButton == null ? EditorSceneManager.GetActiveScene().path : this.chosenSceneButton.text;
            else
                text = customSceneName;
            if (!this.originalSceneNames.ContainsKey(text))
                return string.Empty;
            string scenePath = this.originalSceneNames[text];
            return scenePath;
        }
        void OpenScene(string sceneName = "", OpenSceneMode mode = OpenSceneMode.Single)
        {
            string scenePath = GetChosenScenePath(sceneName);
            if (string.IsNullOrEmpty(scenePath))
                return;
            EditorSceneManager.OpenScene(scenePath, mode);
        }
        void PlayScene(string sceneName = "")
        {
            string scenePath = GetChosenScenePath(sceneName);
            if (string.IsNullOrEmpty(scenePath))
                return;
            EditorSceneManager.OpenScene(scenePath);
            EditorApplication.isPlaying = true;
        }
        void CacheScene()
        {
            this.cacheSceneName = EditorSceneManager.GetActiveScene().name;
        }
        void ClearCache()
        {
            this.cacheSceneName = string.Empty;
        }
        void ResetChosenButton()
        {
            this.chosenSceneButton = null;
        }
        void SetupSceneButtons()
        {
            SetupSceneList();
        }
        void SetupSearchField()
        {
            this.sceneSearchField.RegisterValueChangedCallback(OnSearchFieldChanged);
        }
        void SetupSceneList()
        {
            this.originalSceneNames = new();
            this.filteredSceneNames = new();
            int sceneCount = EditorBuildSettings.scenes.Length;
            List<string> items = new List<string>();
            for (int i = 0; i < sceneCount; i++)
            {
                string scene = EditorBuildSettings.scenes[i].path;
                string sceneName = scene.Split("/")[^1].Split(".")[0];
                this.originalSceneNames.TryAdd(sceneName, scene);
                items.Add(sceneName);
            }
            SetupListView(items);
        }
        void UpdateSceneList()
        {
            List<string> items = new List<string>();
            if (this.filteredSceneNames == null)
                return;
            foreach (var (k, v) in this.filteredSceneNames)
            {
                items.Add(k);
            }
            SetupListView(items);
        }
        void SetupListView(List<string> items)
        {
            Func<VisualElement> makeItem = () => new Button();
            Action<VisualElement, int> bindItem = (e, i) => 
            {
                Button b = e as Button;
                if (i < 0 || i > items.Count - 1)
                    return;
                b.text = items[i];
                b.clicked += () => this.chosenSceneButton = b;
            };
            
            this.sceneButtonListView.makeItem = makeItem;
            this.sceneButtonListView.bindItem = bindItem;
            this.sceneButtonListView.itemsSource = items;
            this.sceneButtonListView.selectionType = SelectionType.Single;
        }

        void OnSearchFieldChanged(ChangeEvent<string> evt)
        {
            string searchText = evt.newValue.ToLower();
            this.filteredSceneNames = new Dictionary<string, string>();
            foreach (var (k, v) in this.originalSceneNames)
            {
                if (k.ToLower().Contains(searchText))
                    filteredSceneNames.TryAdd(k, v);
            }
            UpdateSceneList();
        }
    }
}
