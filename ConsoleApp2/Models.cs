using System;
using System.Collections.Generic;

namespace LivinParis
{
    public class Client
    {
        public int IdClient;
        public string Nom;
        public string Prenom;
        public string Adresse;
        public string Email;
        public string Telephone;
        
        public string ToString()
        {
            return "[" + IdClient + "] " + Prenom + " " + Nom;
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
        
        public string ToString()
        {
            return "[" + IdCuisinier + "] " + Prenom + " " + Nom;
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
}