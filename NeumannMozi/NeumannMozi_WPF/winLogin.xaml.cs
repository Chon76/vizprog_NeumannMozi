﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Security.Cryptography; //md5 titkositas

namespace NeumannMozi_WPF {
    public partial class winLogin : Window {
        public winLogin() {
            // Create edm(database) object
            edmNeumannMoziContainer = new edmNeumannMoziContainer();

            SetDataDirectory();
            InitializeComponent();
        }

        #region CORE_VARIABLES
        // Databse object
        private edmNeumannMoziContainer edmNeumannMoziContainer;
        // Login data
        
        public static string loginUserName;
        public static bool loginAdmin;
        public static int loginId;
        #endregion

        #region BUTTON_CLICK_EVENTS

            #region WINDOW
            private void btnExitApp_Click(object sender, RoutedEventArgs e) {
                App.Current.Shutdown();
            }
            private void btnMinimize_Click(object sender, RoutedEventArgs e) {
                WindowState = WindowState.Minimized;
            }
            #endregion

            #region CONTENT
            // Forgatten password
            private void btnForgattenPassword_Click(object sender, RoutedEventArgs e) {
                MessageBox.Show("Nem mukodom :(");
            }
            // Login
            private void btnLogin_Click(object sender, RoutedEventArgs e) {

                if (txtUsername.Text == "Felhasználónév" || txtPassword.Password == "Jelszó") {
                    //adatok kitöltése kötelező alert
                    MessageBox.Show("Adatok kitöltése kötelező.");
                    return;
                }

                MD5 md5Hash = MD5.Create();
                string hashPw = GetMd5Hash(md5Hash, txtPassword.Password);
                var u = (from x in edmNeumannMoziContainer.FelhasznaloSet 
                         where x.Nev == txtUsername.Text && x.Jelszo == hashPw 
                         select new { x.Id, x.Nev, x.Jelszo, x.Admin }
                         ).FirstOrDefault();

                if (u != null && String.Equals(u.Nev, txtUsername.Text)) {
                    loginUserName = u.Nev;
                    loginAdmin = u.Admin;
                    loginId = u.Id;
                    winMain winMain = new winMain();
                    winMain.Show();
                    this.Close();
                    //Beléptetés
                } else {
                    MessageBox.Show("Sikertelen bejelentkezés!\nHibás adatok!");               
                }
            }
            // Register
            private void btnRegister_Click(object sender, RoutedEventArgs e) {
                MD5 md5Hash = MD5.Create();
                string hashPw = GetMd5Hash(md5Hash, txtPassword.Password);
                if (txtUsername.Text == "Felhasználónév" || txtPassword.Password == "Jelszó") {
                    //adatok kitöltése kötelező alert
                    MessageBox.Show("Adatok kitöltése kötelező.");
                    return;
                }

                var uname = (from x in edmNeumannMoziContainer.FelhasznaloSet where x.Nev == txtUsername.Text select new { x.Nev }).FirstOrDefault();
                if (uname != null) {
                    //Létezik már a felhasználó exception
                    MessageBox.Show("Letezik mar felhasznalo ezzel a nevvel.");
                } else {
                    var user = new Felhasznalo {
                        Nev = txtUsername.Text,
                        Jelszo = hashPw,
                        Admin = false
                    };
                    edmNeumannMoziContainer.FelhasznaloSet.Add(user);
                    edmNeumannMoziContainer.SaveChanges();
                    MessageBox.Show("Sikeres regisztráció."); //sikeres
                }
            }
            #endregion

        #endregion

        #region INPUT_EVENTS
        private void LoginInput_GotFocus(object sender, RoutedEventArgs e) {
            if (sender is TextBox) {
                TextBox tb = (TextBox)sender;
                if (tb.Text == "Felhasználónév") {
                    tb.Text = string.Empty;
                }
                
            } else if (sender is PasswordBox) {
                PasswordBox pb = (PasswordBox)sender;
                if (pb.Password == "Jelszó") {
                    pb.Password = string.Empty;
                }
            }
        }
        private void LoginInput_LostFocus(object sender, RoutedEventArgs e) {
            if (sender is TextBox) {
                TextBox tb = (TextBox)sender;
                if (tb.Text == string.Empty) {
                    tb.Text = "Felhasználónév";
                }

            } else if (sender is PasswordBox) {
                PasswordBox pb = (PasswordBox)sender;
                if (pb.Password == string.Empty) {
                    pb.Password = "Jelszó";
                }
            }
        }
        #endregion

        #region DATABASE
        // Set DataDirectory path to Project Solution path (..\..\NeumannMozi\)
        public static void SetDataDirectory() {
            var projectDir = Directory.GetParent(Directory.GetCurrentDirectory());
            projectDir = Directory.GetParent(projectDir.ToString());
            projectDir = Directory.GetParent(projectDir.ToString());
            //
            AppDomain.CurrentDomain.SetData("DataDirectory", projectDir.ToString());
        }
        #endregion

        #region MISC
        // Password encryption(/w md5hash)
        private static string GetMd5Hash(MD5 md5Hash, string input) {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        #endregion
    }
}
