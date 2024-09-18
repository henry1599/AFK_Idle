using System;
using System.Collections.Generic;
using HenryDev.Events;
using HenryDev.Intefaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using HenryDev.Utilities;
using HenryDev.Controllers;
using System.Collections;

namespace HenryDev.Managers
{
    public class SceneOperator
    {
        public eSceneType SceneType;
        public AsyncOperation Operation;
        public Action Callback;
        public bool IsUnloaded;
    }
    public class LoadSceneManager : MonoBehaviour
    {
        public static LoadSceneManager Instance {get; private set;}
        [SerializeField] SceneConfig sceneConfig;
        private Dictionary<eSceneType, string> sceneMap;
        private Dictionary<string, eSceneType> sceneMapByName;
        private Queue<SceneOperator> loadEventsQueue;
        private Queue<SceneOperator> unloadEventsQueue;
        private SceneOperator currentOperation;
        private List<eSceneType> scenesList;
        private bool isInitalized = false;
        private bool isOperationRunning = false;
        eSceneType currentLoadedScene;
        public eSceneType CurrentLoadedScene => this.currentLoadedScene;
        void Awake()
        {
            Init();
        }
        void Update()
        {
            if (!isInitalized)
                Init();

            if (this.currentOperation != null)
            {
                if (this.currentOperation.Operation.isDone == true)
                {
                    this.currentOperation.Callback?.Invoke();

                    if (this.currentOperation.IsUnloaded == true)
                    {
                        Resources.UnloadUnusedAssets();
                        System.GC.Collect();
                    }
                    this.currentOperation = null;
                    this.isOperationRunning = false;
                }
            }
            else if (this.isOperationRunning == false)
            {
                if (unloadEventsQueue.Count > 0)
                {
                    SceneOperator opInfo = unloadEventsQueue.Dequeue();
                    StartCoroutine(UnloadSceneCoroutine(opInfo));
                }
                else if (loadEventsQueue.Count > 0)
                {
                    SceneOperator opInfo = loadEventsQueue.Dequeue();
                    StartCoroutine(LoadSceneCoroutine(opInfo));
                }
            }
        }
        private IEnumerator LoadSceneCoroutine(SceneOperator opInfo)
        {
            this.isOperationRunning = true;
            yield return null;
            LoadScene(opInfo);
        }
        private void LoadScene(SceneOperator opInfo)
        {
            this.currentLoadedScene = opInfo.SceneType;
            string sceneName = SceneTypeToName(opInfo.SceneType);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncOperation != null)
            {
                opInfo.Operation = asyncOperation;
                currentOperation = opInfo;
            }
            else
            {
                this.isOperationRunning = false;
            }
        }
        private IEnumerator UnloadSceneCoroutine(SceneOperator opInfo)
        {
            this.isOperationRunning = true;
            yield return null;

            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(SceneTypeToName(opInfo.SceneType));
            if (asyncOperation != null)
            {
                opInfo.Operation = asyncOperation;
                currentOperation = opInfo;
            }
            else
            {
                this.isOperationRunning = false;
            }
        }
        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            
            LoadSceneEvents.LOAD_SCENE -= HandleLoadScene;
            LoadSceneEvents.UNLOAD_SCENE -= HandleUnloadScene;
            LoadSceneEvents.UNLOAD_CURRENT_SCENE -= HandleUnloadCurrentScene;
            LoadSceneEvents.SET_ACTIVE_SCENE -= HandleSetActiveScene;
            LoadSceneEvents.IS_SCENE_ACTIVE -= HandleIsSceneActive;
        }
        private void Init()
        {
            if (isInitalized)
                return;
            if (Instance == null)
                Instance = this;
            sceneMap = new Dictionary<eSceneType, string>();
            sceneMapByName = new Dictionary<string, eSceneType>();
            int num = sceneConfig.SceneDict.Count;
            foreach (var (key, val) in this.sceneConfig.SceneDict)
            {
                sceneMap.Add(key, val);
                if (!sceneMapByName.ContainsKey(val))
                {
                    sceneMapByName.Add(val, key);
                }
            }

            loadEventsQueue = new Queue<SceneOperator>();
            unloadEventsQueue = new Queue<SceneOperator>();
            this.scenesList = new List<eSceneType>();
            
            LoadSceneEvents.LOAD_SCENE += HandleLoadScene;
            LoadSceneEvents.UNLOAD_SCENE += HandleUnloadScene;
            LoadSceneEvents.UNLOAD_CURRENT_SCENE += HandleUnloadCurrentScene;
            LoadSceneEvents.SET_ACTIVE_SCENE += HandleSetActiveScene;
            LoadSceneEvents.IS_SCENE_ACTIVE += HandleIsSceneActive;

            isInitalized = true;
        }

        private void HandleUnloadCurrentScene(Action action)
        {
            if (this.currentLoadedScene == eSceneType.Unknown)
            {
                this.Log("Current scene is UNKNOWN", Color.yellow);
                return;
            }
            HandleUnloadScene(this.currentLoadedScene, action);
        }

        private bool HandleIsSceneActive(eSceneType type)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == SceneTypeToName(type))
                return true;
            else
                return false;
        }


        private void HandleSetActiveScene(eSceneType type)
        {
            if (!isInitalized)
                Init();

            if (sceneMap.ContainsKey(type))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneTypeToName(type)));
            }
        }


        private void HandleUnloadScene(eSceneType type, Action action)
        {
            if (!isInitalized)
                Init();

            if (!this.scenesList.Contains(type))
            {
                Debug.LogFormat("Already unload scene {0}\n{1}", type.ToString(), StackTraceUtility.ExtractStackTrace());
                return;
            }
            if (sceneMap.ContainsKey(type) && IsSceneLoaded(type))
            {
                SceneOperator opInfo = new SceneOperator();
                opInfo.SceneType = type;
                opInfo.Callback = action;
                opInfo.IsUnloaded = true;
                unloadEventsQueue.Enqueue(opInfo);
                this.scenesList.Remove(type);
            }
        }


        private void HandleLoadScene(eSceneType type, Action action)
        {
            if (!isInitalized)
                Init();
            if (this.scenesList.Contains(type))
            {
                Debug.LogFormat("Already load scene {0}\n{1}", type.ToString(), StackTraceUtility.ExtractStackTrace());
                return;
            }
            if (sceneMap.ContainsKey(type))
            {
                SceneOperator opInfo = new SceneOperator();
                opInfo.SceneType = type;
                opInfo.Callback = action;
                opInfo.IsUnloaded = false;
                loadEventsQueue.Enqueue(opInfo);
                this.scenesList.Add(type);
            }
        }


        public string SceneTypeToName(eSceneType sceneType)
        {
            return this.sceneMap[sceneType];
        }

        public eSceneType SceneNameToType(string sceneName)
        {
            if (sceneMapByName.ContainsKey(sceneName))
            {
                return this.sceneMapByName[sceneName];
            }
            return eSceneType.Unknown;
        }
        public bool IsSceneLoaded(eSceneType sceneType)
        {
            if (!this.sceneMap.ContainsKey(sceneType))
                return false;
            string sceneName = SceneTypeToName(sceneType);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }
    }
}
