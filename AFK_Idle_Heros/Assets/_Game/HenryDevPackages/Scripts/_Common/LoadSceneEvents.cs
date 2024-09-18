using System;
using UnityEngine.SceneManagement;

namespace HenryDev.Events
{
    public static class LoadSceneEvents
    {
        public static Action<eSceneType, Action> LOAD_SCENE;
        public static Action<eSceneType, Action> UNLOAD_SCENE; 
        public static Action<Action> UNLOAD_CURRENT_SCENE;
        public static Action<eSceneType> SET_ACTIVE_SCENE;
        public static Func<eSceneType, bool> IS_SCENE_ACTIVE;
    }
}
