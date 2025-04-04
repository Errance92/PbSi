using System;
using System.Diagnostics;
using SkiaSharp;

namespace Karaté
{
    /// <summary>
    /// Represente le graphe avec les liens entre les differents noeuds 
    /// </summary>
    public class Graphe
    {
        List<Noeud> noeuds;
        List<Lien> liens;
        Dictionary<int, List<int>> listeAdjacence = new Dictionary<int, List<int>>();
        Dictionary<string, List<string>> listeAdjacenceNom = new Dictionary<string, List<string>>();
        int[,] matriceAdjacence;

        /// <summary>
        /// Recupere la liste de noeuds
        /// </summary>
        public List<Noeud> Noeuds
        {
            get { return noeuds; }
        }

        /// <summary>
        /// recupere la liste de liens
        /// </summary>
        public List<Lien> Liens
        {
            get { return liens; }
        }

        /// <summary>
        /// recupere la liste d'adjacence
        /// </summary>
        public Dictionary<int, List<int>> ListeAdjacence
        {
            get { return listeAdjacence; }
        }

        public Dictionary<string, List<string>> ListeAdjacenceNom
        {
            get { return listeAdjacenceNom; }
        }

        /// <summary>
        /// recupere la matrice d'adjacence
        /// </summary>
        public int[,] MatriceAdjacence
        {
            get { return matriceAdjacence; }
        }

        /// <summary>
        /// Constructeur de la classe graphe 
        /// </summary>
        public Graphe()
        {
            noeuds = new List<Noeud>();
            liens = new List<Lien>();

            Dictionary<string, Noeud> station = new Dictionary<string, Noeud>();

            string filepathStation = "MetroParis.csv";
            string separation = ",";
            string[] lignesFichier = File.ReadAllLines(filepathStation);

            for (int i = 1; i < lignesFichier.Length; i++)
            {
                string ligne = lignesFichier[i];
                if (ligne.Trim() != "")
                {
                    string[] partie = ligne.Split(separation);
                    if (partie.Length >= 3)
                    {
                        string libelleLigne = partie[1].Trim();
                        string nomStation = partie[2].Trim();
                        string cle = nomStation + "_" + libelleLigne;
                        double latitude = Convert.ToDouble(partie[4].Trim());
                        double longitude = Convert.ToDouble(partie[3].Trim());
                        if (station.ContainsKey(nomStation) == false)
                        {
                            Noeud n = new Noeud(station.Count + 1, nomStation, latitude, longitude);
                            station.Add(nomStation, n);
                            noeuds.Add(n);
                        }
                    }
                }
            }

            string filepathLinks = "Liens.csv";
            string[] lignesLiens = File.ReadAllLines(filepathLinks);

            for (int i = 1; i < lignesLiens.Length; i++)
            {
                string ligne = lignesLiens[i];
                if (ligne.Trim() != "")
                {
                    string[] partie = ligne.Split(separation);
                    if (partie.Length >= 4)
                    {
                        string stationDepart = partie[1].Trim();
                        string stationArrivee = partie[2].Trim();
                        int temps = Convert.ToInt32(partie[3].Trim());
                        string ligne_metro = partie[0].Trim();

                        string cleDepart = stationDepart + "_" + ligne_metro;
                        string cleArrivee = stationArrivee + "_" + ligne_metro;

                        Noeud un = null;
                        Noeud deux = null;

                        if (station.ContainsKey(stationDepart))
                        {
                            un = station[stationDepart];
                        }
                        if (station.ContainsKey(stationArrivee))
                        {
                            deux = station[stationArrivee];
                        }

                        if (un != null && deux != null)
                        {
                            liens.Add(new Lien(un, deux, temps, ligne_metro));
                        }
                    }
                }
            }
            matriceAdjacence = new int[noeuds.Count, noeuds.Count];
            RemplirMatriceAdj(matriceAdjacence);
            RemplirListeAdj();
            RemplirListeAdjNom();
        }

