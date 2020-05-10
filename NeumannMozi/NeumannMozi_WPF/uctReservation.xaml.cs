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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeumannMozi_DAL;

namespace NeumannMozi_WPF {
    /// <summary>
    /// Interaction logic for uctReservation.xaml
    /// </summary>
    public partial class uctReservation : UserControl {
        private edmNeumannMoziContainer edmNeumannMoziContainer;
        public uctReservation() {
            edmNeumannMoziContainer = new edmNeumannMoziContainer();
            InitializeComponent();
            GetCurrentShowTimes();
        }


        public uctReservation(Button choosenFilm) : this() {
            btnFilmCard.DataContext = choosenFilm.DataContext;
            
        }

        private void GetCurrentShowTimes() {
            var currentDateTime = DateTime.Now; //jelenlegi datumot lekeri

            var query = (from x in edmNeumannMoziContainer.FilmSet
                         join vetit in edmNeumannMoziContainer.VetitesSet on x.Id equals vetit.FilmId
                         where vetit.Kezdete > currentDateTime
                         orderby vetit.Kezdete
                         select new {
                             id = x.Id,
                             PosterImage = x.Poszter,
                             Title = x.Cim,
                             Director = x.Rendezo,
                             Cast = x.Szereplok,
                             Description = x.Leiras,
                             AgeRating = x.Korhatar,
                             Length = x.Hossz,
                             Category = x.Kategoria,
                             ScreeningDates = vetit.Kezdete,
                             TeremId = vetit.TeremId //többit igy hozza lehet adni vetites tablabol ha kell
                         }).ToList();   
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            
        }
    }
}
