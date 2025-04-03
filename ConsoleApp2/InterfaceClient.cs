using System;
using System.Collections.Generic;
using Karaté;

public class UserInterface
{
        private readonly DbAccess db;
        
        public UserInterface()
        {
            db = new DbAccess();
        }
        
        public void Run()
        {
            bool exit = false;
            
            while (!exit)
            {
                Console.Clear();
                AfficherMenu();
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1": ClientModule(); break;
                    case "2": CuisinierModule(); break;
                    case "3": PlatModule(); break;
                    case "4": CommandeModule(); break;
                    case "0": exit = true; break;
                    default: 
                        Console.WriteLine("Option invalide.");
                        WaitForKey();
                        break;
                }
            }
            
            db.FermerConnection();
        }
    /// <summary>
    /// affiche le menu principal
    /// </summary>
    private void AfficherMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== LIV'IN PARIS ===");
            Console.ResetColor();
            Console.WriteLine("1. Gestion des Clients");
            Console.WriteLine("2. Gestion des Cuisiniers");
            Console.WriteLine("3. Gestion des Plats");
            Console.WriteLine("3. Gestion des Commandes");
            Console.WriteLine("0. Quitter");
            Console.Write("\nVotre choix: ");
        }

    #region Client

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
                    case "1": AjouterClient(); break;
                    case "2": ModifierClient(); break;
                    case "3": SupprimerClient(); break;
                    case "4": AfficherToutClients(); break;
                    case "0": exit = true; break;
                    default: 
                        Console.WriteLine("Option invalide.");
                        WaitForKey();
                        break;
                }
            }
        }

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
    #endregion

    #region Cuisinier

    private void CuisinierModule()
        {
            bool exit = false;
            
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== MODULE CUISINIER ===");
                Console.ResetColor();
                Console.WriteLine("1. Ajouter un cuisinier");
                Console.WriteLine("2. Afficher tous les cuisiniers");
                Console.WriteLine("0. Retour au menu principal");
                Console.Write("\nVotre choix: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1": AjouterCuisinier(); break;
                    case "2": AfficherToutCuisiniers(); break;
                    case "0": exit = true; break;
                    default: 
                        Console.WriteLine("Option invalide.");
                        WaitForKey();
                        break;
                }
            }
        }
        
        private void AjouterCuisinier()
        {
            Console.Clear();
            Console.WriteLine("=== AJOUTER UN CUISINIER ===\n");
            
            Cuisinier cuisinier = new Cuisinier
            {
                IdCuisinier = db.ObtenirProchainCuisinier()
            };
            
            Console.Write("Nom: ");
            cuisinier.Nom = Console.ReadLine();
            
            Console.Write("Prénom: ");
            cuisinier.Prenom = Console.ReadLine();
            
            Console.Write("Adresse: ");
            cuisinier.Adresse = Console.ReadLine();
            
            Console.Write("Email (optionnel): ");
            cuisinier.Email = Console.ReadLine();
            
            Console.Write("Téléphone (optionnel): ");
            cuisinier.Telephone = Console.ReadLine();
            
            if (db.AjouterCuisinier(cuisinier))
                Console.WriteLine("\nCuisinier ajouté avec succès!");
            else
                Console.WriteLine("\nErreur lors de l'ajout du cuisinier.");
                
            WaitForKey();
        }
        
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
                Console.WriteLine($"\nTotal: {cuisiniers.Count} cuisinier(s)");
            }
            
            WaitForKey();
        }

    #endregion

    #region Plat

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
            Console.WriteLine("5. Afficher tous les plat");
            Console.WriteLine("0. Retour au menu principal");
            Console.Write("\nVotre choix: ");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1": AjouterPlat(); break;
                case "2": ModifierPlat(); break;
                case "3": SupprimerPlat(); break;
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

        if (db.MAJPlat(plat))
            Console.WriteLine("\nPlat modifié avec succès !");
        else
            Console.WriteLine("\nErreur lors de la modification du plat.");

        WaitForKey();
    }

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
                    case "1": CreeCommande(); break;
                    case "2": AfficherCommande(); break;
                    case "3": AfficherToutCommande(); break;
                    case "0": exit = true; break;
                    default: 
                        Console.WriteLine("Option invalide.");
                        WaitForKey();
                        break;
                }
            }
        }


    private void CreeCommande()
        {
            Console.Clear();
            Console.WriteLine("=== CRÉER UNE COMMANDE ===\n");
            
            // Sélection du client
            Console.Write("ID du client: ");
            if (!int.TryParse(Console.ReadLine(), out int clientId))
            {
                Console.WriteLine("ID client invalide.");
                WaitForKey();
                return;
            }
            
            Client client = db.ObtenirClientID(clientId);
            
            if (client == null)
            {
                Console.WriteLine($"Aucun client trouvé avec l'ID {clientId}.");
                WaitForKey();
                return;
            }
            
            // Création de la commande
            Commande commande = new Commande
            {
                IdClient = clientId,
                DateCommande = DateTime.Now,
                StatuCommande = "En cours",
                Lignes = new List<Ligne>()
            };
            
            // Ajout de ligne(s)
            bool addingLines = true;
            decimal totalAmount = 0;
            
            while (addingLines)
            {
                Console.WriteLine("\n=== AJOUTER UNE LIGNE ===");
                
                Ligne ligne = new Ligne
                {
                    Plats = new List<Plat>()
                };
                
                // Sélection du plat
                Console.Write("ID du plat: ");
                if (!int.TryParse(Console.ReadLine(), out int platId))
                {
                    Console.WriteLine("ID plat invalide.");
                    continue;
                }
                
                Plat plat = db.RecuperePlatID(platId);
                
                if (plat == null)
                {
                    Console.WriteLine($"Aucun plat trouvé avec l'ID {platId}.");
                    continue;
                }
                
                // Quantité
                Console.Write("Quantité: ");
                if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0)
                {
                    Console.WriteLine("Quantité invalide.");
                    continue;
                }
                
                ligne.Quantite = quantite;
                ligne.Plats.Add(plat);
                ligne.PrixTotal = plat.PrixParPersonne * quantite;
                totalAmount += ligne.PrixTotal;
                
                // Date de livraison (optionnelle)
                Console.Write("Date de livraison (JJ/MM/AAAA): ");
                string dateLivraisonStr = Console.ReadLine();
                if (DateTime.TryParse(dateLivraisonStr, out DateTime dateLivraison))
                {
                    ligne.DateLivraison = dateLivraison;
                }
                
                // Lieu de livraison
                Console.Write("Lieu de livraison: ");
                ligne.Lieu = Console.ReadLine();
                
                commande.Lignes.Add(ligne);
                
                Console.Write("\nAjouter une autre ligne? (O/N): ");
                addingLines = Console.ReadLine().ToUpper() == "O";
            }
            
            commande.Montant = totalAmount;
            
            // Mode de paiement
            Console.Write("\nMode de paiement: ");
            commande.Paiement = Console.ReadLine();
            
            // Confirmation
            Console.WriteLine($"\nTotal de la commande: {commande.Montant}€");
            Console.Write("Confirmer la commande? (O/N): ");
            
            if (Console.ReadLine().ToUpper() != "O")
            {
                Console.WriteLine("Création de commande annulée.");
                WaitForKey();
                return;
            }
            
            int commandeId = db.AjouterCommande(commande);
            
            if (commandeId > 0)
                Console.WriteLine($"\nCommande #{commandeId} créée avec succès!");
            else
                Console.WriteLine("\nErreur lors de la création de la commande.");
                
            WaitForKey();
        }
    /// <summary>
    /// affiche la commande
    /// </summary>
    private void AfficherCommande()
    {
        Console.Clear();
        Console.WriteLine("=== DÉTAILS D'UNE COMMANDE ===\n");

        Console.Write("ID de la commande: ");
        int commandeId;
        if (!int.TryParse(Console.ReadLine(), out commandeId))
        {
            Console.WriteLine("ID commande invalide.");
            WaitForKey();
            return;
        }

        Commande commande = db.RecupereCommandeID(commandeId);

        if (commande == null)
        {
            Console.WriteLine("Aucune commande trouvée avec l'ID " + commandeId + ".");
            WaitForKey();
            return;
        }

        Client client = db.ObtenirClientID(commande.IdClient);

        Console.WriteLine("\nCommande #" + commande.IdCommande);
        Console.WriteLine("Date: " + commande.DateCommande);

        string clientString;
        if (client == null)
        {
            clientString = "Inconnu";
        }
        else
        {
            clientString = client.ToString();
        }
        Console.WriteLine("Client: " + clientString);

        Console.WriteLine("Statut: " + commande.StatuCommande);
        Console.WriteLine("Montant: " + commande.Montant + "€");

        string paiementStr;
        if (commande.Paiement == null)
        {
            paiementStr = "Non défini";
        }
        else
        {
            paiementStr = commande.Paiement;
        }
        Console.WriteLine("Paiement: " + paiementStr);

        Console.WriteLine("\nLignes de commande:");
        foreach (var ligne in commande.Lignes)
        {
            // Ici, on suppose que chaque ligne comporte au moins un plat dans la liste.
            string platNom = "";
            if (ligne.Plats != null && ligne.Plats.Count > 0)
            {
                platNom = ligne.Plats[0].NomPlat;
            }
            Console.WriteLine("- Ligne #" + ligne.IdLigne + ": " + ligne.Quantite + "x " + platNom + " = " + ligne.PrixTotal + "€");

            string livraisonStr;
            if (!ligne.DateLivraison.HasValue)
            {
                livraisonStr = "Non définie";
            }
            else
            {
                livraisonStr = ligne.DateLivraison.Value.ToString("dd/MM/yyyy");
            }

            string lieuStr;
            if (ligne.Lieu == null)
            {
                lieuStr = "Non défini";
            }
            else
            {
                lieuStr = ligne.Lieu;
            }
            Console.WriteLine("  Livraison: " + livraisonStr + " - " + lieuStr);
        }

        WaitForKey();
    }

    private void AfficherToutCommande()
    {
        Console.Clear();
        Console.WriteLine("=== LISTE DES COMMANDES ===\n");

        List<Commande> commandes = db.RecupererToutCommandes();

        if (commandes.Count == 0)
        {
            Console.WriteLine("Aucune commande trouvée.");
        }
        else
        {
            foreach (Commande commande in commandes)
            {
                Client client = db.ObtenirClientID(commande.IdClient);
                string clientString;
                if (client == null)
                {
                    clientString = "Inconnu";
                }
                else
                {
                    clientString = client.ToString();
                }
                Console.WriteLine(commande.ToString() + " - Client: " + clientString + " - " + commande.StatuCommande);
            }
            Console.WriteLine("\nTotal: " + commandes.Count + " commande(s)");
        }

        WaitForKey();
    }


    #endregion 
    private void WaitForKey()
        {
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
    }
