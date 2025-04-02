using System;
using System.Collections.Generic;

namespace LivinParis
{
    // Modèles pour les entités principales
    public class Client
    {
        public int IdClient { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        
        public override string ToString() => $"[{IdClient}] {Prenom} {Nom}";
    }

    public class Cuisinier
    {
        public int IdCuisinier { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        
        public override string ToString() => $"[{IdCuisinier}] {Prenom} {Nom}";
    }

    public class Plat
    {
        public int IdPlat { get; set; }
        public string NomPlat { get; set; }
        public string Type { get; set; }
        public int Stock { get; set; }
        public string Origine { get; set; }
        public string RegimeAlimentaire { get; set; }
        public string Ingredient { get; set; }
        public string LienPhoto { get; set; }
        public DateTime DateFabrication { get; set; }
        public decimal PrixParPersonne { get; set; }
        public DateTime DatePeremption { get; set; }
        
        public override string ToString() => $"[{IdPlat}] {NomPlat} - {PrixParPersonne}€/pers";
    }

    public class Commande
    {
        public int IdCommande { get; set; }
        public string StatuCommande { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }
        public string Paiement { get; set; }
        public int IdClient { get; set; }
        public List<Ligne> Lignes { get; set; } = new List<Ligne>();
        
        public override string ToString() => $"Commande #{IdCommande} - {DateCommande.ToShortDateString()} - {Montant}€";
    }

    public class Ligne
    {
        public int IdLigne { get; set; }
        public int Quantite { get; set; }
        public decimal PrixTotal { get; set; }
        public DateTime? DateLivraison { get; set; }
        public string Lieu { get; set; }
        public int IdCommande { get; set; }
        public List<Plat> Plats { get; set; } = new List<Plat>();
    }

    public class Itineraire
    {
        public int IdItineraire { get; set; }
        public decimal DistanceKm { get; set; }
        public int DureeMin { get; set; }
        public string Chemin { get; set; }
    }
}
