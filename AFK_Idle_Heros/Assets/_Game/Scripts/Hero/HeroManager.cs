using System.Collections;
using System.Collections.Generic;
using HenryDev.Gameplay;
using HenryDev.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AFK.Idle
{
    public class HeroManager : MonoBehaviour
    {
        public static HeroManager Instance { get; private set; }
        [BoxGroup("Attack Zone"), SerializeField] Transform sightPoint;
        [BoxGroup("Attack Zone"), SerializeField] Vector2 boxSightSize;
        [SerializeField] HeroLoader heroLoader;
        [SerializeField] Controller heroController;
        private string chosenHeroId = "H0001";
        private GlobalStatsSaveData globalStatsSaveData;
        public UnitData Data => this.heroLoader?.Data;
        public GlobalStatsSaveData StatsData => this.globalStatsSaveData;
        UnitMonoBehaviour behaviour;
        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        void Start()
        {
            var profileData = ProfileManager.Instance.Data;
            this.globalStatsSaveData = profileData.GlobalStatsSaveData;

            this.chosenHeroId = GetChosenHero();
            var prefab = this.heroLoader.LoadHero(this.chosenHeroId);
            this.heroController?.Setup(prefab);
            this.behaviour = prefab.GetComponent<UnitMonoBehaviour>();

            this.behaviour?.Setup(this.globalStatsSaveData, this.heroController, this.sightPoint, this.boxSightSize, "Enemy");
        }
        void OnDestroy()
        {   
            if (Instance == this)
                Instance = null;
        }
        public string GetChosenHero()
        {
            // Hard code for now
            return "H0001";
        }

        void OnDrawGizmos()
        {
            if (this.sightPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(this.sightPoint.position, boxSightSize);
            }
        }
    }
}
