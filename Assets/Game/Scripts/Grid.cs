using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public class Grid<T>
    {
        public UnityAction GridUpdated;
        public UnityAction<int, int, T> GridObjectValueUpdated;
        public UnityAction<int, int, T> GridObjectUpdated;

        public T[,] GridReference { get; }

        public Grid(int width, int height, Func<Grid<T>, int, int, T> newElemFunc)
        {
            GridReference = new T[width, height];
            for (var i = 0; i < GridReference.GetLength(0); i++)
            {
                for (var j = 0; j < GridReference.GetLength(1); j++)
                {
                    GridReference[i, j] = newElemFunc(this, i, j);
                }
            }

            GridUpdated?.Invoke();
        }

        public void SetGridObject(int x, int y, T value)
        {
            if (!PositionInBounds(x, y))
            {
                Debug.LogWarning("Input is outside of array bounds.");
                return;
            }

            GridReference[x, y] = value;
            GridObjectUpdated?.Invoke(x, y, value);
        }

        public void TriggerGridObjectValueUpdated(int x, int y)
        {
            if (PositionInBounds(x, y))
                GridObjectValueUpdated?.Invoke(x, y, GridReference[x, y]);
        }

        public T GetGridObject(int x, int y)
        {
            return !PositionInBounds(x, y) ? default(T) : GridReference[x, y];
        }

        public bool PositionInBounds(int x, int y)
        {
            return (x >= 0 && x < GridReference.GetLength(0) && y >= 0 && y < GridReference.GetLength(1));
        }
    }
}