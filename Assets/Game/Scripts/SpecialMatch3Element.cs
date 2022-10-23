using System.Collections.Generic;

namespace Game.Scripts
{
    public abstract class SpecialMatch3Element : Match3Element
    {
        public abstract HashSet<GridElement> HandleSpecialAbility(GridModel gridModel, Match3Element pairedElement, GridElement self);
    }
}