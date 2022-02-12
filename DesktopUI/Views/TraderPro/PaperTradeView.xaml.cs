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

namespace DesktopUI.Views.TraderPro
{
    /// <summary>
    /// Interaction logic for PaperTradeView.xaml
    /// </summary>
    public partial class PaperTradeView : UserControl
    {

        //Point currentPoint = new Point();

        public PaperTradeView()
        {
            InitializeComponent();
        }

        //public void MouseDown_Chart(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //    {
        //        currentPoint = e.GetPosition(this);
        //    }
        //}

        //public void MouseMove_Chart(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Line line = new Line();

        //        line.Stroke = Brushes.Aqua;
        //        line.X1 = currentPoint.X;
        //        line.Y1 = currentPoint.Y-200;
        //        line.X2 = e.GetPosition(this).X;
        //        line.Y2 = e.GetPosition(this).Y-200;

        //        currentPoint = e.GetPosition(this);

        //        canvas.Children.Add(line);
        //    }
        //}
    }
}
