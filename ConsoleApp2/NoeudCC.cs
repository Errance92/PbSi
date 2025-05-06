using System;

namespace Karaté
{
	public class NoeudCC
	{
		private int id;
		private string nom;

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

