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
        public HeroController Hero {get; private set;}
        public UnitData Data => this.heroData;
        public HeroController LoadHero(string id)
        {
            this.heroData = this.unitsConfig.GetUnitData(id);
            if (this.heroData == null)
            {
                return null;
            }
            this.heroPrefab = Instantiate(this.heroData.Prefab, this.spawnPosition.position, Quaternion.identity, this.container);
            Hero = this.heroPrefab.SafeAddComponent<HeroController>();
            return Hero;
        }
    }
}
