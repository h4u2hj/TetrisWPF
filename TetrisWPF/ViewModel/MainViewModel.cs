using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using TetrisWPFModel.Persistence;
using TetrisWPFModel;

namespace TetrisWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ImageSource[] _tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../Assets/Single Blocks/TileEmpty2.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/LightBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Blue.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Orange.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Yellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Green.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Purple.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Single Blocks/Red.png", UriKind.Relative))
        };

        private readonly ImageSource[] _blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../Assets/Shape Blocks/BlockEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/I.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/J.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/L.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/O.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/S.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/T.png", UriKind.Relative)),
            new BitmapImage(new Uri("../Assets/Shape Blocks/Z.png", UriKind.Relative))
        };

        private ITetrisDataAccess _dataAccess = new TetrisDataAccess();
        private GameState _gameState = null!;
        private BlockCell[,] _imageControls = null!;
        private bool _isPaused;
        private DateTime _startTime;
        private DateTime _pauseStartTime;
        private double _pauseMinutes;
        private double _pauseSeconds;

        public ObservableCollection<BlockCell> GameCanvasImages { get; private set; }

        public ImageSource NextBlockImage { get; private set; } =
            new BitmapImage(new Uri("../Assets/start.png", UriKind.Relative));

        public event EventHandler? LoadGame;
        public event EventHandler<GameState>? SaveGame;


        public DelegateCommand NewEasyGameCommand { get; private set; }
        public DelegateCommand NewMediumGameCommand { get; private set; }
        public DelegateCommand NewHardGameCommand { get; private set; }
        public DelegateCommand? KeyInputCommand { get; private set; }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand PauseCommand { get; private set; }
        public DelegateCommand PinkCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }

        public int CanvasWidth { get; private set; } = 420;
        public int CanvasHeight { get; private set; } = 570;
        public SolidColorBrush BackgroundColor { get; private set; } = new SolidColorBrush(Colors.Gray);

        public string Score => _gameState == null ? "Score: 0" : $"Score: {_gameState.Score.ToString()}";
        public string PausedText { get; private set; } = "Pause";
        public string TimeText { get; private set; } = "";

        public bool GameStartingVisibility { get; private set; } = true;
        public bool PausedVisibility { get; private set; } = false;
        public bool NewGameVisibility { get; private set; } = false;
        public bool SaveVisibility { get; private set; } = false;

        public MainViewModel()
        {
            GameCanvasImages = new ObservableCollection<BlockCell>();

            NewEasyGameCommand = new DelegateCommand(async void (param) => await NewEasyGame());
            NewMediumGameCommand = new DelegateCommand(async void (param) => await NewMediumGame());
            NewHardGameCommand = new DelegateCommand(async void (param) => await NewHardGameAsync());
            NewGameCommand = new DelegateCommand(async void (param) => await NewGame());
            PauseCommand = new DelegateCommand(param => Pause());
            PinkCommand = new DelegateCommand(param => PinkMode());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
        }

        private void SetupGameCanvas(GameGrid grid, int cellSize)
        {
            _imageControls = new BlockCell[grid.Rows, grid.Columns];
            GameCanvasImages.Clear();

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    BlockCell imageControl = new BlockCell(_tileImages[0])
                    {
                        Width = cellSize,
                        Height = cellSize,
                        TopPosition = (r - 2) * cellSize + 10,
                        LeftPosition = c * cellSize
                    };
                    GameCanvasImages.Add(imageControl);
                    _imageControls[r, c] = imageControl;
                }
            }

            OnPropertyChanged(nameof(GameCanvasImages));
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    _imageControls[r, c].ImageSource = _tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                _imageControls[p.Row, p.Column].ImageSource = _tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextBlockImage = _blockImages[next.Id];
            OnPropertyChanged(nameof(NextBlockImage));
        }

        private void Draw()
        {
            DrawGrid(_gameState.GameGrid);
            DrawBlock(_gameState.CurrentBlock);
            DrawNextBlock(_gameState.BlockQueue);
        }

        private async Task GameLoop()
        {
            Draw();
            while (!_gameState.GameOver)
            {
                await Task.Delay(600);
                if (!_isPaused)
                {
                    _gameState.MoveBlockDown();
                    Draw();
                }
            }


            StopGame();
            NextBlockImage = new BitmapImage(new Uri("../Assets/start.png", UriKind.Relative));
            OnPropertyChanged(nameof(NextBlockImage));
            ScoreChanged(_gameState, EventArgs.Empty);
            PausedVisibility = false;
            OnPropertyChanged(nameof(PausedVisibility));
            TimeText =
                $"Time: {(DateTime.Now - _startTime).TotalMinutes - _pauseMinutes:F0} mins\n\t{((DateTime.Now - _startTime).TotalSeconds - _pauseSeconds) % 60:F0} sec";
            OnPropertyChanged(nameof(TimeText));
        }

        private void OnKeyDown(string key)
        {
            switch (key)
            {
                case "l":
                    _gameState.MoveBlockLeft();
                    break;
                case "r":
                    _gameState.MoveBlockRight();
                    break;
                case "d":
                    _gameState.MoveBlockDown();
                    break;
                case "u":
                    _gameState.RotateBlockCW();
                    break;
                case "z":
                    _gameState.RotateBlockCCW();
                    break;
                case "x":
                    _gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw();
        }


        //Commands and methods from here

        private async Task NewEasyGame()
        {
            _gameState = new GameState(18, 4, _dataAccess);
            _gameState.ScoreChanged += ScoreChanged;
            await InitializeGameAsync();
        }

        private async Task NewMediumGame()
        {
            _gameState = new GameState(18, 8, _dataAccess);
            _gameState.ScoreChanged += ScoreChanged;
            await InitializeGameAsync();
        }

        private async Task NewHardGameAsync()
        {
            _gameState = new GameState(_dataAccess);
            _gameState.ScoreChanged += ScoreChanged;
            await InitializeGameAsync();
        }

        public async Task InitializeGameAsync()
        {
            GameStartingVisibility = false;
            PausedVisibility = true;
            OnPropertyChanged(nameof(GameStartingVisibility));
            OnPropertyChanged(nameof(PausedVisibility));

            switch (_gameState.GameDifficulty)
            {
                case GameDifficulty.Easy:
                    SetupGameCanvas(_gameState.GameGrid, 35);
                    CanvasHeight = 570;
                    CanvasWidth = 140;
                    break;
                case GameDifficulty.Medium:
                    SetupGameCanvas(_gameState.GameGrid, 35);
                    CanvasHeight = 570;
                    CanvasWidth = 280;
                    break;
                case GameDifficulty.Hard:
                    SetupGameCanvas(_gameState.GameGrid, 35);
                    CanvasHeight = 570;
                    CanvasWidth = 420;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            OnPropertyChanged(nameof(CanvasHeight));
            OnPropertyChanged(nameof(CanvasWidth));

            Draw();
            _startTime = DateTime.Now;
            KeyInputCommand = new DelegateCommand(param => OnKeyDown(param?.ToString() ?? string.Empty));
            OnPropertyChanged(nameof(KeyInputCommand));
            await GameLoop();
        }

        private void StopGame()
        {
            KeyInputCommand = null;
            OnPropertyChanged(nameof(KeyInputCommand));
            NewGameVisibility = true;
            OnPropertyChanged(nameof(NewGameVisibility));
        }

        private void ContinueGame()
        {
            KeyInputCommand = new DelegateCommand(param => OnKeyDown(param?.ToString() ?? string.Empty));
            OnPropertyChanged(nameof(KeyInputCommand));
            _isPaused = false;

            SaveVisibility = false;
            OnPropertyChanged(nameof(SaveVisibility));
            GameStartingVisibility = false;
            PausedVisibility = true;
            OnPropertyChanged(nameof(GameStartingVisibility));
            OnPropertyChanged(nameof(PausedVisibility));
            PausedText = "Pause";
            OnPropertyChanged(nameof(PausedText));
            NewGameVisibility = false;
            OnPropertyChanged(nameof(NewGameVisibility));
        }

        private async Task NewGame()
        {
            bool gameWasOver = _gameState.GameOver;
            if (_gameState.GameDifficulty == GameDifficulty.Easy)
            {
                _gameState = new GameState(18, 4, _dataAccess);
            }
            else if (_gameState.GameDifficulty == GameDifficulty.Medium)
            {
                _gameState = new GameState(18, 8, _dataAccess);
            }
            else
            {
                _gameState = new GameState(_dataAccess);
            }

            _pauseSeconds = 0;
            _pauseMinutes = 0;
            ContinueGame();
            _startTime = DateTime.Now;
            ScoreChanged(_gameState, EventArgs.Empty);
            _gameState.ScoreChanged += ScoreChanged;
            TimeText = "";
            OnPropertyChanged(nameof(TimeText));

            if (gameWasOver)
            {
                await GameLoop();
            }
        }

        private void Pause()
        {
            if (!_isPaused)
            {
                _pauseStartTime = DateTime.Now;
                StopGame();
                _isPaused = true;
                SaveVisibility = true;
                OnPropertyChanged(nameof(SaveVisibility));
                PausedText = "Play";
                OnPropertyChanged(nameof(PausedText));
            }
            else
            {
                _pauseMinutes += (DateTime.Now - _pauseStartTime).TotalMinutes;
                _pauseSeconds += (DateTime.Now - _pauseStartTime).TotalSeconds;
                ContinueGame();
            }
        }

        private void PinkMode()
        {
            for (int i = 0; i < _tileImages.Length; i++)
            {
                _tileImages[i] = new BitmapImage(new Uri($"../Assets/Single Blocks/Pink{i}.png", UriKind.Relative));
            }

            BackgroundColor = new SolidColorBrush(Color.FromArgb(255, 247, 166, 200));
            OnPropertyChanged(nameof(BackgroundColor));
            Draw();
        }

        public void LoadSave(GameState gameState)
        {
            _gameState = gameState;
            _gameState.ScoreChanged += ScoreChanged;
            ScoreChanged(_gameState, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, _gameState);
        }

        private void ScoreChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Score));
        }
    }
}