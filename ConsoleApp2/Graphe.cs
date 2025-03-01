using System;
using System.Diagnostics;
using SkiaSharp;

namespace Karaté
{
    public class Graphe
    {
        List<Noeud> noeuds;
        List<Lien> liens;
        Dictionary<int, List<int>> listeAdjacence = new Dictionary<int, List<int>>();
        int[,] matriceAdjacence;
        string filepath;

        public List<Noeud> Noeuds
        {
            get { return noeuds; }
        }

        public List<Lien> Liens
        {
            get { return liens; }
        }

        public Dictionary<int, List<int>> ListeAdjacence
        {
            get { return listeAdjacence; }
        }

        public int[,] MatriceAdjacence
        {
            get { return matriceAdjacence; }
        }


        public Graphe()
        {
            noeuds = new List<Noeud>(); 
            liens = new List<Lien>();
            this.filepath = "soc-karate.mtx";
            List<string> lignes = new List<string>();
            string separation = " ";
            string[] lignesFichier = File.ReadAllLines(filepath);

            foreach (string ligne in lignesFichier)
            {
                if (ligne.Trim() != "" && !ligne.Trim().StartsWith("%"))
                {
                    string[] partie = ligne.Split(separation);//, StringSplitOptions.RemoveEmptyEntries);

                    if (partie.Length == 2)
                    {
                        Noeud un = null;
                        Noeud deux = null;

                        for (int i = 0; i < noeuds.Count; i++)
                        {
                            if (noeuds[i].Identifiant == Convert.ToInt32(partie[0]))
                            {
                                un = noeuds[i];
                            }
                        }

                        if (un == null)
                        {
                            un = new Noeud(Convert.ToInt32(partie[0]));
                            noeuds.Add(un);
                        }

                        for (int i = 0; i < noeuds.Count; i++)
                        {
                            if (noeuds[i].Identifiant == Convert.ToInt32(partie[1]))
                            {
                                deux = noeuds[i];
                            }
                        }

                        if (deux == null)
                        {
                            deux = new Noeud(Convert.ToInt32(partie[1]));
                            noeuds.Add(deux);
                        }

                        liens.Add(new Lien(un, deux));
                    }
                }
            }
            matriceAdjacence = new int[noeuds.Count, noeuds.Count];
            RemplirMatriceAdj(matriceAdjacence);
            RemplirListeAdj();
        }

        public void Propriété()
        {
            Console.WriteLine("Taille du graphe : " + liens.Count);
            Console.WriteLine("Ordre du graphe : " + matriceAdjacence.GetLength(0));
            for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
            {
                if (listeAdjacence.ContainsKey(i + 1))
                {                 
                    int degre = listeAdjacence[i + 1].Count;
                    Console.WriteLine("Le degré du sommet " + (i + 1) + " est de " + degre);
                }
            }
        }

        public void RemplirMatriceAdj(int [,]matriceAdjacence)
        {
            for (int i = 0; i < noeuds.Count - 1; i++)
            {
                for (int j = 0; j < noeuds.Count - 1 - i; j++)
                {
                    if (noeuds[j].Identifiant > noeuds[j + 1].Identifiant)
                    {                       
                        Noeud t = noeuds[j];
                        noeuds[j] = noeuds[j + 1];
                        noeuds[j + 1] = t;
                    }
                }
            }

            foreach (var lien in liens)
            {
                int a = -1;
                int b = -1;

                for (int i = 0; i < noeuds.Count; i++)
                {
                    if (noeuds[i].Identifiant == Convert.ToInt32(lien.NoeudUn.Identifiant))
                    {
                        a = i; 
                    }
                }

                for (int i = 0; i < noeuds.Count; i++)
                {
                    if (noeuds[i].Identifiant == Convert.ToInt32(lien.NoeudDeux.Identifiant))
                    {
                        b = i;
                    }
                }

                if (a != -1 && b != -1)
                {
                    matriceAdjacence[a, b] = 1;
                    matriceAdjacence[b, a] = 1;
                }
            }
        }


