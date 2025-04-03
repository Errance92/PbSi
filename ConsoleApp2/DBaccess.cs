using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

public class DbAccess
{
    private readonly string connectionString;
    private MySqlConnection connection;
    public DbAccess()
    {
        connectionString = "Server=localhost;Port=3306;Database=Liv'in Paris;Uid=root;Pwd=password;CharSet=utf8;";
        Console.WriteLine("Connexion à la base de données initialisée");
    }

    private MySqlConnection Connection()
    {
        try
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
                Console.WriteLine("Connexion créée.");
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Console.WriteLine("Connexion ouverte.");
            }

            return connection;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Erreur lors de la connexion à la base de données: " + ex.Message);
            throw;
        }
    }

    public void FermerConnection()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            Console.WriteLine("Connexion fermée.");
        }
    }

    public DataTable ExecuterRequete(string requete, params MySqlParameter[] parametres)
    {
        using (var commande = new MySqlCommand(requete, Connection()))
        {
            if (parametres != null)
                commande.Parameters.AddRange(parametres);

            var dataTable = new DataTable();
            using (var adapter = new MySqlDataAdapter(commande))
            {
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
    }

    public int ExecuterRequeteMAJ(string requete, params MySqlParameter[] parametres)
    {
        using (var commande = new MySqlCommand(requete, Connection()))
        {
            if (parametres != null)
                commande.Parameters.AddRange(parametres);

            return commande.ExecuteNonQuery();
        }
    }

    public object ExecuterRequeteScalaire(string requete, params MySqlParameter[] parametres)
    {
        using (var commande = new MySqlCommand(requete, Connection()))
        {
            if (parametres != null)
                commande.Parameters.AddRange(parametres);

            return commande.ExecuteScalar();
        }
    }

    // ===== MÉTHODES POUR CLIENTS =====

    public List<Client> RecupererClients()
    {
        var clients = new List<Client>();
        var table = ExecuterRequete("SELECT * FROM Client");

        foreach (DataRow row in table.Rows)
        {
            Client client = new Client();
            client.IdClient = Convert.ToInt32(row["id_client"]);
            client.Nom = row["nom"].ToString();
            client.Prenom = row["prenom"].ToString();
            client.Adresse = row["addresse"].ToString();

            if (row["email"] == DBNull.Value)
            {
                client.Email = null;
            }
            else
            {
                client.Email = row["email"].ToString();
            }

            if (row["telephone"] == DBNull.Value)
            {
                client.Telephone = null;
            }
            else
            {
                client.Telephone = row["telephone"].ToString();
            }

            clients.Add(client);
        }

        return clients;
    }

    public Client ObtenirClientID(int id)
    {
        var table = ExecuterRequete("SELECT * FROM Client WHERE id_client = @id",
            new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
            return null;

        var row = table.Rows[0];
        Client client = new Client();
        client.IdClient = Convert.ToInt32(row["id_client"]);
        client.Nom = row["nom"].ToString();
        client.Prenom = row["prenom"].ToString();
        client.Adresse = row["addresse"].ToString();

        if (row["email"] == DBNull.Value)
        {
            client.Email = null;
        }
        else
        {
            client.Email = row["email"].ToString();
        }

        if (row["telephone"] == DBNull.Value)
        {
            client.Telephone = null;
        }
        else
        {
            client.Telephone = row["telephone"].ToString();
        }

        return client;
    }

    public bool AjouterClients(Client client)
    {
        string requete = @"INSERT INTO Client (id_client, nom, prenom, addresse, email, telephone)
                     VALUES (@id, @nom, @prenom, @adresse, @email, @telephone)";

        MySqlParameter paramId = new MySqlParameter("@id", client.IdClient);
        MySqlParameter paramNom = new MySqlParameter("@nom", client.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", client.Prenom);
        MySqlParameter paramAdresse = new MySqlParameter("@adresse", client.Adresse);

        MySqlParameter paramEmail;
        if (client.Email == null)
        {
            paramEmail = new MySqlParameter("@email", DBNull.Value);
        }
        else
        {
            paramEmail = new MySqlParameter("@email", client.Email);
        }

        MySqlParameter paramTelephone;
        if (client.Telephone == null)
        {
            paramTelephone = new MySqlParameter("@telephone", DBNull.Value);
        }
        else
        {
            paramTelephone = new MySqlParameter("@telephone", client.Telephone);
        }

        var parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramAdresse,
        paramEmail,
        paramTelephone
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }

    public bool MAJClient(Client client)
    {
        string requete = @"UPDATE Client SET nom = @nom, prenom = @prenom, addresse = @adresse, 
                     email = @email, telephone = @telephone WHERE id_client = @id";

        MySqlParameter paramId = new MySqlParameter("@id", client.IdClient);
        MySqlParameter paramNom = new MySqlParameter("@nom", client.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", client.Prenom);
        MySqlParameter paramAdresse = new MySqlParameter("@adresse", client.Adresse);

        MySqlParameter paramEmail;
        if (client.Email == null)
        {
            paramEmail = new MySqlParameter("@email", DBNull.Value);
        }
        else
        {
            paramEmail = new MySqlParameter("@email", client.Email);
        }

        MySqlParameter paramTelephone;
        if (client.Telephone == null)
        {
            paramTelephone = new MySqlParameter("@telephone", DBNull.Value);
        }
        else
        {
            paramTelephone = new MySqlParameter("@telephone", client.Telephone);
        }

        var parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramAdresse,
        paramEmail,
        paramTelephone
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }

    public bool SupprimerClient(int id)
    {
        return ExecuterRequeteMAJ("DELETE FROM Client WHERE id_client = @id",
                                 new MySqlParameter("@id", id)) > 0;
    }

    public int ObtenirProchainClient()
    {
        object resultat = ExecuterRequeteScalaire("SELECT MAX(id_client) FROM Client");
        if (resultat == DBNull.Value)
        {
            return 1;
        }
        else
        {
            return Convert.ToInt32(resultat) + 1;
        }
    }

    public static List<Client> TrierParNom(List<Client> clients)
    {
        clients.Sort(ComparerParNom);
        return clients;
    }

    private static int ComparerParNom(Client a, Client b)
    {
        return a.Nom.CompareTo(b.Nom);
    }

    public static List<Client> TrierParAdress(List<Client> clients)
    {
        clients.Sort(ComparerParAdress);
        return clients;
    }

    private static int ComparerParAdress(Client a, Client b)
    {
        return a.Adresse.CompareTo(b.Nom);
    }

    // ===== MÉTHODES POUR CUISINIERS =====

    public List<Cuisinier> RecupererCuisinier()
    {
        var cuisiniers = new List<Cuisinier>();
        var table = ExecuterRequete("SELECT * FROM Cuisinier");

        foreach (DataRow row in table.Rows)
        {
            Cuisinier c = new Cuisinier();
            c.IdCuisinier = Convert.ToInt32(row["id_cuisinier"]);
            c.Nom = row["nom"].ToString();
            c.Prenom = row["prenom"].ToString();
            c.Adresse = row["addresse"].ToString();

            if (row["email"] == DBNull.Value)
            {
                c.Email = null;
            }
            else
            {
                c.Email = row["email"].ToString();
            }

            if (row["telephone"] == DBNull.Value)
            {
                c.Telephone = null;
            }
            else
            {
                c.Telephone = row["telephone"].ToString();
            }

            cuisiniers.Add(c);
        }

        return cuisiniers;
    }

    public Cuisinier ObtenirCuisinierID(int id)
    {
        var table = ExecuterRequete("SELECT * FROM Cuisinier WHERE id_cuisinier = @id",
                                     new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
            return null;

        DataRow row = table.Rows[0];
        Cuisinier c = new Cuisinier();
        c.IdCuisinier = Convert.ToInt32(row["id_cuisinier"]);
        c.Nom = row["nom"].ToString();
        c.Prenom = row["prenom"].ToString();
        c.Adresse = row["addresse"].ToString();

        if (row["email"] == DBNull.Value)
        {
            c.Email = null;
        }
        else
        {
            c.Email = row["email"].ToString();
        }

        if (row["telephone"] == DBNull.Value)
        {
            c.Telephone = null;
        }
        else
        {
            c.Telephone = row["telephone"].ToString();
        }

        return c;
    }

    public bool AjouterCuisinier(Cuisinier cuisinier)
    {
        string requete = @"INSERT INTO Cuisinier (id_cuisinier, nom, prenom, addresse, email, telephone)
                     VALUES (@id, @nom, @prenom, @adresse, @email, @telephone)";

        MySqlParameter paramId = new MySqlParameter("@id", cuisinier.IdCuisinier);
        MySqlParameter paramNom = new MySqlParameter("@nom", cuisinier.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", cuisinier.Prenom);
        MySqlParameter paramAdresse = new MySqlParameter("@adresse", cuisinier.Adresse);

        MySqlParameter paramEmail;
        if (cuisinier.Email == null)
        {
            paramEmail = new MySqlParameter("@email", DBNull.Value);
        }
        else
        {
            paramEmail = new MySqlParameter("@email", cuisinier.Email);
        }

        MySqlParameter paramTelephone;
        if (cuisinier.Telephone == null)
        {
            paramTelephone = new MySqlParameter("@telephone", DBNull.Value);
        }
        else
        {
            paramTelephone = new MySqlParameter("@telephone", cuisinier.Telephone);
        }

        var parametres = new MySqlParameter[]
        {
        paramId, paramNom, paramPrenom, paramAdresse, paramEmail, paramTelephone
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }

    public int ObtenirProchainCuisinier()
    {
        object result = ExecuterRequeteScalaire("SELECT MAX(id_cuisinier) FROM Cuisinier");
        if (result == DBNull.Value)
        {
            return 1;
        }
        else
        {
            return Convert.ToInt32(result) + 1;
        }
    }

    // ===== MÉTHODES POUR PLATS =====

    public List<Plat> RecupererToutPlat()
    {
        var plats = new List<Plat>();
        DataTable table = ExecuterRequete("SELECT * FROM Plat");

        foreach (DataRow row in table.Rows)
        {
            Plat plat = new Plat();
            plat.IdPlat = Convert.ToInt32(row["id_plat"]);
            plat.NomPlat = row["nom_plat"].ToString();
            plat.Type = row["type"].ToString();
            plat.Stock = Convert.ToInt32(row["stock"]);

            if (row["origine"] == DBNull.Value)
            {
                plat.Origine = null;
            }
            else
            {
                plat.Origine = row["origine"].ToString();
            }

            if (row["regime_alimentaire"] == DBNull.Value)
            {
                plat.RegimeAlimentaire = null;
            }
            else
            {
                plat.RegimeAlimentaire = row["regime_alimentaire"].ToString();
            }

            if (row["ingredient"] == DBNull.Value)
            {
                plat.Ingredient = null;
            }
            else
            {
                plat.Ingredient = row["ingredient"].ToString();
            }

            if (row["lien_photo"] == DBNull.Value)
            {
                plat.LienPhoto = null;
            }
            else
            {
                plat.LienPhoto = row["lien_photo"].ToString();
            }

            if (row["date_fabrication"] == DBNull.Value)
            {
                plat.DateFabrication = DateTime.MinValue;
            }
            else
            {
                plat.DateFabrication = Convert.ToDateTime(row["date_fabrication"]);
            }

            plat.PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]);

            if (row["date_peremption"] == DBNull.Value)
            {
                plat.DatePeremption = DateTime.MinValue;
            }
            else
            {
                plat.DatePeremption = Convert.ToDateTime(row["date_peremption"]);
            }

            plats.Add(plat);
        }

        return plats;
    }

    public Plat RecuperePlatID(int id)
    {
        DataTable table = ExecuterRequete("SELECT * FROM Plat WHERE id_plat = @id",
            new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
            return null;

        DataRow row = table.Rows[0];
        Plat plat = new Plat();
        plat.IdPlat = Convert.ToInt32(row["id_plat"]);
        plat.NomPlat = row["nom_plat"].ToString();
        plat.Type = row["type"].ToString();
        plat.Stock = Convert.ToInt32(row["stock"]);

        if (row["origine"] == DBNull.Value)
        {
            plat.Origine = null;
        }
        else
        {
            plat.Origine = row["origine"].ToString();
        }

        if (row["regime_alimentaire"] == DBNull.Value)
        {
            plat.RegimeAlimentaire = null;
        }
        else
        {
            plat.RegimeAlimentaire = row["regime_alimentaire"].ToString();
        }

        if (row["ingredient"] == DBNull.Value)
        {
            plat.Ingredient = null;
        }
        else
        {
            plat.Ingredient = row["ingredient"].ToString();
        }

        if (row["lien_photo"] == DBNull.Value)
        {
            plat.LienPhoto = null;
        }
        else
        {
            plat.LienPhoto = row["lien_photo"].ToString();
        }

        if (row["date_fabrication"] == DBNull.Value)
        {
            plat.DateFabrication = DateTime.MinValue;
        }
        else
        {
            plat.DateFabrication = Convert.ToDateTime(row["date_fabrication"]);
        }

        plat.PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]);

        if (row["date_peremption"] == DBNull.Value)
        {
            plat.DatePeremption = DateTime.MinValue;
        }
        else
        {
            plat.DatePeremption = Convert.ToDateTime(row["date_peremption"]);
        }

        return plat;
    }

    public int RecupererPlatSuivant()
    {
        object result = ExecuterRequeteScalaire("SELECT MAX(id_plat) FROM Plat");
        if (result == DBNull.Value)
        {
            return 1;
        }
        else
        {
            return Convert.ToInt32(result) + 1;
        }
    }

    public bool AjouterPlat(Plat plat)
    {
        string requete = @"INSERT INTO Plat (id_plat, nom_plat, type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption)
                     VALUES (@id, @nom, @type, @stock, @origine, @regime, @ingredient, @lien, @dateFab, @prix, @datePer)";

        MySqlParameter paramId = new MySqlParameter("@id", plat.IdPlat);
        MySqlParameter paramNom = new MySqlParameter("@nom", plat.NomPlat);
        MySqlParameter paramType = new MySqlParameter("@type", plat.Type);
        MySqlParameter paramStock = new MySqlParameter("@stock", plat.Stock);

        MySqlParameter paramOrigine;
        if (plat.Origine == null)
        {
            paramOrigine = new MySqlParameter("@origine", DBNull.Value);
        }
        else
        {
            paramOrigine = new MySqlParameter("@origine", plat.Origine);
        }

        MySqlParameter paramRegime;
        if (plat.RegimeAlimentaire == null)
        {
            paramRegime = new MySqlParameter("@regime", DBNull.Value);
        }
        else
        {
            paramRegime = new MySqlParameter("@regime", plat.RegimeAlimentaire);
        }

        MySqlParameter paramIngredient;
        if (plat.Ingredient == null)
        {
            paramIngredient = new MySqlParameter("@ingredient", DBNull.Value);
        }
        else
        {
            paramIngredient = new MySqlParameter("@ingredient", plat.Ingredient);
        }

        MySqlParameter paramLien;
        if (plat.LienPhoto == null)
        {
            paramLien = new MySqlParameter("@lien", DBNull.Value);
        }
        else
        {
            paramLien = new MySqlParameter("@lien", plat.LienPhoto);
        }

        MySqlParameter paramDateFab;
        if (plat.DateFabrication == DateTime.MinValue)
        {
            paramDateFab = new MySqlParameter("@dateFab", DBNull.Value);
        }
        else
        {
            paramDateFab = new MySqlParameter("@dateFab", plat.DateFabrication);
        }

        MySqlParameter paramPrix = new MySqlParameter("@prix", plat.PrixParPersonne);

        MySqlParameter paramDatePer;
        if (plat.DatePeremption == DateTime.MinValue)
        {
            paramDatePer = new MySqlParameter("@datePer", DBNull.Value);
        }
        else
        {
            paramDatePer = new MySqlParameter("@datePer", plat.DatePeremption);
        }

        MySqlParameter[] parametres = new MySqlParameter[]
        {
        paramId, paramNom, paramType, paramStock, paramOrigine, paramRegime, paramIngredient, paramLien, paramDateFab, paramPrix, paramDatePer
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }

    // ===== MÉTHODES POUR COMMANDES =====


    public List<Commande> RecupererToutCommandes()
    {
        var commandes = new List<Commande>();
        DataTable table = ExecuterRequete("SELECT * FROM Commande ORDER BY date_commande DESC");

        foreach (DataRow row in table.Rows)
        {
            int idCommande = Convert.ToInt32(row["id_commande"]);

            string paiement;
            if (row["paiement"] == DBNull.Value)
            {
                paiement = null;
            }
            else
            {
                paiement = row["paiement"].ToString();
            }

            Commande commande = new Commande();
            commande.IdCommande = idCommande;
            commande.StatuCommande = row["statu_commande"].ToString();
            commande.DateCommande = Convert.ToDateTime(row["date_commande"]);
            commande.Montant = Convert.ToDecimal(row["montant"]);
            commande.Paiement = paiement;
            commande.IdClient = Convert.ToInt32(row["id_client"]);
            commande.Lignes = LignesCommandes(idCommande);

            commandes.Add(commande);
        }

        return commandes;
    }

    public Commande RecupereCommandeID(int id)
    {
        DataTable table = ExecuterRequete("SELECT * FROM Commande WHERE id_commande = @id",
                                           new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
            return null;

        DataRow row = table.Rows[0];
        Commande commande = new Commande();
        commande.IdCommande = id;
        commande.StatuCommande = row["statu_commande"].ToString();
        commande.DateCommande = Convert.ToDateTime(row["date_commande"]);
        commande.Montant = Convert.ToDecimal(row["montant"]);

        if (row["paiement"] == DBNull.Value)
        {
            commande.Paiement = null;
        }
        else
        {
            commande.Paiement = row["paiement"].ToString();
        }

        commande.IdClient = Convert.ToInt32(row["id_client"]);
        commande.Lignes = LignesCommandes(id);

        return commande;
    }

    public List<Ligne> LignesCommandes(int commandeId)
    {
        var lignes = new List<Ligne>();
        DataTable table = ExecuterRequete("SELECT * FROM Ligne WHERE id_commande = @id",
            new MySqlParameter("@id", commandeId));

        foreach (DataRow row in table.Rows)
        {
            int idLigne = Convert.ToInt32(row["id_ligne"]);
            int quantite = Convert.ToInt32(row["quantite"]);
            decimal prixTotal = Convert.ToDecimal(row["prix_total"]);

            DateTime? dateLivraison;
            if (row["date_livraison"] == DBNull.Value)
            {
                dateLivraison = null;
            }
            else
            {
                dateLivraison = Convert.ToDateTime(row["date_livraison"]);
            }

            string lieu;
            if (row["lieu"] == DBNull.Value)
            {
                lieu = null;
            }
            else
            {
                lieu = row["lieu"].ToString();
            }

            Ligne ligne = new Ligne();
            ligne.IdLigne = idLigne;
            ligne.Quantite = quantite;
            ligne.PrixTotal = prixTotal;
            ligne.DateLivraison = dateLivraison;
            ligne.Lieu = lieu;
            ligne.IdCommande = commandeId;
            ligne.Plats = PlatLigne(idLigne);

            lignes.Add(ligne);
        }

        return lignes;
    }

    public List<Plat> PlatLigne(int ligneId)
    {
        var plats = new List<Plat>();
        string requete = @"SELECT p.* FROM Plat p 
                       JOIN Ligne_Plat lp ON p.id_plat = lp.id_plat 
                       WHERE lp.id_ligne = @id";
        DataTable table = ExecuterRequete(requete, new MySqlParameter("@id", ligneId));

        foreach (DataRow row in table.Rows)
        {
            Plat plat = new Plat();
            plat.IdPlat = Convert.ToInt32(row["id_plat"]);
            plat.NomPlat = row["nom_plat"].ToString();
            plat.Type = row["type"].ToString();
            plat.Stock = Convert.ToInt32(row["stock"]);

            if (row["origine"] == DBNull.Value)
            {
                plat.Origine = null;
            }
            else
            {
                plat.Origine = row["origine"].ToString();
            }

            if (row["regime_alimentaire"] == DBNull.Value)
            {
                plat.RegimeAlimentaire = null;
            }
            else
            {
                plat.RegimeAlimentaire = row["regime_alimentaire"].ToString();
            }

            if (row["ingredient"] == DBNull.Value)
            {
                plat.Ingredient = null;
            }
            else
            {
                plat.Ingredient = row["ingredient"].ToString();
            }

            if (row["lien_photo"] == DBNull.Value)
            {
                plat.LienPhoto = null;
            }
            else
            {
                plat.LienPhoto = row["lien_photo"].ToString();
            }

            if (row["date_fabrication"] == DBNull.Value)
            {
                plat.DateFabrication = DateTime.MinValue;
            }
            else
            {
                plat.DateFabrication = Convert.ToDateTime(row["date_fabrication"]);
            }

            plat.PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]);

            if (row["date_peremption"] == DBNull.Value)
            {
                plat.DatePeremption = DateTime.MinValue;
            }
            else
            {
                plat.DatePeremption = Convert.ToDateTime(row["date_peremption"]);
            }

            plats.Add(plat);
        }

        return plats;
    }

    public int AjouterCommande(Commande commande)
    {
        // Vérifier si le client existe
        if (ObtenirClientID(commande.IdClient) == null)
            return -1;

        // Commencer une transaction
        using (var transaction = Connection().BeginTransaction())
        {
            try
            {
                // Insérer la commande
                string query = @"INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client)
                             VALUES (@id, @status, @date, @montant, @paiement, @idClient)";

                int nextId = RecupereProchaineCommande();

                MySqlParameter paramId = new MySqlParameter("@id", nextId);
                MySqlParameter paramStatus = new MySqlParameter("@status", commande.StatuCommande);
                MySqlParameter paramDate = new MySqlParameter("@date", commande.DateCommande);
                MySqlParameter paramMontant = new MySqlParameter("@montant", commande.Montant);
                MySqlParameter paramIdClient = new MySqlParameter("@idClient", commande.IdClient);

                MySqlParameter paramPaiement;
                if (commande.Paiement == null)
                {
                    paramPaiement = new MySqlParameter("@paiement", DBNull.Value);
                }
                else
                {
                    paramPaiement = new MySqlParameter("@paiement", commande.Paiement);
                }

                var parameters = new MySqlParameter[]
                {
                paramId, paramStatus, paramDate, paramMontant, paramPaiement, paramIdClient
                };

                ExecuterRequeteMAJ(query, parameters);

                // Ajouter les lignes de commande
                foreach (var ligne in commande.Lignes)
                {
                    int nextLigneId = RecupereProchaineLigne();

                    // Insérer la ligne
                    string ligneQuery = @"INSERT INTO Ligne (id_ligne, quantite, prix_total, date_livraison, lieu, id_commande)
                                      VALUES (@id, @quantite, @prixTotal, @dateLivraison, @lieu, @idCommande)";

                    MySqlParameter paramLigneId = new MySqlParameter("@id", nextLigneId);
                    MySqlParameter paramQuantite = new MySqlParameter("@quantite", ligne.Quantite);
                    MySqlParameter paramPrixTotal = new MySqlParameter("@prixTotal", ligne.PrixTotal);

                    MySqlParameter paramDateLivraison;
                    if (!ligne.DateLivraison.HasValue)
                    {
                        paramDateLivraison = new MySqlParameter("@dateLivraison", DBNull.Value);
                    }
                    else
                    {
                        paramDateLivraison = new MySqlParameter("@dateLivraison", ligne.DateLivraison.Value);
                    }

                    MySqlParameter paramLieu;
                    if (ligne.Lieu == null)
                    {
                        paramLieu = new MySqlParameter("@lieu", DBNull.Value);
                    }
                    else
                    {
                        paramLieu = new MySqlParameter("@lieu", ligne.Lieu);
                    }

                    MySqlParameter paramIdCommande = new MySqlParameter("@idCommande", nextId);

                    var ligneParams = new MySqlParameter[]
                    {
                    paramLigneId, paramQuantite, paramPrixTotal, paramDateLivraison, paramLieu, paramIdCommande
                    };

                    ExecuterRequeteMAJ(ligneQuery, ligneParams);

                    // Associer les plats à la ligne
                    foreach (var plat in ligne.Plats)
                    {
                        string platQuery = "INSERT INTO Ligne_Plat (id_ligne, id_plat) VALUES (@idLigne, @idPlat)";

                        MySqlParameter paramIdLigne = new MySqlParameter("@idLigne", nextLigneId);
                        MySqlParameter paramIdPlat = new MySqlParameter("@idPlat", plat.IdPlat);

                        ExecuterRequeteMAJ(platQuery, paramIdLigne, paramIdPlat);
                    }
                }

                transaction.Commit();
                return nextId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public int RecupereProchaineCommande()
    {
        object result = ExecuterRequeteScalaire("SELECT MAX(id_commande) FROM Commande");
        if (result == DBNull.Value)
        {
            return 1;
        }
        else
        {
            return Convert.ToInt32(result) + 1;
        }
    }

    public int RecupereProchaineLigne()
    {
        object result = ExecuterRequeteScalaire("SELECT MAX(id_ligne) FROM Ligne");
        if (result == DBNull.Value)
        {
            return 1;
        }
        else
        {
            return Convert.ToInt32(result) + 1;
        }
    }
}