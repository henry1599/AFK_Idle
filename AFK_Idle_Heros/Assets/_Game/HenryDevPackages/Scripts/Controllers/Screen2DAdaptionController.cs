using System.Collections;
using System.Collections.Generic;
using HenryDev.Intefaces;
using UnityEngine;

namespace HenryDev.Controllers
{
    public class Screen2DAdaptionController : IScreen2DAdaption
    {
        public Screen2DAdaptionController() { }
        public Vector2 GetScaleByScreen(Vector2 objectReferencedSize, Vector2 screenSizeReference, Vector2 screenSize)
        {
            return new Vector2(
                screenSize.x * objectReferencedSize.x / screenSizeReference.x,
                screenSize.y * objectReferencedSize.y / screenSizeReference.y
            );
        }
    }
}