        public void RemplirListeAdj()
        {
            for (int i = 0; i < noeuds.Count - 1; i++)
            {
                for (int j = 0; j < noeuds.Count - 1 - i; j++)
                {
                    if (noeuds[j].Identifiant > noeuds[j + 1].Identifiant)
                    {

                        Noeud t = noeuds[j];
                        noeuds[j] = noeuds[j + 1];
                        noeuds[j + 1] = t;
                    }
                }
            }

            foreach (var noeud in noeuds)
            {
                listeAdjacence[noeud.Identifiant] = new List<int>();
            }

            foreach (var noeud in noeuds)
            {
                listeAdjacence[noeud.Identifiant] = new List<int>();
            }

            foreach (var lien in liens)
            {
                Noeud un = null;
                Noeud deux = null;

                if (listeAdjacence.ContainsKey(lien.NoeudUn.Identifiant))
                {
                    un = lien.NoeudUn;
                }

                if (listeAdjacence.ContainsKey(lien.NoeudDeux.Identifiant))
                {
                    deux = lien.NoeudDeux;
                }

                if (un != null && deux != null)
                {
                    listeAdjacence[un.Identifiant].Add(deux.Identifiant);
                    listeAdjacence[deux.Identifiant].Add(un.Identifiant);
                }
            }

            foreach (var key in listeAdjacence.Keys.ToList())
            {
                listeAdjacence[key].Sort(); // Trier les voisins de chaque noeud
            }
        }

        public string BFS (int debut)
        {
            string res = "";
            if (debut <= 0 || debut > matriceAdjacence.GetLength(0))
            {

            }
            else
            {
                bool[] visite = new bool[matriceAdjacence.GetLength(0)];
                List<int> parcours = new List<int>();
                parcours.Add(debut);
                visite[debut - 1] = true;

                for (int i = 0; i < parcours.Count; i++)
                {
                    int noeud = parcours[i];
                    res += noeud + " ";

                    if (listeAdjacence.ContainsKey(noeud))
                    {
                        foreach (int voisin in listeAdjacence[noeud])
                        {
                            if (!visite[voisin - 1])
                            {
                                visite[voisin - 1] = true;
                                parcours.Add(voisin);
                            }
                        }
                    }
                }
            }
            return res;
        }

        public string DFS(int debut)
        {
            string res = "";
            if (debut <= 0 || debut > matriceAdjacence.GetLength(0))
            {
              
            }
            else
            {
                bool[] visite = new bool[matriceAdjacence.GetLength(0)];
                List<int> parcours = new List<int>();
                parcours.Add(debut);
                visite[debut - 1] = true;

                for (int i = 0; i < parcours.Count; i++)
                {
                    int noeud = parcours[i];
                    res += noeud + " ";

                    if (listeAdjacence.ContainsKey(noeud))
                    {
                        foreach (int voisin in listeAdjacence[noeud])
                        {
                            if (!visite[voisin - 1])
                            {
                                visite[voisin - 1] = true;
                                parcours.Insert(0, voisin);
                            }
                        }
                    }
                }
            }

            return res;
        }

        public bool Connexe()
        {
            bool res = false;
            string dfs = DFS(noeuds[0].Identifiant);
            string dfsSansEspace = dfs.Replace(" ", "");

            if (dfsSansEspace.Length == noeuds.Count.ToString().Length)
            {
                res = true; 
            }
            else
            {
                res = false; 
            }

            return res;
        }

