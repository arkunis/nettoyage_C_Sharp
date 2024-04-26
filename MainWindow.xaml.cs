using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
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

namespace nettoyage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DirectoryInfo winTemps;
        public DirectoryInfo appTemps;
        string version = "1.0.0";
        public MainWindow()
        {
            InitializeComponent();
            winTemps = new DirectoryInfo(@"C:\Windows\Temp");
            appTemps = new DirectoryInfo(System.IO.Path.GetTempPath());
            getDate();
            CheckActu();
        }

        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(f => f.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        public void ClearTempData(DirectoryInfo dir) { 

            foreach(FileInfo file in dir.GetFiles())
            {
                try
                { 
                    file.Delete();
                    Console.WriteLine(file.FullName);

                } catch(Exception ex) {
                    continue;
                }
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(di.FullName);

                }catch (Exception ex)
                {
                    continue;
                }
            }

        }

        /// <summary>
        /// Analyse des dossier et fichier temporaires
        /// </summary>
        public void AnalyseFolder()
        {
            Console.WriteLine("Début de l'analyse");
            long totalSize = 0;
            try
            {
                totalSize += DirSize(winTemps) / 1000000;
                totalSize += DirSize(appTemps) / 1000000;

            }catch(Exception ex)
            {
                Console.WriteLine("Impossible d'analyser les dossiers : " + ex.Message);
            }
      
            espace.Content = totalSize + "Mb";
            titre.Content = "Analyse terminé";
            date.Content = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("fr-FR"));
            saveDate();
        }

        private void analyser_Copy_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours");
            Nettoyer.Content = "Nettoyage en cours";

            Clipboard.Clear();

            try
            {
                ClearTempData(winTemps);
            }catch (Exception ex) {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            try
            {
                ClearTempData(appTemps);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            titre.Content = "Nettoyage effectué";
            Nettoyer.Content = "Nettoyage terminé";
            espace.Content = "0Mb";
            Nettoyer.IsEnabled = false;
            Nettoyer.Foreground = new SolidColorBrush(Colors.Black);

        }

        public void CheckActu()
        {
            string url = "https://voltpass.fr/actu.txt";
            using (WebClient client = new WebClient())
            {
                string actualite = client.DownloadString(url);
                if(actualite != String.Empty)
                {
                    actu.Content = actualite;
                    actu.Visibility = Visibility.Visible;
                    boxActu.Visibility = Visibility.Visible;

                }
            }
        }

        /// <summary>
        /// Vérification de la version
        /// </summary>
        public void checkVersion()
        {
            string url = "https://voltpass.fr/version.txt";
            using (WebClient client = new WebClient())
            {
                string versionTxt = client.DownloadString(url);
                if (version != versionTxt)
                {
                    MessageBox.Show("Voulez-vous mettre à jour le logiciel ?", "Mise à jour requis", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                else
                {
                    MessageBox.Show("Logiciel à jour", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void saveDate()
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("fr-FR"));

            File.WriteAllText("date.txt", date);
        }

        public void getDate()
        {
            if (File.Exists("date.txt"))
            {
                string dateTxt = File.ReadAllText("date.txt");

                if (dateTxt != String.Empty)
                {
                    date.Content = dateTxt;
                }
            }
        }

        private void maj_Click(object sender, RoutedEventArgs e)
        {
            checkVersion();
        }

        private void web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://coursdudev.com/")
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolder();
            if(Nettoyer.IsEnabled == false)
            {
                Nettoyer.IsEnabled = true;
                Nettoyer.Content = "NETTOYER";
                Nettoyer.Foreground = new SolidColorBrush(Colors.White);
            }
        }
    }
}