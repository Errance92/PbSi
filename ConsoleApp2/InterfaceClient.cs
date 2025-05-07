using System;
using System.Collections.Generic;
using Karaté;

public class UserInterface
{
    private readonly DbAccess db;
    private Utilisateur utilisateurConnecte;

    public UserInterface()
    {
        db = new DbAccess();
    }

    /// <summary>
    /// Point d'entrée principal de l'interface utilisateur.
    /// </summary>
    public void Run()
    {
        // Démarrer l'authentification
        AuthentificationInterface authInterface = new AuthentificationInterface();
        utilisateurConnecte = authInterface.DemarrerAuthentification();

        if (utilisateurConnecte == null)
        {
            Console.WriteLine("Au revoir!");
            return;
        }

        // Menu principal basé sur le rôle de l'utilisateur
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            AfficherMenuPrincipal();

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Client")
                        ClientModule();
                    else
                        AfficherAccesRefuse();
                    break;
                case "2":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
                        CuisinierModule();
                    else
                        AfficherAccesRefuse();
                    break;
                case "3":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
                        PlatModule();
                    else
                        AfficherAccesRefuse();
                    break;
                case "4":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Client")
                        CommandeModule();
                    else
                        AfficherAccesRefuse();
                    break;
                case "5":
                    if (utilisateurConnecte.Role == "Admin")
                        GestionUtilisateursModule();
                    else
                        AfficherAccesRefuse();
                    break;
                case "6":
                    // Se déconnecter
                    exit = true;
                    utilisateurConnecte = null;
                    Console.WriteLine("Déconnexion réussie. Au revoir!");
                    WaitForKey();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }

