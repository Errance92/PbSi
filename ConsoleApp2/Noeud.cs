using System;

namespace Karaté
{
    /// <summary>
    /// Représente un noeud
    /// </summary>
    public class Noeud
    {
        /// <summary>
        /// Identifiant du noeud
        /// </summary>
        private int identifiant;
        private string station;
        private double latitude;
        private double longitude;

        /// <summary>
        /// Constructeur du noeud 
        /// </summary>
        /// <param name="identifiant">Identifiant du noeud</param>
        public Noeud(int identifiant, string station, double latitude, double longitude)
        {
            if (identifiant <= 0)
            {
                Console.WriteLine("Il y a une erreur");
            }
            else
            {
                this.identifiant = identifiant;
            }
            this.station = station;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        /// Recupere l'identifiant du noeud
        /// </summary>
        public int Identifiant
        {
            get { return identifiant; }
        }

        public string Station
        {
            get { return station; }
        }

        public double Latitude
        {
            get { return latitude; }
        }

        public double Longitude
        {
            get { return longitude; }
        }
    }
}