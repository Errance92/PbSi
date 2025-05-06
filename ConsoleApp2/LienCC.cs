using System;
namespace Karaté
{
    /// <summary>
    /// classe lien pour la relation client cuinsier
    /// </summary>
	public class LienCC
	{
		private NoeudCC noeudccUn;
        private NoeudCC noeudccDeux;

        /// <summary>
        /// constructeur de liens
        /// </summary>
        /// <param name="noeudccUn"></param>
        /// <param name="noeudccDeux"></param>
        public LienCC(NoeudCC noeudccUn, NoeudCC noeudccDeux)
        {
            this.noeudccUn = noeudccUn;
            this.noeudccDeux = noeudccDeux;
        }

        /// <summary>
        /// Recupere le premier noeud du lien
        /// </summary>
        public NoeudCC NoeudccUn
        {
            get { return noeudccUn; }
        }

        /// <summary>
        /// Recupere le deuxième noeud du lien
        /// </summary>
        public NoeudCC NoeudccDeux
        {
            get { return noeudccDeux; }
        }
    }
}

