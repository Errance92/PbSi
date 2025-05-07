using System;
using System.Collections.Generic;
using System.Linq;

public class StatistiquesModule
{
    private readonly DbAccess db;

    public StatistiquesModule()
    {
        db = new DbAccess();
    }

    public void Run()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== MODULE STATISTIQUES ===");
            Console.ResetColor();
            Console.WriteLine("1. Chiffre d'affaires par mois");
            Console.WriteLine("2. Clients fréquents");
            Console.WriteLine("3. Plats populaires");
            Console.WriteLine("4. Cuisiniers avec plats exclusifs");
            Console.WriteLine("5. Clients sans commande");
            Console.WriteLine("6. Temps moyen de préparation par cuisinier");
            Console.WriteLine("7. Nombre de clients par métro");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");

            string choix = Console.ReadLine();
            Console.Clear();

            switch (choix)
            {
                case "1": AfficherChiffreAffairesParMois(); break;
                case "2": AfficherClientsFrequents(); break;
                case "3": AfficherPlatsPopulaires(); break;
                case "4": AfficherCuisiniersPlatsExclusifs(); break;
                case "5": AfficherClientsSansCommande(); break;
                case "6": AfficherTempsMoyenPreparation(); break;
                case "7": AfficherNombreClientsParMetro(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }

    private void AfficherChiffreAffairesParMois()
    {
        Console.WriteLine("=== CHIFFRE D'AFFAIRES PAR MOIS ===\n");

        Console.Write("Entrez l'année (AAAA): ");
        if (!int.TryParse(Console.ReadLine(), out int annee))
        {
            Console.WriteLine("Année invalide.");
            WaitForKey();
            return;
        }

        Dictionary<int, double> resultats = db.ObtenirChiffreAffairesParMois(annee);

        if (resultats.Count == 0)
        {
            Console.WriteLine($"Aucune donnée disponible pour l'année {annee}.");
        }
        else
        {
            Console.WriteLine($"\nChiffre d'affaires mensuel pour {annee}:");
            Console.WriteLine("--------------------------------------");
            string[] mois = { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };

            double total = 0;
            foreach (var kvp in resultats)
            {
                Console.WriteLine($"{mois[kvp.Key - 1]}: {kvp.Value:N2} €");
                total += kvp.Value;
            }

            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Total: {total:N2} €");
        }

        WaitForKey();
    }

    private void AfficherClientsFrequents()
    {
        Console.WriteLine("=== CLIENTS FRÉQUENTS ===\n");

        Console.Write("Nombre minimum de commandes: ");
        if (!int.TryParse(Console.ReadLine(), out int minCommandes) || minCommandes < 1)
        {
            Console.WriteLine("Nombre invalide.");
            WaitForKey();
            return;
        }

        Dictionary<int, int> clientsFrequents = db.ObtenirClientsFrequents(minCommandes);

        if (clientsFrequents.Count == 0)
        {
            Console.WriteLine($"Aucun client n'a passé {minCommandes} commandes ou plus.");
        }
        else
        {
            Console.WriteLine($"\nClients ayant passé au moins {minCommandes} commandes:");
            Console.WriteLine("----------------------------------------------");

            foreach (var kvp in clientsFrequents)
            {
                Client client = db.ObtenirClientID(kvp.Key);
                if (client != null)
                {
                    Console.WriteLine($"{client.Nom} {client.Prenom}: {kvp.Value} commandes - Montant total: {client.MontantAchat:N2} €");
                }
            }
        }

        WaitForKey();
    }

    private void AfficherPlatsPopulaires()
    {
        Console.WriteLine("=== PLATS POPULAIRES ===\n");

        Console.Write("Nombre minimum de commandes: ");
        if (!int.TryParse(Console.ReadLine(), out int minCommandes) || minCommandes < 1)
        {
            Console.WriteLine("Nombre invalide.");
            WaitForKey();
            return;
        }

        List<Tuple<int, string, int>> platsPopulaires = db.ObtenirPlatsPopulaires(minCommandes);

        if (platsPopulaires.Count == 0)
        {
            Console.WriteLine($"Aucun plat n'a été commandé {minCommandes} fois ou plus.");
        }
        else
        {
            Console.WriteLine($"\nPlats commandés au moins {minCommandes} fois:");
            Console.WriteLine("---------------------------------------------");

            foreach (var plat in platsPopulaires)
            {
                Console.WriteLine($"{plat.Item2}: {plat.Item3} commandes");
            }
        }

        WaitForKey();
    }

    private void AfficherCuisiniersPlatsExclusifs()
    {
        Console.WriteLine("=== CUISINIERS AVEC PLATS EXCLUSIFS ===\n");

        Console.Write("Type de plat pour la comparaison (petit dej/dej/diner): ");
        string type = Console.ReadLine();

        if (type != "petit dej" && type != "dej" && type != "diner")
        {
            Console.WriteLine("Type de plat invalide.");
            WaitForKey();
            return;
        }

        List<Cuisinier> cuisiniers = db.ObtenirCuisiniersPlatsExclusifs(type);

        if (cuisiniers.Count == 0)
        {
            Console.WriteLine($"Aucun cuisinier ne propose de plat plus cher que tous les plats de type '{type}'.");
        }
        else
        {
            Console.WriteLine($"\nCuisiniers proposant des plats plus chers que tous les plats de type '{type}':");
            Console.WriteLine("--------------------------------------------------------------------------");

            foreach (var cuisinier in cuisiniers)
            {
                Plat plat = db.RecuperePlatID(cuisinier.IdPlat ?? 0);
                if (plat != null)
                {
                    Console.WriteLine($"{cuisinier.Prenom} {cuisinier.Nom}: {plat.NomPlat} - {plat.PrixParPersonne} € par personne");
                }
            }
        }

        WaitForKey();
    }

    private void AfficherClientsSansCommande()
    {
        Console.WriteLine("=== CLIENTS SANS COMMANDE ===\n");

        List<Client> clients = db.ObtenirClientsSansCommande();

        if (clients.Count == 0)
        {
            Console.WriteLine("Tous les clients ont passé au moins une commande.");
        }
        else
        {
            Console.WriteLine("Clients n'ayant jamais passé de commande:");
            Console.WriteLine("----------------------------------------");

            foreach (var client in clients)
            {
                Console.WriteLine($"{client.Nom} {client.Prenom} - Email: {client.Email ?? "Non défini"} - Tél: {client.Telephone ?? "Non défini"}");
            }

            Console.WriteLine($"\nTotal: {clients.Count} client(s) sans commande");
        }

        WaitForKey();
    }

    private void AfficherTempsMoyenPreparation()
    {
        Console.WriteLine("=== TEMPS MOYEN DE PRÉPARATION PAR CUISINIER ===\n");

        Dictionary<int, double> tempsMoyen = db.ObtenirTempsMoyenPreparationParCuisinier();

        if (tempsMoyen.Count == 0)
        {
            Console.WriteLine("Aucune donnée disponible sur les temps de préparation.");
        }
        else
        {
            Console.WriteLine("Temps moyen de préparation par cuisinier:");
            Console.WriteLine("----------------------------------------");

            foreach (var kvp in tempsMoyen)
            {
                Cuisinier cuisinier = db.ObtenirCuisinierID(kvp.Key);
                if (cuisinier != null)
                {
                    Console.WriteLine($"{cuisinier.Prenom} {cuisinier.Nom}: {kvp.Value:N0} minutes");
                }
            }
        }

        WaitForKey();
    }

    private void AfficherNombreClientsParMetro()
    {
        Console.WriteLine("=== NOMBRE DE CLIENTS PAR STATION DE MÉTRO ===\n");

        Dictionary<string, int> clientsParMetro = db.ObtenirNombreClientsParMetro();

        if (clientsParMetro.Count == 0)
        {
            Console.WriteLine("Aucune donnée disponible.");
        }
        else
        {
            Console.WriteLine("Nombre de clients par station de métro:");
            Console.WriteLine("-------------------------------------");

            foreach (var kvp in clientsParMetro)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value} client(s)");
            }
        }

        WaitForKey();
    }

    private void WaitForKey()
    {
        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
        Console.ReadKey();
    }
}