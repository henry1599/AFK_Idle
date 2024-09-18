using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HenryDev.Utilities
{
    public static class GameObjExt
    {
        public static void StripCloneName(this GameObject gameObject)
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
        }
        public static void DeleteChildren(this Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
    public static class ObjectExt
    {
        public static string ToJson(this object obj)
        {
            return JsonUtility.ToJson(obj);
        }
        public static T FromJson<T>(this string json) where T : class
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}
