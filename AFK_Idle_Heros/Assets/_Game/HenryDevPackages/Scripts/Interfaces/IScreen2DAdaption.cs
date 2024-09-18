using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HenryDev.Intefaces
{
    public interface IScreen2DAdaption
    {
        Vector2 GetScaleByScreen(Vector2 objectReferencedSize, Vector2 screenSizeReference, Vector2 screenSize);
    }
}
