using System.Windows.Media;

namespace TetrisWPF.ViewModel
{
    public class BlockCell : ViewModelBase
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public double TopPosition { get; set; }
        public double LeftPosition { get; set; }


        private ImageSource _source;

        public ImageSource ImageSource
        {
            get => _source;
            set
            {
                _source = value;
                OnPropertyChanged();
            }
        }

        public BlockCell(ImageSource source)
        {
            _source = source;
        }
        
    }
}
