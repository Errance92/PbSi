using System;
using System.Collections.Generic;
using Karaté;

public class AuthentificationInterface
{
    private readonly DbAccess db;

    public AuthentificationInterface()
    {
        db = new DbAccess();
    }

    /// <summary>
    /// Démarre l'interface d'authentification et gère le flux de connexion et d'inscription.
    /// </summary>
    /// <returns>L'utilisateur authentifié ou null si déconnexion.</returns>
    public Utilisateur DemarrerAuthentification()
    {
        bool continuer = true;
        Utilisateur utilisateurConnecte = null;

        while (continuer && utilisateurConnecte == null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== LIV'IN PARIS - AUTHENTIFICATION ===");
            Console.ResetColor();
            Console.WriteLine("1. Se connecter");
            Console.WriteLine("2. S'inscrire");
            Console.WriteLine("0. Quitter");
            Console.Write("\nVotre choix: ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    utilisateurConnecte = SeConnecter();
                    break;
                case "2":
                    Sinscrire();
                    break;
                case "0":
                    continuer = false;
                    break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }

        return utilisateurConnecte;
    }

    /// <summary>
    /// Gère le processus de connexion d'un utilisateur.
    /// </summary>
    /// <returns>L'utilisateur connecté ou null si échec</returns>
    private Utilisateur SeConnecter()
    {
        Console.Clear();
        Console.WriteLine("=== CONNEXION ===\n");

        Console.Write("Nom d'utilisateur: ");
        string nomUtilisateur = Console.ReadLine();

        Console.Write("Mot de passe: ");
        string motDePasse = Console.ReadLine();

        Utilisateur utilisateur = db.AuthenticateUser(nomUtilisateur, motDePasse);

        if (utilisateur == null)
        {
            Console.WriteLine("\nNom d'utilisateur ou mot de passe incorrect.");
            WaitForKey();
            return null;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nConnexion réussie! Bienvenue, " + utilisateur.NomUtilisateur + "!");
        Console.ResetColor();
        WaitForKey();

        return utilisateur;
    }

    /// <summary>
    /// Gère le processus d'inscription d'un nouvel utilisateur.
    /// </summary>
    private void Sinscrire()
    {
        Console.Clear();
        Console.WriteLine("=== INSCRIPTION ===\n");

        Console.Write("Nom d'utilisateur: ");
        string nomUtilisateur = Console.ReadLine();

        // Vérifier si le nom d'utilisateur existe déjà
        if (db.NomUtilisateurExiste(nomUtilisateur))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nCe nom d'utilisateur est déjà pris. Veuillez en choisir un autre.");
            Console.ResetColor();
            WaitForKey();
            return;
        }

        Console.Write("Mot de passe: ");
        string motDePasse = Console.ReadLine();

        Console.WriteLine("\nChoisissez votre rôle:");
        Console.WriteLine("1. Client");
        Console.WriteLine("2. Cuisinier");
        Console.Write("\nVotre choix: ");

        string choixRole = Console.ReadLine();
        string role = "";
        int? idReference = null;

        switch (choixRole)
        {
            case "1":
                role = "Client";
                idReference = CreerCompteClient();
                break;
            case "2":
                role = "Cuisinier";
                idReference = CreerCompteCuisinier();
                break;
            default:
                Console.WriteLine("Option invalide. Inscription annulée.");
                WaitForKey();
                return;
        }

        if (idReference == null)
        {
            // Si la création du profil client/cuisinier a échoué
            return;
        }

        Utilisateur nouvelUtilisateur = new Utilisateur
        {
            IdUtilisateur = db.ObtenirProchainIdUtilisateur(),
            NomUtilisateur = nomUtilisateur,
            MotDePasse = motDePasse,
            Role = role,
            IdReference = idReference
        };

        if (db.AjouterUtilisateur(nouvelUtilisateur))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nInscription réussie! Vous pouvez maintenant vous connecter.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nErreur lors de l'inscription. Veuillez réessayer ultérieurement.");
            Console.ResetColor();
        }

        WaitForKey();
    }

