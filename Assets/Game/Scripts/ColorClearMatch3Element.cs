using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    [CreateAssetMenu(fileName = "ColorClearMatch3Element", menuName = "Match3/New ColorClear Match3 Element", order = 0)]
    public class ColorClearMatch3Element : SpecialMatch3Element
    {
        public override HashSet<GridElement> HandleSpecialAbility(GridModel gridModel, Match3Element pairedElement, GridElement self)
        {
            var hashSet = new HashSet<GridElement>();

            foreach (var gridElement in gridModel.Grid.GridReference)
            {
                if (gridElement.Match3Element == pairedElement)
                    hashSet.Add(gridElement);
            }
            
            return hashSet;
        }
    }
}