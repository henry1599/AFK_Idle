using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HenryDev.Managers
{
    [CreateAssetMenu(menuName = "Configs/ManagerConfig")]
    public class ManagerConfig : ScriptableObject
    {
        public List<GameObject> ManagerPrefabs;
    }
}
