namespace ZeroChat.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ApplicationViewModel dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}
