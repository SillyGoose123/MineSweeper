namespace MineSweeper
{
    // Handles the PlayField States
    public class MineManager

    {
        private readonly bool[][] isMine;
        public int Width { get => isMine.Length; }
        public int Height { get => isMine[0].Length; }

        private readonly int amountOfBombs;
        public int AmountOfBombs { get => amountOfBombs; }
        public readonly Action OnGameOver;
        private readonly Action OnGameWin;

        private List<int[]> wasChecked = [];

        private int missingVisibleFields;

        static T[][] GenerateSizeArray<T>(int size, T defaultValue)
        {
            T[][] array = new T[size][];
            for (int i = 0; i < size; i++)
            {
                array[i] = new T[size];
                for (int j = 0; j < size; j++)
                {
                    array[i][j] = defaultValue;
                }
            }
            return array;
        }

        public MineManager(int level, Action onGameOver, Action onGameWin)
        {
            int size = level * 5;
            amountOfBombs = (size) / 5 * 4;
            missingVisibleFields = size * size - amountOfBombs;
            isMine = GenerateMines(size, AmountOfBombs);
            OnGameOver = onGameOver;
            OnGameWin = onGameWin;
        }

        static bool[][] GenerateMines(int size, int amountOfBombs)
        {
            bool[][] isMine = GenerateSizeArray(size, false);
            Random random = new();

            for (int i = 0; i < amountOfBombs; i++)
            {
                int x = random.Next(0, size);
                int y = random.Next(0, size);

                while (isMine[x][y]) // redo if mine
                {
                    x = random.Next(0, size);
                    y = random.Next(0, size);
                }

                isMine[x][y] = true;
            }

            return isMine;
        }


        public VisibleFieldState[] Check(int x, int y)
        {
            if (isMine[x][y])
            {             
                return [new VisibleFieldState(FieldType.Mine, x, y)];
            }

            int counter = ScanAround(x, y);
            if (counter > 0)
            {
                wasChecked.Add([x, y]);
                return [new(counter, x, y)];
            }

            //LOGIC TO REMOVE ALL UNTIL NUMBERS
            List<VisibleFieldState> updatedFields = [];
            List<int[]> shouldCheck = [[x,y]];
            
            while(shouldCheck.Count > 0)
            {
                int[] cords = shouldCheck[0];
                shouldCheck.RemoveAt(0);
                wasChecked.Add(cords);

                int count = ScanAround(cords[0], cords[1]);
                if (count > 0)
                {
                    updatedFields.Add(new(count, cords[0], cords[1]));
                    continue;
                }
                shouldCheck.AddRange(GetFieldAround(cords[0], cords[1], wasChecked, shouldCheck)); 

                updatedFields.Add(new(FieldType.Clear, cords[0], cords[1]));
            }
            return [..updatedFields];
        }

        public void CheckForWin(int amount)
        {
            missingVisibleFields -= amount;
            if(missingVisibleFields == 0)
            {
                OnGameWin();
            }
        }

        private List<int[]>GetFieldAround(int startX, int startY, List<int[]>? ignore, List<int[]>? ignore2)
        {       
            List<int[]> fields = [];
            for (int moveX = -1; moveX < 2; moveX++)
            {
                for (int moveY = -1; moveY < 2; moveY++)
                {
                    int x = moveX + startX;
                    int y = moveY + startY;

                    bool isInBounds = x >= 0 && x < Width && y >= 0 && y < Height;
                    bool isNotStartingPoint = !(startX == x && startY == y);
                    bool isNotIgnored = ignore == null || ignore2 == null || (!ignore2.Any((cords) => cords[0] == x && cords[1] == y) && !ignore.Any((cords) => cords[0] == x && cords[1] == y)) ;
                    if (isInBounds && isNotIgnored && isNotStartingPoint)
                    {
                        fields.Add([x, y]);
                    }
                }
            }
            return fields;
        }

        private int ScanAround(int startX, int startY)
        {
            int counter = 0;
            foreach (int[] cords in GetFieldAround(startX, startY, null, null))
            {
                if (isMine[cords[0]][cords[1]]) counter++;
            }
            return counter;
        }
    }
}
