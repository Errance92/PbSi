using System;
namespace Karaté
{
    public class Graphe
    {
        public List<Noeud> Noeuds { get; set; }
        public List<Lien> Liens { get; set; }
        public Dictionary<int, List<int>> ListeAdjacence { get; set; }
        public int[,] MatriceAdjacence { get; set; }

        public Graphe()
        {
            Noeuds = new List<Noeud>();
            Liens = new List<Lien>();
            ListeAdjacence = new Dictionary<int, List<int>>();
        }


        public void AjouterNoeud(Noeud noeud)
        {
            Noeuds.Add(noeud);
            ListeAdjacence[noeud.Id] = new List<int>();
        }

        public void AjouterLien(Lien lien)
        {
            Liens.Add(lien);
            ListeAdjacence[lien.Noeud1.Id].Add(lien.Noeud2.Id);
            ListeAdjacence[lien.Noeud2.Id].Add(lien.Noeud1.Id);
        }

        public void InitialiserMatriceAdjacence()
        {
            int taille = Noeuds.Count;
            MatriceAdjacence = new int[taille, taille];

            for (int i = 0; i < taille; i++)
            {
                for (int j = 0; j < taille; j++)
                {
                    MatriceAdjacence[i, j] = 0;
                }
            }

            foreach (var lien in Liens)
            {
                int index1 = Noeuds.FindIndex(n => n.Id == lien.Noeud1.Id);
                int index2 = Noeuds.FindIndex(n => n.Id == lien.Noeud2.Id);
                MatriceAdjacence[index1, index2] = 1;
                MatriceAdjacence[index2, index1] = 1;
            }
        }
    }

}
