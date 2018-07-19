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
using System.Windows.Shapes;
using TwilightImperium.ProgressTracker.Game;

namespace TwilightImperium.ProgressTracker.Views
{
    /// <summary>
    /// Interaction logic for ObjectiveSelectorWindow.xaml
    /// </summary>
    public partial class ObjectiveSelectorWindow : Window
    {
        public ObjectiveSelectorWindow(ObjectiveSelectorVM vm)
        {

            DataContext = vm;
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.PreviewKeyDown += (sender, args) =>
            {
                if (args.Key == Key.Return || args.Key == Key.Enter)
                {
                    Button_Click(null, null);
                    args.Handled = true;
                }

                if (args.Key == Key.Escape)
                    Button_Click_1(null, null);
            };
            InitializeComponent();

        }

        public ObjectiveCard[] ReturnCards = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReturnCards = ((ObjectiveSelectorVM)DataContext).GetSelectedObjectives();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
