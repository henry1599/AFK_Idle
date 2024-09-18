using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace HenryDev.Managers
{
    [System.Serializable]
    public class SceneConfigDict: SerializableDictionaryBase<eSceneType, string> {}
    [CreateAssetMenu(menuName = "Configs/SceneConfig")]
    public class SceneConfig : ScriptableObject
    {
        public SceneConfigDict SceneDict;
    }
}
