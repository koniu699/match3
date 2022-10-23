using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "BombMatch3Element", menuName = "Match3/New Bomb Match3 Element", order = 0)]
    public class BombMatch3Element : SpecialMatch3Element
    {
        [SerializeField] int range = 2;
        
        public override HashSet<GridElement> HandleSpecialAbility(GridModel gridModel, Match3Element pairedElement, GridElement self)
        {
            var hashSet = new HashSet<GridElement>();

            for (var i = -range; i <= range; i++)
            {
                for (var j = -range; j <= range; j++)
                {
                    var posX = self.X + i;
                    var posY = self.Y + j;
                    if (gridModel.Grid.PositionInBounds(posX, posY))
                    {
                        hashSet.Add(gridModel.Grid.GetGridObject(posX, posY));
                    }
                }
            }
            return hashSet;
        }
    }
}