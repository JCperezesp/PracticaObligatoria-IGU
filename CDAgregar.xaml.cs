using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PracticaObligatoria
{
    /// <summary>
    /// Lógica de interacción para CDAgregar.xaml
    /// </summary>
    public partial class CDAgregar : Window
    {
        private bool datosObligatoriosCompleto,datosRepostajeCompleto;
        private bool hayDatos, KilometrosReposBien = false;
        public CDAgregar()
        {
            InitializeComponent();
            datosRepostajeCompleto = true;
            hayDatos = false;
            cajaMatricula.Focus();
        }

        //contructor para modificar
        public CDAgregar(Vehiculo v)
        {
            InitializeComponent();
            cajaMarca.Text = v.Marca;
            cajaMatricula.Text = v.Matricula;
            cajaKM.Text = v.KilometrosAct.ToString();
            datosRepostajeCompleto = true;
            datosObligatoriosCompleto = true;
            hayDatos = true;
            DesactivarCajasRepos();
            cajaMatricula.Focus();
        }

        private void Mas_Click(object sender, RoutedEventArgs e)
        {
            double intervalo;
            double maximo;

            if(sender == MasLitros)
            {
                intervalo = 1;
                maximo = 200.0F;
                if(LitrosRepos < (Int32)Math.Truncate(maximo))
                    cajaLitros.Text = (LitrosRepos + intervalo).ToString();          
            }else if(sender == MasCoste)
            {
                intervalo = 1;
                maximo = 400.0F;
                if (CosteRepos < (Int32)Math.Truncate(maximo))
                    cajaCoste.Text = (CosteRepos + intervalo).ToString();

            }
            else if (sender == MasKm)
            {
                intervalo = 10;
                maximo = 200000.0F;
                if (KMrepos < (Int32)Math.Truncate(maximo))
                    cajaKMRepos.Text = (KMrepos + intervalo).ToString();
            }
            else if(sender == MasKilometros)
            {
                intervalo = 10;
                maximo = 200000.0F;
                if (Kilometros < (Int32)Math.Truncate(maximo))
                    cajaKM.Text = (Kilometros + intervalo).ToString();
            }

        }

        private void Menos_Click(object sender, RoutedEventArgs e) {

            double intervalo;
            double minimo = 0;

            if (sender == MenosLitros)
            {
                intervalo = 1;
                if (LitrosRepos-intervalo > (Int32)Math.Truncate(minimo))
                    cajaLitros.Text = (LitrosRepos - intervalo).ToString();
                else
                    cajaLitros.Text = "0";
            }
            else if (sender == MenosCoste)
            {
                intervalo = 1;
                if (CosteRepos - intervalo > (Int32)Math.Truncate(minimo))
                    cajaCoste.Text = (CosteRepos - intervalo).ToString();
                else cajaCoste.Text = "0";
            }
            else if (sender == MenosKm)
            {
                intervalo = 10;
                if (KMrepos - intervalo > (Int32)Math.Truncate(minimo))
                    cajaKMRepos.Text = (KMrepos - intervalo).ToString();
                else cajaKMRepos.Text = "0";
            }
            else if(sender == MenosKilometros)
            {
                intervalo = 10;
                if (Kilometros - intervalo > minimo)
                    cajaKM.Text = (Kilometros - intervalo).ToString();
                else cajaKM.Text = "0";
            
            }
        }  
        private void CajaDeFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( !string.IsNullOrWhiteSpace(cajaMarca.Text)|| !string.IsNullOrWhiteSpace(cajaMatricula.Text) || !string.IsNullOrWhiteSpace(cajaKM.Text) ||
               !string.IsNullOrWhiteSpace(CajaDeFecha.Text) || !string.IsNullOrWhiteSpace(cajaLitros.Text) || !string.IsNullOrWhiteSpace(cajaCoste.Text) || !string.IsNullOrWhiteSpace(cajaKMRepos.Text))//Si hay algun caracter en alguna de las cajas
                hayDatos = true;
            else hayDatos = false;
            if (CajaDeFecha.Text.Length > 0)
                CajaDeFecha.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            else CajaDeFecha.BorderBrush = new SolidColorBrush(Colors.DarkRed);

            if ((CajaDeFecha.Text.Length > 0 && !string.IsNullOrWhiteSpace(cajaLitros.Text) && !string.IsNullOrWhiteSpace(cajaCoste.Text) && KilometrosReposBien)     //Comprobamos que, o bien no se ha introducido repostaje, o si se ha introducido está completo
    || (CajaDeFecha.Text.Length == 0 && string.IsNullOrWhiteSpace(cajaLitros.Text) && string.IsNullOrWhiteSpace(cajaCoste.Text) && string.IsNullOrWhiteSpace(cajaKMRepos.Text)))
                datosRepostajeCompleto = true;      //En ambos casos la variable datosRepCompleto es true (para mayor facilidad a la hora de desbloquear los botones)
            else datosRepostajeCompleto = false;

            if (datosRepostajeCompleto && datosObligatoriosCompleto)
            {
                if (!botonAceptar.IsEnabled)
                {
                    botonAceptar.IsEnabled = true;
                    botonAceptar.IsDefault = true;
                    botonCancelar.IsDefault = false;
                }
            }
            else
            {
                if (botonAceptar.IsEnabled)
                {
                    botonAceptar.IsDefault = false;
                    botonAceptar.IsEnabled = false;                  
                    botonCancelar.IsDefault = true;
                }
            }
        }

        private void cajaTexto_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Controlamos kilómetros
            if (!string.IsNullOrWhiteSpace(cajaKMRepos.Text) && !string.IsNullOrWhiteSpace(cajaKM.Text) && KMrepos > Kilometros)
                KilometrosReposBien = true;
            else KilometrosReposBien = false;
            if (KilometrosReposBien)
            {
                panelAvisosKMRepIni.Visibility = Visibility.Collapsed;
                cajaKMRepos.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                if (!string.IsNullOrWhiteSpace(CajaDeFecha.Text) && !string.IsNullOrWhiteSpace(cajaLitros.Text) && !string.IsNullOrWhiteSpace(cajaCoste.Text))  //hay que volver a comprobar si el resto de cajas de repos tienen algo
                    datosRepostajeCompleto = true;
                else datosRepostajeCompleto = false;
            }

            else
            {
                panelAvisosKMRepIni.Visibility = Visibility.Visible;
                AvisoKM.ToolTip = "El valor del cuentakilómetros en el momento de repostar debe ser mayor a:" + Kilometros;
                cajaKMRepos.BorderBrush = new SolidColorBrush(Colors.DarkRed);
            }
            if (!string.IsNullOrWhiteSpace(cajaMarca.Text) || !string.IsNullOrWhiteSpace(cajaMatricula.Text) || !string.IsNullOrWhiteSpace(cajaKM.Text) ||
                !string.IsNullOrWhiteSpace(CajaDeFecha.Text) || !string.IsNullOrWhiteSpace(cajaLitros.Text) || !string.IsNullOrWhiteSpace(cajaCoste.Text) || !string.IsNullOrWhiteSpace(cajaKMRepos.Text))//Si hay algun caracter en alguna de las cajas
                hayDatos = true;
            else hayDatos = false;

            TextBox c = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(c.Text))
                c.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            else c.BorderBrush = new SolidColorBrush(Colors.DarkRed);

            if (!string.IsNullOrWhiteSpace(cajaMarca.Text) && !string.IsNullOrWhiteSpace(cajaMatricula.Text) && !string.IsNullOrWhiteSpace(cajaKM.Text))
                datosObligatoriosCompleto = true;
            else datosObligatoriosCompleto = false;

            if (datosObligatoriosCompleto && datosRepostajeCompleto)
            {
                botonAceptar.IsEnabled = true;
                botonAceptar.IsDefault = true;
                botonCancelar.IsDefault = false;
            }
            else{
                botonAceptar.IsEnabled = false;
                botonCancelar.IsDefault = true;
            }

        }
        private void cajaRepos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cajaKMRepos.Text) && !string.IsNullOrWhiteSpace(cajaKM.Text) && KMrepos > Kilometros)
                KilometrosReposBien = true;
            else KilometrosReposBien = false;
            if (!string.IsNullOrWhiteSpace(cajaMarca.Text) || !string.IsNullOrWhiteSpace(cajaMatricula.Text) || !string.IsNullOrWhiteSpace(cajaKM.Text) ||
                !string.IsNullOrWhiteSpace(CajaDeFecha.Text) || !string.IsNullOrWhiteSpace(cajaLitros.Text) || !string.IsNullOrWhiteSpace(cajaCoste.Text) || !string.IsNullOrWhiteSpace(cajaKMRepos.Text))//Si hay algun caracter en alguna de las cajas
                hayDatos = true;
            else hayDatos = false;


            TextBox c = (TextBox)sender;
            if (!string.IsNullOrWhiteSpace(c.Text)) { 
                if(c == cajaKMRepos)
                {
                    if(KilometrosReposBien)
                    {
                        panelAvisosKMRepIni.Visibility = Visibility.Collapsed;
                        c.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                    }
                    else
                    {
                        panelAvisosKMRepIni.Visibility = Visibility.Visible;
                        AvisoKM.ToolTip = "El valor del cuentakilómetros en el momento de repostar debe ser mayor a:" + Kilometros;
                        c.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                    }
                }
                else c.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                }
            else c.BorderBrush = new SolidColorBrush(Colors.DarkRed);

            if ((CajaDeFecha.Text.Length > 0 && !string.IsNullOrWhiteSpace(cajaLitros.Text) && !string.IsNullOrWhiteSpace(cajaCoste.Text) && KilometrosReposBien)     //Comprobamos que, o bien no se ha introducido repostaje, o si se ha introducido está completo
    || (CajaDeFecha.Text.Length == 0 && string.IsNullOrWhiteSpace(cajaLitros.Text) && string.IsNullOrWhiteSpace(cajaCoste.Text) && string.IsNullOrWhiteSpace(cajaKMRepos.Text)))
                datosRepostajeCompleto = true;      //En ambos casos la variable datosRepCompleto es true (para mayor facilidad a la hora de desbloquear los botones)
            else datosRepostajeCompleto = false;

            if (datosRepostajeCompleto && datosObligatoriosCompleto)
            {
                if (!botonAceptar.IsEnabled)
                {
                    botonAceptar.IsEnabled = true;
                    botonAceptar.IsDefault = true;
                    botonCancelar.IsDefault = false;
                }
            }
            else
            {
                if (botonAceptar.IsEnabled)
                {
                    botonAceptar.IsEnabled = false;
                    botonAceptar.IsDefault = false;
                    botonCancelar.IsDefault = true;
                }
            }
        }

        private void Ayuda_Click(object sender, RoutedEventArgs e)
        {
            string msg = "DATOS OBLIGATORIOS: \nMatrícula: matrícula única del automóvil\nMarca: Fabricante del vehículo\nKilómetros: Valor inicial del cuentakilómetros con " +
                "el que se harán los cálculos, puede ser cero\n\nDATOS DE REPOSTAJE (esta columna no es obligatoria):\nFecha: fecha en la que se efectúa el repostaje\nLitros: Cantidad repostada\nCoste: precio de la operación\n" +
                "Kilómetros: Valor del cuentakilómetros en el momento de repostar.";
            MessageBox.Show(msg, "Ayuda", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DesactivarCajasRepos()
        {
            panelAvisosKMRepIni.Visibility = Visibility.Collapsed;

            cajaKM.IsEnabled = false; cajaKMRepos.IsEnabled = false;      //desactivamos las cajas de repostaje para esta opción
            cajaCoste.IsEnabled = false; cajaLitros.IsEnabled = false;
            CajaDeFecha.IsEnabled = false;

            cajaKM.Background = new SolidColorBrush(Colors.LightGray);
            cajaKMRepos.Background = new SolidColorBrush(Colors.LightGray);
            cajaLitros.Background = new SolidColorBrush(Colors.LightGray);
            cajaCoste.Background = new SolidColorBrush(Colors.LightGray);
            CajaDeFecha.Background = new SolidColorBrush(Colors.LightGray);

            cajaKM.BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource("Amarillo1"));
            cajaKMRepos.BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource("Amarillo1"));
            cajaLitros.BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource("Amarillo1"));
            cajaCoste.BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource("Amarillo1"));
            CajaDeFecha.BorderBrush = new SolidColorBrush((Color)Application.Current.FindResource("Amarillo1"));

            MasCoste.IsEnabled = false; MasLitros.IsEnabled = false;    MasKilometros.IsEnabled = false;
            MenosCoste.IsEnabled = false;   MenosLitros.IsEnabled = false;  MenosKilometros.IsEnabled = false;
            MasKm.IsEnabled = false;    MenosKm.IsEnabled = false;
        }

        private void botonAceptar_Click(object sender, RoutedEventArgs e)
        {
            if(RepostajeIntroducido && KMrepos <= Kilometros)
            {
                String msg = "Los kilómetros de repostaje deben ser una cantidad mayor a los kilómetros iniciales del vehículo";
                String titulo = "Error";
                MessageBoxButton botones = MessageBoxButton.OK;
                MessageBoxImage icono = MessageBoxImage.Error;
                MessageBox.Show(msg, titulo, botones, icono);
                botonAceptar.IsDefault = true;
            }
            else this.DialogResult = true;
        }

        private void botonCancelar_Click(object sender, RoutedEventArgs e)
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

        //public TextBox CajaMatricula { get { return cajaMatricula; } }
        public string TextoMatricula { get { return cajaMatricula.Text; } }
        public string TextoMarca { get { return cajaMarca.Text; } }
        public long Kilometros { get {
                if (string.IsNullOrWhiteSpace(cajaKM.Text)) return 0L;
                else 
                {
                    long l;
                    if (long.TryParse(cajaKM.Text, out l))
                        return long.Parse(cajaKM.Text);
                    else return (long)Math.Truncate(double.Parse(cajaKM.Text));
                }} }
        public string FechaRepos { get { return CajaDeFecha.Text; } }
        public double CosteRepos { get
            {
                if (string.IsNullOrWhiteSpace(cajaCoste.Text)) return 0L;
                double l;
                if (double.TryParse(cajaCoste.Text, out l))
                    return l;
                else return 0.0f;
            }
        }
        public long KMrepos { get 
            {
                if (string.IsNullOrWhiteSpace(cajaKMRepos.Text)) return 0L;
                long l;
                if (long.TryParse(cajaKMRepos.Text, out l))
                    return long.Parse(cajaKMRepos.Text);
                else return (long)Math.Truncate(double.Parse(cajaKMRepos.Text));
            } 
        }

        public int LitrosRepos { get 
            {
                if(string.IsNullOrWhiteSpace(cajaLitros.Text)) return 0;
                int l;
                if (int.TryParse(cajaLitros.Text, out l))
                    return int.Parse(cajaLitros.Text);
                else return (int)Math.Truncate(double.Parse(cajaLitros.Text));
            } }

        public bool RepostajeIntroducido {
            get { 
                if(CajaDeFecha.Text.Length > 0 && cajaLitros.Text.Length > 0 && cajaCoste.Text.Length > 0) return true;
                else return false;
            }
        }
    
    }
}
