using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AFK.Idle
{
    [CreateAssetMenu(fileName = "UnitsConfig", menuName = "AFK Config/UnitsConfig", order = 1)]
    public class UnitsConfig: ScriptableObject
    {
        public static string UNIT_DATA_PATH = "UnitData/";
        public List<string> Ids;
        private Dictionary<string, UnitData> cachedUnitData = new Dictionary<string, UnitData>();
        public UnitData GetUnitData(string id)
        {
            if (cachedUnitData.ContainsKey(id))
            {
                return cachedUnitData[id];
            }
            var unitData = Resources.Load(UNIT_DATA_PATH + id) as UnitData;
            if (unitData == null)
            {
                Debug.Log("UnitData not found");
                return null;
            }
            cachedUnitData.TryAdd(id, unitData);
            return unitData;
        }
        public GameObject GetUnitPrefab(string id)
        {
            var data = GetUnitData(id);
            if (data == null)
            {
                return null;
            }
            return data.Prefab;
        }
        public Sprite GetUnitIcon(string id)
        {
            var data = GetUnitData(id);
            if (data == null)
            {
                return null;
            }
            return data.Icon;
        }
#if UNITY_EDITOR
        [Button]
        public void LoadIds()
        {
            Ids = new List<string>();
            var unitDatas = Resources.LoadAll(UNIT_DATA_PATH);
            foreach (var unitData in unitDatas)
            {
                var unitDataInstance = unitData as UnitData;
                if (unitDataInstance == null)
                {
                    continue;
                }
                Ids.Add(unitDataInstance.Code);
            }
        }
#endif
    }
}