        public List<List<int>> Cycles()
        {
            var cycles = new List<List<int>>();
            var visite = new bool[noeuds.Count];
            var pileActuel = new bool[noeuds.Count];
            var chemin = new List<int>();

            Stack<int> pile = new Stack<int>();

            for (int i = 0; i < noeuds.Count; i++)
            {
                if (!visite[i])
                {
                    pile.Push(i);
                    chemin.Clear();

                    while (pile.Count > 0)
                    {
                        int noeudActuel = pile.Peek();

                        if (!visite[noeudActuel])
                        {
                            visite[noeudActuel] = true;
                            pileActuel[noeudActuel] = true;
                            chemin.Add(noeudActuel);
                        }

                        bool toutVoisin = true;
                        bool cycleTrouve = false;

                        foreach (var voisin in listeAdjacence[noeuds[noeudActuel].Identifiant])
                        {
                            int indiceVoisin = -1;

                            for (int j = 0; j < noeuds.Count; j++)
                            {
                                if (noeuds[j].Identifiant == voisin)
                                {
                                    indiceVoisin = j;
                                }
                            }

                            if (indiceVoisin != -1)
                            {
                                if (!visite[indiceVoisin])
                                {
                                    pile.Push(indiceVoisin);
                                    toutVoisin = false;
                                }
                                else if (pileActuel[indiceVoisin])
                                {
                                    List<int> cycle = new List<int>();
                                    int indexCycle = chemin.IndexOf(indiceVoisin);

                                    if (indexCycle >= 0 && indexCycle < chemin.Count)
                                    {
                                        for (int p = indexCycle; p < chemin.Count; p++)
                                        {
                                            cycle.Add(chemin[p]);
                                        }
                                        cycles.Add(cycle);
                                        cycleTrouve = true;                                       
                                    }
                                }
                            }
                        }
                        if (chemin.Count > 0)
                        {
                            chemin.RemoveAt(chemin.Count - 1);  
                        }
                        if (toutVoisin || cycleTrouve)
                        {
                            pile.Pop();
                            pileActuel[noeudActuel] = false;
                        }
                    }
                }
            }
            return cycles;
        }


        // Méthode pour dessiner le graphe avec SkiaSharp
        public void DessinerGraphe()
        {
            int largeur = 1800;
            int hauteur = 1800;
            using (var surface = SKSurface.Create(new SKImageInfo(largeur, hauteur)))
            {
                var toile = surface.Canvas;
                toile.Clear(SKColors.White);

                float centreX = largeur / 2;
                float centreY = hauteur / 2;
                float rayon = 700; 
                int nombreDeNoeuds = noeuds.Count;
                float anglePas = 360f / nombreDeNoeuds;

                var positions = new Dictionary<int, SKPoint>();

                for (int i = 0; i < nombreDeNoeuds; i++)
                {
                    float angle = anglePas * i;
                    float x = centreX + rayon * (float)Math.Cos(angle * Math.PI / 180);
                    float y = centreY + rayon * (float)Math.Sin(angle * Math.PI / 180);
                    positions[noeuds[i].Identifiant] = new SKPoint(x, y);
                }

                foreach (var lien in liens)
                {
                    var point1 = positions[lien.NoeudUn.Identifiant];
                    var point2 = positions[lien.NoeudDeux.Identifiant];

                    toile.DrawLine(point1, point2, new SKPaint
                    {
                        Color = SKColors.Gray,
                        StrokeWidth = 2
                    });
                }

                foreach (var noeud in noeuds)
                {
                    var point = positions[noeud.Identifiant];
                    toile.DrawCircle(point, 30, new SKPaint
                    {
                        Color = SKColors.Blue,
                        Style = SKPaintStyle.Fill
                    });

                    toile.DrawText(noeud.Identifiant.ToString(), point.X - 2, point.Y + 9, new SKPaint
                    {
                        Color = SKColors.White,
                        TextSize = 30,
                        TextAlign = SKTextAlign.Center
                    });
                }

                using (var flux = File.OpenWrite("graphe.png"))
                {
                    surface.Snapshot().Encode().SaveTo(flux);
                }

                Console.WriteLine("Le graphe a été dessiné et sauvegardé sous 'graphe.png'.");
            }
        }

    }
}
