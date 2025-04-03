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
        // Chaîne de connexion codée directement dans la classe - REMPLACEZ PAR VOS VALEURS
        connectionString = "Server=localhost;Port=3306;Database=Liv'in Paris;Uid=root;Pwd=password;CharSet=utf8;";

        // Pour le débogage, affichez un message
        Console.WriteLine("Connexion à la base de données initialisée...");
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

    public List<Plat> GetAllPlats()
        {
            var plats = new List<Plat>();
            var table = ExecuterRequete("SELECT * FROM Plat");
            
            foreach (DataRow row in table.Rows)
            {
                plats.Add(MapRowToPlat(row));
            }
            
            return plats;
        }
        
        public Plat GetPlatById(int id)
        {
            var table = ExecuterRequete("SELECT * FROM Plat WHERE id_plat = @id", 
                new MySqlParameter("@id", id));
                
            if (table.Rows.Count == 0)
                return null;
                
            return MapRowToPlat(table.Rows[0]);
        }
        
        private Plat MapRowToPlat(DataRow row)
        {
            return new Plat
            {
                IdPlat = Convert.ToInt32(row["id_plat"]),
                NomPlat = row["nom_plat"].ToString(),
                Type = row["type"].ToString(),
                Stock = Convert.ToInt32(row["stock"]),
                Origine = row["origine"] == DBNull.Value ? null : row["origine"].ToString(),
                RegimeAlimentaire = row["regime_alimentaire"] == DBNull.Value ? null : row["regime_alimentaire"].ToString(),
                Ingredient = row["ingredient"] == DBNull.Value ? null : row["ingredient"].ToString(),
                LienPhoto = row["lien_photo"] == DBNull.Value ? null : row["lien_photo"].ToString(),
                DateFabrication = row["date_fabrication"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["date_fabrication"]),
                PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]),
                DatePeremption = row["date_peremption"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["date_peremption"])
            };
        }

    public int GetNextPlatId()
    {
        object result = ExecuterRequeteScalaire("SELECT MAX(id_plat) FROM Plat");
        return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
    }

    public bool AddPlat(Plat plat)
    {
        string query = @"INSERT INTO Plat (id_plat, nom_plat, type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption)
                     VALUES (@id, @nom, @type, @stock, @origine, @regime, @ingredient, @lien, @dateFab, @prix, @datePer)";

        var parameters = new MySqlParameter[]
        {
        new MySqlParameter("@id", plat.IdPlat),
        new MySqlParameter("@nom", plat.NomPlat),
        new MySqlParameter("@type", plat.Type),
        new MySqlParameter("@stock", plat.Stock),
        new MySqlParameter("@origine", plat.Origine ?? (object)DBNull.Value),
        new MySqlParameter("@regime", plat.RegimeAlimentaire ?? (object)DBNull.Value),
        new MySqlParameter("@ingredient", plat.Ingredient ?? (object)DBNull.Value),
        new MySqlParameter("@lien", plat.LienPhoto ?? (object)DBNull.Value),
        new MySqlParameter("@dateFab", plat.DateFabrication == DateTime.MinValue ? (object)DBNull.Value : plat.DateFabrication),
        new MySqlParameter("@prix", plat.PrixParPersonne),
        new MySqlParameter("@datePer", plat.DatePeremption == DateTime.MinValue ? (object)DBNull.Value : plat.DatePeremption)
        };

        return ExecuterRequeteMAJ(query, parameters) > 0;
    }

    // ===== MÉTHODES POUR COMMANDES =====


    public List<Commande> GetAllCommandes()
        {
            var commandes = new List<Commande>();
            var table = ExecuterRequete("SELECT * FROM Commande ORDER BY date_commande DESC");
            
            foreach (DataRow row in table.Rows)
            {
                int idCommande = Convert.ToInt32(row["id_commande"]);
                commandes.Add(new Commande
                {
                    IdCommande = idCommande,
                    StatuCommande = row["statu_commande"].ToString(),
                    DateCommande = Convert.ToDateTime(row["date_commande"]),
                    Montant = Convert.ToDecimal(row["montant"]),
                    Paiement = row["paiement"] == DBNull.Value ? null : row["paiement"].ToString(),
                    IdClient = Convert.ToInt32(row["id_client"]),
                    Lignes = GetLignesForCommande(idCommande)
                });
            }
            
            return commandes;
        }
        
        public Commande GetCommandeById(int id)
        {
            var table = ExecuterRequete("SELECT * FROM Commande WHERE id_commande = @id", 
                new MySqlParameter("@id", id));
                
            if (table.Rows.Count == 0)
                return null;
                
            var row = table.Rows[0];
            return new Commande
            {
                IdCommande = id,
                StatuCommande = row["statu_commande"].ToString(),
                DateCommande = Convert.ToDateTime(row["date_commande"]),
                Montant = Convert.ToDecimal(row["montant"]),
                Paiement = row["paiement"] == DBNull.Value ? null : row["paiement"].ToString(),
                IdClient = Convert.ToInt32(row["id_client"]),
                Lignes = GetLignesForCommande(id)
            };
        }
        
        public List<Ligne> GetLignesForCommande(int commandeId)
        {
            var lignes = new List<Ligne>();
            var table = ExecuterRequete("SELECT * FROM Ligne WHERE id_commande = @id", 
                new MySqlParameter("@id", commandeId));
                
            foreach (DataRow row in table.Rows)
            {
                int idLigne = Convert.ToInt32(row["id_ligne"]);
                lignes.Add(new Ligne
                {
                    IdLigne = idLigne,
                    Quantite = Convert.ToInt32(row["quantite"]),
                    PrixTotal = Convert.ToDecimal(row["prix_total"]),
                    DateLivraison = row["date_livraison"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["date_livraison"]),
                    Lieu = row["lieu"] == DBNull.Value ? null : row["lieu"].ToString(),
                    IdCommande = commandeId,
                    Plats = GetPlatsForLigne(idLigne)
                });
            }
            
            return lignes;
        }
        
        public List<Plat> GetPlatsForLigne(int ligneId)
        {
            var plats = new List<Plat>();
            var query = @"SELECT p.* FROM Plat p 
                        JOIN Ligne_Plat lp ON p.id_plat = lp.id_plat 
                        WHERE lp.id_ligne = @id";
                        
            var table = ExecuterRequete(query, new MySqlParameter("@id", ligneId));
            
            foreach (DataRow row in table.Rows)
            {
                plats.Add(MapRowToPlat(row));
            }
            
            return plats;
        }

    public int AddCommande(Commande commande)
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

                int nextId = GetNextCommandeId();

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
                    int nextLigneId = GetNextLigneId();

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

    public int GetNextCommandeId()
        {
            object result = ExecuterRequeteScalaire("SELECT MAX(id_commande) FROM Commande");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
        
        public int GetNextLigneId()
        {
            object result = ExecuterRequeteScalaire("SELECT MAX(id_ligne) FROM Ligne");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
    }
