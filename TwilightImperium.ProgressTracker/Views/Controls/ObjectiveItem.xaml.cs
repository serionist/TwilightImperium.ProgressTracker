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

namespace TwilightImperium.ProgressTracker.Views.Controls
{
    /// <summary>
    /// Interaction logic for ObjectiveItem.xaml
    /// </summary>
    public partial class ObjectiveItem : UserControl
    {
        public ObjectiveItem()
        {
            InitializeComponent();

        }

        public static DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(ObjectiveItem), new PropertyMetadata(false));

        public bool ShowCheckBox
        {
            get => (bool) GetValue(ShowCheckBoxProperty);
            set => SetValue(ShowCheckBoxProperty, value);
        }
    }
}
