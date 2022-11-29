using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace HobbyCards.SaveData
{
    public class SaveData
    {
        const string SaveFile = "savedata.json";

        public int CurrentCard { get; set; } = 0;

        public List<HobbyCard> ShuffledHobbyCards { get; set; } = new List< HobbyCard >();

        public List<HobbyCard> HobbyCards {  get; set; } = new List< HobbyCard >();

        public DateTime? LastDrawDay { get; set; } = DateTime.MinValue;

        public bool IsShuffled { get; set; }

        [JsonIgnore]
        public SaveData LoadedData { get; set; }

        public void Load()
        {
            if( !File.Exists( SaveFile ) )
            {
                File.Create( SaveFile ).Close();
            }

            var contents = File.ReadAllText( SaveFile );

            LoadedData = JsonConvert.DeserializeObject<SaveData>( contents );

            if( LoadedData == null )
            {
                LoadedData = new SaveData();
            }

            if( LoadedData.LastDrawDay == null )
            {
                LoadedData.LastDrawDay = DateTime.Now.AddDays( -1 );
            }
        }

        public void Save()
        {
            File.WriteAllText( SaveFile, JsonConvert.SerializeObject( LoadedData, Formatting.Indented ) );
            Load();
        }

        public bool CanSaveNewCard( string inName )
        {
            inName = inName ?? "";
            inName = inName.Trim();

            if( string.IsNullOrWhiteSpace( inName ) )
            {
                return false;
            }

            return LoadedData.HobbyCards.FirstOrDefault( hc => hc.Name.Equals( inName, System.StringComparison.InvariantCultureIgnoreCase )) == null;
        }

        public void Delete( string inName )
        {
            var hc = LoadedData.HobbyCards.FirstOrDefault( hc => hc.Name.Equals( inName, System.StringComparison.InvariantCultureIgnoreCase ) );
            
            if( hc != null )
            {
                LoadedData.HobbyCards.Remove( hc );

                Save();
                Load();
            }

            hc = LoadedData.ShuffledHobbyCards.FirstOrDefault( hc => hc.Name.Equals( inName, System.StringComparison.InvariantCultureIgnoreCase ) );

            if ( hc != null )
            {
                LoadedData.ShuffledHobbyCards.Remove( hc );

                Save();
                Load();
            }
        }

        public void Shuffle()
        {
            LoadedData.ShuffledHobbyCards.Clear();

            var hcCount = LoadedData.HobbyCards.Count;

            var swappedIndexes = new List<int>();
            var rand = new Random();

            var tempHobbyCards = new List<HobbyCard>( LoadedData.HobbyCards );

            for(int i = 0; i < hcCount; i++)
            {
                int swapWith = -1;

                do
                {
                    swapWith = rand.Next() % hcCount;
                }
                while( swappedIndexes.Contains( swapWith ) );

                if( swapWith == -1 )
                {
                    throw new Exception( "swapWith cannot be -1!" );
                }
                
                var tempHc = tempHobbyCards[ i ];
                tempHobbyCards[ i ] = tempHobbyCards[ swapWith ];
                tempHobbyCards[ swapWith ] = tempHc;
            }

            LoadedData.ShuffledHobbyCards = tempHobbyCards;
            LoadedData.IsShuffled = true;

            Save();
            Load();
        }

        public void DrawNextCard()
        {
            if( LoadedData.CurrentCard < LoadedData.ShuffledHobbyCards.Count )
            {
                LoadedData.LastDrawDay = DateTime.Now;
                LoadedData.CurrentCard++;
            }
            else if( LoadedData.CurrentCard >= LoadedData.ShuffledHobbyCards.Count )
            {
                Shuffle();
                LoadedData.CurrentCard = 0;
                DrawNextCard();
            }

            Save();
            Load();
        }

        public DateTime NextDrawDay()
        {
            var nextDrawDay = new DateTime( LoadedData.LastDrawDay.Value.Year, LoadedData.LastDrawDay.Value.Month, LoadedData.LastDrawDay.Value.Day, 0, 0, 1 ).AddDays(1);

            return nextDrawDay;
        }

        public TimeSpan TimeToNextDrawDay()
        {
            var nextDrawDay = NextDrawDay();

            var diff = nextDrawDay - DateTime.Now;

            if( diff.TotalSeconds <= 0 )
            {
                return new TimeSpan();
            }

            return new TimeSpan( diff.Hours, diff.Minutes, diff.Seconds );
        }

        public bool CanDrawNextCard()
        {
            return TimeToNextDrawDay().TotalSeconds <= 0 && LoadedData.IsShuffled;
        }

        public string GetCurrentDrawnCard()
        {
            var currentCard = LoadedData.CurrentCard - 1;

            if( currentCard < 0 )
            {
                return "No Card Selected";
            }

            if( LoadedData.ShuffledHobbyCards.Count > 0 && LoadedData.IsShuffled )
            {
                return LoadedData.ShuffledHobbyCards[ currentCard ].Name;
            }

            return "No Card Selected";            
        }
    }

    public class HobbyCard
    {
        public string Name { get; set; }
    }
}
