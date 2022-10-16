namespace Game.Scripts.ScriptableEvents
{
    public struct GridValueUpdatedArgs
    {
        public int X;
        public int Y;
        public GridElement GridElement;

        public GridValueUpdatedArgs(int x, int y, GridElement updatedElement)
        {
            X = x;
            Y = y;
            GridElement = updatedElement;
        }
    }
}