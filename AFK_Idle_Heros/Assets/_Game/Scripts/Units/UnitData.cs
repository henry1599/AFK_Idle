using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFK.Idle
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "AFK Config/UnitData", order = 1)]
    public class UnitData : ScriptableObject
    {
        public string Name;
        public string Code;
        public string SPUMCode;
        public GameObject Prefab;
        public Sprite Icon;
        public eUnitType Type;
        public eUnitAttackKind AttackKind;
    }
}
