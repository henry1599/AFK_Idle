using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFK.Idle
{
    public class HeroManager : MonoBehaviour
    {
        public static HeroManager Instance { get; private set; }
        [SerializeField] HeroLoader heroLoader;
        private HeroController heroController;
        private string chosenHeroId = "H0001";
        public UnitData Data => this.heroLoader?.Data;
        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        void Start()
        {
            this.chosenHeroId = GetChosenHero();
            this.heroController = this.heroLoader.LoadHero(this.chosenHeroId);
            this.heroController?.Setup();
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
    }
}
