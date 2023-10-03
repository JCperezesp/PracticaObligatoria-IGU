using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace PracticaObligatoria
{
    public class Repostaje
    {
        private string fecha;
        private long kmRepos;
        private int litros;
        private double coste;

        public Repostaje(string fecha, long kmRepos, int litros, double coste)
        {
            this.fecha = fecha;
            this.kmRepos = kmRepos;
            this.litros = litros;
            this.coste = coste;           
        }
        
        //constructor aleatorio
        public Repostaje(long kmReposAnteriores, Random random)
        {
            fecha = random.Next(31).ToString()+"/"+(1+random.Next(11)).ToString()+"/"+(1900+random.Next(DateTime.Now.Year-1900)).ToString();
            kmRepos = kmReposAnteriores + random.Next(789);
            litros = random.Next(80);
            coste = random.NextDouble() * 123.0F;
        }

        public string Fecha { get { return fecha; } set { fecha = value; } }
        public long KmRepos { get { return kmRepos; } set { kmRepos = value; } }    
        public int Litros { get { return litros; } set { litros = value; } }
        public double Coste { get { return coste; } set { coste = value; } }
    }
}