        /// <summary>
        /// Affiche les propriétés du graphe (bonus)
        /// </summary>
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

        /// <summary>
        /// Rempli la matrice d'adjacence
        /// </summary>
        /// <param name="matriceAdjacence"></param>
        public void RemplirMatriceAdj(int[,] matriceAdjacence)
        {
            for (int i = 0; i < noeuds.Count; i++)
            {
                for (int j = 0; j < noeuds.Count; j++)
                {
                    matriceAdjacence[i, j] = 0;
                }
            }

            foreach (var lien in liens)
            {
                int a = lien.NoeudUn.Identifiant - 1;
                int b = lien.NoeudDeux.Identifiant - 1;

                if (matriceAdjacence[a, b] == 0 || lien.Ponderation < matriceAdjacence[a, b])
                {
                    matriceAdjacence[a, b] = lien.Ponderation;
                    matriceAdjacence[b, a] = lien.Ponderation;
                }
            }
        }

        /// <summary>
        /// Rempli la liste d'adjacence
        /// </summary>
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
                listeAdjacence[key].Sort();
            }
        }
        /// <summary>
        /// Construit la liste d’adjacence avec les noms de stations au lieu des identifiants.
        /// </summary>

        public void RemplirListeAdjNom()
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
                listeAdjacenceNom[noeud.Station] = new List<string>();
            }

            foreach (var lien in liens)
            {
                Noeud un = null;
                Noeud deux = null;

                if (listeAdjacenceNom.ContainsKey(lien.NoeudUn.Station))
                {
                    un = lien.NoeudUn;
                }

                if (listeAdjacenceNom.ContainsKey(lien.NoeudDeux.Station))
                {
                    deux = lien.NoeudDeux;
                }

                if (un != null && deux != null)
                {
                    listeAdjacenceNom[un.Station].Add(deux.Station);
                    listeAdjacenceNom[deux.Station].Add(un.Station);
                }
            }

        }
        /// <summary>
        /// Effectue un parcours en largeur (BFS) à partir d’un nœud donné.
        /// </summary>
        /// <param name="debut">Identifiant du nœud de départ.</param>
        /// <returns>Le parcours sous forme de chaîne de caractères.</returns>

        public string BFS(int debut)
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

        /// <summary>
        /// Effectue le parcours en profondeur a partir d'un noeud
        /// </summary>
        /// <param name="debut">noeud a partir duquel on commence le parcours</param>
        /// <returns>Le parcours sous forme d'une chaine de caracteres</returns>
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

        /// <summary>
        /// verifie si le graphe est connexe en verifiant si le parcours DFS parcours bien tout les noeuds
        /// si le nombre de noeuds dans DFS = le nombre de noeuds total alors connexe
        /// </summary>
        /// <returns>true si connexe, false sinon</returns>
        public bool Connexe()
        {
            bool res = true;
            string dfs = DFS(noeuds[0].Identifiant);
            string dfsSansEspace = dfs.Replace(" ", "");

            if ((dfsSansEspace.Length) + 1 != noeuds.Count)
            {
                res = false;
            }
            else
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// trouve les cycles d'un graphe (je crois qu'il fonctionne pas super bien)
        /// </summary>
        /// <returns>une liste de cycle</returns>
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


        /// <summary>
        /// dessine le graphe avec SkiSharp 
        /// </summary>
        public void DessinerGraphe()
        {
            int largeur = 4900;
            int hauteur = 4900;
            using (var surface = SKSurface.Create(new SKImageInfo(largeur, hauteur)))
            {
                var toile = surface.Canvas;
                // Fond beige clair
                toile.Clear(SKColors.Beige);

                // Calcul des positions en fonction des vraies coordonnées géographiques
                // (Supposant que vos nœuds disposent de Latitude et Longitude)
                double minLat = double.MaxValue, maxLat = double.MinValue;
                double minLon = double.MaxValue, maxLon = double.MinValue;
                foreach (Noeud n in noeuds)
                {
                    if (n.Latitude < minLat) minLat = n.Latitude;
                    if (n.Latitude > maxLat) maxLat = n.Latitude;
                    if (n.Longitude < minLon) minLon = n.Longitude;
                    if (n.Longitude > maxLon) maxLon = n.Longitude;
                }
                double marge = 0.008;  // Marge plus importante
                minLat -= marge; maxLat += marge;
                minLon -= marge; maxLon += marge;

                var positions = new Dictionary<int, SKPoint>();
                foreach (Noeud n in noeuds)
                {
                    float x = (float)((n.Longitude - minLon) / (maxLon - minLon) * largeur);
                    // Inverser l'axe y pour que la latitude élevée soit en haut
                    float y = (float)(hauteur - ((n.Latitude - minLat) / (maxLat - minLat) * hauteur));
                    positions[n.Identifiant] = new SKPoint(x, y);
                }

                // Préparation pour le décalage des liens superposés :
                // Clé : "minID_maxID" (pour une paire de nœuds) ; valeur : nombre total de liens pour cette paire.
                Dictionary<string, int> compteLiens = new Dictionary<string, int>();
                // Clé : même clé, valeur : compteur pour le décalage lors du dessin.
                Dictionary<string, int> indiceActuel = new Dictionary<string, int>();

                // Calcul du nombre de liens par paire
                foreach (var lien in liens)
                {
                    int id1 = lien.NoeudUn.Identifiant;
                    int id2 = lien.NoeudDeux.Identifiant;
                    int mini = Math.Min(id1, id2);
                    int maxi = Math.Max(id1, id2);
                    string cle = mini + "_" + maxi;
                    if (!compteLiens.ContainsKey(cle))
                    {
                        compteLiens[cle] = 0;
                    }
                    compteLiens[cle]++;
                }
                // Initialisation du compteur pour chaque clé
                foreach (var kvp in compteLiens)
                {
                    indiceActuel[kvp.Key] = 0;
                }

                // Dessin des liens avec décalage si nécessaire
                foreach (var lien in liens)
                {
                    int id1 = lien.NoeudUn.Identifiant;
                    int id2 = lien.NoeudDeux.Identifiant;
                    int mini = Math.Min(id1, id2);
                    int maxi = Math.Max(id1, id2);
                    string cle = mini + "_" + maxi;

                    // Récupération des positions initiales
                    SKPoint p1 = positions[id1];
                    SKPoint p2 = positions[id2];

                    // Calcul de l'offset s'il y a plusieurs liens pour cette paire
                    int total = compteLiens[cle];
                    int index = indiceActuel[cle];
                    indiceActuel[cle] = index + 1;
                    float decalageBase = 5f; // espacement de base
                                             // Calcul de la position d'offset (centrer les offsets)
                    float decalageMultiplier = (float)(index - (total - 1) / 2.0);
                    // Calcul d'un vecteur perpendiculaire à la ligne reliant p1 à p2
                    SKPoint diff = new SKPoint(p2.X - p1.X, p2.Y - p1.Y);
                    float longueur = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                    SKPoint perp = new SKPoint(0, 0);
                    if (longueur != 0)
                    {
                        perp = new SKPoint(-diff.Y / longueur, diff.X / longueur);
                    }
                    SKPoint offset = new SKPoint(perp.X * decalageBase * decalageMultiplier, perp.Y * decalageBase * decalageMultiplier);

                    SKPoint nouveauP1 = new SKPoint(p1.X + offset.X, p1.Y + offset.Y);
                    SKPoint nouveauP2 = new SKPoint(p2.X + offset.X, p2.Y + offset.Y);

                    SKColor couleurLigne = CouleurLigne(lien.Ligne);
                    toile.DrawLine(nouveauP1, nouveauP2, new SKPaint
                    {
                        Color = couleurLigne,
                        StrokeWidth = 7
                    });
                }

                // Utilisez un dictionnaire insensible à la casse et normalisez les clés en supprimant les espaces superflus.
                Dictionary<string, int> lignesParStation = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var noeud in noeuds)
                {
                    // On récupère les lignes distinctes pour lesquelles ce nœud est impliqué
                    HashSet<string> lignesDistinctes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var lien in liens)
                    {
                        // Si ce nœud est le noeud de départ ou d'arrivée du lien, on ajoute la ligne
                        if (lien.NoeudUn.Identifiant == noeud.Identifiant || lien.NoeudDeux.Identifiant == noeud.Identifiant)
                        {
                            lignesDistinctes.Add(lien.Ligne);
                        }
                    }
                    // On enregistre le nombre de lignes distinctes pour ce noeud (station)
                    lignesParStation[noeud.Station.Trim()] = lignesDistinctes.Count;
                }


                // Dessin des nœuds
                foreach (var noeud in noeuds)
                {
                    var point = positions[noeud.Identifiant];
                    int rayon;
                    string nomNormalise = noeud.Station.Trim();
                    int nbLignes = 0;
                    if (lignesParStation.ContainsKey(nomNormalise))
                    {
                        nbLignes = lignesParStation[nomNormalise];
                    }
                    switch (nbLignes)
                    {
                        case 1:
                            rayon = 11;
                            break;
                        case 2:
                            rayon = 14;
                            break;
                        case 3:
                            rayon = 17;
                            break;
                        case 4:
                            rayon = 20;
                            break;
                        case 5:
                            rayon = 23;
                            break;
                        default:
                            rayon = 110;
                            break;
                    }
                    toile.DrawCircle(point, rayon, new SKPaint
                    {
                        Color = SKColors.White,
                        Style = SKPaintStyle.Fill
                    });

                    toile.DrawCircle(point, rayon, new SKPaint
                    {
                        Color = SKColors.Black,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1
                    });

                    toile.DrawText(noeud.Station, point.X, point.Y + 3, new SKPaint
                    {
                        Color = SKColors.Black,
                        TextSize = 17,
                        TextAlign = SKTextAlign.Center
                    });
                }

                using (var flux = System.IO.File.OpenWrite("graphe.png"))
                {
                    surface.Snapshot().Encode().SaveTo(flux);
                }
                Console.WriteLine("Le graphe a été dessiné et sauvegardé sous 'graphe.png'.");
            }
        }



        /// <summary>
        /// Retourne la couleur associée à une ligne de métro
        /// </summary>
        private SKColor CouleurLigne(string ligne)
        {
            switch (ligne)
            {
                case "1": return SKColors.Yellow;
                case "2": return SKColors.DarkBlue;
                case "3": return SKColors.Brown;
                case "3bis": return SKColors.Blue;
                case "4": return SKColors.Fuchsia;
                case "5": return SKColors.Orange;
                case "6": return SKColors.LightGreen;
                case "7": return SKColors.Pink;
                case "7bis": return SKColors.Cyan;
                case "8": return SKColors.MediumOrchid;
                case "9": return SKColors.OliveDrab;
                case "10": return SKColors.Goldenrod;
                case "11": return SKColors.SaddleBrown;
                case "12": return SKColors.ForestGreen;
                case "13": return SKColors.SkyBlue;
                case "14": return SKColors.Violet;
                default: return SKColors.Gray;
            }
        }

        private Tuple<int[], int[], string[]> CalculerDijkstra(string depart)
        {
            int[] distances = new int[noeuds.Count];
            bool[] visite = new bool[noeuds.Count];
            int[] precedent = new int[noeuds.Count];
            string[] ligneUtilisee = new string[noeuds.Count];

            for (int i = 0; i < noeuds.Count; i++)
            {
                distances[i] = 10000;
                visite[i] = false;
                precedent[i] = -1;
                ligneUtilisee[i] = "";
            }

            int id = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(depart, StringComparison.OrdinalIgnoreCase))
                {
                    id = i;
                }
            }
            if (id == -1)
            {
                throw new Exception("Station de départ introuvable : " + depart);
            }
            distances[id] = 0;
            ligneUtilisee[id] = "";

            for (int i = 0; i < noeuds.Count; i++)
            {
                int a = -1;
                int minimum = 10000;
                for (int j = 0; j < noeuds.Count; j++)
                {
                    if (visite[j] == false)
                    {
                        if (distances[j] < minimum)
                        {
                            minimum = distances[j];
                            a = j;
                        }
                    }
                }
                if (a != -1)
                {
                    visite[a] = true;
                    for (int k = 0; k < liens.Count; k++)
                    {
                        Lien lienCourant = liens[k];
                        int indice1 = lienCourant.NoeudUn.Identifiant - 1;
                        int indice2 = lienCourant.NoeudDeux.Identifiant - 1;
                        if (indice1 == a || indice2 == a)
                        {
                            int v = (indice1 == a) ? indice2 : indice1;
                            if (!visite[v] && distances[a] != 10000)
                            {
                                string ligneCourante = "";
                                if (lienCourant != null)
                                {
                                    ligneCourante = lienCourant.Ligne;
                                }
                                int coutChangement = 0;
                                if (a != id)
                                {
                                    if (ligneUtilisee[a] != "" && !ligneUtilisee[a].Equals(ligneCourante, StringComparison.OrdinalIgnoreCase))
                                    {
                                        coutChangement = 8;
                                    }
                                }
                                int nouvelleDistance = distances[a] + lienCourant.Ponderation + coutChangement;
                                if (nouvelleDistance < distances[v])
                                {
                                    distances[v] = nouvelleDistance;
                                    precedent[v] = a;
                                    ligneUtilisee[v] = ligneCourante;
                                }
                            }
                        }
                    }
                }
            }
            return Tuple.Create(distances, precedent, ligneUtilisee);
        }
        /// <summary>
        /// Récupère le plus court chemin entre deux stations à l’aide de l’algorithme de Dijkstra.
        /// </summary>
        /// <param name="depart">Nom de la station de départ.</param>
        /// <param name="arrivee">Nom de la station d’arrivée.</param>
        /// <returns>Liste représentant le chemin avec les changements de ligne.</returns>

        public List<string> DijkstraChemin(string depart, string arrivee)
        {
            Tuple<int[], int[], string[]> res = CalculerDijkstra(depart);
            int[] distances = res.Item1;
            int[] precedent = res.Item2;
            string[] ligneUtilisee = res.Item3;

            int id = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(arrivee, StringComparison.OrdinalIgnoreCase))
                {
                    id = i;
                }
            }
            if (id == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + arrivee);
            }
            if (distances[id] == 10000)
            {
                return new List<string>();
            }

            List<int> idChemin = new List<int>();
            int idActuel = id;
            for (int i = 0; i < noeuds.Count; i++)
            {
                idChemin.Add(idActuel);
                if (precedent[idActuel] == -1)
                {
                    break; 
                }
                else
                {
                    idActuel = precedent[idActuel];
                }
            }

            int c = idChemin.Count;
            for (int i = 0; i < c / 2; i++)
            {
                int temp = idChemin[i];
                idChemin[i] = idChemin[c - i - 1];
                idChemin[c - i - 1] = temp;
            }

            List<string> chemin = new List<string>();
            chemin.Add(noeuds[idChemin[0]].Station);
            for (int i = 0; i < idChemin.Count - 1; i++)
            {
                int indiceDepart = idChemin[i];
                int indiceArrivee = idChemin[i + 1];
                string ligneSegment = ligneUtilisee[indiceArrivee];
                string segment = "-> (Ligne : " + ligneSegment + ") " + noeuds[indiceArrivee].Station;
                chemin.Add(segment);
            }
            return chemin;
        }
        /// <summary>
        /// Calcule le coût du chemin le plus court entre deux stations avec Dijkstra.
        /// </summary>
        /// <param name="depart">Station de départ.</param>
        /// <param name="arrivee">Station d’arrivée.</param>
        /// <returns>Le coût total du trajet.</returns>

        public int DijkstraCout(string depart, string arrivee)
        {
            Tuple<int[], int[], string[]> resultat = CalculerDijkstra(depart);
            int[] distances = resultat.Item1;
            int id = -1;

            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(arrivee, StringComparison.OrdinalIgnoreCase))
                {
                    id = i;
                }
            }
            if (id == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + arrivee);
            }
            return distances[id];
        }
        /// <summary>
        /// Calcule les plus courts chemins depuis une station source à l’aide de l’algorithme de Bellman-Ford.
        /// </summary>
        /// <param name="stationDepart">Nom de la station de départ.</param>
        /// <returns>Un tuple avec les distances, les prédécesseurs, et les lignes utilisées.</returns>

        private Tuple<int[], int[], string[]> CalculerBellmanFord(string stationDepart)
        {
            int[] distances = new int[noeuds.Count];
            int[] predecesseurs = new int[noeuds.Count];
            string[] lignesUtilisees = new string[noeuds.Count];

            for (int i = 0; i < noeuds.Count; i++)
            {
                distances[i] = int.MaxValue;
                predecesseurs[i] = -1;
                lignesUtilisees[i] = "";
            }

            int source = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationDepart, StringComparison.OrdinalIgnoreCase))
                {
                    source = i;
                }
            }
            if (source == -1)
            {
                throw new Exception("Station de départ introuvable : " + stationDepart);
            }
            distances[source] = 0;
            lignesUtilisees[source] = "";

            for (int i = 0; i < noeuds.Count - 1; i++)
            {
                for (int k = 0; k < liens.Count; k++)
                {
                    Lien lienCourant = liens[k];
                    int idxA = lienCourant.NoeudUn.Identifiant - 1;
                    int idxB = lienCourant.NoeudDeux.Identifiant - 1;

                    if (distances[idxA] != int.MaxValue)
                    {
                        int coutChangement = 0;
                        if (idxA != source)
                        {
                            if (lignesUtilisees[idxA] != "" && !lignesUtilisees[idxA].Equals(lienCourant.Ligne, StringComparison.OrdinalIgnoreCase))
                            {
                                coutChangement = 8;
                            }
                        }
                        int nouvelleDistance = distances[idxA] + lienCourant.Ponderation + coutChangement;
                        if (nouvelleDistance < distances[idxB])
                        {
                            distances[idxB] = nouvelleDistance;
                            predecesseurs[idxB] = idxA;
                            lignesUtilisees[idxB] = lienCourant.Ligne;
                        }
                    }

                    if (distances[idxB] != int.MaxValue)
                    {
                        int coutChangement = 0;
                        if (idxB != source)
                        {
                            if (lignesUtilisees[idxB] != "" && !lignesUtilisees[idxB].Equals(lienCourant.Ligne, StringComparison.OrdinalIgnoreCase))
                            {
                                coutChangement = 8;
                            }
                        }
                        int nouvelleDistance = distances[idxB] + lienCourant.Ponderation + coutChangement;
                        if (nouvelleDistance < distances[idxA])
                        {
                            distances[idxA] = nouvelleDistance;
                            predecesseurs[idxA] = idxB;
                            lignesUtilisees[idxA] = lienCourant.Ligne;
                        }
                    }
                }
            }
            return Tuple.Create(distances, predecesseurs, lignesUtilisees);
        }
        /// <summary>
        /// Retourne le plus court chemin entre deux stations selon Bellman-Ford.
        /// </summary>
        /// <param name="stationDepart">Nom de la station de départ.</param>
        /// <param name="stationArrivee">Nom de la station d’arrivée.</param>
        /// <returns>Liste des stations traversées avec les lignes empruntées.</returns>

        public List<string> BellmanFordChemin(string stationDepart, string stationArrivee)
        {
            Tuple<int[], int[], string[]> resultat = CalculerBellmanFord(stationDepart);
            int[] distances = resultat.Item1;
            int[] predecesseurs = resultat.Item2;
            string[] lignesUtilisees = resultat.Item3;

            int indiceArrivee = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationArrivee, StringComparison.OrdinalIgnoreCase))
                {
                    indiceArrivee = i;
                }
            }
            if (indiceArrivee == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + stationArrivee);
            }
            if (distances[indiceArrivee] == int.MaxValue)
            {
                return new List<string>(); 
            }

            List<int> indicesChemin = new List<int>();
            int courant = indiceArrivee;
            for (int i = 0; i < noeuds.Count; i++)
            {
                indicesChemin.Add(courant);
                if (predecesseurs[courant] == -1)
                {
                    break;
                }
                else
                {
                    courant = predecesseurs[courant];
                }
            }
            int compteur = indicesChemin.Count;
            for (int i = 0; i < compteur / 2; i++)
            {
                int temp = indicesChemin[i];
                indicesChemin[i] = indicesChemin[compteur - i - 1];
                indicesChemin[compteur - i - 1] = temp;
            }

            List<string> chemin = new List<string>();
            chemin.Add(noeuds[indicesChemin[0]].Station);
            for (int i = 0; i < indicesChemin.Count - 1; i++)
            {
                int indiceDep = indicesChemin[i];
                int indiceArr = indicesChemin[i + 1];
                string segment = "-> (Ligne : " + lignesUtilisees[indiceArr] + ") " + noeuds[indiceArr].Station;
                chemin.Add(segment);
            }
            return chemin;
        }
        /// <summary>
        /// Retourne le coût du trajet le plus court selon Bellman-Ford.
        /// </summary>
        /// <param name="stationDepart">Nom de la station de départ.</param>
        /// <param name="stationArrivee">Nom de la station d’arrivée.</param>
        /// <returns>Le coût minimal du trajet.</returns>

        public int BellmanFordCout(string stationDepart, string stationArrivee)
        {
            Tuple<int[], int[], string[]> resultat = CalculerBellmanFord(stationDepart);
            int[] distances = resultat.Item1;
            int indiceArrivee = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationArrivee, StringComparison.OrdinalIgnoreCase))
                {
                    indiceArrivee = i;
                }
            }
            if (indiceArrivee == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + stationArrivee);
            }
            return distances[indiceArrivee];
        }
        /// <summary>
        /// Calcule toutes les distances minimales entre toutes les paires de stations avec Floyd-Warshall.
        /// </summary>
        /// <returns>Un tuple contenant la matrice de distances et la matrice de précédents.</returns>

        private Tuple<int[,], int[,]> CalculerFloydWarshall()
        {
            int min = 10000;
            int[,] distance = new int[noeuds.Count, noeuds.Count];
            int[,] precedent = new int[noeuds.Count, noeuds.Count];

            for (int i = 0; i < noeuds.Count; i++)
            {
                for (int j = 0; j < noeuds.Count; j++)
                {
                    if (i == j)
                    {
                        distance[i, j] = 0;
                        precedent[i, j] = -1;
                    }
                    else if (matriceAdjacence[i, j] != 0)
                    {
                        distance[i, j] = matriceAdjacence[i, j];
                        precedent[i, j] = i;
                    }
                    else
                    {
                        distance[i, j] = min;
                        precedent[i, j] = -1;
                    }
                }
            }

            for (int k = 0; k < noeuds.Count; k++)
            {
                for (int i = 0; i < noeuds.Count; i++)
                {
                    for (int j = 0; j < noeuds.Count; j++)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            precedent[i, j] = precedent[k, j];
                        }
                    }
                }
            }
            return Tuple.Create(distance, precedent);
        }
        /// <summary>
        /// Donne le chemin optimal entre deux stations selon Floyd-Warshall.
        /// </summary>
        /// <param name="stationDepart">Station de départ.</param>
        /// <param name="stationArrivee">Station d’arrivée.</param>
        /// <returns>Liste des stations à parcourir, avec indication des lignes.</returns>

        public List<string> FloydChemin(string stationDepart, string stationArrivee)
        {
            Tuple<int[,], int[,]> res = CalculerFloydWarshall();
            int[,] distance = res.Item1;
            int[,] precedent = res.Item2;
            int min = 10000;

            int indiceDepart = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationDepart, StringComparison.OrdinalIgnoreCase))
                {
                    indiceDepart = i;
                }
            }
            if (indiceDepart == -1)
            {
                throw new Exception("Station de départ introuvable : " + stationDepart);
            }

            int indiceArrivee = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationArrivee, StringComparison.OrdinalIgnoreCase))
                {
                    indiceArrivee = i;
                }
            }
            if (indiceArrivee == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + stationArrivee);
            }
            if (distance[indiceDepart, indiceArrivee] >= min)
            {
                return new List<string>(); 
            }

            List<int> indicesChemin = new List<int>();
            int actuel = indiceArrivee;
            for (int i = 0; i < noeuds.Count; i++)
            {
                indicesChemin.Add(actuel);
                if (precedent[indiceDepart, actuel] == -1)
                {
                    break;
                }
                else
                {
                    actuel = precedent[indiceDepart, actuel];
                }
            }

            int compteur = indicesChemin.Count;
            for (int i = 0; i < compteur / 2; i++)
            {
                int temp = indicesChemin[i];
                indicesChemin[i] = indicesChemin[compteur - i - 1];
                indicesChemin[compteur - i - 1] = temp;
            }

            List<string> chemin = new List<string>();
            chemin.Add(noeuds[indicesChemin[0]].Station);
            for (int i = 0; i < indicesChemin.Count - 1; i++)
            {
                int dep = indicesChemin[i];
                int arr = indicesChemin[i + 1];
                string ligneSegment = "";
                for (int j = 0; j < liens.Count; j++)
                {
                    int idx1 = liens[j].NoeudUn.Identifiant - 1;
                    int idx2 = liens[j].NoeudDeux.Identifiant - 1;
                    if ((idx1 == dep && idx2 == arr) || (idx1 == arr && idx2 == dep))
                    {
                        ligneSegment = liens[j].Ligne;
                        j = liens.Count;
                    }
                }
                string seg = "-> (Ligne: " + ligneSegment + ") " + noeuds[arr].Station;
                chemin.Add(seg);
            }
            return chemin;
        }
        /// <summary>
        /// Retourne le coût minimal entre deux stations en utilisant l’algorithme de Floyd-Warshall.
        /// </summary>
        /// <param name="stationDepart">Station de départ.</param>
        /// <param name="stationArrivee">Station d’arrivée.</param>
        /// <returns>Coût minimal entre les deux stations.</returns>

        public int FloydCout(string stationDepart, string stationArrivee)
        {
            Tuple<int[,], int[,]> res = CalculerFloydWarshall();
            int[,] distance = res.Item1;
            int min = 10000;

            int indiceDepart = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationDepart, StringComparison.OrdinalIgnoreCase))
                {
                    indiceDepart = i;
                }
            }
            if (indiceDepart == -1)
            {
                throw new Exception("Station de départ introuvable : " + stationDepart);
            }

            int indiceArrivee = -1;
            for (int i = 0; i < noeuds.Count; i++)
            {
                if (noeuds[i].Station.Equals(stationArrivee, StringComparison.OrdinalIgnoreCase))
                {
                    indiceArrivee = i;
                }
            }
            if (indiceArrivee == -1)
            {
                throw new Exception("Station d'arrivée introuvable : " + stationArrivee);
            }
            return distance[indiceDepart, indiceArrivee];
        }
    }


}
