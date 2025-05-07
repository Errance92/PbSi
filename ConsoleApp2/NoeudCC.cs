using System;

namespace Karaté
{
    /// <summary>
    /// class noeud pour les relations clients cusiniers
    /// </summary>
	public class NoeudCC
	{
		private int id;
		private string nom;

        /// <summary>
        /// constructeur de noeuds cc 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nom"></param>
		public NoeudCC(int id, string nom)
		{
            if (id <= 0)
            {
                Console.WriteLine("Il y a une erreur");
            }
            else
            {
                this.id = id;
            }
            this.nom = nom;
		}

        public int Id
        {
            get { return id; }
        }

        public string Nom
        {
            get { return nom; }
        }
    }
}

