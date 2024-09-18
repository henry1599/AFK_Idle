using HenryDev.Controllers;
using HenryDev.Events;
using HenryDev.Intefaces;
using UnityEngine;

namespace HenryDev.Managers
{
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance {get; private set;}
        [SerializeField] Animator transitionAnimator;
        private ITransition controller;
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            this.controller = new TransitionController(this.transitionAnimator);

            UIEvents.TRANSITION_IN += HandleTransitionIn;
            UIEvents.TRANSITION_OUT += HandleTransitionOut;
        }
        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            
            UIEvents.TRANSITION_IN -= HandleTransitionIn;
            UIEvents.TRANSITION_OUT -= HandleTransitionOut;
        }

        private void HandleTransitionOut()
        {
            this.controller?.TransitionOut();
        }

        private void HandleTransitionIn()
        {
            this.controller?.TransitionIn();
        }
    }
}
