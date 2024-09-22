using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HenryDev
{
    
    [SerializeField, InlineEditor]
    public class Grid<T> : IGrid<T> where T : ICell
    {
        private T[,] cells;
        private int width;
        private int height;
        private float cellSize;
        public Vector2Int Size => new Vector2Int(this.width, this.height);
        public int Width => this.width;
        public int Height => this.height;
        public float CellSize => this.cellSize;

        public T this[int x, int y]
        {
            get => cells[x, y];
            set => cells[x, y] = value;
        }
        public T this[Vector2Int pos]
        {
            get => cells[pos.x, pos.y];
            set => cells[pos.x, pos.y] = value;
        }
        public T this[int index]
        {
            get => cells[index % width, index / width];
            set => cells[index % width, index / width] = value;
        }
        public Grid(int width, int height, float cellSize = 1)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            cells = new T[width, height];
        }
        public Grid(IGrid<T> grid)
        {
            this.width = grid.Width;
            this.height = grid.Height;
            cells = new T[width, height];
            LoopGrid((x, y) =>
            {
                this[x, y] = grid[x, y];
            });
        }

        public void LoopGrid(Action<T> action)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    action(cells[x, y]);
                }
            }
        }
        public void LoopGrid(Action<int, int> action)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    action(x, y);
                }
            }
        }
        public void UpdateCell(int x, int y, bool flag, params Grid<T>[] childrenGrids)
        {
            cells[x, y].Flag = flag;
            foreach (var grid in childrenGrids)
            {
                if (grid.Size != this.Size)
                    continue;
                grid.cells[x, y].Flag = flag;
            }
        }
    }
}
