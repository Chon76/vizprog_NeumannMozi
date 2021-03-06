﻿using System;
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
using NeumannMozi_DAL; //adatbazis

namespace NeumannMozi_WPF {
    /// <summary>
    /// Interaction logic for uctShowTimes.xaml
    /// </summary>
    public partial class uctShowtimes : UserControl {
        public uctShowtimes() {
            // Create edm(database) object
            edmNeumannMoziContainer = new edmNeumannMoziContainer();
            InitializeComponent();
            GetCurrentShowTimes();
        }

        #region CORE_VARIABLES
        private edmNeumannMoziContainer edmNeumannMoziContainer;
        #endregion

        #region BUTTON_CLICK_EVENTS
        private void btnFilmCard_Click(object sender, RoutedEventArgs e) {
            FilmData ClickedFilm = ((Button)sender).Tag as FilmData;
            // Open ticket reservation screen
            ((winMain)Application.Current.MainWindow).wpCurrentContent.Children.Clear();
            ((winMain)Application.Current.MainWindow).wpCurrentContent.Children.Add(new uctReservation(ClickedFilm));
            ((winMain)Application.Current.MainWindow).svContent.ScrollToTop();

        }
        #endregion

        #region DATABASE

        private List<string> GetScreeningDates(int filmId) {
            List<string> vetitString = new List<string>();
            int dateCounter = 0;
            var currentDateTime = DateTime.Now;
            foreach (var x in edmNeumannMoziContainer.VetitesSet) {
                if (x.FilmId == filmId) {
                    if (x.Kezdete > currentDateTime) {
                        if (++dateCounter > 2) {
                            vetitString.Add("További időpontok");
                            return vetitString;
                        }
                        vetitString.Add(x.Kezdete.ToString());
                    }
                }
            }
            return vetitString;
        }
        // Return screening dates and rooms in string list
        private List<string> GetComboboxSource(int filmId) {
            List<string> vetitString = new List<string>();
            var currentDateTime = DateTime.Now;
            foreach (var x in edmNeumannMoziContainer.VetitesSet) {
                if (x.FilmId == filmId) {
                    if (x.Kezdete > currentDateTime) {
                        vetitString.Add(x.Kezdete.ToString() + " - " + x.Terem.Nev);
                    }
                }
            }
            return vetitString;
        }
        private List<string> GetRoomName(int filmId) {
            List<string> roomName = new List<string>();
            var currentDateTime = DateTime.Now;
            foreach (var x in edmNeumannMoziContainer.VetitesSet) {
                if (x.FilmId == filmId) {
                    if (x.Kezdete > currentDateTime) {
                        roomName.Add(x.Terem.Nev);
                    }
                }
            }
            return roomName;
        }

        private void GetCurrentShowTimes() {
            var filmLista = new List<FilmData>();
            foreach (var x in edmNeumannMoziContainer.FilmSet) {
                if (GetScreeningDates(x.Id).Count > 0) {
                    filmLista.Add(new FilmData() {
                        Id = x.Id,
                        PosterImage = x.Poszter,
                        Title = x.Cim,
                        Director = x.Rendezo,
                        Cast = x.Szereplok,
                        Description = x.Leiras,
                        AgeRating = x.Korhatar,
                        Length = x.Hossz,
                        Category = x.Kategoria,
                        RoomNameForDates = GetRoomName(x.Id),
                        ScreeningDates = GetScreeningDates(x.Id),
                        ComboBoxSource = GetComboboxSource(x.Id)
                    });
                }
            }

            foreach (var filmData in filmLista) {
                if (filmData.Description.Length > 300) {
                    filmData.Description = filmData.Description.Substring(0, 300) + "...";
                }
                 
            }
            ictrCurrentShowtimes.ItemsSource = filmLista;
        }

        #endregion
    }

}
