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
                res = res = "[" + IdClient + "] - " + Nom.ToUpper() + " - " + Prenom + " - " + NumRue + " rue " + NomRue + " " + Ville + " - " + Email + " - " + Telephone + " - " + MontantAchat + "€ - " + Metro.ToLower();
            }
        
        return res; 
        }
    }

    public class Cuisinier
    {
        public int IdCuisinier;
        public string Nom;
        public string Prenom;
        public string Adresse;
        public string Email;
        public string Telephone;
        
        public  string ToString()
        {
            return "[" + IdCuisinier + "] - " + Nom + " - " + Prenom + " - "+Adresse + " - " + Email + " - " + Telephone;
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
        public decimal Montant;
        public string Paiement;
        public int IdClient;
        public List<Ligne> Lignes = new List<Ligne>();
        
        public string ToString()
        {
            return "Commande #" + IdCommande + " - " + DateCommande.ToShortDateString() + " - " + Montant + "€";
        }
    }

    public class Ligne
    {
        public int IdLigne;
        public int Quantite;
        public decimal PrixTotal;
        public DateTime? DateLivraison;
        public string Lieu;
        public int IdCommande;
        public List<Plat> Plats = new List<Plat>();
    }

    public class Itineraire
    {
        public int IdItineraire;
        public decimal DistanceKm;
        public int DureeMin;
        public string Chemin;
    }