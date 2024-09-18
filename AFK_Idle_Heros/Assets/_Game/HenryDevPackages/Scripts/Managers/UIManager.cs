using System;
using System.Collections;
using System.Collections.Generic;
using HenryDev.Events;
using UnityEngine;

namespace HenryDev.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance {get; private set;}
        [SerializeField] GameObject uiBlockPanel;
        void Awake()
        {
            if (Instance == null)
                Instance = this;

            UIEvents.SET_UI_BLOCK += HandleSetUIBlock;
        }
        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
                
            UIEvents.SET_UI_BLOCK -= HandleSetUIBlock;
        }

        private void HandleSetUIBlock(bool isBlock)
        {
            if (this.uiBlockPanel == null)
                return;
            this.uiBlockPanel.SetActive(isBlock);
        }
    }
}
