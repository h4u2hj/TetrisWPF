using System.Windows;

namespace TetrisWPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /*private readonly ImageSource[] _tileImages = new ImageSource[]
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
        
        private bool _isPaused = false;
        private DateTime _startTime;
        private DateTime _pauseStartTime;
        private double _pauseSeconds = 0;
        //private Image[,] _imageControls = null!;
        private double _pauseMinutes = 0;
        //private ITetrisDataAccess _dataAccess = new TetrisDataAccess();
        private MainViewModel _viewModel = new MainViewModel();
        private GameState _gameState = null!;
        
        
        
        
        /// <summary>
        /// Initializes and sets the picturebox controls according to the game grid
        /// </summary>
        /// <param name="grid">Model's game grid</param>
        /// <returns>Grid of picturebox controls</returns>
        private Image[,] SetupGameCanvas(GameGrid grid, int cellsize)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellsize,
                        Height = cellsize,
                    };
                    
                    Canvas.SetTop(imageControl, (r-2) * cellsize + 10);
                    Canvas.SetLeft(imageControl, c * cellsize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }


        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    _imageControls[r,c].Source = _tileImages[id];
                }
            }
        }
        
        
        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                _imageControls[p.Row, p.Column].Source = _tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = _blockImages[next.Id];
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(_gameState.BlockQueue);
            ScoreLabel.Text = $"Score: {_gameState.Score}";
        }
        

        private async Task GameLoop()
        {
            _viewModel.Draw(_gameState);
            while (!_gameState.GameOver)
            {
                await Task.Delay(600);
                if (!_isPaused)
                {
                    _gameState.MoveBlockDown();
                    _viewModel.Draw(_gameState);
                }
            }
            pauseButton.Visibility = Visibility.Hidden;
            StopGame();
            NextImage.Source = null;
            ScoreLabel.Text = $"Final Score: {_gameState.Score}";
            timeLabel.Text = $"Time: { (DateTime.Now - _startTime).TotalMinutes - _pauseMinutes:F0} mins\n\t{ ((DateTime.Now - _startTime).TotalSeconds - _pauseSeconds) % 60:F0} sec";
        }
        

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    _gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    _gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    _gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    _gameState.RotateBlockCCW();
                    break;
                case Key.X:
                    _gameState.DropBlock();
                    break;
                default:
                    return;
            }
            _viewModel.Draw(_gameState);
        }
        
        
        //Button event handlers from here
        
        private async void NewGame_Click(object sender, RoutedEventArgs e)
        {
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
            timeLabel.Text = "Time";
            ContinueGame();
            _startTime = DateTime.Now;
            if (!_isPaused)
            {
                //await GameLoop();
            }
        }*/

        /*private void newEasyGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState(18, 4, _dataAccess);
            InitializeGame();
        }

        private void newMediumGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState(18, 8, _dataAccess);
            InitializeGame();
        }

        private void newHardGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameState = new GameState(_dataAccess);
            InitializeGame();
        }

        private async void InitializeGame()
        {
            loadButton.Visibility = Visibility.Hidden;
            newEasyGameButton.Visibility = Visibility.Hidden; ;
            newMediumGameButton.Visibility = Visibility.Hidden; ;
            newHardGameButton.Visibility = Visibility.Hidden; ;
            pauseButton.Visibility = Visibility.Visible; ;

            switch (_gameState.GameDifficulty)
            {
                case GameDifficulty.Easy:
                    _viewModel.SetupGameCanvas(_gameState.GameGrid, 35);
                    //GameCanvas.Width = 140;
                    //GameCanvas.Height = 570;
                    break;
                case GameDifficulty.Medium:
                    _viewModel.SetupGameCanvas(_gameState.GameGrid, 35);
                    //GameCanvas.Width = 280;
                    //GameCanvas.Height = 570;
                    break;
                case GameDifficulty.Hard:
                    _viewModel.SetupGameCanvas(_gameState.GameGrid, 35);
                    //GameCanvas.Width = 420;
                    //GameCanvas.Height = 570;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _viewModel.Draw(_gameState);
            _startTime = DateTime.Now;
            KeyDown += OnKeyDown;
            await GameLoop();
        }
        
        private void ContinueGame()
        {
            KeyDown += OnKeyDown;
            _isPaused = false;
            loadButton.Visibility = Visibility.Hidden;
            saveButton.Visibility = Visibility.Hidden;
            pinkButton.Visibility = Visibility.Visible;
            newEasyGameButton.Visibility = Visibility.Hidden;
            newMediumGameButton.Visibility = Visibility.Hidden;
            newHardGameButton.Visibility = Visibility.Hidden;
            pauseButton.Content = "Pause";
            newGameButton.Visibility = Visibility.Hidden;
            pauseButton.Visibility = Visibility.Visible;
        }

        private void StopGame()
        {
            this.KeyDown -= OnKeyDown;
            newGameButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPaused)
            {
                _pauseStartTime = DateTime.Now;
                StopGame();
                _isPaused = true;
                saveButton.Visibility = Visibility.Visible;
                pinkButton.Visibility = Visibility.Visible;
                pauseButton.Content = "Play";
            }
            else
            {
                _pauseMinutes += (DateTime.Now - _pauseStartTime).TotalMinutes;
                _pauseSeconds += (DateTime.Now - _pauseStartTime).TotalSeconds;
                ContinueGame();
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = "TXT files (*.txt)|*.txt"
            };

            if (_openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _gameState = new GameState(_dataAccess, _openFileDialog.FileName);
                    //InitializeGame();
                }
                catch (TetrisDataException ex)
                {
                    MessageBox.Show("Error in loading game!" + Environment.NewLine + "Incorrect path or type" + Environment.NewLine + ex.Message, "Error!");
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = "TXT files (*.txt)|*.txt"
            };

            if (_saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _gameState.SaveGame(_saveFileDialog.FileName);
                }
                catch (TetrisDataException ex)
                {
                    MessageBox.Show("Error in saving game!" + Environment.NewLine + "Incorrect path or type" + Environment.NewLine + ex.Message, "Error!");
                }
            }
        }
        
        private void pinkButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _tileImages.Length; i++)
            {
                _tileImages[i] = new BitmapImage(new Uri($"../Assets/Single Blocks/Pink{i}.png", UriKind.Relative));
            }

            GameCanvas.Background = new SolidColorBrush(Color.FromArgb(255, 247, 166, 200));
            Draw(_gameState);
        }*/
    }
}