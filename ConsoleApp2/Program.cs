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
            var relations = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(2, 1), new Tuple<int, int>(3, 1), new Tuple<int, int>(4, 1),
                new Tuple<int, int>(5, 1), new Tuple<int, int>(6, 1), new Tuple<int, int>(7, 1),
                new Tuple<int, int>(8, 1), new Tuple<int, int>(9, 1), new Tuple<int, int>(11, 1),
                new Tuple<int, int>(12, 1), new Tuple<int, int>(13, 1), new Tuple<int, int>(14, 1),
                new Tuple<int, int>(18, 1), new Tuple<int, int>(20, 1), new Tuple<int, int>(22, 1),
                new Tuple<int, int>(32, 1), new Tuple<int, int>(3, 2), new Tuple<int, int>(4, 2),
                new Tuple<int, int>(8, 2), new Tuple<int, int>(14, 2), new Tuple<int, int>(18, 2),
                new Tuple<int, int>(20, 2), new Tuple<int, int>(22, 2), new Tuple<int, int>(31, 2),
                new Tuple<int, int>(4, 3), new Tuple<int, int>(8, 3), new Tuple<int, int>(9, 3),
                new Tuple<int, int>(10, 3), new Tuple<int, int>(14, 3), new Tuple<int, int>(28, 3),
                new Tuple<int, int>(29, 3), new Tuple<int, int>(33, 3), new Tuple<int, int>(8, 4),
                new Tuple<int, int>(13, 4), new Tuple<int, int>(14, 4), new Tuple<int, int>(7, 5),
                new Tuple<int, int>(11, 5), new Tuple<int, int>(7, 6), new Tuple<int, int>(11, 6),
                new Tuple<int, int>(17, 6), new Tuple<int, int>(17, 7), new Tuple<int, int>(31, 9),
                new Tuple<int, int>(33, 9), new Tuple<int, int>(34, 9), new Tuple<int, int>(34, 10),
                new Tuple<int, int>(34, 14), new Tuple<int, int>(33, 15), new Tuple<int, int>(34, 15),
                new Tuple<int, int>(33, 16), new Tuple<int, int>(34, 16), new Tuple<int, int>(33, 19),
                new Tuple<int, int>(34, 19), new Tuple<int, int>(34, 20), new Tuple<int, int>(33, 21),
                new Tuple<int, int>(34, 21), new Tuple<int, int>(33, 23), new Tuple<int, int>(34, 23),
                new Tuple<int, int>(26, 24), new Tuple<int, int>(28, 24), new Tuple<int, int>(30, 24),
                new Tuple<int, int>(33, 24), new Tuple<int, int>(34, 24), new Tuple<int, int>(26, 25),
                new Tuple<int, int>(28, 25), new Tuple<int, int>(32, 25), new Tuple<int, int>(32, 26),
                new Tuple<int, int>(30, 27), new Tuple<int, int>(34, 27), new Tuple<int, int>(34, 28),
                new Tuple<int, int>(32, 29), new Tuple<int, int>(34, 29), new Tuple<int, int>(33, 30),
                new Tuple<int, int>(34, 30), new Tuple<int, int>(33, 31), new Tuple<int, int>(34, 31),
                new Tuple<int, int>(33, 32), new Tuple<int, int>(34, 32), new Tuple<int, int>(34, 33)
            };

            var listeAdjacence = new Dictionary<int, List<int>>();

            foreach (var relation in relations)
            {
                if (!listeAdjacence.ContainsKey(relation.Item1))
                {
                    listeAdjacence[relation.Item1] = new List<int>();
                }
                if (!listeAdjacence.ContainsKey(relation.Item2))
                {
                    listeAdjacence[relation.Item2] = new List<int>();
                }

                listeAdjacence[relation.Item1].Add(relation.Item2);
                listeAdjacence[relation.Item2].Add(relation.Item1); // Relation réciproque
            }

            // Dessiner le graphe avec SkiaSharp
            var width = 1800;
            var height = 1800;
            using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                var positions = new Dictionary<int, SKPoint>();

                // Calculer les positions des nœuds sur un cercle
                float centerX = width / 2;
                float centerY = height / 2;
                float radius = 700; // Rayon du cercle
                int nodeCount = listeAdjacence.Count;

                // Calculer un angle de séparation entre chaque nœud
                float angleStep = 360f / nodeCount;

                // Placer les nœuds en cercle
                List<int> noeudsList = new List<int>(listeAdjacence.Keys);
                for (int i = 0; i < nodeCount; i++)
                {
                    float angle = angleStep * i;
                    float x = centerX + radius * (float)Math.Cos(angle * Math.PI / 180);
                    float y = centerY + radius * (float)Math.Sin(angle * Math.PI / 180);
                    positions[noeudsList[i]] = new SKPoint(x, y);
                }


                // Dessiner les arêtes
                foreach (var relation in relations)
                {
                    var point1 = positions[relation.Item1];
                    var point2 = positions[relation.Item2];

                    canvas.DrawLine(point1, point2, new SKPaint
                    {
                        Color = SKColors.Black,
                        StrokeWidth = 2
                    });
                }

                // Dessiner les nœuds en cercle
                foreach (var noeud in listeAdjacence.Keys)
                {
                    var point = positions[noeud];
                    canvas.DrawCircle(point, 20, new SKPaint
                    {
                        Color = SKColors.Black,
                        Style = SKPaintStyle.Fill
                    });
                    canvas.DrawText(noeud.ToString(), point.X - 2, point.Y + 9, new SKPaint
                    {
                        Color = SKColors.White,
                        TextSize = 20,
                        TextAlign = SKTextAlign.Center
                    });
                }


                // Sauvegarder l'image
                using (var stream = System.IO.File.OpenWrite("graphe.png"))
                {
                    surface.Snapshot().Encode().SaveTo(stream);
                }

                Console.WriteLine("Le graphe a été dessiné et sauvegardé sous 'graphe.png'.");
            }

            // Ouvrir l'image générée
            Process.Start("open", "graphe.png");
        }
    }
}
