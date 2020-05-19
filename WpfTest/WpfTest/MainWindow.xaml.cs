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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTest {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
            NavigationWindow win = new NavigationWindow();
            win.Content = new StoryboardsExample();
            win.Show();
		}
	}
    public class StoryboardsExample : Page {
        public StoryboardsExample() {
            this.WindowTitle = "Storyboards Example";
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Margin = new Thickness(20);

            Rectangle myRectangle = new Rectangle();
            myRectangle.Name = "MyRectangle";

            // Create a name scope for the page.
            NameScope.SetNameScope(this, new NameScope());

            this.RegisterName(myRectangle.Name, myRectangle);
            myRectangle.Width = 100;
            myRectangle.Height = 100;
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Blue);
            this.RegisterName("MySolidColorBrush", mySolidColorBrush);
            myRectangle.Fill = mySolidColorBrush;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 100;
            myDoubleAnimation.To = 200;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTargetName(myDoubleAnimation, myRectangle.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation,
                new PropertyPath(Rectangle.WidthProperty));

            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Colors.Blue;
            myColorAnimation.To = Colors.Red;
            myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(myColorAnimation, mySolidColorBrush);
            Storyboard.SetTargetProperty(myColorAnimation,
                new PropertyPath(SolidColorBrush.ColorProperty));
            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Children.Add(myColorAnimation);

            myRectangle.MouseEnter += delegate (object sender, MouseEventArgs e) {
                myStoryboard.Begin(this);
            };

            myStackPanel.Children.Add(myRectangle);
            this.Content = myStackPanel;
        }
    }
}
