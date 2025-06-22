using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labirynt
{
    public partial class Form1 : Form
    {
        Point p = new Point(60, 60);   // Pozycja startowa kursora (dostosuj do swojego labiryntu)
        bool ok = false;
        int sekundy = 0;
        int rekord = int.MaxValue;
        int przeszkodaKierunek = 1;    // 1 = w dół, -1 = w górę
        int przeszkodaTopMin = 120;
        int przeszkodaTopMax = 250;
        string sciezkaRekord = "rekord.txt";

        public Form1()
        {
            InitializeComponent();
        }

        private void MouseEnter(object sender, EventArgs e)
        {
            if (ok)
            {
                SystemSounds.Hand.Play(); // Dźwięk przy błędzie (dotknięcie ściany)
                Cursor.Position = PointToScreen(p);
            }
        }

        private void MouseEnter_Koniec(object sender, EventArgs e)
        {
            SystemSounds.Exclamation.Play(); // Dźwięk zwycięstwa
            timer1.Stop();
            timerPrzeszkoda.Stop();

            if (sekundy < rekord)
            {
                rekord = sekundy;
                labelRekord.Text = "Rekord: " + rekord + " s";
                // Zapis nowego rekordu do pliku
                System.IO.File.WriteAllText(sciezkaRekord, rekord.ToString()); 
                MessageBox.Show("Gratulacje, udało ci się!\nTwój czas: " + sekundy + " s\nNowy rekord!");
            }
            else
            {
                MessageBox.Show("Gratulacje, udało ci się!\nTwój czas: " + sekundy + " s");
            }

            button1.Visible = true;
            ok = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sekundy = 0;
            labelCzas.Text = "Czas: 0 s";
            labelRekord.Text = rekord == int.MaxValue ? "Rekord: brak" : "Rekord: " + rekord + " s";
            timer1.Start();
            timerPrzeszkoda.Start();
            Cursor.Position = PointToScreen(p);
            button1.Visible = false;
            ok = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sekundy++;
            labelCzas.Text = "Czas: " + sekundy + " s";
        }

        private void timerPrzeszkoda_Tick(object sender, EventArgs e)
        {
            // Przesuwanie przeszkody góra-dół 
            int newTop = panelPrzeszkoda.Top + przeszkodaKierunek * 2; // szybkość ruchu
            if (newTop < przeszkodaTopMin || newTop > przeszkodaTopMax) // zakres ruchu przeszkody
            {
                przeszkodaKierunek *= -1; // zmiana kierunku
            }
            else
            {
                panelPrzeszkoda.Top = newTop;
            }

            // Kolizja z przeszkodą
            Point lokalizacjaMyszy = this.PointToClient(Cursor.Position);
            if (panelPrzeszkoda.Bounds.Contains(lokalizacjaMyszy) && ok)
            {
                SystemSounds.Hand.Play();
                Cursor.Position = PointToScreen(p);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Cursor.Position = PointToScreen(p);
            ok = false;
            button1.Visible = true;

            // Wczytanie rekordu z pliku, jeśli istnieje
            if (System.IO.File.Exists(sciezkaRekord)) 
            {
                string tekst = System.IO.File.ReadAllText(sciezkaRekord);
                if (int.TryParse(tekst, out int wczytanyRekord))
                {
                    rekord = wczytanyRekord;
                    labelRekord.Text = "Rekord: " + rekord + " s";
                }
                else
                {
                    rekord = int.MaxValue;
                    labelRekord.Text = "Rekord: brak";
                }
            }
            else
            {
                rekord = int.MaxValue;
                labelRekord.Text = "Rekord: brak";
            }
        }

        private void labelCzas_Click(object sender, EventArgs e)
        {
            // Puste zdarzenie
        }
    }
}
