using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HenryDev
{
    public interface IGrid<T> where T : ICell
    {
        T this[int x, int y] { get; set; }
        T this[Vector2Int pos] { get; set; }
        T this[int index] { get; set; }
        Vector2Int Size { get; }
        int Width { get; }
        int Height { get; }
        float CellSize { get; }
        void LoopGrid(Action<T> action);
        void LoopGrid(Action<int, int> action);
    }
    public interface ICell
    {
        int X { get; }
        int Y { get; }
        float Size { get; }
        Vector2 Position { get; }
        bool Flag { get; set; }
        void SetupPosition(Transform pivot, int gridWidth, int gridHeight);
    }
}