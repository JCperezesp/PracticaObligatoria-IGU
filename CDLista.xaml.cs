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
    /// Lógica de interacción para CDLista.xaml
    /// </summary>
    public class SeleccionVehiculoEventArgs
    {
         public string matriculaVehiculoSeleccionado;
         public int indiceRepostajeSeleccionado;
    }
       
    public partial class CDLista : Window
    {

        public event EventHandler<SeleccionVehiculoEventArgs> SeleccionVehiculo;
        public event EventHandler<SeleccionVehiculoEventArgs> AgregarRepostajePulsado;
        public event EventHandler LimpiarListaPulsado;

        private ObservableCollection<Vehiculo> listaDeVehiculos = null;
        public CDLista(ObservableCollection<Vehiculo> list)
        {
            InitializeComponent();
            listaDeVehiculos = list;
            ListaVehiculos.ItemsSource = listaDeVehiculos;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void botonEliminarV_Click(object sender, RoutedEventArgs e)
        {
            if(IndiceVehiculoSeleccionado >=0)  //redundante pero por precaución
            {
                switch (MostrarCajaMensajeYesNo("¿Eliminar permanentemente el registro?"))
                {
                    case MessageBoxResult.Yes:
                        EliminarVehiculoSeleccionado();
                        break;
                    case MessageBoxResult.No:
                        break;
                }            
            }

        }

        private void botonModificarV_Click(object sender, RoutedEventArgs e)
        {
            if(IndiceVehiculoSeleccionado >=0)
            {
                Vehiculo v = (Vehiculo)VehiculoSeleccionado;
                CDAgregar cdModif = new CDAgregar(v);
                cdModif.Owner = this;
                cdModif.ShowDialog();
                if(cdModif.DialogResult == true)
                {

                    foreach (Vehiculo item in listaDeVehiculos)
                    {
                        if (v!=item && item.Matricula.Equals(cdModif.TextoMatricula))
                        {
                            MostrarCajaMensajeError("Coche con matrícula ya existente");
                            return;
                        }
                    }
                    v.Matricula = cdModif.TextoMatricula;
                    v.Marca = cdModif.TextoMarca;
                }
            }
        }

        private void botonAgregarR_Click(object sender, RoutedEventArgs e)
        {
            if(IndiceVehiculoSeleccionado >= 0) //redundante pero por precaución
            {
                SeleccionVehiculoEventArgs args = new SeleccionVehiculoEventArgs();
                Vehiculo v = (Vehiculo)VehiculoSeleccionado;
                args.matriculaVehiculoSeleccionado = v.Matricula;
                OnAgregarRepostajePulsado(args);
            }
        }


        private void botonEliminarR_Click(object sender, RoutedEventArgs e)
        {
            if(ListaRepostajes.SelectedIndex >= 0)  //redundante pero por precaución
            {
                switch(MostrarCajaMensajeYesNo("¿Eliminar permanentemente el/los registro/s?"))
                {
                    case MessageBoxResult.Yes:
                        while(ListaRepostajes.SelectedIndex >= 0)
                        {
                            listaDeVehiculos.ElementAt(IndiceVehiculoSeleccionado).Repostajes.RemoveAt(ListaRepostajes.SelectedIndex);
                        }
                        break;
                    case MessageBoxResult.No:   break;
                }              
            }  

        }

        //Esta función es para que se limpie automáticamente la gráfica sin tener que llamar a limpiarAreaDibujo() en mainwindow      
        private void EliminarVehiculoSeleccionado()
        {
            listaDeVehiculos.ElementAt(IndiceVehiculoSeleccionado).Repostajes.Clear();  //limpiamos la lista Repostajes para que se dispare el evento CollectionChanged
            listaDeVehiculos.RemoveAt(IndiceVehiculoSeleccionado);
        }

        private void ListaVehiculos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeleccionVehiculoEventArgs args = new SeleccionVehiculoEventArgs();
            if (IndiceVehiculoSeleccionado >= 0)   //Controlamos que tenemos un item seleccionado
            {
                if(!botonEliminarV.IsEnabled)           //Si el botón está desactivado
                    botonEliminarV.IsEnabled = true;    //activamos la opión de poder eliminar un elemento
                if(!botonModificarV.IsEnabled)
                    botonModificarV.IsEnabled= true;
                botonAgregarR.IsEnabled = true;         //activamos la opcion de agregar repostaje también
                Vehiculo v = (Vehiculo)VehiculoSeleccionado;
                args.matriculaVehiculoSeleccionado = v.Matricula;
                ListaRepostajes.ItemsSource = v.Repostajes;
            }
            else  //Si no hay nada seleccionado
            {              
                botonEliminarV.IsEnabled=false;     //desactivamos botones
                botonModificarV.IsEnabled = false;
                botonAgregarR.IsEnabled = false;
                args.matriculaVehiculoSeleccionado = null;   
                ListaRepostajes.ItemsSource = null;  
                ListaRepostajes.Items.Clear();
            }                     
            OnSeleccionVehiculo(args);
        }

        private void ListaRepostajes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaRepostajes.SelectedIndex >= 0)  //Si hay un elemento seleccionado           
                botonEliminarR.IsEnabled = true;
            else botonEliminarR.IsEnabled = false;
        }

        public void RefrescarTablas()
        {
            ListaRepostajes.Items.Refresh();
            ListaVehiculos.Items.Refresh();
        }

        public void DeseleccionarTablaVehiculos()
        {
            ListaVehiculos.UnselectAll();
        }

        //==========================================
        //      MENÚ
        //==========================================
        private void MenuLimpiar_Click(object sender, RoutedEventArgs e)
        {
            if( listaDeVehiculos.Count > 0)
            {
                String msg = "Se perderán todos los datos, ¿confirmar?";
                String titulo = "Limpiar archivo";
                MessageBoxButton botones = MessageBoxButton.YesNo;
                MessageBoxImage icono = MessageBoxImage.Question;
                switch (MessageBox.Show(msg, titulo, botones, icono))
                {
                    case MessageBoxResult.Yes:
                        OnLimpiarListaPulsado(new EventArgs());
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else { MostrarCajaMensajeError("El archivo ya está vacío"); }

            
        }
        private void OrdenarMatricula_Click(object sender, RoutedEventArgs e)
        {
            OrdenarLista("Matricula");
        }
        private void OrdenarMarca_Click(object sender, RoutedEventArgs e)
        {
            OrdenarLista("Marca");
        }
        private void OrdenarKm_Click(object sender, RoutedEventArgs e)
        {
            OrdenarLista("Kilometros");
        }
        

        private ObservableCollection<Vehiculo> OrdenarLista(string propertyName)
        {
            ObservableCollection<Vehiculo> temp = null;
            switch(propertyName)
            {
                case "Matricula":                    
                    temp = new ObservableCollection<Vehiculo>(listaDeVehiculos.OrderBy(x => x.Matricula));
                    listaDeVehiculos.Clear();
                    foreach (Vehiculo vehiculo in temp) listaDeVehiculos.Add(vehiculo);
                    break;
                case "Marca":
                    temp = new ObservableCollection<Vehiculo>(listaDeVehiculos.OrderBy(x => x.Marca));
                    listaDeVehiculos.Clear();
                    foreach (Vehiculo vehiculo in temp) listaDeVehiculos.Add(vehiculo);
                    break;
                case "Kilometros":
                    temp = new ObservableCollection<Vehiculo>(listaDeVehiculos.OrderBy(x => x.KilometrosAct));
                    listaDeVehiculos.Clear();
                    foreach (Vehiculo vehiculo in temp) listaDeVehiculos.Add(vehiculo);
                    break;
                default:break;
            }
            return temp;
        }

        private void VRandom_Click(object sender, RoutedEventArgs e)
        {
            listaDeVehiculos.Add(new Vehiculo());
        }

        //===============================
        //      VIRTUAL
        //================================
        protected virtual void OnSeleccionVehiculo(SeleccionVehiculoEventArgs e)
        {
            if(this.SeleccionVehiculo != null) this.SeleccionVehiculo.Invoke(this, e);
        }

        protected virtual void OnAgregarRepostajePulsado(SeleccionVehiculoEventArgs e)
        {
            AgregarRepostajePulsado?.Invoke(this, e);
        }

        protected virtual void OnLimpiarListaPulsado(EventArgs e)
        {
           LimpiarListaPulsado.Invoke(this, e);
        }



        //MSGTEXTBOXS
        public MessageBoxResult MostrarCajaMensajeYesNo(string msg)
        {
            String titulo = "Eliminar";
            MessageBoxButton botones = MessageBoxButton.YesNo;
            MessageBoxImage icono = MessageBoxImage.Question;
            return MessageBox.Show(msg, titulo, botones, icono);
        }

        private void MostrarCajaMensajeError(string msg)
        {
            String titulo = "Error";
            MessageBoxButton botones = MessageBoxButton.OK;
            MessageBoxImage icono = MessageBoxImage.Error;
            MessageBox.Show(msg, titulo, botones, icono);

        }

        //PROPIEDADES     
        public object VehiculoSeleccionado
        {
            get { return ListaVehiculos.SelectedItem; }
            set { ListaVehiculos.SelectedItem = value;}
        }
        public int IndiceVehiculoSeleccionado
        {
            get { return ListaVehiculos.SelectedIndex;}
            set
            {
                ListaVehiculos.SelectedIndex = value;
            }
        }

    }
}