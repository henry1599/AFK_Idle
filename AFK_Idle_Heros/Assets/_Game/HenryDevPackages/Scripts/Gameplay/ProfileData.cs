using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HenryDev.Utilities;

namespace HenryDev.Gameplay
{
    [System.Serializable]
    public class ProfileData
    {
        public List<HeroSaveData> HeroSaveDatas;
        GlobalStatsSaveData GlobalStatsSaveData;
        public float Volume;
        public ProfileData()
        {
            this.HeroSaveDatas = new List<HeroSaveData>();
            this.GlobalStatsSaveData = new GlobalStatsSaveData();
            this.Volume = 0.5f;
        }
    }
    [System.Serializable]
    public class HeroSaveData
    {
        public string Name;
        public string Id;
        public int Rarity;
        public int Level;
        public int CurrentExp;
        public int LifetimeExp;
    }
    [System.Serializable]
    public class GlobalStatsSaveData
    {
        public string Attack;
        public string Health;
        public float AttackSpeed;
        public string HealthRegen;
        public float CriticalChance;
        public float CriticalDamage;
        public GlobalStatsSaveData()
        {
            this.Attack = "5";
            this.Health = "10";
            this.AttackSpeed = 1;
            this.HealthRegen = "1";
            this.CriticalChance = 0.05f;
            this.CriticalDamage = 1.5f;
        }
    }
}
