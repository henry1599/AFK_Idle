using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFK.Idle
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "AFK Config/UnitData", order = 1)]
    public class UnitData : ScriptableObject
    {
        public string UnitName;
        public string UnitCode;
        public string SPUMCode;
        public GameObject UnitPrefab;
        public Sprite UnitIcon;
    }
}
