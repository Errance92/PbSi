using System;

public class LoginInterface
{
    private readonly DbAccess db;
    private readonly AuthenticationManager auth;

    public LoginInterface(AuthenticationManager auth)
    {
        this.db = new DbAccess();
        this.auth = auth;
    }

    public bool ShowLoginScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== LIV'IN PARIS - AUTHENTIFICATION ===");
        Console.ResetColor();

        Console.WriteLine("1. Se connecter");
        Console.WriteLine("2. Créer un compte");
        Console.WriteLine("0. Quitter");
        Console.Write("\nVotre choix: ");

        string choix = Console.ReadLine();

        switch (choix)
        {
            case "1":
                return ConnexionUtilisateur();
            case "2":
                return CreationCompte();
            case "0":
                return false;
            default:
                Console.WriteLine("Option invalide.");
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                return ShowLoginScreen();
        }
    }

    private bool ConnexionUtilisateur()
    {
        Console.Clear();
        Console.WriteLine("=== CONNEXION UTILISATEUR ===\n");

        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Mot de passe: ");
        string motDePasse = Console.ReadLine();

        if (auth.Connecter(email, motDePasse))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nConnexion réussie!");
            Console.ResetColor();
            Console.WriteLine($"Bienvenue, {email} ({auth.UtilisateurConnecte.Role})");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nEmail ou mot de passe incorrect.");
            Console.ResetColor();
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return ShowLoginScreen();
        }
    }

    private bool CreationCompte()
    {
        Console.Clear();
        Console.WriteLine("=== CRÉATION DE COMPTE ===\n");

        Console.Write("Email: ");
        string email = Console.ReadLine();

        if (db.EmailExiste(email))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nCet email est déjà utilisé.");
            Console.ResetColor();
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return CreationCompte();
        }

        Console.Write("Mot de passe: ");
        string motDePasse = Console.ReadLine();

        Console.Write("Confirmer le mot de passe: ");
        string confirmMotDePasse = Console.ReadLine();

        if (motDePasse != confirmMotDePasse)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nLes mots de passe ne correspondent pas.");
            Console.ResetColor();
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return CreationCompte();
        }

        Console.WriteLine("\nType de compte:");
        Console.WriteLine("1. Client");
        Console.WriteLine("2. Cuisinier");
        Console.WriteLine("3. Administrateur");
        Console.Write("\nVotre choix: ");

        string choixType = Console.ReadLine();
        string role;
        int? idReference = null;

        switch (choixType)
        {
            case "1":
                role = "CLIENT";
                idReference = CreerOuSelectClient();
                break;
            case "2":
                role = "CUISINIER";
                idReference = CreerOuSelectCuisinier();
                break;
            case "3":
                role = "ADMIN";
                break;
            default:
                Console.WriteLine("Option invalide.");
                Console.WriteLine("Appuyez sur une touche pour réessayer...");
                Console.ReadKey();
                return CreationCompte();
        }

        if (idReference == -1)
        {
            return ShowLoginScreen();
        }

        if (auth.CreerCompte(email, motDePasse, role, idReference))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nCompte créé avec succès!");
            Console.ResetColor();
            Console.WriteLine("Vous pouvez maintenant vous connecter.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return ConnexionUtilisateur();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nErreur lors de la création du compte.");
            Console.ResetColor();
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return CreationCompte();
        }
    }

    private int CreerOuSelectClient()
    {
        Console.Clear();
        Console.WriteLine("=== ASSOCIATION CLIENT ===\n");

        Console.WriteLine("1. Sélectionner un client existant");
        Console.WriteLine("2. Créer un nouveau client");
        Console.WriteLine("0. Annuler");
        Console.Write("\nVotre choix: ");

        string choix = Console.ReadLine();

        switch (choix)
        {
            case "1":
                return SelectionnerClient();
            case "2":
                return CreerNouveauClient();
            case "0":
                return -1;
            default:
                Console.WriteLine("Option invalide.");
                Console.WriteLine("Appuyez sur une touche pour réessayer...");
                Console.ReadKey();
                return CreerOuSelectClient();
        }
    }

    private int SelectionnerClient()
    {
        Console.Clear();
        Console.WriteLine("=== SÉLECTION CLIENT ===\n");

        var clients = db.RecupererClients();

        if (clients.Count == 0)
        {
            Console.WriteLine("Aucun client n'existe. Vous devez en créer un nouveau.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return CreerNouveauClient();
        }

        foreach (var client in clients)
        {
            Console.WriteLine($"{client.IdClient}: {client.Nom} {client.Prenom}");
        }

        Console.Write("\nID du client à associer (0 pour annuler): ");
        if (!int.TryParse(Console.ReadLine(), out int idClient) || idClient < 0)
        {
            Console.WriteLine("ID invalide.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return SelectionnerClient();
        }

        if (idClient == 0)
            return -1;

        Client client = db.ObtenirClientID(idClient);
        if (client == null)
        {
            Console.WriteLine("Client non trouvé.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return SelectionnerClient();
        }

        return idClient;
    }

    private int CreerNouveauClient()
    {
        Console.Clear();
        Console.WriteLine("=== CRÉATION CLIENT ===\n");

        Client client = new Client();
        client.IdClient = db.ObtenirProchainIDClient();

        Console.Write("Nom: ");
        client.Nom = Console.ReadLine();

        Console.Write("Prénom: ");
        client.Prenom = Console.ReadLine();

        Console.Write("Numéro de rue: ");
        if (!int.TryParse(Console.ReadLine(), out int numRue))
        {
            Console.WriteLine("Numéro invalide. Réessayez.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return CreerNouveauClient();
        }
        client.NumRue = numRue;

        Console.Write("Rue: ");
        client.NomRue = Console.ReadLine();

        Console.Write("Ville: ");
        client.Ville = Console.ReadLine();

        Console.Write("Métro le plus proche: ");
        client.Metro = Console.ReadLine();

        client.Email = null;  // sera rempli par l'email du compte

        Console.Write("Téléphone: ");
        client.Telephone = Console.ReadLine();

        client.MontantAchat = 0;

        if (db.AjouterClients(client))
        {
            Console.WriteLine("\nClient créé avec succès!");
            return client.IdClient;
        }
        else
        {
            Console.WriteLine("\nErreur lors de la création du client.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return CreerNouveauClient();
        }
    }

    private int CreerOuSelectCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== ASSOCIATION CUISINIER ===\n");

        Console.WriteLine("1. Sélectionner un cuisinier existant");
        Console.WriteLine("2. Créer un nouveau cuisinier");
        Console.WriteLine("0. Annuler");
        Console.Write("\nVotre choix: ");

        string choix = Console.ReadLine();

        switch (choix)
        {
            case "1":
                return SelectionnerCuisinier();
            case "2":
                return CreerNouveauCuisinier();
            case "0":
                return -1;
            default:
                Console.WriteLine("Option invalide.");
                Console.WriteLine("Appuyez sur une touche pour réessayer...");
                Console.ReadKey();
                return CreerOuSelectCuisinier();
        }
    }

    private int SelectionnerCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== SÉLECTION CUISINIER ===\n");

        var cuisiniers = db.RecupererCuisinier();

        if (cuisiniers.Count == 0)
        {
            Console.WriteLine("Aucun cuisinier n'existe. Vous devez en créer un nouveau.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return CreerNouveauCuisinier();
        }

        foreach (var cuisinier in cuisiniers)
        {
            Console.WriteLine($"{cuisinier.IdCuisinier}: {cuisinier.Nom} {cuisinier.Prenom}");
        }

        Console.Write("\nID du cuisinier à associer (0 pour annuler): ");
        if (!int.TryParse(Console.ReadLine(), out int idCuisinier) || idCuisinier < 0)
        {
            Console.WriteLine("ID invalide.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return SelectionnerCuisinier();
        }

        if (idCuisinier == 0)
            return -1;

        Cuisinier cuisinier = db.ObtenirCuisinierID(idCuisinier);
        if (cuisinier == null)
        {
            Console.WriteLine("Cuisinier non trouvé.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return SelectionnerCuisinier();
        }

        return idCuisinier;
    }

    private int CreerNouveauCuisinier()
    {
        Console.Clear();
        Console.WriteLine("=== CRÉATION CUISINIER ===\n");

        Cuisinier cuisinier = new Cuisinier();
        cuisinier.IdCuisinier = db.ObtenirProchainCuisinier();

        Console.Write("Nom: ");
        cuisinier.Nom = Console.ReadLine();

        Console.Write("Prénom: ");
        cuisinier.Prenom = Console.ReadLine();

        Console.Write("Numéro de rue: ");
        if (!int.TryParse(Console.ReadLine(), out int numRue))
        {
            Console.WriteLine("Numéro invalide. Réessayez.");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            return CreerNouveauCuisinier();
        }
        cuisinier.NumRue = numRue;

        Console.Write("Rue: ");
        cuisinier.NomRue = Console.ReadLine();

        Console.Write("Ville: ");
        cuisinier.Ville = Console.ReadLine();

        Console.Write("Métro le plus proche: ");
        cuisinier.Metro = Console.ReadLine();

        cuisinier.Email = null;  // sera rempli par l'email du compte

        Console.Write("Téléphone: ");
        cuisinier.Telephone = Console.ReadLine();

        // Proposition de plats existants
        Console.WriteLine("\nSouhaitez-vous associer un plat existant ?");
        Console.WriteLine("1. Oui");
        Console.WriteLine("2. Non");
        Console.Write("Votre choix: ");

        string choixPlat = Console.ReadLine();
        if (choixPlat == "1")
        {
            var plats = db.RecupererToutPlat();
            if (plats.Count > 0)
            {
                foreach (var plat in plats)
                {
                    Console.WriteLine($"{plat.IdPlat}: {plat.NomPlat} - {plat.PrixParPersonne} €");
                }

                Console.Write("\nID du plat à associer: ");
                if (int.TryParse(Console.ReadLine(), out int idPlat))
                {
                    Plat plat = db.RecuperePlatID(idPlat);
                    if (plat != null)
                    {
                        cuisinier.IdPlat = idPlat;
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucun plat existant. Vous pourrez en associer un plus tard.");
                cuisinier.IdPlat = null;
            }
        }
        else
        {
            cuisinier.IdPlat = null;
        }

        if (db.AjouterCuisinier(cuisinier))
        {
            Console.WriteLine("\nCuisinier créé avec succès!");
            return cuisinier.IdCuisinier;
        }
        else
        {
            Console.WriteLine("\nErreur lors de la création du cuisinier.");
            Console.WriteLine("Appuyez sur une touche pour réessayer...");
            Console.ReadKey();
            return CreerNouveauCuisinier();
        }
    }
}