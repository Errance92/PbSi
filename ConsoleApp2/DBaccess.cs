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
        connectionString = "Server=localhost;Port=3306;Database=Liv'in Paris;Uid=root;Pwd=root;CharSet=utf8;";
        Console.WriteLine("Connexion à la base de données initialisée");
    }

    #region Connexion

    /// <summary>
    /// Établit une connexion MySQL si elle n'existe pas ou n'est pas ouverte.
    /// </summary>
    /// <returns>Une instance ouverte de <see cref="MySqlConnection"/>.</returns>

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
    /// <summary>
    /// Ferme la connexion MySQL si elle est ouverte.
    /// </summary>

    public void FermerConnection()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            Console.WriteLine("Connexion fermée.");
        }
    }

    #endregion

    #region Requete
    /// <summary>
    /// Exécute une requête SQL SELECT avec des paramètres facultatifs et retourne les résultats dans un DataTable.
    /// </summary>
    /// <param name="requete">La requête SQL à exécuter.</param>
    /// <param name="parametres">Les paramètres associés à la requête.</param>
    /// <returns>Un DataTable contenant les résultats.</returns>

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
    /// <summary>
    /// Exécute une requête SQL de mise à jour (INSERT, UPDATE, DELETE) avec des paramètres facultatifs.
    /// </summary>
    /// <param name="requete">La requête SQL à exécuter.</param>
    /// <param name="parametres">Les paramètres associés à la requête.</param>
    /// <returns>Le nombre de lignes affectées par la requête.</returns>

    public int ExecuterRequeteMAJ(string requete, params MySqlParameter[] parametres)
    {
        using (var commande = new MySqlCommand(requete, Connection()))
        {
            if (parametres != null)
                commande.Parameters.AddRange(parametres);

            return commande.ExecuteNonQuery();
        }
    }
    /// <summary>
    /// Exécute une requête SQL qui retourne une seule valeur (ex: COUNT, MAX, etc.).
    /// </summary>
    /// <param name="requete">La requête SQL à exécuter.</param>
    /// <param name="parametres">Les paramètres associés à la requête.</param>
    /// <returns>La valeur scalaire retournée par la requête.</returns>

    public object ExecuterRequeteScalaire(string requete, params MySqlParameter[] parametres)
    {
        using (var commande = new MySqlCommand(requete, Connection()))
        {
            if (parametres != null)
                commande.Parameters.AddRange(parametres);

            return commande.ExecuteScalar();
        }
    }
    #endregion

    #region Clients
    /// <summary>
    /// Récupère l'ensemble des clients présents dans la base de données.
    /// </summary>
    /// <returns>Une liste d'objets <see cref="Client"/> représentant les clients.</returns>

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
            client.NumRue = Convert.ToInt32(row["numero_rue"]);
            client.NomRue = row["rue"].ToString();
            client.Ville = row["ville"].ToString();
            client.Metro = row["metro"].ToString();
           
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

            if (row["montant_achat"] == DBNull.Value)
            {
                client.MontantAchat = 0;
            }
            else
            {
                client.MontantAchat = Convert.ToDouble(row["montant_achat"]);
            }
            clients.Add(client);
        }

        return clients;
    }
    /// <summary>
    /// Récupère un client spécifique à partir de son identifiant unique.
    /// </summary>
    /// <param name="id">L'identifiant du client.</param>
    /// <returns>Un objet <see cref="Client"/> correspondant ou null si non trouvé.</returns>

    public Client ObtenirClientID(int id)
    {
        var table = ExecuterRequete("SELECT * FROM Client WHERE id_client = @id",
            new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
        {
            return null;
        }

        var row = table.Rows[0];
        Client client = new Client();
        client.IdClient = Convert.ToInt32(row["id_client"]);
        client.Nom = row["nom"].ToString();
        client.Prenom = row["prenom"].ToString();
        client.NumRue = Convert.ToInt32(row["numero_rue"]);
        client.NomRue = row["rue"].ToString();
        client.Ville = row["ville"].ToString();
        client.Metro = row["metro"].ToString();

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

        if (row["montant_achat"] == DBNull.Value)
        {
            client.MontantAchat = 0;
        }
        else
        {
            client.MontantAchat = Convert.ToDouble(row["montant_achat"]);
        }

        return client;
    }
    /// <summary>
    /// Ajoute un nouveau client dans la base de données.
    /// </summary>
    /// <param name="client">Le client à ajouter.</param>
    /// <returns>True si l'insertion a réussi, sinon false.</returns>

    public bool AjouterClients(Client client)
    {
        string requete = @"INSERT INTO Client (id_client, nom, prenom, numero_rue, rue, ville, metro, email, telephone, montant_achat)
                     VALUES (@id, @nom, @prenom, @numRue, @rue, @ville, @metro, @email, @telephone, @montantAchat)";

        MySqlParameter paramId = new MySqlParameter("@id", client.IdClient);
        MySqlParameter paramNom = new MySqlParameter("@nom", client.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", client.Prenom);
        MySqlParameter paramNumRue = new MySqlParameter("@numRue", client.NumRue);
        MySqlParameter paramRue = new MySqlParameter("@rue", client.NomRue);
        MySqlParameter paramVille = new MySqlParameter("@ville", client.Ville);
        MySqlParameter paramMetro = new MySqlParameter("@metro", client.Metro);

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

        MySqlParameter paramMontantAchat = new MySqlParameter("@montantAchat", client.MontantAchat);
        if (client.MontantAchat == null)
        {
            paramMontantAchat = new MySqlParameter("@montantAchat", 0);
        }
        else
        {
            paramMontantAchat = new MySqlParameter("@montantAchat", client.MontantAchat);
        }

        var parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramNumRue,
        paramRue,
        paramVille,
        paramMetro,
        paramEmail,
        paramTelephone,
        paramMontantAchat
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }
    /// <summary>
    /// Met à jour les informations d'un client existant.
    /// </summary>
    /// <param name="client">Le client avec les données mises à jour.</param>
    /// <returns>True si la mise à jour a réussi, sinon false.</returns>

    public bool MAJClient(Client client)
    {
        string requete = @"UPDATE Client 
                       SET nom = @nom, 
                           prenom = @prenom,
                           numero_rue = @numRue,
                           ville = @ville,
                           rue = @rue,
                           email = @email,
                           telephone = @telephone,
                           montant_achat = @montant,
                           metro = @metro
                       WHERE id_client = @id";


        MySqlParameter paramId = new MySqlParameter("@id", client.IdClient);
        MySqlParameter paramNom = new MySqlParameter("@nom", client.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", client.Prenom);
        MySqlParameter paramNumRue = new MySqlParameter("@numRue", client.NumRue);
        MySqlParameter paramVille = new MySqlParameter("@ville", client.Ville);
        MySqlParameter paramRue = new MySqlParameter("@rue", client.NomRue);
        MySqlParameter paramMetro = new MySqlParameter("@metro", client.Metro);


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

        MySqlParameter paramMontant;
        if (client.MontantAchat == null)
        {
            paramMontant = new MySqlParameter("@montant", 0);
        }
        else
        {
            paramMontant = new MySqlParameter("@montant", client.MontantAchat);
        }

        MySqlParameter[] parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramNumRue,
        paramVille,
        paramRue,
        paramEmail,
        paramTelephone,
        paramMontant,
        paramMetro
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }
    /// <summary>
    /// Supprime un client à partir de son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant du client à supprimer.</param>
    /// <returns>True si la suppression a réussi, sinon false.</returns>

    public bool SupprimerClient(int id)
    {
        return ExecuterRequeteMAJ("DELETE FROM Client WHERE id_client = @id",
                                 new MySqlParameter("@id", id)) > 0;
    }
    /// <summary>
    /// Récupère l'identifiant disponible suivant pour l'ajout d'un nouveau client.
    /// </summary>
    /// <returns>L'identifiant suivant disponible.</returns>

    public int ObtenirProchainIDClient()
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
    /// <summary>
    /// Trie une liste de clients par ordre alphabétique de nom.
    /// </summary>
    /// <param name="clients">Liste des clients à trier.</param>
    /// <returns>La liste triée par nom.</returns>

    public static List<Client> TrierParNom(List<Client> clients)
    {
        clients.Sort(ComparerParNom);
        return clients;
    }
    /// <summary>
    /// Compare deux clients selon leur nom.
    /// </summary>
    /// <param name="a">Premier client.</param>
    /// <param name="b">Deuxième client.</param>
    /// <returns>Un entier indiquant l'ordre de tri.</returns>

    private static int ComparerParNom(Client a, Client b)
    {
        return a.Nom.CompareTo(b.Nom);
    }
    /// <summary>
    /// Trie une liste de clients par nom de rue.
    /// </summary>
    /// <param name="clients">Liste des clients à trier.</param>
    /// <returns>La liste triée par rue.</returns>

    public static List<Client> TrierParRue(List<Client> clients)
    {
        clients.Sort(ComparerParRue);
        return clients;
    }
    /// <summary>
    /// Compare deux clients selon leur rue.
    /// </summary>
    /// <param name="a">Premier client.</param>
    /// <param name="b">Deuxième client.</param>
    /// <returns>Un entier indiquant l'ordre de tri.</returns>

    private static int ComparerParRue(Client a, Client b)
    {
        return a.NomRue.CompareTo(b.NomRue);
    }
    /// <summary>
    /// Trie une liste de clients par montant d'achat décroissant.
    /// </summary>
    /// <param name="clients">Liste des clients à trier.</param>
    /// <returns>La liste triée par montant d'achat.</returns>

    public static List<Client> TrierParMontant(List<Client> clients)
    {
        clients.Sort(ComparerParMontant);
        return clients;
    }
    /// <summary>
    /// Compare deux clients selon leur montant d'achat.
    /// </summary>
    /// <param name="a">Premier client.</param>
    /// <param name="b">Deuxième client.</param>
    /// <returns>Un entier indiquant l'ordre de tri décroissant.</returns>

    private static int ComparerParMontant(Client a, Client b)
    {
        return b.MontantAchat.CompareTo(a.MontantAchat);
    }
    #endregion

    #region Cuisinier
    /// <summary>
    /// Récupère tous les cuisiniers présents dans la base de données.
    /// </summary>
    /// <returns>Une liste d'objets <see cref="Cuisinier"/>.</returns>

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
            c.NumRue = Convert.ToInt32(row["numero_rue"]);
            c.NomRue = row["rue"].ToString();
            c.Ville = row["ville"].ToString();
            c.Metro = row["metro"].ToString();

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

            if (row["id_plat"] == DBNull.Value)
            {
                c.IdPlat = null;
            }
            else
            {
                c.IdPlat = Convert.ToInt32(row["id_plat"]);
            }

            cuisiniers.Add(c);
        }

        return cuisiniers;
    }
    /// <summary>
    /// Récupère un cuisinier à partir de son identifiant unique.
    /// </summary>
    /// <param name="id">L'identifiant du cuisinier.</param>
    /// <returns>Un objet <see cref="Cuisinier"/> ou null si non trouvé.</returns>

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
        c.NumRue = Convert.ToInt32(row["numero_rue"]);
        c.NomRue = row["rue"].ToString();
        c.Ville = row["ville"].ToString();
        c.Metro = row["metro"].ToString();


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
        if (row["id_plat"] == DBNull.Value)
        {
            c.IdPlat = null;
        }
        else
        {
            c.IdPlat = Convert.ToInt32(row["id_plat"]);
        }

        return c;
    }
    /// <summary>
    /// Ajoute un nouveau cuisinier à la base de données.
    /// </summary>
    /// <param name="cuisinier">Le cuisinier à ajouter.</param>
    /// <returns>True si l'ajout a réussi, sinon false.</returns>

    public bool AjouterCuisinier(Cuisinier cuisinier)
    {
        string requete = @"INSERT INTO Cuisinier (id_cuisinier, nom, prenom, numero_rue, rue, ville, email, telephone, metro, id_plat)
                       VALUES (@id, @nom, @prenom, @numRue, @rue, @ville, @email, @telephone, @metro, @idPlat)";

        MySqlParameter paramId = new MySqlParameter("@id", cuisinier.IdCuisinier);
        MySqlParameter paramNom = new MySqlParameter("@nom", cuisinier.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", cuisinier.Prenom);
        MySqlParameter paramNumRue = new MySqlParameter("@numRue", cuisinier.NumRue);
        MySqlParameter paramRue = new MySqlParameter("@rue", cuisinier.NomRue);
        MySqlParameter paramVille = new MySqlParameter("@ville", cuisinier.Ville);
        MySqlParameter paramMetro = new MySqlParameter("@metro", cuisinier.Metro);

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

        MySqlParameter paramIdPlat;
        if (cuisinier.IdPlat == null)
        {
            paramIdPlat = new MySqlParameter("@idPlat", DBNull.Value);
        }
        else
        {
            paramIdPlat = new MySqlParameter("@idPlat", cuisinier.IdPlat);
        }

        var parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramNumRue,
        paramRue,
        paramVille,
        paramEmail,
        paramTelephone,
        paramMetro,
        paramIdPlat
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }
    /// <summary>
    /// Récupère l'identifiant disponible suivant pour un nouveau cuisinier.
    /// </summary>
    /// <returns>L'identifiant suivant disponible.</returns>

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
    /// <summary>
    /// Met à jour les informations d’un cuisinier existant dans la base.
    /// </summary>
    /// <param name="cuisinier">Le cuisinier avec les informations mises à jour.</param>
    /// <returns>True si la mise à jour a réussi, sinon false.</returns>

    public bool MAJCuisinier(Cuisinier cuisinier)
    {
        string requete = @"UPDATE Cuisinier
                   SET nom = @nom,
                       prenom = @prenom,
                       numero_rue = @numRue,
                       rue = @rue,
                       ville = @ville,
                       email = @email,
                       telephone = @telephone,
                       metro = @metro,
                       id_plat = @idPlat
                   WHERE id_cuisinier = @id";

        MySqlParameter paramId = new MySqlParameter("@id", cuisinier.IdCuisinier);
        MySqlParameter paramNom = new MySqlParameter("@nom", cuisinier.Nom);
        MySqlParameter paramPrenom = new MySqlParameter("@prenom", cuisinier.Prenom);
        MySqlParameter paramNumRue = new MySqlParameter("@numRue", cuisinier.NumRue);
        MySqlParameter paramRue = new MySqlParameter("@rue", cuisinier.NomRue);
        MySqlParameter paramVille = new MySqlParameter("@ville", cuisinier.Ville);
        MySqlParameter paramMetro = new MySqlParameter("@metro", cuisinier.Metro);

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

        MySqlParameter paramIdPlat;
        if (cuisinier.IdPlat == null)
        {
            paramIdPlat = new MySqlParameter("@idPlat", DBNull.Value);
        }
        else
        {
            paramIdPlat = new MySqlParameter("@idPlat", cuisinier.IdPlat);
        }

        MySqlParameter[] parametres = new MySqlParameter[]
        {
        paramId,
        paramNom,
        paramPrenom,
        paramNumRue,
        paramRue,
        paramVille,
        paramEmail,
        paramTelephone,
        paramMetro,
        paramIdPlat
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }
    /// <summary>
    /// Supprime un cuisinier en fonction de son identifiant.
    /// </summary>
    /// <param name="id">L’identifiant du cuisinier à supprimer.</param>
    /// <returns>True si la suppression a réussi, sinon false.</returns>

    public bool SupprimerCuisinier(int id)
    {
        return ExecuterRequeteMAJ("DELETE FROM Cuisinier WHERE id_cuisinier = @id",
            new MySqlParameter("@id", id)) > 0;
    }

    #endregion

    #region Plat
    /// <summary>
    /// Récupère tous les plats disponibles dans la base de données.
    /// </summary>
    /// <returns>Une liste d'objets <see cref="Plat"/> représentant les plats.</returns>

    public List<Plat> RecupererToutPlat()
    {
        var plats = new List<Plat>();
        DataTable table = ExecuterRequete("SELECT * FROM Plat");

        foreach (DataRow row in table.Rows)
        {
            Plat plat = new Plat();
            plat.IdPlat = Convert.ToInt32(row["id_plat"]);
            plat.NomPlat = row["nom_plat"].ToString();
            plat.Type = row["_type"].ToString();
            plat.Stock = Convert.ToInt32(row["stock"]);
            plat.PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]);

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
    /// <summary>
    /// Récupère un plat spécifique à partir de son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant du plat.</param>
    /// <returns>Un objet <see cref="Plat"/> ou null si non trouvé.</returns>

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
        plat.Type = row["_type"].ToString();
        plat.Stock = Convert.ToInt32(row["stock"]);
        plat.PrixParPersonne = Convert.ToDecimal(row["prix_par_personne"]);

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
    /// <summary>
    /// Récupère l'identifiant disponible suivant pour ajouter un nouveau plat.
    /// </summary>
    /// <returns>L'identifiant suivant disponible.</returns>

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
    /// <summary>
    /// Ajoute un nouveau plat dans la base de données.
    /// </summary>
    /// <param name="plat">Le plat à ajouter.</param>
    /// <returns>True si l'ajout a réussi, sinon false.</returns>

    public bool AjouterPlat(Plat plat)
    {
        string requete = @"INSERT INTO Plat (id_plat, nom_plat, _type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption)
                     VALUES (@id, @nom, @type, @stock, @origine, @regime, @ingredient, @lien, @dateFab, @prix, @datePer)";

        MySqlParameter paramId = new MySqlParameter("@id", plat.IdPlat);
        MySqlParameter paramNom = new MySqlParameter("@nom", plat.NomPlat);
        MySqlParameter paramType = new MySqlParameter("@type", plat.Type);
        MySqlParameter paramStock = new MySqlParameter("@stock", plat.Stock);
        MySqlParameter paramPrix = new MySqlParameter("@prix", plat.PrixParPersonne);


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
    /// <summary>
    /// Met à jour les informations d’un plat existant dans la base.
    /// </summary>
    /// <param name="plat">Le plat avec les données à jour.</param>
    /// <returns>True si la mise à jour a réussi, sinon false.</returns>

    public bool MAJPlat(Plat plat)
    {
        string requete = @"UPDATE Plat
                       SET nom_plat = @nom,
                           _type = @type,
                           stock = @stock,
                           origine = @origine,
                           regime_alimentaire = @regime,
                           ingredient = @ingredient,
                           lien_photo = @lien,
                           date_fabrication = @dateFab,
                           prix_par_personne = @prix,
                           date_peremption = @datePer
                       WHERE id_plat = @id";

        MySqlParameter paramId = new MySqlParameter("@id", plat.IdPlat);
        MySqlParameter paramNom = new MySqlParameter("@nom", plat.NomPlat);
        MySqlParameter paramType = new MySqlParameter("@type", plat.Type);
        MySqlParameter paramStock = new MySqlParameter("@stock", plat.Stock);
        MySqlParameter paramPrix = new MySqlParameter("@prix", plat.PrixParPersonne);

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
        paramId,
        paramNom,
        paramType,
        paramStock,
        paramOrigine,
        paramRegime,
        paramIngredient,
        paramLien,
        paramDateFab,
        paramPrix,
        paramDatePer
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }
    /// <summary>
    /// Supprime un plat de la base de données via son identifiant.
    /// </summary>
    /// <param name="id">L’identifiant du plat à supprimer.</param>
    /// <returns>True si la suppression a réussi, sinon false.</returns>

    public bool SupprimerPlat(int id)
    {
        return ExecuterRequeteMAJ("DELETE FROM Plat WHERE id_plat = @id",
                                  new MySqlParameter("@id", id)) > 0;
    }

    #endregion

    #region Statistiques
    /// <summary>
    /// Récupère le chiffre d'affaires total par mois sur une année donnée.
    /// Utilise GROUP BY et SUM.
    /// </summary>
    /// <param name="annee">L'année pour laquelle on veut les statistiques</param>
    /// <returns>Un dictionnaire avec le mois comme clé et le chiffre d'affaires comme valeur</returns>
    public Dictionary<int, double> ObtenirChiffreAffairesParMois(int annee)
    {
        Dictionary<int, double> resultat = new Dictionary<int, double>();
        string requete = @"SELECT MONTH(date_commande) AS mois, SUM(montant) AS chiffre_affaires 
                     FROM Commande 
                     WHERE YEAR(date_commande) = @annee 
                     GROUP BY MONTH(date_commande) 
                     ORDER BY mois";

        DataTable table = ExecuterRequete(requete, new MySqlParameter("@annee", annee));

        foreach (DataRow row in table.Rows)
        {
            int mois = Convert.ToInt32(row["mois"]);
            double chiffreAffaires = Convert.ToDouble(row["chiffre_affaires"]);
            resultat.Add(mois, chiffreAffaires);
        }

        return resultat;
    }

    /// <summary>
    /// Récupère le nombre de commandes par client, pour les clients ayant plus de X commandes.
    /// Utilise GROUP BY et HAVING.
    /// </summary>
    /// <param name="minCommandes">Nombre minimal de commandes</param>
    /// <returns>Un dictionnaire avec l'ID du client comme clé et le nombre de commandes comme valeur</returns>
    public Dictionary<int, int> ObtenirClientsFrequents(int minCommandes)
    {
        Dictionary<int, int> resultat = new Dictionary<int, int>();

        string requete = @"SELECT id_client, COUNT(*) AS nombre_commandes 
                     FROM Commande 
                     GROUP BY id_client 
                     HAVING COUNT(*) >= @minCommandes
                     ORDER BY nombre_commandes DESC";

        DataTable table = ExecuterRequete(requete, new MySqlParameter("@minCommandes", minCommandes));

        foreach (DataRow row in table.Rows)
        {
            int idClient = Convert.ToInt32(row["id_client"]);
            int nombreCommandes = Convert.ToInt32(row["nombre_commandes"]);
            resultat.Add(idClient, nombreCommandes);
        }

        return resultat;
    }

    /// <summary>
    /// Récupère les plats populaires (commandés plus de X fois).
    /// Utilise LEFT JOIN, GROUP BY et HAVING.
    /// </summary>
    /// <param name="minCommandes">Nombre minimal de commandes</param>
    /// <returns>Une liste des plats populaires avec leurs statistiques</returns>
    public List<Tuple<int, string, int>> ObtenirPlatsPopulaires(int minCommandes)
    {
        List<Tuple<int, string, int>> resultat = new List<Tuple<int, string, int>>();

        string requete = @"SELECT p.id_plat, p.nom_plat, COUNT(c.id_commande) AS nombre_commandes 
                     FROM Plat p 
                     LEFT JOIN Commande c ON p.id_plat = c.id_plat 
                     GROUP BY p.id_plat, p.nom_plat 
                     HAVING COUNT(c.id_commande) >= @minCommandes
                     ORDER BY nombre_commandes DESC";

        DataTable table = ExecuterRequete(requete, new MySqlParameter("@minCommandes", minCommandes));

        foreach (DataRow row in table.Rows)
        {
            int idPlat = Convert.ToInt32(row["id_plat"]);
            string nomPlat = row["nom_plat"].ToString();
            int nombreCommandes = Convert.ToInt32(row["nombre_commandes"]);
            resultat.Add(new Tuple<int, string, int>(idPlat, nomPlat, nombreCommandes));
        }

        return resultat;
    }

    /// <summary>
    /// Récupère la liste des cuisiniers qui proposent un plat dont le prix est supérieur à celui de tous les plats d'un type donné.
    /// Utilise ALL.
    /// </summary>
    /// <param name="type">Type de plat pour la comparaison</param>
    /// <returns>Liste des cuisiniers concernés</returns>
    public List<Cuisinier> ObtenirCuisiniersPlatsExclusifs(string type)
    {
        List<Cuisinier> resultat = new List<Cuisinier>();

        string requete = @"SELECT c.* FROM Cuisinier c 
                     JOIN Plat p ON c.id_plat = p.id_plat 
                     WHERE p.prix_par_personne > ALL (
                         SELECT prix_par_personne FROM Plat WHERE _type = @type
                     )";

        DataTable table = ExecuterRequete(requete, new MySqlParameter("@type", type));

        foreach (DataRow row in table.Rows)
        {
            Cuisinier c = new Cuisinier();
            c.IdCuisinier = Convert.ToInt32(row["id_cuisinier"]);
            c.Nom = row["nom"].ToString();
            c.Prenom = row["prenom"].ToString();
            c.NumRue = Convert.ToInt32(row["numero_rue"]);
            c.NomRue = row["rue"].ToString();
            c.Ville = row["ville"].ToString();
            c.Metro = row["metro"].ToString();

            if (row["email"] != DBNull.Value)
                c.Email = row["email"].ToString();

            if (row["telephone"] != DBNull.Value)
                c.Telephone = row["telephone"].ToString();

            if (row["id_plat"] != DBNull.Value)
                c.IdPlat = Convert.ToInt32(row["id_plat"]);

            resultat.Add(c);
        }

        return resultat;
    }

    /// <summary>
    /// Récupère les clients qui n'ont pas encore passé de commande.
    /// Utilise EXISTS avec négation.
    /// </summary>
    /// <returns>Liste des clients sans commande</returns>
    public List<Client> ObtenirClientsSansCommande()
    {
        List<Client> resultat = new List<Client>();

        string requete = @"SELECT * FROM Client c 
                     WHERE NOT EXISTS (
                         SELECT 1 FROM Commande cmd WHERE cmd.id_client = c.id_client
                     )";

        DataTable table = ExecuterRequete(requete);

        foreach (DataRow row in table.Rows)
        {
            Client client = new Client();
            client.IdClient = Convert.ToInt32(row["id_client"]);
            client.Nom = row["nom"].ToString();
            client.Prenom = row["prenom"].ToString();
            client.NumRue = Convert.ToInt32(row["numero_rue"]);
            client.NomRue = row["rue"].ToString();
            client.Ville = row["ville"].ToString();
            client.Metro = row["metro"].ToString();

            if (row["email"] != DBNull.Value)
                client.Email = row["email"].ToString();

            if (row["telephone"] != DBNull.Value)
                client.Telephone = row["telephone"].ToString();

            if (row["montant_achat"] != DBNull.Value)
                client.MontantAchat = Convert.ToDouble(row["montant_achat"]);

            resultat.Add(client);
        }

        return resultat;
    }

    /// <summary>
    /// Obtient le temps moyen de préparation des commandes par cuisinier.
    /// Utilise GROUP BY et AVG.
    /// </summary>
    /// <returns>Dictionnaire avec ID du cuisinier comme clé et temps moyen comme valeur</returns>
    public Dictionary<int, double> ObtenirTempsMoyenPreparationParCuisinier()
    {
        Dictionary<int, double> resultat = new Dictionary<int, double>();

        string requete = @"SELECT c.id_cuisinier, AVG(TIMESTAMPDIFF(MINUTE, cmd.date_commande, l.date_livraison)) AS temps_moyen
                     FROM Cuisinier c
                     JOIN Commande cmd ON c.id_cuisinier = cmd.id_cuisinier
                     JOIN Livraison l ON cmd.id_commande = l.id_commande
                     GROUP BY c.id_cuisinier";

        DataTable table = ExecuterRequete(requete);

        foreach (DataRow row in table.Rows)
        {
            int idCuisinier = Convert.ToInt32(row["id_cuisinier"]);
            double tempsMoyen = Convert.ToDouble(row["temps_moyen"]);
            resultat.Add(idCuisinier, tempsMoyen);
        }

        return resultat;
    }

    /// <summary>
    /// Obtient le nombre de clients par station de métro.
    /// Utilise GROUP BY.
    /// </summary>
    /// <returns>Dictionnaire avec le nom de la station comme clé et le nombre de clients comme valeur</returns>
    public Dictionary<string, int> ObtenirNombreClientsParMetro()
    {
        Dictionary<string, int> resultat = new Dictionary<string, int>();

        string requete = @"SELECT metro, COUNT(*) AS nombre_clients
                     FROM Client
                     GROUP BY metro
                     ORDER BY nombre_clients DESC";

        DataTable table = ExecuterRequete(requete);

        foreach (DataRow row in table.Rows)
        {
            string metro = row["metro"].ToString();
            int nombreClients = Convert.ToInt32(row["nombre_clients"]);
            resultat.Add(metro, nombreClients);
        }

        return resultat;
    }
    #endregion

    #region Utilisateurs
    /// <summary>
    /// Vérifie si les identifiants fournis correspondent à un utilisateur valide.
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="motDePasse">Mot de passe de l'utilisateur</param>
    /// <returns>L'utilisateur authentifié ou null si échec</returns>
    public Utilisateur Authentifier(string email, string motDePasse)
    {
        string requete = "SELECT * FROM Utilisateur WHERE email = @email AND mot_de_passe = @motDePasse";

        MySqlParameter paramEmail = new MySqlParameter("@email", email);
        MySqlParameter paramMotDePasse = new MySqlParameter("@motDePasse", motDePasse);

        DataTable table = ExecuterRequete(requete, paramEmail, paramMotDePasse);

        if (table.Rows.Count == 0)
            return null;

        DataRow row = table.Rows[0];
        Utilisateur utilisateur = new Utilisateur
        {
            IdUtilisateur = Convert.ToInt32(row["id_utilisateur"]),
            Email = row["email"].ToString(),
            MotDePasse = row["mot_de_passe"].ToString(),
            Role = row["role"].ToString()
        };

        if (row["id_reference"] != DBNull.Value)
            utilisateur.IdReference = Convert.ToInt32(row["id_reference"]);

        return utilisateur;
    }

    /// <summary>
    /// Vérifie si un email est déjà utilisé.
    /// </summary>
    /// <param name="email">Email à vérifier</param>
    /// <returns>True si l'email est déjà utilisé, sinon False</returns>
    public bool EmailExiste(string email)
    {
        string requete = "SELECT COUNT(*) FROM Utilisateur WHERE email = @email";
        object resultat = ExecuterRequeteScalaire(requete, new MySqlParameter("@email", email));
        return Convert.ToInt32(resultat) > 0;
    }

    /// <summary>
    /// Crée un nouvel utilisateur.
    /// </summary>
    /// <param name="utilisateur">Données de l'utilisateur à créer</param>
    /// <returns>True si la création a réussi, sinon False</returns>
    public bool CreerUtilisateur(Utilisateur utilisateur)
    {
        string requete = @"INSERT INTO Utilisateur (email, mot_de_passe, role, id_reference) 
                     VALUES (@email, @motDePasse, @role, @idReference)";

        MySqlParameter paramEmail = new MySqlParameter("@email", utilisateur.Email);
        MySqlParameter paramMotDePasse = new MySqlParameter("@motDePasse", utilisateur.MotDePasse);
        MySqlParameter paramRole = new MySqlParameter("@role", utilisateur.Role);
        MySqlParameter paramIdReference;

        if (utilisateur.IdReference.HasValue)
            paramIdReference = new MySqlParameter("@idReference", utilisateur.IdReference.Value);
        else
            paramIdReference = new MySqlParameter("@idReference", DBNull.Value);

        try
        {
            return ExecuterRequeteMAJ(requete, paramEmail, paramMotDePasse, paramRole, paramIdReference) > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Récupère tous les utilisateurs.
    /// </summary>
    /// <returns>Liste des utilisateurs</returns>
    public List<Utilisateur> ObtenirTousUtilisateurs()
    {
        List<Utilisateur> utilisateurs = new List<Utilisateur>();
        string requete = "SELECT * FROM Utilisateur";

        DataTable table = ExecuterRequete(requete);

        foreach (DataRow row in table.Rows)
        {
            Utilisateur utilisateur = new Utilisateur
            {
                IdUtilisateur = Convert.ToInt32(row["id_utilisateur"]),
                Email = row["email"].ToString(),
                MotDePasse = row["mot_de_passe"].ToString(),
                Role = row["role"].ToString()
            };

            if (row["id_reference"] != DBNull.Value)
                utilisateur.IdReference = Convert.ToInt32(row["id_reference"]);

            utilisateurs.Add(utilisateur);
        }

        return utilisateurs;
    }

    /// <summary>
    /// Obtient un utilisateur par son email.
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <returns>L'utilisateur correspondant ou null</returns>
    public Utilisateur ObtenirUtilisateurParEmail(string email)
    {
        string requete = "SELECT * FROM Utilisateur WHERE email = @email";
        DataTable table = ExecuterRequete(requete, new MySqlParameter("@email", email));

        if (table.Rows.Count == 0)
            return null;

        DataRow row = table.Rows[0];
        Utilisateur utilisateur = new Utilisateur
        {
            IdUtilisateur = Convert.ToInt32(row["id_utilisateur"]),
            Email = row["email"].ToString(),
            MotDePasse = row["mot_de_passe"].ToString(),
            Role = row["role"].ToString()
        };

        if (row["id_reference"] != DBNull.Value)
            utilisateur.IdReference = Convert.ToInt32(row["id_reference"]);

        return utilisateur;
    }
    #endregion

    #region Commandes
    /// <summary>
    /// Récupère toutes les commandes enregistrées dans la base de données.
    /// </summary>
    /// <returns>Une liste d'objets <see cref="Commande"/> représentant les commandes.</returns>

    public List<Commande> RecupererCommandes()
    {
        List<Commande> commandes = new List<Commande>();

        var table = ExecuterRequete("SELECT * FROM Commande");

        foreach (DataRow row in table.Rows)
        {
            Commande commande = new Commande();

            commande.IdCommande = Convert.ToInt32(row["id_commande"]);
            commande.StatuCommande = row["statu_commande"].ToString();
            commande.DateCommande = Convert.ToDateTime(row["date_commande"]);
            commande.Montant = Convert.ToDouble(row["montant"]);

            if (row["paiement"] == DBNull.Value)
            {
                commande.Paiement = null;
            }
            else
            {
                commande.Paiement = row["paiement"].ToString();
            }

            if (row["id_client"] == DBNull.Value)
            {
                commande.IdClient = -1; 
            }
            else
            {
                commande.IdClient = Convert.ToInt32(row["id_client"]);
            }

            if (row.Table.Columns.Contains("id_cuisinier"))
            {
                if (row["id_cuisinier"] == DBNull.Value)
                {
                    commande.IdCuisinier = -1; 
                }
                else
                {
                    commande.IdCuisinier = Convert.ToInt32(row["id_cuisinier"]);
                }
            }

            commandes.Add(commande);
        }

        return commandes;
    }
    /// <summary>
    /// Récupère une commande à partir de son identifiant unique.
    /// </summary>
    /// <param name="id">L’identifiant de la commande.</param>
    /// <returns>Un objet <see cref="Commande"/> ou null si non trouvé.</returns>

    public Commande ObtenirCommandeParId(int id)
    {
        var table = ExecuterRequete("SELECT * FROM Commande WHERE id_commande = @id", new MySqlParameter("@id", id));

        if (table.Rows.Count == 0)
        {
            return null;
        }

        var row = table.Rows[0];
        Commande commande = new Commande();

        commande.IdCommande = Convert.ToInt32(row["id_commande"]);
        commande.StatuCommande = row["statu_commande"].ToString();
        commande.DateCommande = Convert.ToDateTime(row["date_commande"]);
        commande.Montant = Convert.ToDouble(row["montant"]);

        if (row["paiement"] == DBNull.Value)
        {
            commande.Paiement = null;
        }
        else
        {
            commande.Paiement = row["paiement"].ToString();
        }

        if (row["id_client"] == DBNull.Value)
        {
            commande.IdClient = -1;
        }
        else
        {
            commande.IdClient = Convert.ToInt32(row["id_client"]);
        }

        if (row.Table.Columns.Contains("id_cuisinier"))
        {
            if (row["id_cuisinier"] == DBNull.Value)
            {
                commande.IdCuisinier = -1;
            }
            else
            {
                commande.IdCuisinier = Convert.ToInt32(row["id_cuisinier"]);
            }
        }

        return commande;
    }

    /// <summary>
    /// Ajoute une nouvelle commande dans la base de données.
    /// </summary>
    /// <param name="commande">La commande à insérer.</param>
    /// <returns>True si l'insertion a réussi, sinon false.</returns>

    public bool AjouterCommande(Commande commande)
    {
        string requete = @"INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client, id_cuisinier)
                       VALUES (@id, @statut, @dateCom, @montant, @paiement, @idClient, @idCuisinier)";

        MySqlParameter paramId = new MySqlParameter("@id", commande.IdCommande);
        MySqlParameter paramStatut = new MySqlParameter("@statut", commande.StatuCommande);
        MySqlParameter paramDate = new MySqlParameter("@dateCom", commande.DateCommande);
        MySqlParameter paramMontant = new MySqlParameter("@montant", commande.Montant);
        MySqlParameter paramIdClient = new MySqlParameter("@idClient", commande.IdClient);
        MySqlParameter paramIdCuisinier = new MySqlParameter("@idCuisinier", commande.IdCuisinier);

        MySqlParameter paramPaiement;
        if (commande.Paiement == null)
        {
            paramPaiement = new MySqlParameter("@paiement", DBNull.Value);
        }
        else
        {
            paramPaiement = new MySqlParameter("@paiement", commande.Paiement);
        }

        MySqlParameter[] parametres = new MySqlParameter[]
        {
        paramId,
        paramStatut,
        paramDate,
        paramMontant,
        paramPaiement,
        paramIdClient,
        paramIdCuisinier
        };

        return ExecuterRequeteMAJ(requete, parametres) > 0;
    }

    /// <summary>
    /// Récupère l'identifiant disponible suivant pour une nouvelle commande.
    /// </summary>
    /// <returns>L'identifiant suivant disponible.</returns>

    public int ObtenirProchainIdCommande()
    {
        object resultat = ExecuterRequeteScalaire("SELECT MAX(id_commande) FROM Commande");
        return (resultat == DBNull.Value) ? 1 : Convert.ToInt32(resultat) + 1;
    }

    #endregion
}
