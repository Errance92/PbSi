using System;

public class AuthenticationManager
{
    private readonly DbAccess db;
    private Utilisateur utilisateurConnecte;

    public AuthenticationManager()
    {
        db = new DbAccess();
        utilisateurConnecte = null;
    }

    public Utilisateur UtilisateurConnecte
    {
        get { return utilisateurConnecte; }
    }

    public bool EstConnecte
    {
        get { return utilisateurConnecte != null; }
    }

    public bool EstAdmin
    {
        get { return EstConnecte && utilisateurConnecte.Role == "ADMIN"; }
    }

    public bool EstCuisinier
    {
        get { return EstConnecte && utilisateurConnecte.Role == "CUISINIER"; }
    }

    public bool EstClient
    {
        get { return EstConnecte && utilisateurConnecte.Role == "CLIENT"; }
    }

    public bool Connecter(string email, string motDePasse)
    {
        utilisateurConnecte = db.Authentifier(email, motDePasse);
        return EstConnecte;
    }

    public void Deconnecter()
    {
        utilisateurConnecte = null;
    }

    public bool CreerCompte(string email, string motDePasse, string role, int? idReference)
    {
        if (db.EmailExiste(email))
        {
            return false;
        }

        Utilisateur utilisateur = new Utilisateur
        {
            Email = email,
            MotDePasse = motDePasse,
            Role = role,
            IdReference = idReference
        };

        return db.CreerUtilisateur(utilisateur);
    }

    public bool PeutAccederModule(string module)
    {
        if (!EstConnecte)
            return false;

        switch (module)
        {
            case "ADMIN":
                return EstAdmin;
            case "CLIENT":
                return EstAdmin || EstClient;
            case "CUISINIER":
                return EstAdmin || EstCuisinier;
            case "COMMANDE":
                return EstConnecte;
            case "PLAT":
                return EstAdmin || EstCuisinier;
            case "STATISTIQUES":
                return EstAdmin;
            default:
                return false;
        }
    }
}