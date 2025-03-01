using System;

namespace Karaté
{
    /// <summary>
    /// Represente un lien entre deux noeuds
    /// </summary>
    public class Lien
    {

        private Noeud noeudUn;
        private Noeud noeudDeux;

        /// <summary>
        /// Constructeur de la classe Lien
        /// </summary>
        /// <param name="noeudUn">premier noeud</param>
        /// <param name="noeudDeux">deuxieme noeud</param>
        public Lien(Noeud noeudUn, Noeud noeudDeux)
        {
            this.noeudUn = noeudUn;
            this.noeudDeux = noeudDeux;
        }

        /// <summary>
        /// Recupere le premier noeud du lien
        /// </summary>
        public Noeud NoeudUn
        {
            get { return noeudUn; }
        }

        /// <summary>
        /// Recupere le deuxième noeud du lien
        /// </summary>
        public Noeud NoeudDeux
        {
            get { return noeudDeux; }
        }
    }
}