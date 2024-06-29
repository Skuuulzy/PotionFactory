namespace Components.Grid
{
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public float Size { get; }
        public bool ContainsObject { get; private set; }

        public Cell(int x, int y, float size, bool containsObject)
        {
            X = x;
            Y = y;
            Size = size;
            ContainsObject = containsObject;
        }

        public void AddMachineToCell()
        {
            ContainsObject = true;
        }

        public void RemoveMachineFromCell()
        {
            ContainsObject = false;
        }
    }
}