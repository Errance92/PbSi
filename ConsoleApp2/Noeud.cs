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

        /// <summary>
        /// Constructeur du noeud 
        /// </summary>
        /// <param name="identifiant">Identifiant du noeud</param>
        public Noeud(int identifiant, string station)
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
    }
}