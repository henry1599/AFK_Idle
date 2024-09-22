using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HenryDev
{   
    [SerializeField, InlineEditor]
    public class Cell : ICell
    {
        private int x;
        private int y;
        private float size;
        public Vector2 position;
        public Cell(int x, int y, float size = 1)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.Flag = false;
        }
        public int X => this.x;
        public int Y => this.y;
        public float Size => this.size;
        public Vector2 Position => position;

        public bool Flag { get; set; }

        public void SetupPosition(Transform pivot, int gridWidth, int gridHeight)
        {
            this.position = new Vector2(X, Y) * Size + (Vector2)pivot.position - new Vector2(gridWidth, gridHeight) * Size * 0.5f;
        }
    }
}
