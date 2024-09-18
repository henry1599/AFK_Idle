using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HenryDev.Utilities;

namespace HenryDev.Gameplay
{
    [System.Serializable]
    public class ProfileData
    {
        public int HighScore;
        public float Volume;
        public ProfileData()
        {
            this.HighScore = 0;
            this.Volume = 0.5f;
        }
    }
}
