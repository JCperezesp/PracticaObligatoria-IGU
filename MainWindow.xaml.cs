using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
//using System.IO;

namespace PracticaObligatoria
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CDLista cdLista = null;
        private CDOtros cdResumen = null;
        private static readonly int numAtributosGrafica1 = 3;
        private static readonly int MARGENXG2 = 40;
        private static readonly int MARGENXG1 = 60;
        private static readonly int MARGENYCOSTE = 50, MARGENYCONSUMO = 25, MARGENYKM = 35;
        private static readonly int MARGENYABAJO = 50;
        private readonly double xrealmin;
        private readonly double xrealmax;
        private double yrealmin;
        private double yrealmax;
        private double xCanvas0, xCanvasMax, yCanvas0, yCanvasMax;
        Line ejexG1 = null;

        private bool vehiculoSeleccionadoEnTabla = false;     //para controlar qué gráfica dibujar en cada momento
        private bool soloMediaCoste = false;
        private bool soloMediaConsumo = false;
        private bool soloKilometros = false;


        //private List<Vehiculo> listaDeVehiculos;
        private ObservableCollection<Vehiculo> listaDeVehiculos;
        public MainWindow()
        {
            InitializeComponent();
            xrealmin = 0; xrealmax = 500;
            listaDeVehiculos = new ObservableCollection<Vehiculo>();
            listaDeVehiculos.CollectionChanged += ListaDeVehiculos_CollectionChanged;

        }
       

        private void barraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {

            }
        }

        private void cerrar_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
            Application.Current.Shutdown();
        }

        private void maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                AnchoCanvas.Maximum = 2000;
                this.WindowState = WindowState.Maximized;
                restaurar_m.IsEnabled = true;
                maximizar_m.IsEnabled = false;
            }

            else
            {
                AnchoCanvas.Maximum = 1500;
                this.WindowState = WindowState.Normal;
                restaurar_m.IsEnabled = false;
                maximizar_m.IsEnabled = true;
            }
        }

        private void minimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;          
        }

        private void BotonVerTodo_Click(object sender, RoutedEventArgs e)
        {
            BotonVerTodo.Visibility = Visibility.Collapsed;
            soloMediaConsumo = soloMediaCoste = soloKilometros = false;    //ponemos los booleanos a false;
            if (vehiculoSeleccionadoEnTabla)
            {
                vehiculoSeleccionadoEnTabla = false;
                cdLista.DeseleccionarTablaVehiculos();  //deseleccionamos para dibujar la gr.general

            }
            soloMediaConsumo = false; leyendaMediaCoste.IsChecked = false;
            soloMediaCoste = false; leyendaMediaConsumo.IsChecked = false;
            soloKilometros = false; leyendaKilometros.IsChecked = false;
            DibujarGrafica1();
        }
        private void BotonAgregar_Click(object sender, RoutedEventArgs e)
        {
            CDAgregar ventana = new CDAgregar();
            ventana.Owner = this;
            ventana.ShowDialog();

            if (ventana.DialogResult == true)
            {
                Vehiculo v1;

                foreach (Vehiculo v in listaDeVehiculos)
                { if (v.Matricula.Equals(ventana.TextoMatricula)) {
                        MostrarMensajeError("Coche con matrícula ya existente");
                        return;
                    }
                }

                if (ventana.RepostajeIntroducido)
                {
                    v1 = new Vehiculo(ventana.TextoMatricula, ventana.TextoMarca, ventana.Kilometros, ventana.FechaRepos, ventana.KMrepos, ventana.LitrosRepos, ventana.CosteRepos);
                    listaDeVehiculos.Add(v1);

                }
                else
                {
                    v1 = new Vehiculo(ventana.TextoMatricula, ventana.TextoMarca, ventana.Kilometros);
                    listaDeVehiculos.Add(v1);
                }
            }
        }

        

        private void BotonOtros_Click(object sender, RoutedEventArgs e)
        {
            if (cdResumen == null)
            {
                cdResumen = new CDOtros(listaDeVehiculos);
                cdResumen.Owner = this;

                cdResumen.Show();
                cdResumen.Closed += CdResumen_Closed;
                cdResumen.AgregarVehiculo += BotonAgregar_Click;
            }
        }

        private void CdResumen_Closed(object sender, EventArgs e)
        {
            if(cdResumen != null)
            {
                cdResumen = null;
            }
        }

        //-------- EVENTOS DE COLLECTIONCHANGED ------------------
        private void Repostajes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (cdLista != null)
            {
                DibujarGrafica2((Vehiculo)cdLista.VehiculoSeleccionado);
            }
            else
            {

            }
        }

        private void ListaDeVehiculos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Vehiculo v in e.NewItems)
                {
                    v.Repostajes.CollectionChanged += Repostajes_CollectionChanged;
                }
            }

            if (e.OldItems != null)
                vehiculoSeleccionadoEnTabla = false;

            if (!vehiculoSeleccionadoEnTabla)
            {
                DibujarGrafica1();

            }
        }

        //===========================================================
        //EVENTO QUE ABRE EL CUADRO DE DIÁLOGO EN MODO NO MODAL
        //===========================================================
        private void BotonLista_Click(object sender, RoutedEventArgs e)
        {
            if (cdLista == null)
            {
                cdLista = new CDLista(listaDeVehiculos);
                cdLista.Owner = this;

                cdLista.Show();
                cdLista.Closed += CDlista_Cerrar;
                cdLista.AgregarRepostajePulsado += ClickEnAgregarRepostaje;
                cdLista.SeleccionVehiculo += CdLista_SeleccionVehiculo;
                cdLista.LimpiarListaPulsado += CdLista_LimpiarListaPulsado;
            }

        }
        private void CDlista_Cerrar(object sender, EventArgs e)
        {
            if (cdLista != null)
            {
                cdLista = null;
                vehiculoSeleccionadoEnTabla = false;
                DibujarGrafica1();

            }
        }

        private void CdLista_LimpiarListaPulsado(object sender, EventArgs e)
        {
            //eliminamos todos los elementos de todas las listas
            if (cdLista != null) {
                listaDeVehiculos.Clear();
            }
        }

        private void CdLista_SeleccionVehiculo(object sender, SeleccionVehiculoEventArgs e)
        {
            if (e.matriculaVehiculoSeleccionado != null && cdLista != null) {
                vehiculoSeleccionadoEnTabla = true;
                Vehiculo vSelec = null;
                foreach (Vehiculo vehiculo in listaDeVehiculos)
                {
                    if (vehiculo.Matricula.Equals(e.matriculaVehiculoSeleccionado))
                        vSelec = vehiculo;
                }
                DibujarGrafica2(vSelec);    //dibujamos las polilíneas para el veh´iculo seleccionado en la tabla               
            }
            else //entramos aquí si natricula == null, es decir, no se ha seleccionado ningún vehiculo
            {
                vehiculoSeleccionadoEnTabla = false;
            }
        }

        private void ClickEnAgregarRepostaje(object sender, SeleccionVehiculoEventArgs e)
        {
            Vehiculo temp = null;
            foreach (Vehiculo vehiculo in listaDeVehiculos)
            {
                if (vehiculo.Matricula.Equals(e.matriculaVehiculoSeleccionado))
                    temp = vehiculo;
            }
            CDAgregar2 ventana = new CDAgregar2(temp);
            ventana.Owner = this;
            ventana.ShowDialog();
            if (ventana.DialogResult == true && cdLista != null && cdLista.IndiceVehiculoSeleccionado >= 0)
            {
                Repostaje r = new Repostaje(ventana.TextoFecha, ventana.KMRepos, ventana.Litros, ventana.Coste);
                temp.Repostajes.Add(r);
                //cdLista.RefrescarTablas();
            }

        }

        private void AreaDibujo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("SIZECHANGED");            
            if (cdLista != null && vehiculoSeleccionadoEnTabla) //si tenemos el CD abierto y con un item seleccionado entramos
            {
                DibujarGrafica2((Vehiculo)cdLista.VehiculoSeleccionado);
            }
            else if (!vehiculoSeleccionadoEnTabla)
            {
                DibujarGrafica1();
            }
        }

        //==============================================================================
        //
        //  GRÁFICO 2 (POLILÍNEA)
        //
        //==============================================================================

        Polyline lineaCoste, lineaConsumo, lineaKM;
        Ellipse[] PuntosCoste, PuntosLitros, PuntosKM;
        private void DibujarGrafica2(Vehiculo v)
        {
            limpiarAreaDibujo();    //Ponemos esto arriba para que cuando eliminemos el ultimo repostaje de la lista también se limpie el canvas
            //Console.WriteLine("DIBUJAR GRAFICA2");
            BotonVerTodo.Visibility = Visibility.Visible;
            if (v == null || v.Repostajes.Count == 0)    //Si no hay nada que dibujar volvemos para evitar error de ejec.
                return;
            DibujarLeyendas2(v);
            int numPuntos = v.Repostajes.Count;
            int contador = 0;
            double salto = (xrealmax - xrealmin) / numPuntos;
            bool minimo2 = false;
            if (numPuntos >= 1) minimo2 = true;

            double costemax = v.Repostajes.First().Coste;   //representa el coste máximo que ha tenido el vehículo en un repostaje 
            int litrosmax = v.Repostajes.First().Litros;    //representa el valor máximo de litros registrado en el vehiculo
            long kmmax = v.Kilometros, kmDif;
            if (minimo2)
            {
                //kmDif = v.Repostajes.ElementAt(1).KmRepos - v.Repostajes.First().KmRepos;      
                kmDif = v.Repostajes.First().KmRepos - kmmax;//representa la diferencia maxima de KM de un repostaje a otro
                kmmax = kmDif;
            }
            Point p1, p2, p3;
            double xreal, yreal, yrealescalado, xcanvas, ycanvas;

            foreach (Repostaje r in v.Repostajes)   //establecemos los máximos de la colección para escalar bien la gráfica
            {
                if (r != v.Repostajes.First()) { //los kilómetros los tratamos de forma especial, ignoramos el primer elemento
                    kmDif = r.KmRepos - v.Repostajes.ElementAt(v.Repostajes.IndexOf(r) - 1).KmRepos;      //representa la diferencia maxima de KM de un repostaje a otro
                    if (kmDif > kmmax)
                        kmmax = kmDif;
                }
                if (r.Coste > costemax) costemax = r.Coste;
                if (r.Litros > litrosmax) litrosmax = r.Litros;
            }

            lineaConsumo = new Polyline
            {
                Stroke = (Brush)Application.Current.FindResource("colorLineaLitros"),
                StrokeThickness = 2,

            }; lineaCoste = new Polyline
            {
                Stroke = (Brush)Application.Current.FindResource("colorLineaCoste"),
                StrokeThickness = 2
            }; lineaKM = new Polyline
            {
                Stroke = (Brush)Application.Current.FindResource("colorLineaKM"),
                StrokeThickness = 2
            };
            PuntosCoste = new Ellipse[numPuntos];
            PuntosKM = new Ellipse[numPuntos];
            PuntosLitros = new Ellipse[numPuntos];
            for (double i = xrealmin; i <= xrealmax && contador < numPuntos; i += salto, contador++)
            {
                //LINEA DE COSTES
                yCanvas0 = MARGENYCOSTE;
                xreal = i;
                yreal = v.Repostajes.ElementAt(contador).Coste;
                yrealescalado = yreal * (yrealmax / costemax);

                xcanvas = (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;
                ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                //Console.WriteLine("VALORCOSTECANVA: " + ycanvas.ToString());
                //Console.WriteLine("VALORYCANVAS0: " + yCanvas0.ToString());
                p1 = new Point(xcanvas, ycanvas);
                lineaCoste.Points.Add(p1);
                DibujarPunto(xcanvas, ycanvas, yreal, contador, "colorLineaCoste", PuntosCoste);

                //LINEA DE LITROS
                yCanvas0 = MARGENYCONSUMO;
                yreal = v.Repostajes.ElementAt(contador).Litros;
                yrealescalado = yreal * (yrealmax / litrosmax);
                ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                //Console.WriteLine("VALORCOSTECANVA: " + ycanvas.ToString());
                //Console.WriteLine("VALORYCANVAS0: " + yCanvas0.ToString());
                p2 = new Point(xcanvas, ycanvas);
                lineaConsumo.Points.Add(p2);
                DibujarPunto(xcanvas, ycanvas, yreal, contador, "colorLineaLitros", PuntosLitros);

                //LINEA DE KM
                if (minimo2) {  //si es el primer elemento no se calcula punto

                    yCanvas0 = MARGENYKM;
                    if (contador != 0)//si es el primer elemento se hace con los kminiciales del coche              
                        yreal = v.Repostajes.ElementAt(contador).KmRepos - v.Repostajes.ElementAt(contador - 1).KmRepos;
                    else yreal = v.Repostajes.First().KmRepos - v.Kilometros;
                    yrealescalado = yreal * (yrealmax / kmmax);
                    ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;
                    p3 = new Point(xcanvas, ycanvas);
                    lineaKM.Points.Add(p3);
                    DibujarPunto(xcanvas, ycanvas, yreal, contador, "colorLineaKM", PuntosKM);
                }
            }
            AreaDibujo.Children.Add(lineaCoste);
            AreaDibujo.Children.Add(lineaConsumo);
            AreaDibujo.Children.Add(lineaKM);
        }
        TextBlock[] fechasTB, costeTB, LitrosTB, kmTB;

        Line[] verticalesG2;    

        private void DibujarLeyendas2(Vehiculo v)
        {
            xCanvas0 = MARGENXG2;
            xCanvasMax = AreaDibujo.ActualWidth - MARGENXG2;
            yCanvasMax = AreaDibujo.ActualHeight - MARGENYABAJO;
            yrealmin = 0; yrealmax = 250;       //valores máximos de la gráfica en el esp.coordenadas real

            double costemax = v.Repostajes.First().Coste;
            int litrosmax = v.Repostajes.First().Litros;
            long kmDif = v.Repostajes.First().KmRepos - v.Kilometros, kmDifmax = kmDif;
            int numPuntos = v.Repostajes.Count, contador;
            double salto = (xrealmax - xrealmin) / numPuntos;
            double saltoCanvas = salto * (xCanvasMax - xCanvas0) / (xrealmax - xrealmin);
            double yRealEscalado, ycanvas, xreal, xcanvas;
            double yFechaCanvas = yCanvasMax + (MARGENYABAJO / 2);

            //BUSCAMOS LOS VALORES MÁXIMOS
            foreach (Repostaje r in v.Repostajes)   //establecemos los máximos de la colección para escalar bien la gráfica
            {
                if (r != v.Repostajes.First())
                { //los kilómetros los tratamos de forma especial, ignoramos el primer elemento
                    kmDif = r.KmRepos - v.Repostajes.ElementAt(v.Repostajes.IndexOf(r) - 1).KmRepos;      //representa la diferencia maxima de KM de un repostaje a otro
                    if (kmDif > kmDifmax)
                        kmDifmax = kmDif;
                }
                if (r.Coste > costemax) costemax = r.Coste;
                if (r.Litros > litrosmax) litrosmax = r.Litros;
            }
            int numValoresOrientativosKM = 10, numValoresOrientativosCoste = 14, numValoresOrientativosLitros = 16;
            double intervaloLeyendaCosteY = costemax / numValoresOrientativosCoste;
            double intervaloLeyendaLitrosY = litrosmax / numValoresOrientativosLitros;
            double intervaloLeyendaKmY = kmDifmax / numValoresOrientativosKM;

            //COLOCAMOS LAS FECHAS ABAJO
            verticalesG2 = new Line[numPuntos];
            contador = 0;
            fechasTB = new TextBlock[numPuntos];
            for (double i = xrealmin; i <= xrealmax && contador < numPuntos; contador++, i += salto)
            {
                fechasTB[contador] = new TextBlock
                {
                    Text = v.Repostajes.ElementAt(contador).Fecha,
                    FontSize = Math.Round(12 - (numPuntos * 0.2)),
                    FontStyle = FontStyles.Italic,
                    Name = "fecha" + contador,
                    Width = 100
                };

                xreal = i;
                xcanvas = (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;

                verticalesG2[contador] = new Line
                {
                    StrokeThickness = saltoCanvas,
                    ClipToBounds = true
                };
                if (contador % 2 == 0)
                    verticalesG2[contador].Stroke = Brushes.White;
                else verticalesG2[contador].Stroke = (Brush)Application.Current.FindResource("FondoCanvas");
                verticalesG2[contador].X1 = verticalesG2[contador].X2 = xcanvas;
                verticalesG2[contador].Y1 = 0; verticalesG2[contador].Y2 = AreaDibujo.ActualHeight;
                verticalesG2[contador].Opacity = 0.5f;
                AreaDibujo.Children.Add(verticalesG2[contador]);

                AreaDibujo.Children.Add(fechasTB[contador]);
                Canvas.SetLeft(fechasTB[contador], xcanvas - Math.Min(20.0, saltoCanvas / 4)); Canvas.SetTop(fechasTB[contador], yFechaCanvas);
            }

            //LEYENDA COSTE
            yCanvas0 = MARGENYCOSTE;
            costeTB = new TextBlock[numValoresOrientativosCoste];
            contador = 0;
            for (double i = intervaloLeyendaCosteY; i <= costemax + 1 && contador < numValoresOrientativosCoste; i += intervaloLeyendaCosteY, contador++)
            {
                costeTB[contador] = new TextBlock()
                {
                    FontWeight = FontWeights.Medium,
                    FontStyle = FontStyles.Italic,
                    Foreground = (Brush)Application.Current.FindResource("colorLineaCoste"),
                    Text = "_" + ((int)Math.Round(i)).ToString()
                };
                yRealEscalado = i * yrealmax / costemax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;
                AreaLeyendas.Children.Add(costeTB[contador]);
                Canvas.SetLeft(costeTB[contador], AreaLeyendas.ActualWidth);
                Canvas.SetTop(costeTB[contador], ycanvas - 10.0);
            }

            //LEYENDA LITROS
            yCanvas0 = MARGENYCONSUMO;
            LitrosTB = new TextBlock[numValoresOrientativosLitros];
            contador = 0;
            for (double i = intervaloLeyendaLitrosY; i <= litrosmax + 1 && contador < numValoresOrientativosLitros; i += intervaloLeyendaLitrosY, contador++)
            {
                LitrosTB[contador] = new TextBlock()
                {
                    FontWeight = FontWeights.Medium,
                    FontStyle = FontStyles.Italic,
                    Foreground = (Brush)Application.Current.FindResource("colorLineaLitros"),
                    Text = "_" + ((int)Math.Round(i)).ToString()
                };
                yRealEscalado = i * yrealmax / litrosmax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;
                AreaLeyendasDer.Children.Add(LitrosTB[contador]);
                Canvas.SetRight(LitrosTB[contador], AreaLeyendasDer.ActualWidth - LitrosTB[contador].ActualWidth);
                Canvas.SetTop(LitrosTB[contador], ycanvas - 15.0);
            }

            //LEYENDA KM
            yCanvas0 = MARGENYKM;
            contador = 0;
            kmTB = new TextBlock[numValoresOrientativosKM];
            for (double i = intervaloLeyendaKmY; i <= kmDifmax + 1 && contador < numValoresOrientativosKM; i += intervaloLeyendaKmY, contador++)
            {
                kmTB[contador] = new TextBlock
                {
                    FontWeight = FontWeights.Medium,
                    FontStyle = FontStyles.Italic,
                    Foreground = (Brush)Application.Current.FindResource("colorLineaKM"),
                    Text = (Math.Round(i)).ToString() + "_"
                };

                yRealEscalado = i * yrealmax / kmDifmax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;
                AreaLeyendas.Children.Add(kmTB[contador]);
                Canvas.SetRight(kmTB[contador], 0);
                Canvas.SetTop(kmTB[contador], ycanvas - 10.0);
            }

            //Por último mostramos las leyendas con los colores
            leyendaCoste.Visibility = Visibility.Visible;
            leyendaKM.Visibility = Visibility.Visible;
            leyendaLitros.Visibility = Visibility.Visible;
        }


        //==============================================================================
        //
        //  GRÁFICO 1 (BARRAS)
        //
        //==============================================================================
        Polyline[,] barras;
        private void DibujarGrafica1()
        {
            limpiarAreaDibujo();
            //Console.WriteLine("DIBUJAR GRAFICA1");           
            if (listaDeVehiculos.Count == 0) return;

            dibujarLeyendas1();
            bool dibujarTodo = false;
            if (!soloKilometros && !soloMediaCoste && !soloMediaConsumo)
                dibujarTodo = true;
            else BotonVerTodo.Visibility = Visibility.Visible;      //si vamos a dibujar solo una necesitaremos poder volver

            int numPuntos = listaDeVehiculos.Count;
            int contador = 0;
            double saltoReal = (xrealmax - xrealmin) / numPuntos;
            double mediaCosteMax = listaDeVehiculos.First().MediaCoste;
            double mediaConsumoMax = listaDeVehiculos.First().MediaConsumo;
            double kmMax = listaDeVehiculos.First().KilometrosAct;

            foreach (var item in listaDeVehiculos)
            {
                if (item.MediaCoste > mediaCosteMax)
                    mediaCosteMax = item.MediaCoste;
                if (item.MediaConsumo > mediaConsumoMax)
                    mediaConsumoMax = item.MediaConsumo;
                if (item.KilometrosAct > kmMax)
                    kmMax = item.KilometrosAct;
            }
            double xreal, yreal, yrealescalado, xcanvas, ycanvas;

            double saltoCanvas = saltoReal * ((xCanvasMax - xCanvas0) / (xrealmax - xrealmin));

            double anchoBarras = Math.Min(50, saltoCanvas / (2 * 3));
            barras = new Polyline[numPuntos, numAtributosGrafica1];

            for (double i = xrealmin; i <= xrealmax && contador < numPuntos; i += saltoReal, contador++)
            {
                xreal = i;
                //COSTE
                if (soloMediaCoste || dibujarTodo)
                {
                    yCanvas0 = MARGENYCOSTE;

                    yreal = listaDeVehiculos.ElementAt(contador).MediaCoste;
                    yrealescalado = yreal * (yrealmax / mediaCosteMax);


                    xcanvas = (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;
                    ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                    barras[contador, 0] = new Polyline
                    { Stroke = (Brush)Application.Current.FindResource("BarraCoste"), StrokeThickness = anchoBarras };
                    //Console.WriteLine("VALORCOSTECANVA: " + ycanvas.ToString());
                    barras[contador, 0].Points.Add(new Point(xcanvas, yCanvasMax));
                    barras[contador, 0].Points.Add(new Point(xcanvas, ycanvas));
                    AreaDibujo.Children.Add(barras[contador, 0]);
                }
                //CONSUMO
                if (soloMediaConsumo || dibujarTodo)
                {
                    yCanvas0 = MARGENYCONSUMO;
                    yreal = listaDeVehiculos.ElementAt(contador).MediaConsumo;
                    yrealescalado = yreal * (yrealmax / mediaConsumoMax);


                    xcanvas = (anchoBarras * 5 / 4) + (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;
                    ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                    barras[contador, 1] = new Polyline
                    { Stroke = (Brush)Application.Current.FindResource("BarraConsumo"), StrokeThickness = anchoBarras };
                    barras[contador, 1].Points.Add(new Point(xcanvas, yCanvasMax));
                    barras[contador, 1].Points.Add(new Point(xcanvas, ycanvas));
                    AreaDibujo.Children.Add(barras[contador, 1]);
                }
                //KM
                if (soloKilometros || dibujarTodo) {
                    yCanvas0 = MARGENYKM;
                    yreal = listaDeVehiculos.ElementAt(contador).KilometrosAct;
                    yrealescalado = yreal * (yrealmax / kmMax);


                    xcanvas = 2 * (anchoBarras * 5 / 4) + (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;
                    ycanvas = -(yCanvasMax - yCanvas0) * (yrealescalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                    barras[contador, 2] = new Polyline
                    { Stroke = (Brush)Application.Current.FindResource("BarraKM"), StrokeThickness = anchoBarras };
                    barras[contador, 2].Points.Add(new Point(xcanvas, yCanvasMax));
                    barras[contador, 2].Points.Add(new Point(xcanvas, ycanvas));
                    AreaDibujo.Children.Add(barras[contador, 2]);
                }
            }
            AreaDibujo.Children.Remove(ejexG1); AreaDibujo.Children.Add(ejexG1);        //para que se vea mejor el eje horizontal X
        }

        TextBlock[] matriculaTB, mediaCosteTB, mediaConsumoTB, KmActTB;
        Line[] LCoste, LConsumo, LKm;
        private void dibujarLeyendas1()
        {
            int contador;
            int numVehiculos = listaDeVehiculos.Count;
            long kmmax = listaDeVehiculos.First().KilometrosAct;
            double mediaCosteMax = listaDeVehiculos.First().MediaCoste;
            double mediaConsumoMax = listaDeVehiculos.First().MediaConsumo;
            double yRealEscalado, ycanvas;

            xCanvas0 = MARGENXG1;
            xCanvasMax = AreaDibujo.ActualWidth - MARGENXG2;
            yCanvasMax = AreaDibujo.ActualHeight - MARGENYABAJO;
            yrealmin = 0; yrealmax = 250;       //valores máximos de la gráfica en el esp.coordenadas real

            //------------ LÍNEA DEL EJE X ------------------
            ejexG1 = new Line
            {
                X1 = 0,
                Y1 = yCanvasMax,
                X2 = AreaDibujo.ActualWidth,
                Y2 = yCanvasMax,
                StrokeThickness = 4,
                Stroke = (Brush)Application.Current.FindResource("barraTitulo")
            };
            AreaDibujo.Children.Add(ejexG1);

            //------------ LEYENDAS LATERALES ------------------------------- 
            //BUSCAMOS LOS VALORES MÁXIMOS EN EL MODELO 
            foreach (Vehiculo item in listaDeVehiculos)
            {
                if (item.MediaCoste > mediaCosteMax)
                    mediaCosteMax = item.MediaCoste;
                if (item.MediaConsumo > mediaConsumoMax)
                    mediaConsumoMax = item.MediaConsumo;
                if (item.KilometrosAct > kmmax)
                    kmmax = item.KilometrosAct;
            }

            //Establecemos los intervalos entre marca y marca
            int numValoresLeyendaCoste = 6, numValoresLeyendaConsumo = 6, numValoresLeyendaKm = 6;
            double intervaloLeyendaCoste = mediaCosteMax / numValoresLeyendaCoste;
            double intervaloLeyendaConsumo = mediaConsumoMax / numValoresLeyendaConsumo;
            double intervaloLeyendaKm = kmmax / numValoresLeyendaKm;
            double yMatCanvas = yCanvasMax + (MARGENYABAJO / 2);
            double xreal, xcanvas;

            //MEDIACOSTE
            yCanvas0 = MARGENYCOSTE;
            contador = 0;
            mediaCosteTB = new TextBlock[numValoresLeyendaCoste];
            if (soloMediaCoste) LCoste = new Line[numValoresLeyendaCoste];
            for (double i = intervaloLeyendaCoste; i <= mediaCosteMax + 1 && contador < numValoresLeyendaCoste; i += intervaloLeyendaCoste, contador++)
            {
                if (soloMediaConsumo || soloKilometros) break;
                mediaCosteTB[contador] = new TextBlock()
                {
                    FontStyle = FontStyles.Italic,
                    FontWeight = FontWeights.Medium,
                    Foreground = (Brush)Application.Current.FindResource("leyendaBarraCoste"),
                    Text = ((int)Math.Round(i)).ToString() + "_",
                    MaxWidth = 50,
                };
                yRealEscalado = i * yrealmax / mediaCosteMax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                if (soloMediaCoste && mediaCosteMax != 0)
                {
                    LCoste[contador] = new Line()
                    {
                        Stroke = (Brush)Application.Current.FindResource("leyendaBarraCoste"),
                        StrokeThickness = 1,
                        X1 = xCanvas0 / 2,
                        X2 = xCanvasMax + MARGENXG1 / 2,
                        Y1 = ycanvas,
                        Y2 = ycanvas
                    };
                    AreaDibujo.Children.Add(LCoste[contador]);
                }

                AreaLeyendas.Children.Add(mediaCosteTB[contador]);
                Canvas.SetLeft(mediaCosteTB[contador],AreaLeyendas.ActualWidth);
                Canvas.SetTop(mediaCosteTB[contador], ycanvas - 10.0);
            }

            //MEDIACONSUMO
            yCanvas0 = MARGENYCONSUMO;
            contador = 0;
            mediaConsumoTB = new TextBlock[numValoresLeyendaConsumo];
            if (soloMediaConsumo) LConsumo = new Line[numValoresLeyendaConsumo];
            for (double i = intervaloLeyendaConsumo; i <= mediaConsumoMax + 1 && contador < numValoresLeyendaConsumo; i += intervaloLeyendaConsumo, contador++)
            {
                if (soloKilometros || soloMediaCoste) break;
                mediaConsumoTB[contador] = new TextBlock()
                {
                    FontStyle = FontStyles.Italic,
                    FontWeight = FontWeights.Medium,
                    Foreground = (Brush)Application.Current.FindResource("leyendaBarraConsumo"),
                    Text = "_" + ((int)Math.Round(i)).ToString(),
                    MaxWidth = 50,
                };

                yRealEscalado = i * yrealmax / mediaConsumoMax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                if (soloMediaConsumo && mediaConsumoMax != 0)
                {
                    LConsumo[contador] = new Line()
                    {
                        Stroke = (Brush)Application.Current.FindResource("leyendaBarraConsumo"),
                        StrokeThickness = 1,
                        X1 = xCanvas0 / 2,
                        X2 = xCanvasMax + MARGENXG1 / 2,
                        Y1 = ycanvas,
                        Y2 = ycanvas
                    };
                    AreaDibujo.Children.Add(LConsumo[contador]);
                }
                AreaLeyendasDer.Children.Add(mediaConsumoTB[contador]);
                Canvas.SetLeft(mediaConsumoTB[contador], 0);
                Canvas.SetTop(mediaConsumoTB[contador], ycanvas - 10.0);
            }

            //KILÓMETROS ACTUALES
            contador = 0;
            yCanvas0 = MARGENYKM;
            KmActTB = new TextBlock[numValoresLeyendaKm];
            if (soloKilometros) LKm = new Line[numValoresLeyendaKm];
            for (double i = intervaloLeyendaKm; i <= kmmax && contador < numValoresLeyendaKm; i += intervaloLeyendaKm, contador++)
            {
                if (soloMediaConsumo || soloMediaCoste) break;
                KmActTB[contador] = new TextBlock()
                {
                    FontStyle = FontStyles.Italic,
                    FontWeight = FontWeights.Medium,
                    Foreground = (Brush)Application.Current.FindResource("leyendaBarraKM"),
                    Text = ((int)Math.Round(i)).ToString() + "_",
                    MaxWidth = 50,
                };

                yRealEscalado = i * yrealmax / kmmax;
                ycanvas = -(yCanvasMax - yCanvas0) * (yRealEscalado - yrealmin) / (yrealmax - yrealmin) + yCanvasMax;

                if (soloKilometros && kmmax != 0)
                {
                    LKm[contador] = new Line()
                    {
                        Stroke = (Brush)Application.Current.FindResource("leyendaBarraKM"),
                        StrokeThickness = 1,
                        X1 = xCanvas0 / 2,
                        X2 = xCanvasMax + MARGENXG1 / 2,
                        Y1 = ycanvas,
                        Y2 = ycanvas
                    };
                    AreaDibujo.Children.Add(LKm[contador]);
                }

                AreaLeyendas.Children.Add(KmActTB[contador]);
                Canvas.SetRight(KmActTB[contador], 0);
                Canvas.SetTop(KmActTB[contador], ycanvas - 10.0);
            }
            //COLOCAMOS LAS MATRÍCULAS DE CADA COCHE ABAJO
            contador = 0;
            matriculaTB = new TextBlock[numVehiculos];
            double saltoReal = (xrealmax - xrealmin) / numVehiculos;
            double saltoCanvas = saltoReal * ((xCanvasMax - xCanvas0) / (xrealmax - xrealmin));
            double anchoBarras = Math.Min(50, saltoCanvas / (2 * 3));
            for (double i = xrealmin; i <= xrealmax && contador < numVehiculos; i += saltoReal, contador++)
            {
                matriculaTB[contador] = new TextBlock
                {
                    FontStyle = FontStyles.Italic,
                    FontWeight = FontWeights.Medium,
                    FontSize = Math.Round(14 - (numVehiculos * 0.35 + 0.1) + xCanvasMax*0.002),       //función para a más coches menor tamaño de letra
                    Text = listaDeVehiculos.ElementAt(contador).Matricula
                };

                xreal = i;
                xcanvas = (anchoBarras * 5 / 4) / 2 - numVehiculos + (xCanvasMax - xCanvas0) * (xreal - xrealmin) / (xrealmax - xrealmin) + xCanvas0;
                AreaDibujo.Children.Add(matriculaTB[contador]);
                Canvas.SetLeft(matriculaTB[contador], xcanvas);
                Canvas.SetTop(matriculaTB[contador], yMatCanvas);
            }
            //Por último mostramos las leyendas con los colores
            leyendaMediaCoste.Visibility = Visibility.Visible;
            leyendaKilometros.Visibility = Visibility.Visible;
            leyendaMediaConsumo.Visibility = Visibility.Visible;
        }

        private void DibujarPunto(double xcanvas, double ycanvas, double yreal, int contador, string refColor, Ellipse[] puntos)
        {
            ToolTip t = new ToolTip();
            puntos[contador] = new Ellipse
            {
                Height = 8.0f,
                Width = 8.0f,
                Fill = (Brush)Application.Current.FindResource(refColor),
                ToolTip = " Valor:" + yreal.ToString(),
            };
            AreaDibujo.Children.Add(puntos[contador]);
            Canvas.SetLeft(puntos[contador], xcanvas - 4.0f); Canvas.SetTop(puntos[contador], ycanvas - 4.0f);
        }


        //----------------------------------------------
        //---------- GRÁFICA 1 TOGGLEBUTTONS -----------
        //----------------------------------------------
        private void leyendaKilometros_Checked(object sender, RoutedEventArgs e)
        {
            soloMediaConsumo = false; soloMediaCoste = false; soloKilometros = true;
            DibujarGrafica1();
        }

        private void leyendaMediaConsumo_Checked(object sender, RoutedEventArgs e)
        {
            soloMediaConsumo = true; soloMediaCoste = false; soloKilometros = false;
            DibujarGrafica1();
        }

        private void leyendaMediaCoste_Checked(object sender, RoutedEventArgs e)
        {
            soloMediaConsumo = false; soloMediaCoste = true; soloKilometros = false;
            DibujarGrafica1();
        }
        private void limpiarAreaDibujo()
        {
            AreaDibujo.Children.Clear();
            AreaLeyendas.Children.Clear();
            AreaLeyendasDer.Children.Clear();
            BotonVerTodo.Visibility = Visibility.Collapsed;
            if (!vehiculoSeleccionadoEnTabla || leyendaCoste.Visibility == Visibility.Visible)       //Si una es visible las demás también lo serán
            {
                leyendaCoste.Visibility = Visibility.Collapsed;
                leyendaKM.Visibility = Visibility.Collapsed;
                leyendaLitros.Visibility = Visibility.Collapsed;
                lineaCoste = null;
                lineaConsumo = null;
                lineaKM = null;
            }
            if (vehiculoSeleccionadoEnTabla || leyendaMediaCoste.Visibility == Visibility.Visible)
            {
                leyendaMediaCoste.Visibility = Visibility.Collapsed;
                leyendaMediaConsumo.Visibility = Visibility.Collapsed;
                leyendaKilometros.Visibility = Visibility.Collapsed;
            }
        }

        private void MostrarMensajeError(string msg)
        {
            String titulo = "Error";
            MessageBoxButton botones = MessageBoxButton.OK;
            MessageBoxImage icono = MessageBoxImage.Error;
            MessageBox.Show(msg, titulo, botones, icono);

        }
    }
}
