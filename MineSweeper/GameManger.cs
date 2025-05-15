namespace MineSweeper
{
    public enum GameManagerEvents
    {
        Win,
        Loose,
        MinLevelReached,
        MaxLevelReached,
    }

    // Handles The Game State
    class GameManger
    {
        private static GameManger? instance;
        public static GameManger GetInstance(Action<GameManagerEvents> gameStateChangeHandler)
        {
            instance ??= new GameManger(gameStateChangeHandler);
            return instance;
        }


        private int maxLevel = 1;
        public int Level { get => level;  }
        private int level = 1;
        private MineManager mineManager;
        private PlayField? playField;
        public int AmountOfBombs { get => mineManager.AmountOfBombs; }

        //bool is true if game was won
        private readonly Action<GameManagerEvents> gameStateChangeHandler;

        private MineManager InitMineManager()
        {
            return new(level, () => gameStateChangeHandler(GameManagerEvents.Loose), HandleWin);

        }

        private GameManger(Action<GameManagerEvents> gameStateChangeHandler)
        {
            this.gameStateChangeHandler = gameStateChangeHandler;
            mineManager = InitMineManager();
        }
        private void HandleWin()
        {
            maxLevel++;
            gameStateChangeHandler(GameManagerEvents.Win);
        }

        public void Reset()
        {
            mineManager = InitMineManager();
            playField = null;
        }

        public void Next()
        {
            if (maxLevel <= level) throw new Exception("Next Level need to be unlocked first");

            level++;

            if (maxLevel <= level) gameStateChangeHandler(GameManagerEvents.MaxLevelReached);
        }

        public void Prev()
        {
            if (level <= 1) throw new Exception("First Level Reached");
            level--;

            if(level <= 1) gameStateChangeHandler(GameManagerEvents.MinLevelReached); 
        }

        public PlayField GetPlayField()
        {
            playField ??= new(mineManager);
            return playField;
        }
    }
}
