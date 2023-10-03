using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaObligatoria
{
    public class Vehiculo : INotifyPropertyChanged
    {
        public static readonly int ATRIBUTOSGRAFICA = 3;
        private string matricula;
        private string marca;
        private long kilometrosIni;
        private long kilometrosAct;
        private double mediaConsumo, mediaCoste;
        private ObservableCollection<Repostaje> repostajes;

        public event PropertyChangedEventHandler PropertyChanged;

        public Vehiculo(string matricula, string marca, long kilometros)
        {
            this.matricula = matricula;
            this.marca = marca;
            this.kilometrosIni = kilometros;
            this.KilometrosAct = kilometros;
            this.mediaConsumo = 0.0f;
            this.mediaCoste = 0.0f;
            repostajes = new ObservableCollection<Repostaje>();
            repostajes.CollectionChanged += Repostajes_CollectionChanged;
        }     
        public Vehiculo(string matricula, string marca, long kilometros, string fecha, long kmrepos, int litros,double coste)
        {
            this.matricula = matricula; this.marca = marca; this.kilometrosIni = kilometros;
            this.mediaConsumo = 0.0f;
            this.mediaCoste = 0.0f;
            repostajes = new ObservableCollection<Repostaje>();
            Repostaje r = new Repostaje(fecha,kmrepos,litros,coste);
            repostajes.CollectionChanged += Repostajes_CollectionChanged;
            repostajes.Add(r);
        }
        public Vehiculo()
        {
            int repos;
            repostajes = new ObservableCollection<Repostaje>();
            repostajes.CollectionChanged += Repostajes_CollectionChanged;
            Random rand = new Random();
            repos = rand.Next(16);
            matricula = "APP-"+rand.Next(9999);
            marca = "AppBrand " + rand.Next(99);
            kilometrosIni = KilometrosAct = 0;
            for (int i = 0; i < repos; i++)
            {
                Repostaje r = new Repostaje(kilometrosAct,rand);
                repostajes.Add(r);
            }
        }

        private void Repostajes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {       
            if(repostajes.Count == 0)
            {
                MediaConsumo = 0.0f;
                MediaCoste = 0.0f;
                KilometrosAct = 0;
            }

            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add 
                || (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove))    //Si se han añadido/eliminado items recalculamos
            {
                //Console.WriteLine("ENTRO");
                mediaConsumo = 0.0f;
                mediaCoste = 0.0f;
                if (repostajes.Count > 0)
                {
                long kilometrosTemp = kilometrosIni;
                foreach (Repostaje r in repostajes)
                {
                    MediaConsumo += 100.0* r.Litros / (r.KmRepos - kilometrosTemp);
                    MediaCoste += 100.0 *r.Coste / (r.KmRepos - kilometrosTemp);
                    kilometrosTemp = r.KmRepos;
                }
                    Console.WriteLine("NUMREPS: " + repostajes.Count);
                MediaConsumo /= repostajes.Count;
                MediaCoste /= repostajes.Count;
                KilometrosAct = repostajes.Last().KmRepos;

                }

            }
        }

        //INOTIFYPROPERTYCHANGED
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        //PROPIEDADES
        public string Matricula { get { return matricula; } set { matricula = value; OnPropertyChanged("Matricula"); } }
        public string Marca { get { return marca; } set { marca = value; OnPropertyChanged("Marca"); } }
        public long Kilometros { get { return kilometrosIni; } set {   kilometrosIni = value; OnPropertyChanged("Kilometros"); } }
        public long KilometrosAct { get { return kilometrosAct; } set { kilometrosAct = value; OnPropertyChanged("KilometrosAct"); } }
        public double MediaConsumo { get { return mediaConsumo; } set { mediaConsumo = value; OnPropertyChanged("MediaConsumo"); } }       
        public double MediaCoste { get { return mediaCoste; } set { mediaCoste = value; OnPropertyChanged("MediaCoste"); } }
        public ObservableCollection<Repostaje> Repostajes { get { return repostajes; } set { repostajes = value; OnPropertyChanged("Repostajes"); } }

    }
}
