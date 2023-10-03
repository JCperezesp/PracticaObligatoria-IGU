using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace PracticaObligatoria
{
    /// <summary>
    /// Lógica de interacción para CDOtros.xaml
    /// </summary>
    public partial class CDOtros : Window
    {
        private ObservableCollection<Vehiculo> listaDeVehiculos = null;
        public event RoutedEventHandler AgregarVehiculo;
        public CDOtros(ObservableCollection<Vehiculo> list)
        {
            InitializeComponent();
            listaDeVehiculos = list;
            listaDeVehiculos.CollectionChanged += ListaDeVehiculos_CollectionChanged;

            if (listaDeVehiculos.Count > 0)
            {
                Vehiculo vcoste = listaDeVehiculos.First(), vconsumo = listaDeVehiculos.First(),
                    vkm = listaDeVehiculos.First(), vrepos = listaDeVehiculos.First();
                double mediaCosteMax = listaDeVehiculos.First().MediaCoste;
                long kmmax = listaDeVehiculos.First().KilometrosAct;
                double mediaConsumoMax = listaDeVehiculos.First().MediaConsumo;
                int nrepos = listaDeVehiculos.First().Repostajes.Count;

                foreach (Vehiculo item in listaDeVehiculos)
                {

                    if (item.MediaCoste > mediaCosteMax)
                    {
                        mediaCosteMax = item.MediaCoste;
                        vcoste = item;
                    }

                    if (item.MediaConsumo > mediaConsumoMax)
                    {
                        mediaConsumoMax = item.MediaConsumo;
                        vconsumo = item;
                    }

                    if (item.KilometrosAct > kmmax)
                    {
                        kmmax = item.KilometrosAct;
                        vkm = item;
                    }
                    if (item.Repostajes.Count > nrepos)
                    {
                        nrepos = item.Repostajes.Count;
                        vrepos = item;
                    }

                }

                Matcoste.DataContext = vcoste; Coste.DataContext = vcoste;
                Matconsumo.DataContext = vconsumo; Consumo.DataContext = vconsumo;
                Matkm.DataContext = vkm; Kilom.DataContext = vkm;
                Matrepos.Content = vrepos.Matricula;
                Repos.Content = nrepos;
                mostrarDatos();
            }
            else
            {
                listaVacia();
            }
        }

        private void ListaDeVehiculos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (listaDeVehiculos.Count > 0)
            {
                Vehiculo vcoste = listaDeVehiculos.First(), vconsumo = listaDeVehiculos.First(), 
                    vkm = listaDeVehiculos.First(), vrepos = listaDeVehiculos.First();
                double mediaCosteMax = listaDeVehiculos.First().MediaCoste;
                long kmmax = listaDeVehiculos.First().KilometrosAct;
                double mediaConsumoMax = listaDeVehiculos.First().MediaConsumo;
                int nrepos = listaDeVehiculos.First().Repostajes.Count;

                foreach (Vehiculo item in listaDeVehiculos)
                {

                    if (item.MediaCoste > mediaCosteMax)
                    {
                        mediaCosteMax = item.MediaCoste;
                        vcoste = item;
                    }

                    if (item.MediaConsumo > mediaConsumoMax)
                    {
                        mediaConsumoMax = item.MediaConsumo;
                        vconsumo = item;
                    }

                    if (item.KilometrosAct > kmmax)
                    {
                        kmmax = item.KilometrosAct;
                        vkm = item;
                    }

                    if(item.Repostajes.Count > nrepos)
                    {
                        nrepos = item.Repostajes.Count;
                        vrepos = item;
                    }
                }

                Matcoste.DataContext = vcoste; Coste.DataContext = vcoste;
                Matconsumo.DataContext = vconsumo; Consumo.DataContext = vconsumo;
                Matkm.DataContext = vkm; Kilom.DataContext = vkm;
                Matrepos.Content = vrepos.Matricula;
                Repos.Content = nrepos;
                mostrarDatos();
            }
            else
            {
                listaVacia();
            }
        }

        private void listaVacia()
        {
            BordeConsumo.Visibility = Visibility.Collapsed;
            BordeCoste.Visibility = Visibility.Collapsed;
            BordeKm.Visibility = Visibility.Collapsed;
            BordeRepos.Visibility = Visibility.Collapsed;
            BordeListaVacia.Visibility = Visibility.Visible;
            Matcoste.DataContext = null; Coste.DataContext = null;
            Matconsumo.DataContext = null; Consumo.DataContext = null;
            Matkm.DataContext = null; Consumo.DataContext = null;
        }

        private void mostrarDatos()
        {
            BordeConsumo.Visibility = Visibility.Visible;
            BordeCoste.Visibility = Visibility.Visible;
            BordeKm.Visibility = Visibility.Visible;
            BordeRepos.Visibility = Visibility.Visible;
            BordeListaVacia.Visibility = Visibility.Collapsed;
        }

        private void Añadir_Click(object sender, RoutedEventArgs e)
        {
            OnAgregarVehiculo(new RoutedEventArgs());
        }

        protected virtual void OnAgregarVehiculo(RoutedEventArgs e)
        {
            AgregarVehiculo?.Invoke(this, e);
        }
    }
}
