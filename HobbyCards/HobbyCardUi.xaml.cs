using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HobbyCards
{
    /// <summary>
    /// Interaction logic for HobbyCardUi.xaml
    /// </summary>
    public partial class HobbyCardUi : UserControl
    {
        private MainWindow m_mainWindow;
        public HobbyCardUi( MainWindow inMainWindow )
        {
            m_mainWindow = inMainWindow;
            InitializeComponent();
        }

        public void SetHobbyCardName( string inName )
        {
            lblHobbyCard.Content = inName;
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            MainWindow.m_saveData.Delete( lblHobbyCard.Content.ToString() );
            m_mainWindow.Reload();
        }
    }
}
