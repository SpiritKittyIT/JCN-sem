using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame.Screens
{
    /// <summary>
    /// Interaction logic for NewScoreForm.xaml
    /// </summary>
    public partial class NewScoreForm : UserControl
    {

        private MainWindow _mainWindow;

        public NewScoreForm(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
    }
}
