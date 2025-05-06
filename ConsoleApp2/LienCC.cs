using System;
namespace Karaté
{
	public class LienCC
	{
		private NoeudCC noeudccUn;
        private NoeudCC noeudccDeux;

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

