using System.Windows;
using Microsoft.Win32;
using TetrisWPF.View;
using TetrisWPF.ViewModel;
using TetrisWPFModel;
using TetrisWPFModel.Persistence;

namespace TetrisWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ITetrisDataAccess _dataAccess = null!;
        private MainViewModel _viewModel = null!;
        private MainWindow _window = null!;
        private GameState _gameState = null!;

        private OpenFileDialog? _openFileDialog;
        private SaveFileDialog? _saveFileDialog;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _dataAccess = new TetrisDataAccess();
            _viewModel = new MainViewModel();
            _viewModel.LoadGame += ViewModel_Load;
            _viewModel.SaveGame += ViewModel_Save;

            _window = new MainWindow();
            _window.DataContext = _viewModel;
            _window.Show();
        }

        private void ViewModel_Save(object? sender, GameState gameState)
        {
            if (_saveFileDialog == null)
            {
                _saveFileDialog = new SaveFileDialog
                {
                    Title = "Save game state",
                    RestoreDirectory = true,
                    Filter = "TXT files (*.txt)|*.txt"
                };
            }

            if (_saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    gameState.SaveGame(_saveFileDialog.FileName);
                }
                catch (TetrisDataException ex)
                {
                    MessageBox.Show(
                        "Error in saving game!" + Environment.NewLine + "Incorrect path or type" + Environment.NewLine +
                        ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ViewModel_Load(object? sender, EventArgs e)
        {
            if (_openFileDialog == null)
            {
                _openFileDialog = new OpenFileDialog
                {
                    Title = "Load Tetris save",
                    RestoreDirectory = true,
                    Filter = "TXT files (*.txt)|*.txt"
                };
            }

            if (_openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _gameState = new GameState(_dataAccess, _openFileDialog.FileName);
                    _viewModel.LoadSave(_gameState);

                    await _viewModel.InitializeGameAsync();
                }
                catch (TetrisDataException ex)
                {
                    MessageBox.Show(
                        "Error in loading game!" + Environment.NewLine + "Incorrect path or type" +
                        Environment.NewLine + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}