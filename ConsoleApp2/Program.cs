using System;
using System.Collections.Generic;
using System.Diagnostics;
using Karaté.Karaté;
using SkiaSharp;
using System.Data;
using System.IO;
using System.Text.Json;
using MySql.Data.MySqlClient;


namespace Karaté
{
    public class Program
    {
        public static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== LIV'IN PARIS ===");
            Console.WriteLine("1. Partie PCC / graphe");
            Console.WriteLine("2. Partie BDD");
            string res = Console.ReadLine();
            while (res != "1" && res != "2")
            {
                Console.WriteLine("Saisie incorrect");
                res = Console.ReadLine();
            }
            if (res.ToLower() == "1")
            {
                bool continu = true;
                while (continu)
                {
                    Console.Clear();
                    Console.WriteLine("Veuillez choisir une option :");
                    Console.WriteLine("1: Dessiner le graphe");
                    Console.WriteLine("2: Afficher les propriétés du graphe");
                    Console.WriteLine("3: Afficher la matrice d'adjacence");
                    Console.WriteLine("4: Afficher la liste d'adjacence");
                    Console.WriteLine("5: Effectuer un parcours BFS");
                    Console.WriteLine("6: Effectuer un parcours DFS");
                    Console.WriteLine("7: Vérifier la connexité du graphe");
                    Console.WriteLine("8: Afficher les cycles du graphe");
                    Console.WriteLine("9: Tester Dijkstra (chemin le plus court entre deux stations)");
                    Console.WriteLine("10: Tester coloration de graphe");
                    Console.WriteLine("0: Quitter");
                    Console.Write("Votre choix : ");
                    string choix = Console.ReadLine();
                    Console.Clear();

                    Graphe graphe = new Graphe();

                    switch (choix)
                    {
                        case "1":
                            graphe.DessinerGraphe();
                            break;
                        case "2":
                            graphe.Propriété();
                            break;
                        case "3":
                            AfficherMatriceAdj(graphe.MatriceAdjacence);
                            break;
                        case "4":
                            AfficherListeAdjNom(graphe.ListeAdjacenceNom);
                            break;
                        case "5":
                            AfficherBFS(graphe);
                            break;
                        case "6":
                            AfficherDFS(graphe);
                            break;
                        case "7":
                            Connexite(graphe);
                            break;
                        case "8":
                            AfficherCycles(graphe);
                            break;
                        case "9":
                            TesterDijkstra(graphe);
                            break;
                        case "10":
                            GrapheCC grapheCC = new GrapheCC();
                            grapheCC.ExecuterColoration();
                            break;
                        case "11":
                            TestXMLJSON();
                        break;
                        case "0":
                            continu = false;
                            break;
                        default:
                            Console.WriteLine("Choix non reconnu, veuillez réessayer.");
                            break;
                    }
                    Console.WriteLine("\nTapez 'q' pour revenir au menu principal...");
                    while (Console.ReadLine().ToLower() != "q")
                    {
                        Console.WriteLine("Tapez 'q' pour revenir au menu principal...");
                    }
                }
            }
            else if (res.ToLower() == "2")
            {
                Console.Title = "Liv'in Paris - Application de partage de repas";
                try
                {
                    DbAccess db = new DbAccess();
                    try
                    {
                        var test = db.ExecuterRequete("SELECT COUNT(*) FROM Utilisateur");
                    }
                    catch
                    {
                        string scriptCreationTable = @"
                            CREATE TABLE IF NOT EXISTS Utilisateur (
                                id_utilisateur INT PRIMARY KEY,
                                nom_utilisateur VARCHAR(100) NOT NULL UNIQUE,
                                mot_de_passe VARCHAR(100) NOT NULL,
                                role VARCHAR(20) NOT NULL,
                                id_reference INT
                            );
                            
                            -- Insertion d'un administrateur par défaut
                            INSERT INTO Utilisateur (id_utilisateur, nom_utilisateur, mot_de_passe, role, id_reference) 
                            VALUES (1, 'admin', 'admin123', 'Admin', NULL);
                        ";

                        db.ExecuterRequeteMAJ(scriptCreationTable);
                        Console.WriteLine("Table Utilisateur créée avec succès avec un administrateur par défaut.");
                        Console.WriteLine("Identifiants admin: 'admin' / 'admin123'");
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }

                    UserInterface ui = new UserInterface();
                    ui.Run();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Erreur fatale: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ResetColor();
                    Console.WriteLine("\nAppuyez sur une touche pour quitter...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Affiche la matrice d'adjacence
        /// </summary>
        /// <param name="matriceAdj">matrice d'adjacence a afficher</param>
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

        /// <summary>
        /// Affiche la liste d'adjacence 
        /// </summary>
        /// <param name="listeAdj">Liste d'adjacence a afficher</param>
        static void AfficherListeAdjNom(Dictionary<string, List<string>> listeAdj)
        {
            Console.WriteLine("Liste d'adjacence :");
            foreach (var noeud in listeAdj)
            {
                if (noeud.Value.Count >= 3)
                {
                    Console.Write(noeud.Key + ": ");
                    foreach (var autreNoeud in noeud.Value)
                    {
                        Console.Write(autreNoeud + " ");
                    }
                    Console.WriteLine();
                }
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

        /// <summary>
        /// Affiche le résultat du parcours en largeur 
        /// </summary>
        /// <param name="graphe">Le graphe a traiter</param>
        static void AfficherBFS(Graphe graphe)
        {
            Random r = new Random();
            int aleatoire = r.Next(graphe.Noeuds.Count);
            int debut = graphe.Noeuds[aleatoire].Identifiant;
            string res = graphe.BFS(debut);
            Console.WriteLine("Parcours BFS a partir du noeud " + debut + ": " + res);
        }

        /// <summary>
        /// Affiche le résultat du parcours en profondeur
        /// </summary>
        /// <param name="graphe">Le graphe a traiter</param>
        static void AfficherDFS(Graphe graphe)
        {
            Random r = new Random();
            int aleatoire = r.Next(graphe.Noeuds.Count);
            int debut = graphe.Noeuds[aleatoire].Identifiant;
            string res = graphe.DFS(debut);
            Console.WriteLine("Parcours DFS à partir du noeud " + debut + ": " + res);
        }

        /// <summary>
        /// Affiche si le graphe est connexe ou non
        /// </summary>
        /// <param name="graphe">Le graphe à traiter</param>
        static void Connexite(Graphe graphe)
        {
            bool connexe = graphe.Connexe();
            if (connexe == true)
            {
                Console.WriteLine("Le graphe est connexe");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe");
            }
        }

        /// <summary>
        /// Affiche tous les cycles du graphe
        /// </summary>
        /// <param name="graphe">Le graphe a traiter</param>
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
            Console.WriteLine("Nombre de cycles détectes : " + cycles.Count);
        }

        static void TesterDijkstra(Graphe graphe)
        {
            Console.Write("Entrez la station de départ : ");
            string depart = Console.ReadLine();
            Console.Write("Entrez la station d'arrivée : ");
            string arrivee = Console.ReadLine();
            try
            {
                List<string> chemin = graphe.DijkstraChemin(depart, arrivee);
                int cout = graphe.DijkstraCout(depart, arrivee);
                if (chemin.Count == 0)
                {
                    Console.WriteLine("Aucun chemin trouvé entre " + depart + " et " + arrivee);
                }
                else
                {
                    Console.WriteLine("Chemin le plus court entre " + depart + " et " + arrivee + " :");
                    foreach (string segment in chemin)
                    {
                        Console.WriteLine(segment);
                    }
                    Console.WriteLine("Coût total : " + cout);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// <summary>
        /// Récupère le résultat de la requête SQL dans un DataTable, l’enrobe dans un DataSet 
        /// et écrit directement un fichier XML.
        /// </summary>
        static void ExportToXml(MySqlConnection conn, string sql, string filePath)
        {
            using var cmd = new MySqlCommand(sql, conn);
            using var da = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            var ds = new DataSet(Path.GetFileNameWithoutExtension(filePath));
            ds.Tables.Add(dt);
            ds.WriteXml(filePath);

            Console.WriteLine($"[XML] {filePath} généré.");
        }

        /// <summary>
        /// Lit la requête SQL avec un DataReader et écrit au fur et à mesure un tableau JSON.
        /// </summary>
        static void ExportToJson(MySqlConnection conn, string sql, string filePath)
        {
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            using var fs = File.Create(filePath);
            using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true });

            writer.WriteStartArray();
            while (reader.Read())
            {
                writer.WriteStartObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    writer.WritePropertyName(reader.GetName(i));
                    var val = reader.GetValue(i);
                    JsonSerializer.Serialize(writer, val, val?.GetType() ?? typeof(object));
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();

            Console.WriteLine($"[JSON] {filePath} généré.");
        }

        static void TestXMLJSON()
        {
            string connStr = "Server=localhost;Port=3306;Database=Liv'in Paris;Uid=root;Pwd=password;CharSet=utf8;";
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // XML
            ExportToXml(conn, "SELECT * FROM Client", "export_clients.xml");
            ExportToXml(conn, "SELECT * FROM Cuisinier", "export_cuisiniers.xml");
            ExportToXml(conn, "SELECT * FROM Commande", "export_commandes.xml");

            // JSON
            ExportToJson(conn, "SELECT * FROM Client", "export_clients.json");
            ExportToJson(conn, "SELECT * FROM Cuisinier", "export_cuisiniers.json");
            ExportToJson(conn, "SELECT * FROM Commande", "export_commandes.json");

            conn.Close();
            // **** FIN EXPORTS ****

            Console.WriteLine("Tous les exports sont terminés.");
            Console.ReadKey();
        }

    }
}