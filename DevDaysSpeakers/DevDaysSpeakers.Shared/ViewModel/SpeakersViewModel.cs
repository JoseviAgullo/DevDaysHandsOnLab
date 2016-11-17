using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool _busy { get; set; }

        public bool IsBusy
        {
            get
            {
                return _busy;
            }
            set
            {
                _busy = value;
                OnPropertyChanged();

                GetSpeakersCommand.ChangeCanExecute();
            }
        }

        public ObservableCollection<Speaker> Speakers { get; set; }

        public Command GetSpeakersCommand { get; set; }

        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();

            GetSpeakersCommand = new Command(
                async () => await GetSpeakers(),
                () => !IsBusy);
        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;

                using (var client = new HttpClient())
                {
                    //grab json from server
                    var service = DependencyService.Get<AzureService>();
                    var items = await service.GetSpeakers();

                    Speakers.Clear();

                    Speakers.Add(GetAdditionalSpeaker());

                    foreach (var item in items)
                        Speakers.Add(item);
                }

            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                IsBusy = false;
                if (error != null)
                    await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");
            }
        }

        private Speaker GetAdditionalSpeaker()
        {
            return new Speaker
            {
                Id = "123445",
                Avatar = "https://pbs.twimg.com/profile_images/719826977843126272/MKA1NPHo_400x400.jpg",
                Name = "Josevi Agulló",
                Description = ".Net Technical Consultant at Valtech",
                Title = "Last Monkey",
                Website = "https://twitter.com/Josevi_7"
            };
        }
    }
}
