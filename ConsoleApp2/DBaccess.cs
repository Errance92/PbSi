using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

public class DbAccess
{
    // Chaîne de connexion codée en dur dans la classe
    private readonly string _connectionString;
    private MySqlConnection _connection;
    public DbAccess()
    {
        // Chaîne de connexion codée directement dans la classe - REMPLACEZ PAR VOS VALEURS
        _connectionString = "Server=localhost;Port=3306;Database=Liv'in Paris;Uid=root;Pwd=password;CharSet=utf8;";

        // Pour le débogage, affichez un message
        Console.WriteLine("Connexion à la base de données initialisée...");
    }

    private MySqlConnection GetConnection()
    {
        try
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
                Console.WriteLine("Connexion créée.");
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                Console.WriteLine("Connexion ouverte.");
            }

            return _connection;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Erreur lors de la connexion à la base de données: {ex.Message}");
            throw;
        }
    }

    public void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
            Console.WriteLine("Connexion fermée.");
        }
    }

    // Méthodes génériques d'accès aux données
    public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
    {
        using (var command = new MySqlCommand(query, GetConnection()))
        {
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            var dataTable = new DataTable();
            using (var adapter = new MySqlDataAdapter(command))
            {
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
    }

    public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        using (var command = new MySqlCommand(query, GetConnection()))
        {
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            return command.ExecuteNonQuery();
        }
    }

    public object ExecuteScalar(string query, params MySqlParameter[] parameters)
    {
        using (var command = new MySqlCommand(query, GetConnection()))
        {
            if (parameters != null)
                command.Parameters.AddRange(parameters);

            return command.ExecuteScalar();
        }
    }

    // ===== MÉTHODES POUR CLIENTS =====

    public List<Client> GetAllClients()
    {
        var clients = new List<Client>();
        var table = ExecuteQuery("SELECT * FROM Client");

        foreach (DataRow row in table.Rows)
        {
            clients.Add(new Client
            {
                IdClient = Convert.ToInt32(row["id_client"]),
                Nom = row["nom"].ToString(),
                Prenom = row["prenom"].ToString(),
                Adresse = row["addresse"].ToString(),
                Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
                Telephone = row["telephone"] == DBNull.Value ? null : row["telephone"].ToString()
            });
        }

        return clients;
    }

    public Client GetClientById(int id)
        {
            var table = ExecuteQuery("SELECT * FROM Client WHERE id_client = @id", 
                new MySqlParameter("@id", id));
                
            if (table.Rows.Count == 0)
                return null;
                
            var row = table.Rows[0];
            return new Client
            {
                IdClient = Convert.ToInt32(row["id_client"]),
                Nom = row["nom"].ToString(),
                Prenom = row["prenom"].ToString(),
                Adresse = row["addresse"].ToString(),
                Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
                Telephone = row["telephone"] == DBNull.Value ? null : row["telephone"].ToString()
            };
        }
        
        public bool AddClient(Client client)
        {
            string query = @"INSERT INTO Client (id_client, nom, prenom, addresse, email, telephone)
                           VALUES (@id, @nom, @prenom, @adresse, @email, @telephone)";
            
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", client.IdClient),
                new MySqlParameter("@nom", client.Nom),
                new MySqlParameter("@prenom", client.Prenom),
                new MySqlParameter("@adresse", client.Adresse),
                new MySqlParameter("@email", client.Email ?? (object)DBNull.Value),
                new MySqlParameter("@telephone", client.Telephone ?? (object)DBNull.Value)
            };
            
            return ExecuteNonQuery(query, parameters) > 0;
        }
        
        public bool UpdateClient(Client client)
        {
            string query = @"UPDATE Client SET nom = @nom, prenom = @prenom, addresse = @adresse, 
                          email = @email, telephone = @telephone WHERE id_client = @id";
            
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", client.IdClient),
                new MySqlParameter("@nom", client.Nom),
                new MySqlParameter("@prenom", client.Prenom),
                new MySqlParameter("@adresse", client.Adresse),
                new MySqlParameter("@email", client.Email ?? (object)DBNull.Value),
                new MySqlParameter("@telephone", client.Telephone ?? (object)DBNull.Value)
            };
            
            return ExecuteNonQuery(query, parameters) > 0;
        }
        
        public bool DeleteClient(int id)
        {
            return ExecuteNonQuery("DELETE FROM Client WHERE id_client = @id", 
                new MySqlParameter("@id", id)) > 0;
        }
        
        public int GetNextClientId()
        {
            object result = ExecuteScalar("SELECT MAX(id_client) FROM Client");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
        
        // ===== MÉTHODES POUR CUISINIERS =====
        
        public List<Cuisinier> GetAllCuisiniers()
        {
            var cuisiniers = new List<Cuisinier>();
            var table = ExecuteQuery("SELECT * FROM Cuisinier");
            
            foreach (DataRow row in table.Rows)
            {
                cuisiniers.Add(new Cuisinier
                {
                    IdCuisinier = Convert.ToInt32(row["id_cuisinier"]),
                    Nom = row["nom"].ToString(),
                    Prenom = row["prenom"].ToString(),
                    Adresse = row["addresse"].ToString(),
                    Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
                    Telephone = row["telephone"] == DBNull.Value ? null : row["telephone"].ToString()
                });
            }
            
            return cuisiniers;
        }
        
        public Cuisinier GetCuisinierById(int id)
        {
            var table = ExecuteQuery("SELECT * FROM Cuisinier WHERE id_cuisinier = @id", 
                new MySqlParameter("@id", id));
                
            if (table.Rows.Count == 0)
                return null;
                
            var row = table.Rows[0];
            return new Cuisinier
            {
                IdCuisinier = Convert.ToInt32(row["id_cuisinier"]),
                Nom = row["nom"].ToString(),
                Prenom = row["prenom"].ToString(),
                Adresse = row["addresse"].ToString(),
                Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
                Telephone = row["telephone"] == DBNull.Value ? null : row["telephone"].ToString()
            };
        }
        
        public bool AddCuisinier(Cuisinier cuisinier)
        {
            string query = @"INSERT INTO Cuisinier (id_cuisinier, nom, prenom, addresse, email, telephone)
                           VALUES (@id, @nom, @prenom, @adresse, @email, @telephone)";
            
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@id", cuisinier.IdCuisinier),
                new MySqlParameter("@nom", cuisinier.Nom),
                new MySqlParameter("@prenom", cuisinier.Prenom),
                new MySqlParameter("@adresse", cuisinier.Adresse),
                new MySqlParameter("@email", cuisinier.Email ?? (object)DBNull.Value),
                new MySqlParameter("@telephone", cuisinier.Telephone ?? (object)DBNull.Value)
            };
            
            return ExecuteNonQuery(query, parameters) > 0;
        }
        
        public int GetNextCuisinierId()
        {
            object result = ExecuteScalar("SELECT MAX(id_cuisinier) FROM Cuisinier");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
        
        // ===== MÉTHODES POUR PLATS =====
        
        public List<Plat> GetAllPlats()
        {
            var plats = new List<Plat>();
            var table = ExecuteQuery("SELECT * FROM Plat");
            
            foreach (DataRow row in table.Rows)
            {
                plats.Add(MapRowToPlat(row));
            }
            
            return plats;
        }
        
        public Plat GetPlatById(int id)
        {
            var table = ExecuteQuery("SELECT * FROM Plat WHERE id_plat = @id", 
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

    // ===== MÉTHODES POUR COMMANDES =====
    public int GetNextPlatId()
    {
        object result = ExecuteScalar("SELECT MAX(id_plat) FROM Plat");
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

        return ExecuteNonQuery(query, parameters) > 0;
    }

    public List<Commande> GetAllCommandes()
        {
            var commandes = new List<Commande>();
            var table = ExecuteQuery("SELECT * FROM Commande ORDER BY date_commande DESC");
            
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
            var table = ExecuteQuery("SELECT * FROM Commande WHERE id_commande = @id", 
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
            var table = ExecuteQuery("SELECT * FROM Ligne WHERE id_commande = @id", 
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
                        
            var table = ExecuteQuery(query, new MySqlParameter("@id", ligneId));
            
            foreach (DataRow row in table.Rows)
            {
                plats.Add(MapRowToPlat(row));
            }
            
            return plats;
        }
        
        public int AddCommande(Commande commande)
        {
            // Vérifier si le client existe
            if (GetClientById(commande.IdClient) == null)
                return -1;
                
            // Commencer une transaction
            using (var transaction = GetConnection().BeginTransaction())
            {
                try
                {
                    // Insérer la commande
                    string query = @"INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client)
                                  VALUES (@id, @status, @date, @montant, @paiement, @idClient)";
                    
                    int nextId = GetNextCommandeId();
                    
                    var parameters = new MySqlParameter[]
                    {
                        new MySqlParameter("@id", nextId),
                        new MySqlParameter("@status", commande.StatuCommande),
                        new MySqlParameter("@date", commande.DateCommande),
                        new MySqlParameter("@montant", commande.Montant),
                        new MySqlParameter("@paiement", commande.Paiement ?? (object)DBNull.Value),
                        new MySqlParameter("@idClient", commande.IdClient)
                    };
                    
                    ExecuteNonQuery(query, parameters);
                    
                    // Ajouter les lignes de commande
                    foreach (var ligne in commande.Lignes)
                    {
                        int nextLigneId = GetNextLigneId();
                        
                        // Insérer la ligne
                        string ligneQuery = @"INSERT INTO Ligne (id_ligne, quantite, prix_total, date_livraison, lieu, id_commande)
                                           VALUES (@id, @quantite, @prixTotal, @dateLivraison, @lieu, @idCommande)";
                        
                        var ligneParams = new MySqlParameter[]
                        {
                            new MySqlParameter("@id", nextLigneId),
                            new MySqlParameter("@quantite", ligne.Quantite),
                            new MySqlParameter("@prixTotal", ligne.PrixTotal),
                            new MySqlParameter("@dateLivraison", ligne.DateLivraison.HasValue ? (object)ligne.DateLivraison.Value : DBNull.Value),
                            new MySqlParameter("@lieu", ligne.Lieu ?? (object)DBNull.Value),
                            new MySqlParameter("@idCommande", nextId)
                        };
                        
                        ExecuteNonQuery(ligneQuery, ligneParams);
                        
                        // Associer les plats à la ligne
                        foreach (var plat in ligne.Plats)
                        {
                            string platQuery = "INSERT INTO Ligne_Plat (id_ligne, id_plat) VALUES (@idLigne, @idPlat)";
                            
                            ExecuteNonQuery(platQuery, 
                                new MySqlParameter("@idLigne", nextLigneId),
                                new MySqlParameter("@idPlat", plat.IdPlat));
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
            object result = ExecuteScalar("SELECT MAX(id_commande) FROM Commande");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
        
        public int GetNextLigneId()
        {
            object result = ExecuteScalar("SELECT MAX(id_ligne) FROM Ligne");
            return (result == DBNull.Value) ? 1 : Convert.ToInt32(result) + 1;
        }
    }
