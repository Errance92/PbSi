using System;
using System.Collections.Generic;

    public class Client
    {
        public int IdClient;
        public string Nom;
        public string Prenom;
        public int NumRue;
        public string NomRue;
        public string Ville;
        public string Email;
        public string Telephone;
        public double MontantAchat;
        public string Metro;
        
        public  string ToString()
        {
            string res;
            if (Email.Length == 0 && Telephone.Length == 0)
            {
                res = "[" + IdClient + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + MontantAchat + "€ - " + Metro.ToLower();
            }
            else if (Telephone.Length == 0)
            {
                res = "[" + IdClient + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Email + " - " + MontantAchat + "€ - " + Metro.ToLower();
            }

            else if (Email.Length == 0)
            {   
                res = "[" + IdClient + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Telephone + " - " + MontantAchat + "€ - " + Metro.ToLower();
            }
            else
            {
                res = "[" + IdClient + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Email + " - " + Telephone + " - " + MontantAchat + "€ - " + Metro.ToLower();
            }
        
        return res; 
        }
    }

public class Cuisinier
{
    public int IdCuisinier;
    public string Nom;
    public string Prenom;
    public int NumRue;
    public string NomRue;
    public string Ville;
    public string Email;
    public string Telephone;
    public string Metro;
    public int? IdPlat;


    public override string ToString()
    {
        string res;

        bool emailVide = string.IsNullOrEmpty(Email);
        bool telVide = string.IsNullOrEmpty(Telephone);
        string infoPlat;
        if (IdPlat == null)
        {
            infoPlat = "Aucun plat associé";
        }
        else
        {
            infoPlat = "Plat ID : " + IdPlat;
        }

        if (emailVide && telVide)
        {
            res = "[" + IdCuisinier + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Metro.ToLower() + " - " + infoPlat;
        }
        else if (telVide)
        {
            res = "[" + IdCuisinier + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Email + " - " + Metro.ToLower() + " - " + infoPlat;
        }
        else if (emailVide)
        {
            res = "[" + IdCuisinier + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Telephone + " - " + Metro.ToLower() + " - " + infoPlat;
        }
        else
        {
            res = "[" + IdCuisinier + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Email + " - " + Telephone + " - " + Metro.ToLower() + " - " + infoPlat;
        }

        return res;
    }
}

    public class Plat
    {
        public int IdPlat;
        public string NomPlat;
        public string Type;
        public int Stock;
        public string Origine;
        public string RegimeAlimentaire;
        public string Ingredient;
        public string LienPhoto;
        public DateTime DateFabrication;
        public decimal PrixParPersonne;
        public DateTime DatePeremption;
        
        public string ToString()
        {
            return "[" + IdPlat + "] " + NomPlat + " - " + PrixParPersonne + "€/pers";
        }
    }

    public class Commande
    {
        public int IdCommande;
        public string StatuCommande;
        public DateTime DateCommande;
        public double Montant;
        public string Paiement;
        public int IdClient;
        public int IdCuisinier;
        public int IdPlat;

        public string ToString()
        {
            return "Commande #" + IdCommande + " - " + DateCommande.ToShortDateString() + " - " + Montant + "€";
        }
    }
    public class Utilisateur
    {
        public int IdUtilisateur { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string Role { get; set; } // ADMIN, CUISINIER, CLIENT
        public int? IdReference { get; set; } // ID du client ou du cuisinier associé si applicable

        public override string ToString()
        {
            return $"[{IdUtilisateur}] {Email} - Rôle: {Role}";
        }
    }


public class Itineraire
    {
        public int IdItineraire;
        public decimal DistanceKm;
        public int DureeMin;
        public string Chemin;
    }