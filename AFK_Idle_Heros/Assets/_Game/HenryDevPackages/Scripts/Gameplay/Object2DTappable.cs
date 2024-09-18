using System;
using System.Collections;
using System.Collections.Generic;
using HenryDev.Intefaces;
using HenryDev.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HenryDev.Gameplay
{
    public class Object2DTappable : MonoBehaviour, ITappable<Object2DTappable>
    {
        public Collider2D Col2D 
        {
            get 
            {
                if (this.col2D == null)
                    this.col2D = GetComponent<Collider2D>();
                return this.col2D;
            }
        } protected Collider2D col2D;

        // * Interface
        public event Action<Object2DTappable> OnTapped;
        public event Action<Object2DTappable> OnReleased;
        
        protected bool wasTapped = false;
        void OnMouseDown()
        {
            if (RaycastManager.Instance?.IsPointerOverUIElement() ?? false)
                return;
            if (this.wasTapped)
                return;
            this.wasTapped = true;
            OnTapped?.Invoke(this);
        }
        void OnMouseUp()
        {
            if (RaycastManager.Instance?.IsPointerOverUIElement() ?? false)
                return;
            if (!this.wasTapped)    
                return;
            this.wasTapped = false;
            OnReleased?.Invoke(this);
        }
    }
}
