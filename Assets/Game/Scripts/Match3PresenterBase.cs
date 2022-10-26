using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts
{
    public abstract class Match3PresenterBase : MonoBehaviour
    {
        public abstract void RedrawBoard();
        public abstract Task SwapElements(GridElement firstElement, GridElement secondElement);
        public abstract void ShowElements();

        public abstract void DropElement(int originX, int originY, int destinationY);
        public abstract Task ShowSpawnElements(List<Vector2Int> elementsToSpawn);
    }
}