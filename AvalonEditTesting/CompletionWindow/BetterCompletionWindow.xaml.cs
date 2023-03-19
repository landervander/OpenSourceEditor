using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AvalonEditTesting.CompletionWindow
{
    /// <summary>
    /// Interaction logic for BetterCompletionWindow.xaml
    /// </summary>
    public partial class BetterCompletionWindow : Window
    {
        Point visualLocation, visualLocationTop;
        public void InitializeCompletionDatabase(ObservableCollection<CompletionData> data)
        {
            DataClass.vm.Data = data;
        }
        private void UpdatePosition(TextView textView, TextViewPosition position)
        {
            Point location = textView.PointToScreen(visualLocation - textView.ScrollOffset);

            visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
            visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
            Size completionWindowSize = new Size(this.ActualWidth, this.ActualHeight).TransformToDevice(textView);
            Rect bounds = new Rect(location, completionWindowSize);
            bounds = bounds.TransformFromDevice(textView);
            this.Left = bounds.X;
            this.Top = bounds.Y;
        }
        public void HideCompletionWindow()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void ShowCompletionWindow()
        {
            this.Visibility = Visibility.Visible;
        }

        public void Filter(string text)
        {
            DataClass.vm.d_Data.Clear();
            foreach(var item in DataClass.vm.Data)
            {
                try
                {
                    if (item.Content.ToString().StartsWith(text))
                    {
                        DataClass.vm.d_Data.Add(item);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public BetterCompletionWindow(TextArea textArea)
        {
            InitializeComponent();
            ShowActivated = false;
            this.DataContext = DataClass.vm;
            textArea.Caret.PositionChanged += delegate(object sender, EventArgs e) 
            {
                UpdatePosition(textArea.TextView, new TextViewPosition(textArea.Document.GetLocation(textArea.Caret.Offset)));
            };
        }

        public bool CloseAutomatically { get; set; }
    }
    public class Observable : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public static class Extensions
    {
        public static Size TransformToDevice(this Size size, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
        }
        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }
    }
    public class DataClass : Observable
    {
        private CompletionData _selectedItem;

        public CompletionData SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(); }
        }
        private ObservableCollection<CompletionData> _data;

        public ObservableCollection<CompletionData> Data
        {
            get { return _data; }
            set 
            { 
                _data = value;
            }
        }
        private ObservableCollection<CompletionData> _d_data = new ObservableCollection<CompletionData>();

        public ObservableCollection<CompletionData> d_Data
        {
            get { return _d_data; }
            set
            {
                _d_data = value;
                OnPropertyChanged();
            }
        }
        public static DataClass vm = new DataClass();
    }
    public class CompletionData
    {
        public CompletionData()
        {
        }

        public ClassType Type { get; set; }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Content { get; set; }

        public string Description { get; set; }
    }
    public enum ClassType
    {
        Global,
        GlobalType,
        Function,
        Enum
    }
}
