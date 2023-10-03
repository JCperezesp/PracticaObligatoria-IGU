using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PracticaObligatoria
{
    /// <summary>
    /// Lógica de interacción para CDAgregar2.xaml
    /// </summary>
    public partial class CDAgregar2 : Window
    {

        private bool hayDatos = false;
        private bool KilometrosBien = false, fechaBien=true;
        private Vehiculo vehiculoRepostado = null;
        public CDAgregar2(Vehiculo v)
        {
            InitializeComponent();
            vehiculoRepostado = v;
        }

        private void Mas_Click(object sender, RoutedEventArgs e)
        {
            double intervalo;
            double maximo;

            if (sender == MasLitros)
            {
                intervalo = 1;
                maximo = 200.0F;
                if (Litros < (Int32)Math.Truncate(maximo))
                    cajaLitros.Text = (Litros + intervalo).ToString();
            }
            else if (sender == MasCoste)
            {
                intervalo = 1;
                maximo = 400.0F;
                if (Coste < (Int32)Math.Truncate(maximo))
                    cajaCoste.Text = (Coste + intervalo).ToString();

            }
            else if (sender == MasKilometros)
            {
                intervalo = 10;
                maximo = 200000.0F;
                if (KMRepos < (Int32)Math.Truncate(maximo))
                    cajaKMRepos.Text = (KMRepos + intervalo).ToString();
            }

        }

        private void Menos_Click(object sender, RoutedEventArgs e)
        {

            double intervalo;
            double minimo = 0;

            if (sender == MenosLitros)
            {
                intervalo = 1;
                if (Litros - intervalo > (Int32)Math.Truncate(minimo))
                    cajaLitros.Text = (Litros - intervalo).ToString();
                else
                    cajaLitros.Text = "0";
            }
            else if (sender == MenosCoste)
            {
                intervalo = 1;
                if (Coste - intervalo > (Int32)Math.Truncate(minimo))
                    cajaCoste.Text = (Coste - intervalo).ToString();
                else cajaCoste.Text = "0";
            }
            else if (sender == MenosKilometros)
            {
                intervalo = 10;
                if (KMRepos - intervalo > (Int32)Math.Truncate(minimo))
                    cajaKMRepos.Text = (KMRepos - intervalo).ToString();
                else cajaKMRepos.Text = "0";
            }
        }
        private void CajaDeFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CajaDeFecha.Text.Length > 0 || cajaCoste.Text.Length > 0 || cajaKMRepos.Text.Length > 0 || cajaLitros.Text.Length > 0)
                hayDatos = true;
            else hayDatos = false;

            DateTime dt;
            if (vehiculoRepostado.Repostajes.Count > 0 && DateTime.TryParse(vehiculoRepostado.Repostajes.Last().Fecha, out dt))
            {
                if (0 <= DateTime.Compare(dt, CajaDeFecha.SelectedDate.Value))
                {
                    CajaDeFecha.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                    fechaBien = false;
                    panelAvisosFecha.Visibility = Visibility.Visible;
                    AvisoFecha.ToolTip = "La fecha debe ser posterior a la del último repostaje: " + dt.ToShortDateString();
                }
                else
                {
                    CajaDeFecha.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                    fechaBien = true;
                    panelAvisosFecha.Visibility = Visibility.Collapsed;
                }
            }
            else if (vehiculoRepostado.Repostajes.Count == 0)
            {
                CajaDeFecha.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                fechaBien = true;
                panelAvisosFecha.Visibility = Visibility.Collapsed;
            }
                
            if (fechaBien && cajaCoste.Text.Length > 0 && KilometrosBien  && fechaBien)
            {
                BotonAceptar.IsEnabled = true;
                BotonAceptar.IsDefault = true;
                BotonCancelar.IsDefault = false;
            }
            else
            {
                if (BotonAceptar.IsEnabled)
                {
                    BotonAceptar.IsEnabled = false;
                    BotonAceptar.IsDefault = false;
                    BotonCancelar.IsDefault = true;
                }
            }
        }

        private void cajaTextoRepos_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox c = sender as TextBox;


                if (cajaKMRepos.Text.Length > 0 && (vehiculoRepostado.KilometrosAct < KMRepos))
                    KilometrosBien = true;
                else KilometrosBien = false;

            if (CajaDeFecha.Text.Length > 0 || cajaCoste.Text.Length > 0 || cajaKMRepos.Text.Length > 0 || cajaLitros.Text.Length > 0)
                hayDatos = true;
            else hayDatos = false;

            if (c.Text.Length > 0)
            {
                if (c == cajaKMRepos)
                {
                    if (KilometrosBien)
                    {
                        c.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                        panelAvisosKM.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        c.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                        panelAvisosKM.Visibility = Visibility.Visible;
                    }
                }
                else
                    c.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                
            }

            else
            {
                c.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                if (c == cajaKMRepos)
                    panelAvisosKM.Visibility = Visibility.Visible;

            }
            if (fechaBien && cajaCoste.Text.Length>0 && KilometrosBien && cajaLitros.Text.Length>0)
            {
                BotonAceptar.IsEnabled = true;
                BotonAceptar.IsDefault = true;
                BotonCancelar.IsDefault = false;
            }
            else
            {
                if (BotonAceptar.IsEnabled)
                {
                    BotonAceptar.IsEnabled = false;
                    BotonAceptar.IsDefault = false;
                    BotonCancelar.IsDefault = true;
                }
            }
        }
        private void Ayuda_Click(object sender, RoutedEventArgs e)
        {
            string msg = "DATOS PARA EL REPOSTAJE:\nFecha: fecha en la que se efectúa el repostaje (debe ser posterior a la del último repostaje)\nLitros: Cantidad repostada\nCoste: precio de la operación\n" +
                "Kilómetros: Valor del cuentakilómetros en el momento de repostar.";

            MessageBox.Show(msg,"Ayuda",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void BotonAceptar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BotonCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (hayDatos)
            {
                String msg = "Hay datos sin guardar, ¿Salir?";
                String titulo = "Cancelar";
                MessageBoxButton botones = MessageBoxButton.YesNo;
                MessageBoxImage icono = MessageBoxImage.Warning;
                switch (MessageBox.Show(msg, titulo, botones, icono))
                {
                    case MessageBoxResult.Yes:
                        DialogResult = false;
                        break;
                    case MessageBoxResult.No:
                        break;
                }

            }
            else this.DialogResult = false;
        }

    //PROPIEDADES
        public string TextoFecha { get { return CajaDeFecha.Text; } }
        public long KMRepos
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cajaKMRepos.Text)) return 0L;
                long l;
                if (long.TryParse(cajaKMRepos.Text, out l))
                    return long.Parse(cajaKMRepos.Text);
                else return (long)Math.Truncate(double.Parse(cajaKMRepos.Text));
            }
        }
        public int Litros
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cajaLitros.Text)) return 0;
                int l;
                if (int.TryParse(cajaLitros.Text, out l))
                    return int.Parse(cajaLitros.Text);
                else return (int)Math.Truncate(double.Parse(cajaLitros.Text));
            }
        }
        public double Coste
        {
            get
            {
                if (string.IsNullOrWhiteSpace(cajaCoste.Text)) return 0L;
                double l;
                if (double.TryParse(cajaCoste.Text, out l))
                    return l;
                else return 0.0f;
            }
        }


    }
}
