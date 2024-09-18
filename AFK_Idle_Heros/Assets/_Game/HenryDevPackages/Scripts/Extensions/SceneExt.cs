using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes.Test;
using UnityEngine;

namespace HenryDev.Utilities
{
    public static class SceneExt
    {
        public static eSceneType SceneNameToType(this string sceneName) => sceneName.ToLower() switch
        {
            "main" => eSceneType.Main,
            "mainmenu" => eSceneType.MainMenu,
            _ => eSceneType.Unknown
        };
    }
}
