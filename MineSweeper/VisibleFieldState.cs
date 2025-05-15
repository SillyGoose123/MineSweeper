namespace MineSweeper
{
    public enum FieldType
    {
        Clear,
        Mine,
        Number,
    }
    public class VisibleFieldState()
    {
        public readonly FieldType type;
        public readonly int mineCount = 0;
        public readonly int x;
        public readonly int y;

        public VisibleFieldState(FieldType fieldType, int xPos, int yPos) : this()
        {
            type = fieldType;
            x = xPos;
            y = yPos;
        }


        public VisibleFieldState(int count, int xPos, int yPos) : this()
        {
            type = FieldType.Number;
            mineCount = count;
            x = xPos;
            y = yPos;
        }
    }
}