    /// <summary>
    /// Crée un compte client associé à l'utilisateur en cours d'inscription.
    /// </summary>
    /// <returns>L'ID du client créé ou null si échec</returns>
    private int? CreerCompteClient()
    {
        Console.Clear();
        Console.WriteLine("=== CRÉATION DE PROFIL CLIENT ===\n");

        Client client = new Client();
        client.IdClient = db.ObtenirProchainIDClient();

        Console.Write("Nom: ");
        client.Nom = Console.ReadLine();

        Console.Write("Prénom: ");
        client.Prenom = Console.ReadLine();

        Console.Write("Numéro de rue: ");
        if (!int.TryParse(Console.ReadLine(), out int numRue))
        {
            Console.WriteLine("Numéro de rue invalide. Création de compte annulée.");
            WaitForKey();
            return null;
        }
        client.NumRue = numRue;

        Console.Write("Rue: ");
        client.NomRue = Console.ReadLine();

        Console.Write("Ville: ");
        client.Ville = Console.ReadLine();

        Console.Write("Métro le plus proche: ");
        string metro = Console.ReadLine();

        // Vérifier si la station de métro existe
        Graphe g = new Graphe();
        bool stationValide = false;
        foreach (Noeud n in g.Noeuds)
        {
            if (n.Station.ToLower() == metro.ToLower())
            {
                stationValide = true;
                break;
            }
        }

        while (!stationValide)
        {
            Console.WriteLine("Cette station de métro n'existe pas. Veuillez en saisir une valide:");
            metro = Console.ReadLine();

            foreach (Noeud n in g.Noeuds)
            {
                if (n.Station.ToLower() == metro.ToLower())
                {
                    stationValide = true;
                    break;
                }
            }
        }

        client.Metro = metro;

        Console.Write("Email (optionnel): ");
        client.Email = Console.ReadLine();

        Console.Write("Téléphone (optionnel): ");
        client.Telephone = Console.ReadLine();

        client.MontantAchat = 0; // Un nouveau client a 0€ d'achats

        if (db.AjouterClients(client))
        {
            Console.WriteLine("\nProfil client créé avec succès!");
            return client.IdClient;
        }
        else
        {
            Console.WriteLine("\nErreur lors de la création du profil client.");
            WaitForKey();
            return null;
        }
    }

    /// <summary>
    /// Crée un compte cuisinier associé à l'utilisateur en cours d'inscription.
    /// </summary>
    /// <returns>L'ID du cuisinier créé ou null si échec</returns>
    private int? CreerCompteCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== CRÉATION DE PROFIL CUISINIER ===\n");

        Cuisinier cuisinier = new Cuisinier();
        cuisinier.IdCuisinier = db.ObtenirProchainCuisinier();

        Console.Write("Nom: ");
        cuisinier.Nom = Console.ReadLine();

        Console.Write("Prénom: ");
        cuisinier.Prenom = Console.ReadLine();

        Console.Write("Numéro de rue: ");
        if (!int.TryParse(Console.ReadLine(), out int numRue))
        {
            Console.WriteLine("Numéro de rue invalide. Création de compte annulée.");
            WaitForKey();
            return null;
        }
        cuisinier.NumRue = numRue;

        Console.Write("Rue: ");
        cuisinier.NomRue = Console.ReadLine();

        Console.Write("Ville: ");
        cuisinier.Ville = Console.ReadLine();

        Console.Write("Métro le plus proche: ");
        string metro = Console.ReadLine();

        // Vérifier si la station de métro existe
        Graphe g = new Graphe();
        bool stationValide = false;
        foreach (Noeud n in g.Noeuds)
        {
            if (n.Station.ToLower() == metro.ToLower())
            {
                stationValide = true;
                break;
            }
        }

        while (!stationValide)
        {
            Console.WriteLine("Cette station de métro n'existe pas. Veuillez en saisir une valide:");
            metro = Console.ReadLine();

            foreach (Noeud n in g.Noeuds)
            {
                if (n.Station.ToLower() == metro.ToLower())
                {
                    stationValide = true;
                    break;
                }
            }
        }

        cuisinier.Metro = metro;

        Console.Write("Email (optionnel): ");
        cuisinier.Email = Console.ReadLine();

        Console.Write("Téléphone (optionnel): ");
        cuisinier.Telephone = Console.ReadLine();

        // Pour la simplicité, laissons le lien avec le plat pour plus tard
        cuisinier.IdPlat = null;

        if (db.AjouterCuisinier(cuisinier))
        {
            Console.WriteLine("\nProfil cuisinier créé avec succès!");
            return cuisinier.IdCuisinier;
        }
        else
        {
            Console.WriteLine("\nErreur lors de la création du profil cuisinier.");
            WaitForKey();
            return null;
        }
    }

    /// <summary>
    /// Attend que l'utilisateur appuie sur une touche pour continuer.
    /// </summary>
    private void WaitForKey()
    {
        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
        Console.ReadKey();
    }
}