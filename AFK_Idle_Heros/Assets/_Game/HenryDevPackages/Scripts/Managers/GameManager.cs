using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HenryDev.Utilities;
using HenryDev.Events;
using System;

namespace HenryDev.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}
        [SerializeField] ManagerConfig managerConfig;
        [SerializeField] Transform managerContainer;
        private bool isInitialized = false;
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
        }
        void Start()
        {
            Init();
        }
        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        void Init()
        {
            if (this.isInitialized)
                return;
            if (this.managerConfig == null)
                return;
            if (this.managerContainer == null)
                return;
            foreach (var pref in this.managerConfig.ManagerPrefabs)
            {
                var go = Instantiate(pref, this.managerContainer);
                go.StripCloneName();
            }
        }
        /// <summary>
        /// Load Scene with transition in and out without unloading current scene, need to do it manually
        /// </summary>
        /// <param name="sceneType"></param>
        /// <param name="callback"></param>
        public void LoadScene(eSceneType sceneType, Action callback = null)
        {
            StartCoroutine(Cor_LoadScene(sceneType, callback));
        }
        /// <summary>
        /// Load Scene with transition in and out, this also unload current scene if any
        /// </summary>
        /// <param name="sceneType"></param>
        /// <param name="callback"></param>
        public void QuickLoadScene(eSceneType sceneType, Action callback = null)
        {
            StartCoroutine(Cor_QuickLoadScene(sceneType, callback));
        }
        /// <summary>
        /// Reload scene
        /// </summary>
        /// <param name="sceneType"></param>
        /// <param name="callback"></param>
        public void ReloadScene(eSceneType sceneType, Action callback = null)
        {
            StartCoroutine(Cor_ReloadScene(sceneType, callback));
        }
        /// <summary>
        /// Unload a scene, then load another scene
        /// </summary>
        /// <param name="unloadScene"></param>
        /// <param name="loadScene"></param>
        /// <param name="callback"></param>
        public void UnloadAndLoadScene(eSceneType unloadScene, eSceneType loadScene, Action callback = null)
        {
            StartCoroutine(Cor_UnloadAndLoadScene(unloadScene, loadScene, callback));
        }
        IEnumerator Cor_LoadScene(eSceneType sceneType, Action callback = null)
        {
            UIEvents.SET_UI_BLOCK?.Invoke(true);
            UIEvents.TRANSITION_IN?.Invoke();
            yield return new WaitForSeconds(UIConstants.TRANSITION_DURATION);
            LoadSceneEvents.LOAD_SCENE?.Invoke(
                sceneType,
                () => 
                {
                    callback?.Invoke();
                    LoadSceneEvents.SET_ACTIVE_SCENE?.Invoke(sceneType);
                    UIEvents.TRANSITION_OUT?.Invoke();
                    UIEvents.SET_UI_BLOCK?.Invoke(false);
                }
            );
        }
        IEnumerator Cor_ReloadScene(eSceneType sceneType, Action callback = null)
        {
            UIEvents.SET_UI_BLOCK?.Invoke(true);
            UIEvents.TRANSITION_IN?.Invoke();
            yield return new WaitForSeconds(UIConstants.TRANSITION_DURATION);
            LoadSceneEvents.UNLOAD_SCENE?.Invoke(
                sceneType,
                () => 
                {
                    callback?.Invoke();
                    LoadSceneEvents.LOAD_SCENE?.Invoke(
                        sceneType,
                        () => 
                        {
                            LoadSceneEvents.SET_ACTIVE_SCENE?.Invoke(sceneType);
                            UIEvents.TRANSITION_OUT?.Invoke();
                            UIEvents.SET_UI_BLOCK?.Invoke(false);
                        }
                    );
                }
            );
        }
        IEnumerator Cor_QuickLoadScene(eSceneType sceneType, Action callback = null)
        {
            UIEvents.SET_UI_BLOCK?.Invoke(true);
            UIEvents.TRANSITION_IN?.Invoke();
            yield return new WaitForSeconds(UIConstants.TRANSITION_DURATION);
            LoadSceneEvents.UNLOAD_CURRENT_SCENE?.Invoke(() => 
            {
                LoadSceneEvents.LOAD_SCENE?.Invoke(
                    sceneType,
                    () => 
                    {
                        callback?.Invoke();
                        LoadSceneEvents.SET_ACTIVE_SCENE?.Invoke(sceneType);
                        UIEvents.TRANSITION_OUT?.Invoke();
                        UIEvents.SET_UI_BLOCK?.Invoke(false);
                    }
                );
            });
        }
        IEnumerator Cor_UnloadAndLoadScene(eSceneType unloadScene, eSceneType loadScene, Action callback = null)
        {
            UIEvents.SET_UI_BLOCK?.Invoke(true);
            UIEvents.TRANSITION_IN?.Invoke();
            yield return new WaitForSeconds(UIConstants.TRANSITION_DURATION);
            LoadSceneEvents.UNLOAD_SCENE?.Invoke(
                unloadScene,
                () => 
                {
                    LoadSceneEvents.LOAD_SCENE?.Invoke(
                        loadScene,
                        () => 
                        {
                            callback?.Invoke();
                            LoadSceneEvents.SET_ACTIVE_SCENE?.Invoke(loadScene);
                            UIEvents.TRANSITION_OUT?.Invoke();
                            UIEvents.SET_UI_BLOCK?.Invoke(false);
                        }
                    );
                }
            );
        }
        public void ExitGameplay(eSceneType sceneType)
        {
            UnloadAndLoadScene(sceneType, eSceneType.MainMenu);
        }
        public void RestartGameplay(eSceneType sceneType)
        {
            ReloadScene(sceneType);
        }
    }
}
