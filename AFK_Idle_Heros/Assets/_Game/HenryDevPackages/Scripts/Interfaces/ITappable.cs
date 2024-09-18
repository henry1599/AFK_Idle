using System;
using System.Collections;
using System.Collections.Generic;
using HenryDev.Gameplay;
using UnityEngine;

namespace HenryDev.Intefaces
{
    public interface ITappable<T> where T : class
    {
        event Action<T> OnTapped;
        event Action<T> OnReleased;
    }
}
