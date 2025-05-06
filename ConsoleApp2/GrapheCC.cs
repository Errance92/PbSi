using System;
namespace Karaté
{
    using System;
    using System.Collections.Generic;
    using SkiaSharp;

    namespace Karaté
    {
        public class GrapheCC
        {
            private List<NoeudCC> noeudsCC;
            private List<LienCC> liensCC;
            private Dictionary<int, List<int>> listeAdjacenceCC = new Dictionary<int, List<int>>();

            public List<NoeudCC> NoeudsCC
            {
                get { return noeudsCC; }
            }

            public List<LienCC> LiensCC
            {
                get { return liensCC; }
            }

            public Dictionary<int, List<int>> ListeAdjacenceCC
            {
                get { return listeAdjacenceCC; }
            }

            public GrapheCC()
            {
                noeudsCC = new List<NoeudCC>();
                liensCC = new List<LienCC>();

                Dictionary<string, NoeudCC> dicoNoeuds = new Dictionary<string, NoeudCC>();

                DbAccess db = new DbAccess();
                List<Commande> commandes = db.RecupererCommandes();

                foreach (Commande commande in commandes)
                {
                    string cleClient = "Client_" + commande.IdClient;
                    string cleCuisinier = "Cuisinier_" + commande.IdCuisinier;

                    if (!dicoNoeuds.ContainsKey(cleClient))
                    {
                        NoeudCC client = new NoeudCC(commande.IdClient, "Client " + commande.IdClient);
                        dicoNoeuds[cleClient] = client;
                        noeudsCC.Add(client);
                    }

                    if (!dicoNoeuds.ContainsKey(cleCuisinier))
                    {
                        NoeudCC cuisinier = new NoeudCC(commande.IdCuisinier, "Cuisinier " + commande.IdCuisinier);
                        dicoNoeuds[cleCuisinier] = cuisinier;
                        noeudsCC.Add(cuisinier);
                    }

                    NoeudCC n1 = dicoNoeuds[cleClient];
                    NoeudCC n2 = dicoNoeuds[cleCuisinier];
                    LienCC lien = new LienCC(n1, n2);
                    liensCC.Add(lien);
                }

                RemplirListeAdjCC();
            }

            public void RemplirListeAdjCC()
            {
                listeAdjacenceCC = new Dictionary<int, List<int>>();

                for (int i = 0; i < noeudsCC.Count - 1; i++)
                {
                    for (int j = 0; j < noeudsCC.Count - 1 - i; j++)
                    {
                        if (noeudsCC[j].Id > noeudsCC[j + 1].Id)
                        {
                            NoeudCC temp = noeudsCC[j];
                            noeudsCC[j] = noeudsCC[j + 1];
                            noeudsCC[j + 1] = temp;
                        }
                    }
                }

                foreach (NoeudCC noeud in noeudsCC)
                {
                    listeAdjacenceCC[noeud.Id] = new List<int>();
                }

                foreach (LienCC lien in liensCC)
                {
                    NoeudCC un = lien.NoeudccUn;
                    NoeudCC deux = lien.NoeudccDeux;

                    if (listeAdjacenceCC.ContainsKey(un.Id) && listeAdjacenceCC.ContainsKey(deux.Id))
                    {
                        listeAdjacenceCC[un.Id].Add(deux.Id);
                        listeAdjacenceCC[deux.Id].Add(un.Id);
                    }
                }

                foreach (int key in listeAdjacenceCC.Keys.ToList())
                {
                    listeAdjacenceCC[key].Sort();
                }
            }

            public Dictionary<int, int> WelshPowell()
            {
                Dictionary<int, int> couleurs = new Dictionary<int, int>();
                Dictionary<int, int> degres = new Dictionary<int, int>();

                foreach (int id in listeAdjacenceCC.Keys)
                {
                    degres[id] = listeAdjacenceCC[id].Count;
                }

                List<int> noeudsTri = new List<int>(degres.Keys);
                for (int i = 0; i < noeudsTri.Count - 1; i++)
                {
                    for (int j = 0; j < noeudsTri.Count - 1 - i; j++)
                    {
                        if (degres[noeudsTri[j]] < degres[noeudsTri[j + 1]])
                        {
                            int temp = noeudsTri[j];
                            noeudsTri[j] = noeudsTri[j + 1];
                            noeudsTri[j + 1] = temp;
                        }
                    }
                }

                int couleurActuelle = 1;

                foreach (int noeud in noeudsTri)
                {
                    if (!couleurs.ContainsKey(noeud))
                    {
                        couleurs[noeud] = couleurActuelle;

                        for (int i = 0; i < noeudsTri.Count; i++)
                        {
                            int autre = noeudsTri[i];

                            if (!couleurs.ContainsKey(autre))
                            {
                                bool coloriable = true;

                                for (int j = 0; j < listeAdjacenceCC[autre].Count; j++)
                                {
                                    int voisin = listeAdjacenceCC[autre][j];
                                    if (couleurs.ContainsKey(voisin))
                                    {
                                        if (couleurs[voisin] == couleurActuelle)
                                        {
                                            coloriable = false;
                                        }
                                    }
                                }
                                if (coloriable)
                                {
                                    couleurs[autre] = couleurActuelle;
                                }
                            }
                        }
                        couleurActuelle = couleurActuelle + 1;
                    }
                }
                return couleurs;
            }

