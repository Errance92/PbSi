using System;
namespace Karaté
{
    public class Lien
    {
        private Noeud noeudUn;
        private Noeud noeudDeux;

        public Lien(Noeud noeudUn, Noeud noeudDeux)
        {
            this.noeudUn = noeudUn;
            this.noeudDeux = noeudDeux;
        }

        public Noeud NoeudUn
        {
            get { return noeudUn; }
        }

        public Noeud NoeudDeux
        {
            get { return noeudDeux; }
        }
    }
}

