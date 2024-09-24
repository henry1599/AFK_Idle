using System.Collections;
using System.Collections.Generic;
using HenryDev.Utilities;
using UnityEngine;

namespace AFK.Idle
{
    public class HeroLoader : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] Transform spawnPosition;
        [SerializeField] UnitsConfig unitsConfig;
        private UnitData heroData;
        private GameObject heroPrefab;
        public UnitData Data => this.heroData;
        public UnitPrefab LoadHero(string id)
        {
            this.heroData = this.unitsConfig.GetUnitData(id);
            if (this.heroData == null)
            {
                return null;
            }
            this.heroPrefab = Instantiate(this.heroData.Prefab, this.spawnPosition.position, Quaternion.identity, this.container);
            this.heroPrefab.SafeAddComponent<UnitMonoBehaviour>();
            var prefab = this.heroPrefab.GetComponent<UnitPrefab>();
            prefab.Setup(this.heroData);
            return prefab;
        }
    }
}
