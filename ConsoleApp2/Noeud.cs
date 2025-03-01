using System;
namespace Karaté
{
    public class Noeud
    {
        private int identifiant;

        public Noeud(int identifiant)
        {
            if(identifiant <= 0)
            {
                Console.WriteLine("Il y a une erreur");
            }
            else
            {
                this.identifiant = identifiant;
            }
        }

        public int Identifiant
        {
            get { return identifiant; }
        }
    }
}