            public void ProprieteGrapheCC(Dictionary<int, int> coloration)
            {

                List<int> couleursUtilisees = new List<int>();
                foreach (int couleur in coloration.Values)
                {
                    if (!couleursUtilisees.Contains(couleur))
                    {
                        couleursUtilisees.Add(couleur);
                    }
                }

                int nombreCouleurs = couleursUtilisees.Count;
                Console.WriteLine("Nombre minimal de couleurs utilisées : " + nombreCouleurs);

                if (nombreCouleurs == 2)
                {
                    Console.WriteLine("Le graphe peut être biparti).");
                }
                else
                {
                    Console.WriteLine("Le graphe n'est pas biparti.");
                }

                int n = noeudsCC.Count;
                int l = liensCC.Count;
                if (l <= 3 * n - 6)
                {
                    Console.WriteLine("Le graphe peut être planaire");
                }
                else
                {
                    Console.WriteLine("Le graphe est non planaire");
                }

                Console.WriteLine("Groupes indépendants :");
                for (int i = 0; i < couleursUtilisees.Count; i++)
                {
                    int couleur = couleursUtilisees[i];
                    Console.Write("  Couleur " + couleur + " : ");
                    foreach (NoeudCC noeud in noeudsCC)
                    {
                        if (coloration.ContainsKey(noeud.Id) && coloration[noeud.Id] == couleur)
                        {
                            Console.Write(noeud.Nom + "  ");
                        }
                    }
                    Console.WriteLine();
                }
            }

            public void DessinerGrapheColoré(Dictionary<int, int> couleurs)
            {
                int largeur = 1200;
                int hauteur = 800;
                using (var surface = SKSurface.Create(new SKImageInfo(largeur, hauteur)))
                {
                    var canvas = surface.Canvas;
                    canvas.Clear(SKColors.White);

                    Dictionary<int, SKColor> palette = new Dictionary<int, SKColor>
                    {
                        { 1, SKColors.Red },
                        { 2, SKColors.Blue },
                        { 3, SKColors.Green },
                        { 4, SKColors.Orange },
                        { 5, SKColors.Purple },
                        { 6, SKColors.Brown },
                        { 7, SKColors.Pink },
                        { 8, SKColors.Teal }
                    };

                    int rayon = 30;
                    int margeX = 100;
                    int margeY = 100;
                    int espacementX = 150;
                    int espacementY = 150;

                    Dictionary<int, SKPoint> positions = new Dictionary<int, SKPoint>();
                    int x = margeX;
                    int y = margeY;
                    int compteur = 0;

                    foreach (NoeudCC n in noeudsCC)
                    {
                        SKPoint position = new SKPoint(x, y);
                        positions[n.Id] = position;

                        if (couleurs.ContainsKey(n.Id))
                        {
                            int c = couleurs[n.Id];
                            SKColor couleur = palette.ContainsKey(c) ? palette[c] : SKColors.Gray;

                            canvas.DrawCircle(position, rayon, new SKPaint
                            {
                                Color = couleur,
                                Style = SKPaintStyle.Fill
                            });

                            canvas.DrawCircle(position, rayon, new SKPaint
                            {
                                Color = SKColors.Black,
                                Style = SKPaintStyle.Stroke,
                                StrokeWidth = 2
                            });

                            canvas.DrawText(n.Nom, position.X, position.Y - rayon - 10, new SKPaint
                            {
                                Color = SKColors.Black,
                                TextSize = 16,
                                IsAntialias = true,
                                TextAlign = SKTextAlign.Center
                            });
                        }

                        compteur++;
                        x += espacementX;
                        if (x > largeur - margeX)
                        {
                            x = margeX;
                            y += espacementY;
                        }
                    }

                    foreach (LienCC l in liensCC)
                    {
                        if (positions.ContainsKey(l.NoeudccUn.Id) && positions.ContainsKey(l.NoeudccDeux.Id))
                        {
                            SKPoint p1 = positions[l.NoeudccUn.Id];
                            SKPoint p2 = positions[l.NoeudccDeux.Id];

                            canvas.DrawLine(p1, p2, new SKPaint
                            {
                                Color = SKColors.Black,
                                StrokeWidth = 2
                            });
                        }
                    }

                    using (var image = surface.Snapshot())
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite("graphe_colore.png"))
                    {
                        data.SaveTo(stream);
                    }

                    Console.WriteLine("Graphe coloré sauvegardé sous 'graphe_colore.png'");
                }
            }

        }
    }
}

