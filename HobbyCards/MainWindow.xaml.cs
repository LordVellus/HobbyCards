using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HobbyCards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SaveData.SaveData m_saveData = new SaveData.SaveData();

        public MainWindow()
        {
            InitializeComponent();

            m_saveData.Load();
            Reload();

            UpdateShffleButton();

            var nextCardDrawTimer = new DispatcherTimer();
            nextCardDrawTimer.Interval = TimeSpan.FromSeconds( 1 );
            nextCardDrawTimer.Tick += NextCardDrawTimer_Tick;
            nextCardDrawTimer.Start();
        }

        private void NextCardDrawTimer_Tick( object sender, EventArgs e )
        {
            lblTimeToNextCardDraw.Content = $"Time to next card draw { m_saveData.TimeToNextDrawDay() }";
        }

        public void Reload()
        {
            HobbyCardsStackPanel.Children.Clear();

            foreach( var hc in m_saveData.LoadedData.HobbyCards )
            {
                var newHobbyCard = new HobbyCardUi( this );
                newHobbyCard.SetHobbyCardName( hc.Name );

                HobbyCardsStackPanel.Children.Add( newHobbyCard );
            }

            btnDrawNextCard.IsEnabled = m_saveData.CanDrawNextCard();
            lblCurrentHobbyCard.Content = m_saveData.GetCurrentDrawnCard();

            UpdateShffleButton();

            lblPreviousCards.Content = "";

            var previousCards = new StringBuilder();

            for(int i = 0; i < m_saveData.LoadedData.CurrentCard - 1; i++)
            {
                var hc = m_saveData.LoadedData.ShuffledHobbyCards[ i ];
                previousCards.AppendLine( hc.Name );
            }

            lblPreviousCards.Content += previousCards.ToString();
        }

        private void UpdateShffleButton()
        {
            if( m_saveData.LoadedData.IsShuffled )
            {
                btnShuffle.Background = Brushes.Green;
            }
            else
            {
                btnShuffle.Background = Brushes.OrangeRed;
            }
        }

        private void btnAddCardButton_Click( object sender, RoutedEventArgs e )
        {
            if( !m_saveData.CanSaveNewCard( tbAddCardName.Text ) )
            {
                MessageBox.Show( $"Cannot save {tbAddCardName.Text}" );
                return;
            }

            var newHobbyCard = new SaveData.HobbyCard
            {
                Name = tbAddCardName.Text,
            };

            m_saveData.LoadedData.HobbyCards.Add( newHobbyCard );
            m_saveData.LoadedData.ShuffledHobbyCards.Add( newHobbyCard );
            m_saveData.Save();

            Reload();

            tbAddCardName.Text = "";
        }

        private void btnShuffle_Click( object sender, RoutedEventArgs e )
        {
            if( !m_saveData.LoadedData.IsShuffled && m_saveData.LoadedData.HobbyCards.Count > 1 )
            {
                m_saveData.Shuffle();
            }

            UpdateShffleButton();
            Reload();
        }

        private void btnDrawNextCard_Click( object sender, RoutedEventArgs e )
        {
            if( m_saveData.CanDrawNextCard() )
            {
                m_saveData.DrawNextCard();
                Reload();
            }
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            if( MessageBox.Show( "Are you sure you want to draw the next card?", "Draw Next Card Early?", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
                m_saveData.DrawNextCard();
                Reload();
            }
        }
    }
}
