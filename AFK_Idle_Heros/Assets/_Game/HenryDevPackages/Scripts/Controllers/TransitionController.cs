using System.Collections;
using System.Collections.Generic;
using HenryDev.Intefaces;
using HenryDev.Events;
using UnityEngine;
using System;

namespace HenryDev.Controllers
{
    public class TransitionController : ITransition
    {
        public static readonly int TransitionInKey = Animator.StringToHash("In");
        public static readonly int TransitionOutKey = Animator.StringToHash("Out");
        private Animator anim;
        public TransitionController(Animator animator)
        {
            this.anim = animator;
        }
        public void TransitionIn()
        {
            UIEvents.SET_UI_BLOCK?.Invoke(true);
            SetTrigger(TransitionInKey);
        }

        public void TransitionOut()
        {
            SetTrigger(TransitionOutKey);
            UIEvents.SET_UI_BLOCK?.Invoke(false);
        }
        void SetTrigger(int key)
        {
            this.anim?.SetTrigger(key);
        }
    }
}