        db.FermerConnection();
    }

    /// <summary>
    /// Affiche le menu principal adapté au rôle de l'utilisateur.
    /// </summary>
    private void AfficherMenuPrincipal()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== LIV'IN PARIS - {utilisateurConnecte.Role.ToUpper()} ===");
        Console.WriteLine($"Bonjour, {utilisateurConnecte.NomUtilisateur}");
        Console.ResetColor();

        if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Client")
            Console.WriteLine("1. Gestion des Clients");

        if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
            Console.WriteLine("2. Gestion des Cuisiniers");

        if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
            Console.WriteLine("3. Gestion des Plats");

        if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Client")
            Console.WriteLine("4. Gestion des Commandes");

        if (utilisateurConnecte.Role == "Admin")
            Console.WriteLine("5. Gestion des Utilisateurs");

        Console.WriteLine("6. Se déconnecter");
        Console.WriteLine("0. Quitter");

        Console.Write("\nVotre choix: ");
    }

    /// <summary>
    /// Affiche un message d'accès refusé.
    /// </summary>
    private void AfficherAccesRefuse()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nAccès refusé. Vous n'avez pas les droits nécessaires pour cette fonctionnalité.");
        Console.ResetColor();
        WaitForKey();
    }

    #region Gestion des utilisateurs

    /// <summary>
    /// Module pour la gestion des utilisateurs (réservé à l'admin).
    /// </summary>
    private void GestionUtilisateursModule()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== GESTION DES UTILISATEURS ===");
            Console.ResetColor();
            Console.WriteLine("1. Afficher tous les utilisateurs");
            Console.WriteLine("2. Ajouter un utilisateur");
            Console.WriteLine("3. Modifier un utilisateur");
            Console.WriteLine("4. Supprimer un utilisateur");
            Console.WriteLine("0. Retour au menu principal");

            Console.Write("\nVotre choix: ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1": AfficherTousUtilisateurs(); break;
                case "2": AjouterUtilisateur(); break;
                case "3": ModifierUtilisateur(); break;
                case "4": SupprimerUtilisateur(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Affiche la liste de tous les utilisateurs.
    /// </summary>
    private void AfficherTousUtilisateurs()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES UTILISATEURS ===\n");

        List<Utilisateur> utilisateurs = db.RecupererUtilisateurs();

        if (utilisateurs.Count == 0)
        {
            Console.WriteLine("Aucun utilisateur trouvé.");
        }
        else
        {
            foreach (Utilisateur utilisateur in utilisateurs)
            {
                Console.WriteLine(utilisateur.ToString());
            }

            Console.WriteLine("\nTotal: " + utilisateurs.Count + " utilisateur(s)");
        }

        WaitForKey();
    }

    /// <summary>
    /// Ajoute un nouvel utilisateur (admin uniquement).
    /// </summary>
    private void AjouterUtilisateur()
    {
        Console.Clear();
        Console.WriteLine("=== AJOUTER UN UTILISATEUR ===\n");

        Utilisateur nouvelUtilisateur = new Utilisateur();
        nouvelUtilisateur.IdUtilisateur = db.ObtenirProchainIdUtilisateur();

        Console.Write("Nom d'utilisateur: ");
        string nomUtilisateur = Console.ReadLine();

        if (db.NomUtilisateurExiste(nomUtilisateur))
        {
            Console.WriteLine("\nCe nom d'utilisateur existe déjà.");
            WaitForKey();
            return;
        }

        nouvelUtilisateur.NomUtilisateur = nomUtilisateur;

        Console.Write("Mot de passe: ");
        nouvelUtilisateur.MotDePasse = Console.ReadLine();

        Console.WriteLine("\nRôle:");
        Console.WriteLine("1. Admin");
        Console.WriteLine("2. Client");
        Console.WriteLine("3. Cuisinier");
        Console.Write("Votre choix: ");

        string choixRole = Console.ReadLine();

        switch (choixRole)
        {
            case "1":
                nouvelUtilisateur.Role = "Admin";
                nouvelUtilisateur.IdReference = null;
                break;
            case "2":
                nouvelUtilisateur.Role = "Client";
                Console.Write("\nID Client associé: ");
                if (!int.TryParse(Console.ReadLine(), out int idClient))
                {
                    Console.WriteLine("ID Client invalide.");
                    WaitForKey();
                    return;
                }

                if (db.ObtenirClientID(idClient) == null)
                {
                    Console.WriteLine("Ce client n'existe pas.");
                    WaitForKey();
                    return;
                }

                nouvelUtilisateur.IdReference = idClient;
                break;
            case "3":
                nouvelUtilisateur.Role = "Cuisinier";
                Console.Write("\nID Cuisinier associé: ");
                if (!int.TryParse(Console.ReadLine(), out int idCuisinier))
                {
                    Console.WriteLine("ID Cuisinier invalide.");
                    WaitForKey();
                    return;
                }

                if (db.ObtenirCuisinierID(idCuisinier) == null)
                {
                    Console.WriteLine("Ce cuisinier n'existe pas.");
                    WaitForKey();
                    return;
                }

                nouvelUtilisateur.IdReference = idCuisinier;
                break;
            default:
                Console.WriteLine("Option invalide.");
                WaitForKey();
                return;
        }

        if (db.AjouterUtilisateur(nouvelUtilisateur))
        {
            Console.WriteLine("\nUtilisateur ajouté avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de l'ajout de l'utilisateur.");
        }

        WaitForKey();
    }

    /// <summary>
    /// Modifie un utilisateur existant.
    /// </summary>
    private void ModifierUtilisateur()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFIER UN UTILISATEUR ===\n");

        Console.Write("ID de l'utilisateur à modifier: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID invalide.");
            WaitForKey();
            return;
        }

        Utilisateur utilisateur = db.ObtenirUtilisateurParId(id);

        if (utilisateur == null)
        {
            Console.WriteLine("Utilisateur non trouvé.");
            WaitForKey();
            return;
        }

        Console.WriteLine($"Modification de l'utilisateur: {utilisateur.NomUtilisateur} ({utilisateur.Role})");

        Console.Write($"Nouveau nom d'utilisateur [{utilisateur.NomUtilisateur}]: ");
        string nouveauNom = Console.ReadLine();

        if (!string.IsNullOrEmpty(nouveauNom) && nouveauNom != utilisateur.NomUtilisateur)
        {
            if (db.NomUtilisateurExiste(nouveauNom))
            {
                Console.WriteLine("Ce nom d'utilisateur existe déjà.");
                WaitForKey();
                return;
            }

            utilisateur.NomUtilisateur = nouveauNom;
        }

        Console.Write("Nouveau mot de passe (laissez vide pour conserver l'actuel): ");
        string nouveauMotDePasse = Console.ReadLine();

        if (!string.IsNullOrEmpty(nouveauMotDePasse))
        {
            utilisateur.MotDePasse = nouveauMotDePasse;
        }

        Console.WriteLine("\nNouveau rôle:");
        Console.WriteLine("1. Admin");
        Console.WriteLine("2. Client");
        Console.WriteLine("3. Cuisinier");
        Console.WriteLine("4. Conserver le rôle actuel");
        Console.Write("Votre choix: ");

        string choixRole = Console.ReadLine();

        switch (choixRole)
        {
            case "1":
                utilisateur.Role = "Admin";
                utilisateur.IdReference = null;
                break;
            case "2":
                utilisateur.Role = "Client";
                Console.Write("\nID Client associé: ");
                if (!int.TryParse(Console.ReadLine(), out int idClient))
                {
                    Console.WriteLine("ID Client invalide.");
                    WaitForKey();
                    return;
                }

                if (db.ObtenirClientID(idClient) == null)
                {
                    Console.WriteLine("Ce client n'existe pas.");
                    WaitForKey();
                    return;
                }

                utilisateur.IdReference = idClient;
                break;
            case "3":
                utilisateur.Role = "Cuisinier";
                Console.Write("\nID Cuisinier associé: ");
                if (!int.TryParse(Console.ReadLine(), out int idCuisinier))
                {
                    Console.WriteLine("ID Cuisinier invalide.");
                    WaitForKey();
                    return;
                }

                if (db.ObtenirCuisinierID(idCuisinier) == null)
                {
                    Console.WriteLine("Ce cuisinier n'existe pas.");
                    WaitForKey();
                    return;
                }

                utilisateur.IdReference = idCuisinier;
                break;
            case "4":
                // Ne rien changer
                break;
            default:
                Console.WriteLine("Option invalide.");
                WaitForKey();
                return;
        }

        if (db.MAJUtilisateur(utilisateur))
        {
            Console.WriteLine("\nUtilisateur modifié avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de la modification de l'utilisateur.");
        }

        WaitForKey();
    }

    /// <summary>
    /// Supprime un utilisateur existant.
    /// </summary>
    private void SupprimerUtilisateur()
    {
        Console.Clear();
        Console.WriteLine("=== SUPPRIMER UN UTILISATEUR ===\n");

        Console.Write("ID de l'utilisateur à supprimer: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID invalide.");
            WaitForKey();
            return;
        }

        Utilisateur utilisateur = db.ObtenirUtilisateurParId(id);

        if (utilisateur == null)
        {
            Console.WriteLine("Utilisateur non trouvé.");
            WaitForKey();
            return;
        }

        if (utilisateur.Role == "Admin" && utilisateur.IdUtilisateur == 1)
        {
            Console.WriteLine("Vous ne pouvez pas supprimer l'administrateur principal.");
            WaitForKey();
            return;
        }

        if (utilisateur.IdUtilisateur == utilisateurConnecte.IdUtilisateur)
        {
            Console.WriteLine("Vous ne pouvez pas supprimer votre propre compte.");
            WaitForKey();
            return;
        }

        Console.WriteLine($"Êtes-vous sûr de vouloir supprimer l'utilisateur: {utilisateur.NomUtilisateur} ({utilisateur.Role}) ? (O/N)");
        string confirmation = Console.ReadLine().ToUpper();

        if (confirmation != "O")
        {
            Console.WriteLine("Suppression annulée.");
            WaitForKey();
            return;
        }

        if (db.SupprimerUtilisateur(id))
        {
            Console.WriteLine("\nUtilisateur supprimé avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de la suppression de l'utilisateur.");
        }

        WaitForKey();
    }

    #endregion

    #region Client
    /// <summary>
    /// Permet de saisir les informations d'un nouveau client, vérifie la validité du métro, et l'ajoute à la base de données.
    /// </summary>

    private void AjouterClient()
    {
        Console.Clear();
        Console.WriteLine("=== AJOUTER UN CLIENT ===\n");

        Client client = new Client();
        client.IdClient = db.ObtenirProchainIDClient();

        Console.Write("Nom: ");
        client.Nom = Console.ReadLine();

        Console.Write("Prénom: ");
        client.Prenom = Console.ReadLine();

        int numRue;
        Console.Write("Numéro de rue: ");
        string saisieNumRue = Console.ReadLine();
        while (!int.TryParse(saisieNumRue, out numRue))
        {
            Console.WriteLine("Numéro de rue invalide. Veuillez réessayer : ");
            saisieNumRue = Console.ReadLine();
        }
        client.NumRue = numRue;

        Console.Write("Rue: ");
        client.NomRue = Console.ReadLine();

        Console.Write("Ville: ");
        client.Ville = Console.ReadLine();

        Console.Write("Métro le plus proche : ");
        string saisieMetro = Console.ReadLine();
        Graphe g = new Graphe();
        bool test = false;
        foreach (Noeud n in g.Noeuds)
        {
            if (n.Station.ToLower() == saisieMetro.ToLower())
            {
                test = true;
                break;
            }
        }
        while (test == false)
        {
            Console.WriteLine("Ce métro n'existe pas.");
            Console.Write("Métro le plus proche : ");
            saisieMetro = Console.ReadLine();
            foreach (Noeud n in g.Noeuds)
            {
                if (n.Station.ToLower() == saisieMetro.ToLower())
                {
                    test = true;
                    break;
                }
            }
        }
        client.Metro = saisieMetro;

        Console.Write("Email (optionnel): ");
        client.Email = Console.ReadLine();

        Console.Write("Téléphone (optionnel): ");
        client.Telephone = Console.ReadLine();

        client.MontantAchat = 0; // un nouveau client a forcement un montant d'achat total a 0

        if (db.AjouterClients(client))
        {
            Console.WriteLine("\nClient ajouté avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de l'ajout du client.");
        }

        WaitForKey();
    }

    /// <summary>
    /// Modifie les informations d'un client existant en fonction de son ID, avec vérification des saisies et du métro.
    /// </summary>

    private void ModifierClient()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFIER UN CLIENT ===\n");

        Console.Write("ID du client à modifier: ");
        string saisieId = Console.ReadLine();
        int id;
        while (!int.TryParse(saisieId, out id))
        {
            Console.WriteLine("ID invalide. Veuillez réessayer: ");
            saisieId = Console.ReadLine();
        }

        Client client = db.ObtenirClientID(id);

        if (client == null)
        {
            Console.WriteLine("Aucun client trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Modification de " + client.ToString() + "\n");

        Console.Write("Nom [" + client.Nom + "]: ");
        string nom = Console.ReadLine();
        if (!string.IsNullOrEmpty(nom))
        {
            client.Nom = nom;
        }

        Console.Write("Prénom [" + client.Prenom + "]: ");
        string prenom = Console.ReadLine();
        if (!string.IsNullOrEmpty(prenom))
        {
            client.Prenom = prenom;
        }

        Console.Write("Numéro de rue [" + client.NumRue + "]: ");
        string saisieNumRue = Console.ReadLine();
        if (!string.IsNullOrEmpty(saisieNumRue))
        {
            int numRue;
            while (!int.TryParse(saisieNumRue, out numRue))
            {
                Console.WriteLine("Numéro de rue invalide. Veuillez réessayer: ");
                saisieNumRue = Console.ReadLine();
            }
            client.NumRue = numRue;
        }

        Console.Write("Rue [" + client.NomRue + "]: ");
        string rue = Console.ReadLine();
        if (!string.IsNullOrEmpty(rue))
        {
            client.NomRue = rue;
        }

        Console.Write("Ville [" + client.Ville + "]: ");
        string ville = Console.ReadLine();
        if (!string.IsNullOrEmpty(ville))
        {
            client.Ville = ville;
        }

        Console.Write("Métro le plus proche [" + client.Metro + "]: ");
        string saisieMetro = Console.ReadLine();
        Graphe g = new Graphe();
        bool test = false;
        foreach (Noeud n in g.Noeuds)
        {
            if (n.Station.ToLower() == saisieMetro.ToLower())
            {
                test = true;
                break;
            }
        }
        while (test == false)
        {
            Console.WriteLine("Ce métro n'existe pas.");
            Console.Write("Métro le plus proche [" + client.Metro + "]: ");
            saisieMetro = Console.ReadLine();
            foreach (Noeud n in g.Noeuds)
            {
                if (n.Station.ToLower() == saisieMetro.ToLower())
                {
                    test = true;
                    break;
                }
            }
        }

        if (!string.IsNullOrEmpty(saisieMetro))
        {
            client.Metro = saisieMetro;
        }

        Console.Write("Email [" + client.Email + "]: ");
        string email = Console.ReadLine();
        if (!string.IsNullOrEmpty(email))
        {
            client.Email = email;
        }

        Console.Write("Téléphone [" + client.Telephone + "]: ");
        string telephone = Console.ReadLine();
        if (!string.IsNullOrEmpty(telephone))
        {
            client.Telephone = telephone;
        }

        client.MontantAchat = client.MontantAchat; // ne peut etre changee que si commande

        if (db.MAJClient(client))
        {
            Console.WriteLine("\nClient modifié avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de la modification du client");
        }

        WaitForKey();
    }
    /// <summary>
    /// Supprime un client de la base de données après confirmation, en fonction de son ID.
    /// </summary>

    private void SupprimerClient()
    {
        Console.Clear();
        Console.WriteLine("=== SUPPRIMER UN CLIENT ===\n");

        int id;
        string saisieId;
        Console.Write("ID du client à supprimer: ");
        saisieId = Console.ReadLine();
        while (!int.TryParse(saisieId, out id))
        {
            Console.WriteLine("ID invalide. Veuillez réessayer:");
            saisieId = Console.ReadLine();
        }

        Client client = db.ObtenirClientID(id);
        if (client == null)
        {
            Console.WriteLine("Aucun client trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Êtes-vous sûr de vouloir supprimer " + client.ToString() + " ? (O/N)");
        string rep = Console.ReadLine();
        if (rep.ToUpper() != "O")
        {
            Console.WriteLine("Suppression annulée.");
            WaitForKey();
            return;
        }

        if (db.SupprimerClient(id))
            Console.WriteLine("\nClient supprimé avec succès!");
        else
            Console.WriteLine("\nErreur lors de la suppression du client.");

        WaitForKey();
    }
    /// <summary>
    /// Affiche la liste des clients avec la possibilité de trier selon différents critères (nom, rue, montant d'achat).
    /// </summary>

    private void AfficherToutClients()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES CLIENTS ===\n");

        List<Client> clients = db.RecupererClients();

        if (clients.Count == 0)
        {
            Console.WriteLine("Aucun client trouvé.");
        }
        else
        {
            Console.WriteLine("Choisissez le type de tri :");
            Console.WriteLine("1 : Tri par nom");
            Console.WriteLine("2 : Tri par rue");
            Console.WriteLine("3 : Tri par montant d'achat");
            Console.WriteLine("4 : Pas de tri");
            Console.Write("Votre choix : ");
            string choixTri = Console.ReadLine().ToUpper();

            switch (choixTri)
            {
                case "1":
                    DbAccess.TrierParNom(clients);
                    break;
                case "2":
                    DbAccess.TrierParRue(clients);
                    break;
                case "3":
                    DbAccess.TrierParMontant(clients);
                    break;
                case "4":
                    break;
                default:
                    Console.WriteLine("Option invalide. Aucun tri effectué.");
                    break;
            }

            foreach (Client client in clients)
            {
                Console.WriteLine(client.ToString());
            }
            Console.WriteLine("\nTotal: " + clients.Count + " client(s)");
        }

        WaitForKey();
    }
    /// <summary>
    /// Vérifie si une station donnée existe dans une liste de nœuds représentant les stations de métro.
    /// </summary>
    /// <param name="nomStation">Le nom de la station à vérifier.</param>
    /// <param name="listeNoeuds">La liste des nœuds contenant les stations.</param>
    /// <returns>True si la station est valide, sinon False.</returns>

    public static bool EstStationValide(string nomStation, List<Noeud> listeNoeuds)
    {
        foreach (Noeud n in listeNoeuds)
        {
            if (n.Station.Equals(nomStation, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Gère le sous-menu du module client avec les options pour ajouter, modifier, supprimer ou afficher les clients.
    /// </summary>
    private void ClientModule()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== MODULE CLIENT ===");
            Console.ResetColor();
            Console.WriteLine("1. Ajouter un client");
            Console.WriteLine("2. Modifier un client");
            Console.WriteLine("3. Supprimer un client");
            Console.WriteLine("4. Afficher tous les clients");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");
            string choix = Console.ReadLine();
            switch (choix)
            {
                case "1":
                    if (utilisateurConnecte.Role == "Admin")
                        AjouterClient();
                    else
                        AfficherAccesRefuse();
                    break;
                case "2":
                    if (utilisateurConnecte.Role == "Admin" ||
                        (utilisateurConnecte.Role == "Client" &&
                         utilisateurConnecte.IdReference == ObtenirIdClientConnecte()))
                        ModifierClient();
                    else
                        AfficherAccesRefuse();
                    break;
                case "3":
                    if (utilisateurConnecte.Role == "Admin")
                        SupprimerClient();
                    else
                        AfficherAccesRefuse();
                    break;
                case "4": AfficherToutClients(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Obtient l'ID du client associé à l'utilisateur connecté.
    /// </summary>
    private int ObtenirIdClientConnecte()
    {
        if (utilisateurConnecte.Role == "Client" && utilisateurConnecte.IdReference.HasValue)
            return utilisateurConnecte.IdReference.Value;
        return -1;
    }
    #endregion

    #region Cuisinier

    /// <summary>
    /// Permet d'ajouter un nouveau cuisinier avec vérification des informations, validation de la station de métro,
    /// et choix ou création d'un plat associé.
    /// </summary>

    private void AjouterCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== AJOUTER UN CUISINIE ===\n");

        Cuisinier c = new Cuisinier();
        c.IdCuisinier = db.ObtenirProchainCuisinier();

        Console.Write("Nom : ");
        c.Nom = Console.ReadLine();

        Console.Write("Prénom : ");
        c.Prenom = Console.ReadLine();

        int numRue;
        Console.Write("Numéro de rue : ");
        string saisieNum = Console.ReadLine();
        while (!int.TryParse(saisieNum, out numRue))
        {
            Console.WriteLine("Numéro invalide. Réessayez : ");
            saisieNum = Console.ReadLine();
        }
        c.NumRue = numRue;

        Console.Write("Rue : ");
        c.NomRue = Console.ReadLine();

        Console.Write("Ville : ");
        c.Ville = Console.ReadLine();

        Console.Write("Métro : ");
        string metro = Console.ReadLine();
        Graphe g = new Graphe();
        while (!EstStationValide(metro, g.Noeuds))
        {
            Console.WriteLine("Métro inconnu. Réessayez : ");
            metro = Console.ReadLine();
        }
        c.Metro = metro;

        Console.Write("Email (optionnel) : ");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email)) c.Email = null;
        else c.Email = email;

        Console.Write("Téléphone (optionnel) : ");
        string tel = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(tel)) c.Telephone = null;
        else c.Telephone = tel;

        Console.WriteLine("\nSouhaitez-vous sélectionner un plat existant ou en créer un ?");
        Console.WriteLine("1. Choisir un plat existant");
        Console.WriteLine("2. Créer un nouveau plat");
        Console.Write("Votre choix : ");
        string choix = Console.ReadLine();

        if (choix == "1")
        {
            List<Plat> plats = db.RecupererToutPlat();
            foreach (Plat p in plats)
            {
                Console.WriteLine(p.ToString());
            }

            Console.Write("ID du plat à sélectionner : ");
            string saisie = Console.ReadLine();
            int idPlat;
            while (!int.TryParse(saisie, out idPlat) || db.RecuperePlatID(idPlat) == null)
            {
                Console.WriteLine("ID de plat invalide. Réessayez : ");
                saisie = Console.ReadLine();
            }

            c.IdPlat = idPlat;
        }
        else if (choix == "2")
        {
            Plat nouveau = AjouterPlatPourCuisinier();
            if (db.AjouterPlat(nouveau))
            {
                Console.WriteLine("Nouveau plat ajouté.");
                c.IdPlat = nouveau.IdPlat;
            }
            else
            {
                Console.WriteLine("Erreur lors de l’ajout du plat.");
                c.IdPlat = null;
            }
        }

        if (db.AjouterCuisinier(c))
        {
            Console.WriteLine("\nCuisinier ajouté avec succès !");
        }
        else
        {
            Console.WriteLine("\nErreur lors de l'ajout du cuisinier.");
        }

        WaitForKey();
    }
    /// <summary>
    /// Crée un nouveau plat en demandant les informations nécessaires à l'utilisateur.
    /// Cette méthode est utilisée dans le contexte de l'ajout ou modification d’un cuisinier.
    /// </summary>
    /// <returns>Le plat nouvellement créé, ou null en cas d'échec.</returns>

    private Plat AjouterPlatPourCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== AJOUTER UN PLAT ===\n");

        Plat plat = new Plat();
        plat.IdPlat = db.RecupererPlatSuivant();

        Console.Write("Nom du plat : ");
        plat.NomPlat = Console.ReadLine();

        Console.Write("Type (petit dej / dej / diner) : ");
        string type = Console.ReadLine().ToLower();
        while (type != "petit dej" && type != "dej" && type != "diner")
        {
            Console.Write("Type invalide. Re-saisir (petit dej / dej / diner) : ");
            type = Console.ReadLine().ToLower();
        }
        plat.Type = type;

        Console.Write("Stock : ");
        string saisieStock = Console.ReadLine();
        int stock;
        while (!int.TryParse(saisieStock, out stock) || stock < 0)
        {
            Console.Write("Stock invalide. Veuillez entrer un nombre positif : ");
            saisieStock = Console.ReadLine();
        }
        plat.Stock = stock;

        Console.Write("Origine (optionnel) : ");
        plat.Origine = Console.ReadLine();

        Console.Write("Régime alimentaire (optionnel) : ");
        plat.RegimeAlimentaire = Console.ReadLine();

        Console.Write("Ingrédients (optionnel) : ");
        plat.Ingredient = Console.ReadLine();

        Console.Write("Lien photo (optionnel) : ");
        plat.LienPhoto = Console.ReadLine();

        Console.Write("Date de fabrication (jj/mm/aaaa) : ");
        DateTime dateFab;
        while (!DateTime.TryParse(Console.ReadLine(), out dateFab))
        {
            Console.Write("Date invalide. Réessayer (jj/mm/aaaa) : ");
        }
        plat.DateFabrication = dateFab;

        Console.Write("Date de péremption (jj/mm/aaaa) : ");
        DateTime datePer;
        while (!DateTime.TryParse(Console.ReadLine(), out datePer) || datePer < plat.DateFabrication)
        {
            Console.Write("Date invalide (ou antérieure à la fabrication). Réessayer (jj/mm/aaaa) : ");
        }
        plat.DatePeremption = datePer;

        Console.Write("Prix par personne : ");
        string saisiePrix = Console.ReadLine();
        decimal prix;
        while (!decimal.TryParse(saisiePrix, out prix) || prix < 0)
        {
            Console.Write("Prix invalide. Veuillez entrer un nombre positif : ");
            saisiePrix = Console.ReadLine();
        }
        plat.PrixParPersonne = prix;

        bool succes = db.AjouterPlat(plat);
        if (succes)
        {
            Console.WriteLine("\nPlat ajouté avec succès !");
            return plat;
        }
        else
        {
            Console.WriteLine("\nErreur lors de l'ajout du plat.");
            return null;
        }
    }
    /// <summary>
    /// Modifie les informations d’un cuisinier existant, y compris son plat associé,
    /// avec validation des champs et de la station de métro.
    /// </summary>

    private void ModifierCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFIER UN CUISINIER ===\n");

        Console.Write("ID du cuisinier à modifier: ");
        string saisieId = Console.ReadLine();
        int id;
        while (!int.TryParse(saisieId, out id))
        {
            Console.WriteLine("ID invalide. Veuillez réessayer:");
            saisieId = Console.ReadLine();
        }

        Cuisinier cuisinier = db.ObtenirCuisinierID(id);

        if (cuisinier == null)
        {
            Console.WriteLine("Aucun cuisinier trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Modification de " + cuisinier.Nom + " " + cuisinier.Prenom + "\n");

        Console.Write("Nom [" + cuisinier.Nom + "]: ");
        string nom = Console.ReadLine();
        if (!string.IsNullOrEmpty(nom)) cuisinier.Nom = nom;

        Console.Write("Prénom [" + cuisinier.Prenom + "]: ");
        string prenom = Console.ReadLine();
        if (!string.IsNullOrEmpty(prenom)) cuisinier.Prenom = prenom;

        Console.Write("Numéro de rue [" + cuisinier.NumRue + "]: ");
        string saisieNum = Console.ReadLine();
        if (!string.IsNullOrEmpty(saisieNum))
        {
            int numRue;
            while (!int.TryParse(saisieNum, out numRue))
            {
                Console.WriteLine("Numéro invalide. Veuillez réessayer:");
                saisieNum = Console.ReadLine();
            }
            cuisinier.NumRue = numRue;
        }

        Console.Write("Rue [" + cuisinier.NomRue + "]: ");
        string rue = Console.ReadLine();
        if (!string.IsNullOrEmpty(rue)) cuisinier.NomRue = rue;

        Console.Write("Ville [" + cuisinier.Ville + "]: ");
        string ville = Console.ReadLine();
        if (!string.IsNullOrEmpty(ville)) cuisinier.Ville = ville;

        Console.Write("Métro le plus proche [" + cuisinier.Metro + "]: ");
        string saisieMetro = Console.ReadLine();
        if (!string.IsNullOrEmpty(saisieMetro))
        {
            Graphe g = new Graphe();
            while (!EstStationValide(saisieMetro, g.Noeuds))
            {
                Console.WriteLine("Ce métro n'existe pas.");
                Console.Write("Métro le plus proche [" + cuisinier.Metro + "]: ");
                saisieMetro = Console.ReadLine();
            }
            cuisinier.Metro = saisieMetro;
        }

        Console.Write("Email [" + cuisinier.Email + "]: ");
        string email = Console.ReadLine();
        if (!string.IsNullOrEmpty(email)) cuisinier.Email = email;

        Console.Write("Téléphone [" + cuisinier.Telephone + "]: ");
        string tel = Console.ReadLine();
        if (!string.IsNullOrEmpty(tel)) cuisinier.Telephone = tel;

        Console.WriteLine("Souhaitez-vous modifier le plat associé ? (O/N)");
        string choix = Console.ReadLine().ToUpper();
        if (choix == "O")
        {
            Console.WriteLine("1 : Choisir un plat existant");
            Console.WriteLine("2 : Créer un nouveau plat");
            string rep = Console.ReadLine();

            if (rep == "1")
            {
                List<Plat> plats = db.RecupererToutPlat();
                foreach (Plat p in plats)
                {
                    Console.WriteLine(p.ToString());
                }

                Console.Write("ID du plat choisi: ");
                string saisiePlat = Console.ReadLine();
                int idPlat;
                while (!int.TryParse(saisiePlat, out idPlat) || db.RecuperePlatID(idPlat) == null)
                {
                    Console.WriteLine("ID plat invalide. Réessayez:");
                    saisiePlat = Console.ReadLine();
                }
                cuisinier.IdPlat = idPlat;
            }
            else if (rep == "2")
            {
                Plat nouveauPlat = AjouterPlatPourCuisinier();
                cuisinier.IdPlat = nouveauPlat.IdPlat;
            }
        }

        if (db.MAJCuisinier(cuisinier))
        {
            Console.WriteLine("\nCuisinier modifié avec succès!");
        }
        else
        {
            Console.WriteLine("\nErreur lors de la modification.");
        }

        WaitForKey();
    }
    /// <summary>
    /// Affiche la liste de tous les cuisiniers enregistrés dans la base de données.
    /// </summary>

    private void AfficherToutCuisiniers()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES CUISINIERS ===\n");

        List<Cuisinier> cuisiniers = db.RecupererCuisinier();

        if (cuisiniers.Count == 0)
        {
            Console.WriteLine("Aucun cuisinier trouvé.");
        }
        else
        {
            foreach (Cuisinier cuisinier in cuisiniers)
            {
                Console.WriteLine(cuisinier.ToString());
            }
            Console.WriteLine("\nTotal: " + cuisiniers.Count + " cuisinier(s)");
        }

        WaitForKey();
    }
    /// <summary>
    /// Supprime un cuisinier existant après confirmation, à partir de son ID.
    /// </summary>

    private void SupprimerCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== SUPPRIMER UN CUISINIER ===\n");

        Console.Write("ID du cuisinier à supprimer : ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("ID invalide. Réessayez : ");
        }

        Cuisinier cuisinier = db.ObtenirCuisinierID(id);
        if (cuisinier == null)
        {
            Console.WriteLine("Aucun cuisinier trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Êtes-vous sûr de vouloir supprimer : " + cuisinier.Nom + " " + cuisinier.Prenom + " ? (O/N)");
        string rep = Console.ReadLine();
        if (rep.ToUpper() != "O")
        {
            Console.WriteLine("Suppression annulée.");
            WaitForKey();
            return;
        }

        if (db.SupprimerCuisinier(id))
        {
            Console.WriteLine("Cuisinier supprimé avec succès !");
        }
        else
        {
            Console.WriteLine("Erreur lors de la suppression du cuisinier.");
        }

        WaitForKey();
    }

    /// <summary>
    /// Gère le sous-menu du module cuisinier avec les options pour ajouter, modifier, supprimer ou afficher les cuisiniers.
    /// </summary>
    private void CuisinierModule()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== MODULE CUISINIER ===");
            Console.ResetColor();
            Console.WriteLine("1. Ajouter un cuisinier");
            Console.WriteLine("2. Modifier un cuisinier");
            Console.WriteLine("3. Supprimer un cuisinier");
            Console.WriteLine("4. Afficher tous les cuisiniers");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");
            string choix = Console.ReadLine();
            switch (choix)
            {
                case "1":
                    if (utilisateurConnecte.Role == "Admin")
                        AjouterCuisinier();
                    else
                        AfficherAccesRefuse();
                    break;
                case "2":
                    if (utilisateurConnecte.Role == "Admin" ||
                        (utilisateurConnecte.Role == "Cuisinier" &&
                         utilisateurConnecte.IdReference == ObtenirIdCuisinierConnecte()))
                        ModifierCuisinier();
                    else
                        AfficherAccesRefuse();
                    break;
                case "3":
                    if (utilisateurConnecte.Role == "Admin")
                        SupprimerCuisinier();
                    else
                        AfficherAccesRefuse();
                    break;
                case "4": AfficherToutCuisiniers(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Obtient l'ID du cuisinier associé à l'utilisateur connecté.
    /// </summary>
    private int ObtenirIdCuisinierConnecte()
    {
        if (utilisateurConnecte.Role == "Cuisinier" && utilisateurConnecte.IdReference.HasValue)
            return utilisateurConnecte.IdReference.Value;
        return -1;
    }


    #endregion

    #region Plat
    /// <summary>
    /// Gère le sous-menu du module plat avec les options pour ajouter, modifier, supprimer ou afficher des plats.
    /// </summary>

    private void PlatModule()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== MODULE PLAT ===");
            Console.ResetColor();
            Console.WriteLine("1. Ajouter un plat");
            Console.WriteLine("2. Modifier un plat");
            Console.WriteLine("3. Supprimer un plat");
            Console.WriteLine("4. Afficher les details d'un plat");
            Console.WriteLine("5. Afficher tous les plats");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");
            string choix = Console.ReadLine();
            switch (choix)
            {
                case "1":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
                        AjouterPlat();
                    else
                        AfficherAccesRefuse();
                    break;
                case "2":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Cuisinier")
                        ModifierPlat();
                    else
                        AfficherAccesRefuse();
                    break;
                case "3":
                    if (utilisateurConnecte.Role == "Admin")
                        SupprimerPlat();
                    else
                        AfficherAccesRefuse();
                    break;
                case "4": AfficherPlat(); break;
                case "5": AfficherToutPlat(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }
    /// <summary>
    /// Permet d'ajouter un nouveau plat avec toutes ses informations : nom, type, stock, origine, etc.
    /// Inclut la validation des champs saisis.
    /// </summary>

    private void AjouterPlat()
    {
        Console.Clear();
        Console.WriteLine("=== AJOUTER UN PLAT ===\n");

        Plat plat = new Plat();

        plat.IdPlat = db.RecupererPlatSuivant();

        Console.Write("Nom du plat: ");
        plat.NomPlat = Console.ReadLine();

        Console.Write("Type (petit dej / dej / diner) : ");
        string type = Console.ReadLine().ToLower();

        while (type != "petit dej" && type != "dej" && type != "diner")
        {
            Console.WriteLine("Type invalide. Veuillez saisir : petit dej, dej ou diner.");
            Console.Write("Type : ");
            type = Console.ReadLine().ToLower();
        }

        plat.Type = type;

        int stock;
        Console.Write("Stock (quantité) : ");
        while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
        {
            Console.WriteLine("Entrée invalide. Veuillez entrer un entier positif.");
            Console.Write("Stock : ");
        }
        plat.Stock = stock;

        Console.Write("Origine (optionnel): ");
        plat.Origine = Console.ReadLine();

        Console.Write("Régime alimentaire (optionnel): ");
        plat.RegimeAlimentaire = Console.ReadLine();

        Console.Write("Ingrédients (optionnel): ");
        plat.Ingredient = Console.ReadLine();

        Console.Write("Lien photo (optionnel): ");
        plat.LienPhoto = Console.ReadLine();

        DateTime dateFab;
        while (true)
        {
            Console.Write("Date de fabrication (jj/mm/aaaa) : ");
            string saisie = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(saisie))
            {
                dateFab = DateTime.MinValue;
                break;
            }
            if (DateTime.TryParse(saisie, out dateFab)) break;

            Console.WriteLine("Format de date invalide. Réessayez.");
        }
        plat.DateFabrication = dateFab;

        DateTime datePer;
        while (true)
        {
            Console.Write("Date de péremption (jj/mm/aaaa) : ");
            string saisie = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(saisie))
            {
                datePer = DateTime.MinValue;
                break;
            }
            if (DateTime.TryParse(saisie, out datePer))
            {
                if (dateFab != DateTime.MinValue && datePer < dateFab)
                {
                    Console.WriteLine("Erreur : la date de péremption ne peut pas être avant la fabrication.");
                }
                else
                {
                    break;
                }
            }
            else
            {
                Console.WriteLine("Date invalide. Réessayez.");
            }
        }
        plat.DatePeremption = datePer;

        decimal prix;
        Console.Write("Prix par personne (€) : ");
        while (!decimal.TryParse(Console.ReadLine(), out prix) || prix < 0)
        {
            Console.WriteLine("Prix invalide, il doit être un nombre positif.");
            Console.Write("Prix par personne (€) : ");
        }

        plat.PrixParPersonne = prix;

        bool success = db.AjouterPlat(plat);
        if (success)
        {
            Console.WriteLine("\nPlat ajouté avec succès !");
        }
        else
        {
            Console.WriteLine("\nErreur lors de l'ajout du plat.");
        }

        WaitForKey();
    }
    /// <summary>
    /// Affiche tous les plats enregistrés dans la base de données.
    /// </summary>

    private void AfficherToutPlat()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES PLATS ===\n");
        List<Plat> plats = db.RecupererToutPlat();

        if (plats.Count == 0)
        {
            Console.WriteLine("Aucun plat trouvé.");
        }
        else
        {
            foreach (Plat plat in plats)
            {
                Console.WriteLine(plat.ToString());
            }
            Console.WriteLine("\nTotal: " + plats.Count + " commande(s)");
        }

        WaitForKey();
    }
    /// <summary>
    /// Affiche les détails complets d’un plat à partir de son ID.
    /// </summary>

    private void AfficherPlat()
    {
        Console.Clear();
        Console.WriteLine("=== AFFICHER UN PLAT ===\n");

        Console.Write("ID du plat : ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("ID invalide. Veuillez réessayer : ");
        }

        Plat plat = db.RecuperePlatID(id);
        if (plat == null)
        {
            Console.WriteLine("Aucun plat trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("\n--- Détails du plat ---");
        Console.WriteLine("ID : " + plat.IdPlat);
        Console.WriteLine("Nom : " + plat.NomPlat);
        Console.WriteLine("Type : " + plat.Type);
        Console.WriteLine("Stock : " + plat.Stock);

        if (plat.Origine == null)
        {
            Console.WriteLine("Origine : Non défini");
        }
        else
        {
            Console.WriteLine("Origine : " + plat.Origine);
        }

        if (plat.RegimeAlimentaire == null)
        {
            Console.WriteLine("Régime alimentaire : Non défini");
        }
        else
        {
            Console.WriteLine("Régime alimentaire : " + plat.RegimeAlimentaire);
        }

        if (plat.Ingredient == null)
        {
            Console.WriteLine("Ingrédients : Non défini");
        }
        else
        {
            Console.WriteLine("Ingrédients : " + plat.Ingredient);
        }

        if (plat.LienPhoto == null)
        {
            Console.WriteLine("Lien photo : Non défini");
        }
        else
        {
            Console.WriteLine("Lien photo : " + plat.LienPhoto);
        }

        if (plat.DateFabrication == DateTime.MinValue)
        {
            Console.WriteLine("Date de fabrication : Non définie");
        }
        else
        {
            Console.WriteLine("Date de fabrication : " + plat.DateFabrication.ToShortDateString());
        }

        Console.WriteLine("Prix par personne : " + plat.PrixParPersonne + " €");

        if (plat.DatePeremption == DateTime.MinValue)
        {
            Console.WriteLine("Date de péremption : Non définie");
        }
        else
        {
            Console.WriteLine("Date de péremption : " + plat.DatePeremption.ToShortDateString());
        }

        WaitForKey();
    }
    /// <summary>
    /// Permet de modifier les informations d’un plat existant avec vérification et mise à jour des champs.
    /// </summary>

    private void ModifierPlat()
    {
        Console.Clear();
        Console.WriteLine("=== MODIFIER UN PLAT ===\n");

        Console.Write("ID du plat à modifier : ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("ID invalide. Réessayez : ");
        }

        Plat plat = db.RecuperePlatID(id);
        if (plat == null)
        {
            Console.WriteLine("Aucun plat trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Modification de : " + plat.ToString() + "\n");

        Console.Write("Nom [" + plat.NomPlat + "] : ");
        string nom = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nom))
            plat.NomPlat = nom;

        Console.Write("Type [" + plat.Type + "] (petit dej / dej / diner) : ");
        string type = Console.ReadLine().ToLower();
        while (type != "petit dej" && type != "dej" && type != "diner" && type != "")
        {
            Console.WriteLine("Type invalide. Entrez 'petit dej', 'dej' ou 'diner' : ");
            type = Console.ReadLine().ToLower();
        }
        if (!string.IsNullOrWhiteSpace(type))
            plat.Type = type;

        Console.Write("Stock [" + plat.Stock + "] : ");
        string saisieStock = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(saisieStock))
        {
            int stock;
            while (!int.TryParse(saisieStock, out stock) || stock < 0)
            {
                Console.WriteLine("Stock invalide. Veuillez entrer un entier positif : ");
                saisieStock = Console.ReadLine();
            }
            plat.Stock = stock;
        }

        Console.Write("Origine [" + (plat.Origine ?? "non définie") + "] : ");
        string origine = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(origine))
            plat.Origine = origine;

        Console.Write("Régime alimentaire [" + (plat.RegimeAlimentaire ?? "non défini") + "] : ");
        string regime = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(regime))
            plat.RegimeAlimentaire = regime;

        Console.Write("Ingrédients [" + (plat.Ingredient ?? "non défini") + "] : ");
        string ingredient = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(ingredient))
            plat.Ingredient = ingredient;

        Console.Write("Lien photo [" + (plat.LienPhoto ?? "non défini") + "] : ");
        string lien = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(lien))
            plat.LienPhoto = lien;

        Console.Write("Date de fabrication [" + plat.DateFabrication.ToShortDateString() + "] (format JJ/MM/AAAA) : ");
        string saisieDateFab = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(saisieDateFab))
        {
            DateTime dateFab;
            while (!DateTime.TryParse(saisieDateFab, out dateFab))
            {
                Console.WriteLine("Date invalide. Réessayez (JJ/MM/AAAA) : ");
                saisieDateFab = Console.ReadLine();
            }
            plat.DateFabrication = dateFab;
        }

        Console.Write("Date de péremption [" + plat.DatePeremption.ToShortDateString() + "] (format JJ/MM/AAAA) : ");
        string saisieDatePer = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(saisieDatePer))
        {
            DateTime datePer;
            while (!DateTime.TryParse(saisieDatePer, out datePer) || datePer < plat.DateFabrication)
            {
                Console.WriteLine("Date invalide ou antérieure à la date de fabrication. Réessayez : ");
                saisieDatePer = Console.ReadLine();
            }
            plat.DatePeremption = datePer;
        }

        Console.Write("Prix par personne [" + plat.PrixParPersonne + " €] : ");
        string saisiePrix = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(saisiePrix))
        {
            decimal prix;
            while (!decimal.TryParse(saisiePrix, out prix) || prix < 0)
            {
                Console.WriteLine("Prix invalide. Veuillez entrer un prix positif : ");
                saisiePrix = Console.ReadLine();
            }
            plat.PrixParPersonne = prix;
        }

        if (db.MAJPlat(plat))
            Console.WriteLine("\nPlat modifié avec succès !");
        else
            Console.WriteLine("\nErreur lors de la modification du plat.");

        WaitForKey();
    }
    /// <summary>
    /// Supprime un plat de la base de données après confirmation, à partir de son ID.
    /// </summary>

    private void SupprimerPlat()
    {
        Console.Clear();
        Console.WriteLine("=== SUPPRIMER UN PLAT ===\n");

        Console.Write("ID du plat à supprimer : ");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("ID invalide. Réessayez : ");
        }

        Plat plat = db.RecuperePlatID(id);
        if (plat == null)
        {
            Console.WriteLine("Aucun plat trouvé avec l'ID " + id);
            WaitForKey();
            return;
        }

        Console.WriteLine("Êtes-vous sûr de vouloir supprimer : " + plat.ToString() + " ? (O/N)");
        string rep = Console.ReadLine();
        if (rep.ToUpper() != "O")
        {
            Console.WriteLine("Suppression annulée.");
            WaitForKey();
            return;
        }

        if (db.SupprimerPlat(id))
        {
            Console.WriteLine("Plat supprimé avec succès !");
        }
        else
        {
            Console.WriteLine("Erreur lors de la suppression du plat.");
        }

        WaitForKey();
    }

    #endregion

    #region Commande
    /// <summary>
    /// Gère le sous-menu du module commande avec les options pour créer, afficher ou consulter les commandes.
    /// </summary>

    private void CommandeModule()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=== MODULE COMMANDE ===");
            Console.ResetColor();
            Console.WriteLine("1. Créer une commande");
            Console.WriteLine("2. Afficher les détails d'une commande");
            Console.WriteLine("3. Afficher toutes les commandes");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    if (utilisateurConnecte.Role == "Admin" || utilisateurConnecte.Role == "Client")
                        PasserCommande();
                    else
                        AfficherAccesRefuse();
                    break;
                case "2": AfficherCommandeParID(); break;
                case "3": AfficherToutesCommandes(); break;
                case "0": exit = true; break;
                default:
                    Console.WriteLine("Option invalide.");
                    WaitForKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Crée une nouvelle commande en associant un client, un plat, un cuisinier et un moyen de paiement.
    /// Calcule le montant, met à jour le client et affiche le chemin de livraison.
    /// </summary>

    private void PasserCommande()
    {
        Console.Clear();
        Console.WriteLine("=== PASSER UNE COMMANDE ===\n");

        int idClient;

        // Si c'est un client qui passe commande, on utilise son ID automatiquement
        if (utilisateurConnecte.Role == "Client" && utilisateurConnecte.IdReference.HasValue)
        {
            idClient = utilisateurConnecte.IdReference.Value;
        }
        else
        {
            // Sinon, demander l'ID du client (pour l'admin)
            Console.Write("ID du client : ");
            string saisieId = Console.ReadLine();
            while (!int.TryParse(saisieId, out idClient) || db.ObtenirClientID(idClient) == null)
            {
                Console.WriteLine("ID invalide ou client inexistant. Réessayez : ");
                saisieId = Console.ReadLine();
            }
        }

        Client client = db.ObtenirClientID(idClient);

        Console.Clear();
        Console.WriteLine("=== SÉLECTION DU PLAT ===\n");
        List<Plat> platsDispo = db.RecupererToutPlat();

        foreach (var p in platsDispo)
        {
            Console.WriteLine(p.ToString());
        }

        Console.Write("\nID du plat souhaité : ");
        int idPlat;
        string saisiePlat = Console.ReadLine();
        while (!int.TryParse(saisiePlat, out idPlat) || db.RecuperePlatID(idPlat) == null)
        {
            Console.WriteLine("ID invalide. Réessayez : ");
            saisiePlat = Console.ReadLine();
        }

        Plat plat = db.RecuperePlatID(idPlat);

        Console.Write("Quantité (nombre de portions) : ");
        int quantite;
        while (!int.TryParse(Console.ReadLine(), out quantite) || quantite <= 0)
        {
            Console.WriteLine("Quantité invalide. Réessayez : ");
        }

        double montantTotal = Convert.ToDouble(plat.PrixParPersonne * quantite);

        List<Cuisinier> cuisiniers = db.RecupererCuisinier();
        List<Cuisinier> disponibles = new List<Cuisinier>();

        foreach (Cuisinier c in cuisiniers)
        {
            if (c.IdPlat != null && c.IdPlat == plat.IdPlat)
            {
                disponibles.Add(c);
            }
        }

        if (disponibles.Count == 0)
        {
            Console.WriteLine("Aucun cuisinier ne propose ce plat. Annulation.");
            WaitForKey();
            return;
        }

        Console.WriteLine("\nCuisiniers disponibles pour ce plat :");
        for (int i = 0; i < disponibles.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + disponibles[i].ToString());
        }

        Console.Write("Choisissez un cuisinier (par numéro) : ");
        int choixCuisinier;
        string saisieChoix = Console.ReadLine();
        while (!int.TryParse(saisieChoix, out choixCuisinier) || choixCuisinier < 1 || choixCuisinier > disponibles.Count)
        {
            Console.WriteLine("Choix invalide. Réessayez : ");
            saisieChoix = Console.ReadLine();
        }

        Cuisinier cuisinier = disponibles[choixCuisinier - 1];

        Console.Write("Moyen de paiement (carte ou espece) : ");
        string paiement = Console.ReadLine().ToLower();
        while (paiement != "carte" && paiement != "espece")
        {
            Console.WriteLine("Moyen de paiement invalide. Veuillez entrer 'carte' ou 'espece' : ");
            paiement = Console.ReadLine().ToLower();
        }

        Commande commande = new Commande();
        commande.IdCommande = db.ObtenirProchainIdCommande();
        commande.IdClient = client.IdClient;
        commande.IdCuisinier = cuisinier.IdCuisinier;
        commande.StatuCommande = "En cours";
        commande.DateCommande = DateTime.Now;
        commande.Paiement = paiement;
        commande.Montant = montantTotal;

        bool succes = db.AjouterCommande(commande);
        if (succes)
        {
            Console.WriteLine("\nCommande ajoutée avec succès !");

            // Mise à jour du MontantAchat du client
            client.MontantAchat = client.MontantAchat + montantTotal;
            db.MAJClient(client);
            Console.WriteLine("Cout de la commande : " + montantTotal + "€");
        }
        else
        {
            Console.WriteLine("\nErreur lors de l’ajout de la commande.");
            WaitForKey();
            return;
        }

        Graphe g = new Graphe();
        string stationDepart = client.Metro;
        string stationArrivee = cuisinier.Metro;

        List<string> chemin = g.DijkstraChemin(stationDepart, stationArrivee);
        int temps = g.DijkstraCout(stationDepart, stationArrivee);

        if (chemin.Count == 0)
        {
            Console.WriteLine("\nAucun chemin trouvé entre " + stationDepart + " et " + stationArrivee + ".");
        }
        else
        {
            Console.WriteLine("\n--- Chemin de livraison ---");
            for (int i = 0; i < chemin.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + chemin[i]);
            }
        }
        Console.WriteLine("Temps de livraison : " + temps + " minutes");

        WaitForKey();
    }
    /// <summary>
    /// Affiche les détails d'une commande spécifique à partir de son ID.
    /// Inclut les informations sur le client, le cuisinier et le plat associés.
    /// </summary>

    private void AfficherCommandeParID()
    {
        Console.Clear();
        Console.WriteLine("=== AFFICHER UNE COMMANDE PAR ID ===\n");

        Console.Write("ID de la commande : ");
        int idCommande;
        while (!int.TryParse(Console.ReadLine(), out idCommande))
        {
            Console.WriteLine("ID invalide. Veuillez réessayer : ");
        }

        Commande commande = db.ObtenirCommandeParId(idCommande);

        if (commande == null)
        {
            Console.WriteLine("Aucune commande trouvée avec l'ID " + idCommande);
            WaitForKey();
            return;
        }

        Console.WriteLine("\n--- Détails de la commande ---");

        Console.WriteLine("ID : " + commande.IdCommande);
        Console.WriteLine("Date : " + commande.DateCommande.ToString("dd/MM/yyyy HH:mm"));

        Client client = db.ObtenirClientID(commande.IdClient);
        if (client != null)
        {
            Console.WriteLine("Client : " + client.ToString());
        }
        else
        {
            Console.WriteLine("Client : Inconnu");
        }

        Cuisinier cuisinier = db.ObtenirCuisinierID(commande.IdCuisinier);
        if (cuisinier != null)
        {
            Console.WriteLine("Cuisinier : " + cuisinier.ToString());
        }
        else
        {
            Console.WriteLine("Cuisinier : Inconnu");
        }

        Plat plat = db.RecuperePlatID(commande.IdPlat);
        if (plat != null)
        {
            Console.WriteLine("Plat : " + plat.ToString());
        }
        else
        {
            Console.WriteLine("Plat : Inconnu");
        }

        Console.WriteLine("Statut : " + commande.StatuCommande);

        if (commande.Paiement == null || commande.Paiement == "")
        {
            Console.WriteLine("Paiement : Non défini");
        }
        else
        {
            Console.WriteLine("Paiement : " + commande.Paiement);
        }

        Console.WriteLine("Montant : " + commande.Montant + " €");

        WaitForKey();
    }
    /// <summary>
    /// Affiche la liste de toutes les commandes, avec les informations liées au client, plat et cuisinier.
    /// </summary>

    private void AfficherToutesCommandes()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES COMMANDES ===\n");

        List<Commande> commandes = db.RecupererCommandes();

        if (commandes.Count == 0)
        {
            Console.WriteLine("Aucune commande trouvée.");
            WaitForKey();
            return;
        }

        foreach (Commande commande in commandes)
        {
            Client client = db.ObtenirClientID(commande.IdClient);
            Cuisinier cuisinier = db.ObtenirCuisinierID(commande.IdCuisinier);
            Plat plat = db.RecuperePlatID(commande.IdPlat);

            string ligne = "[" + commande.IdCommande + "] " +
                           commande.DateCommande.ToString("dd/MM/yyyy") + " - ";

            if (client != null)
            {
                ligne += "Client : " + client.Nom + " ";
            }
            else
            {
                ligne += "Client : Inconnu ";
            }

            if (plat != null)
            {
                ligne += "- Plat : " + plat.NomPlat + " ";
            }
            else
            {
                ligne += "- Plat : Inconnu ";
            }

            if (cuisinier != null)
            {
                ligne += "- Cuisinier : " + cuisinier.Nom + " ";
            }
            else
            {
                ligne += "- Cuisinier : Inconnu ";
            }

            ligne += "- " + commande.Montant + " € - " + commande.Paiement + " - " + commande.StatuCommande;

            Console.WriteLine(ligne);
        }

        Console.WriteLine("\nTotal : " + commandes.Count + " commande(s)");
        WaitForKey();
    }


    #endregion
    /// <summary>
    /// Attend que l'utilisateur appuie sur une touche pour continuer.
    /// </summary>

    private void WaitForKey()
    {
        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
        Console.ReadKey();
    }
}
