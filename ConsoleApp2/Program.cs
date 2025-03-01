using System;
using System.Collections.Generic;
using System.Diagnostics;
using SkiaSharp;

namespace Karaté
{
    public class Program
    {
        public static void Main()
        {
            Graphe graphe = new Graphe();

            static void AfficherMatriceAdj(int[,] matriceAdj)
            {
                Console.WriteLine("Matrice d'adjacence :");
                for (int i = 0; i < matriceAdj.GetLength(0); i++)
                {
                    for (int j = 0; j < matriceAdj.GetLength(1); j++)
                    {
                        Console.Write(matriceAdj[i, j] + " ");
                    }
                    Console.WriteLine();
                }
            }

            static void AfficherListeAdj(Dictionary<int, List<int>> listeAdj)
            {
                Console.WriteLine("Liste d'adjacence :");
                foreach (var noeud in listeAdj)
                {
                    Console.Write(noeud.Key + ": ");
                    foreach (var autreNoeud in noeud.Value)
                    {
                        Console.Write(autreNoeud + " ");
                    }
                    Console.WriteLine();
                }
            }

            static void AfficherBFS(Graphe graphe)
            {
                Random r = new Random();
                int aleatoire = r.Next(graphe.Noeuds.Count); 
                int debut = graphe.Noeuds[aleatoire].Identifiant; 
                string res = graphe.BFS(debut);
                Console.WriteLine("Parcours BFS à partir du nœud " + debut + ": " + res);
            }

            static void AfficherDFS(Graphe graphe)
            {
                Random r = new Random();
                int aleatoire = r.Next(graphe.Noeuds.Count);
                int debut = graphe.Noeuds[aleatoire].Identifiant;
                string res = graphe.DFS(debut);
                Console.WriteLine("Parcours DFS à partir du nœud " + debut + ": " + res);
            }

            static void AfficherCycles(Graphe graphe)
            {
                var cycles = graphe.Cycles();
                Console.WriteLine("Cycles dans le graphe :");
                foreach (var cycle in cycles)
                {
                    for (int i = 0; i < cycle.Count; i++)
                    {
                        Console.Write(cycle[i]); 
                        Console.Write(" "); 
                    }
                    Console.WriteLine(); 
                }

                Console.WriteLine("Nombre de cycles détectés : " + cycles.Count);
            }

            graphe.DessinerGraphe();
            Console.WriteLine();
            graphe.Propriété();
            Console.WriteLine();
            AfficherMatriceAdj(graphe.MatriceAdjacence);
            Console.WriteLine();
            AfficherListeAdj(graphe.ListeAdjacence);
            Console.WriteLine();
            AfficherBFS(graphe);
            Console.WriteLine();
            AfficherDFS(graphe);
            Console.WriteLine();
            AfficherCycles(graphe);
        }
    }
}
