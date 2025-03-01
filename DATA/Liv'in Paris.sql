CREATE DATABASE IF NOT EXISTS `Liv'in Paris`;
USE `Liv'in Paris`;

CREATE TABLE IF NOT EXISTS Client (
    id_client INT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    addresse VARCHAR(255) NOT NULL,
    email VARCHAR(100),
    telephone VARCHAR(20)
);
CREATE TABLE IF NOT EXISTS Cuisinier (
    id_cuisinier INT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    addresse VARCHAR(255) NOT NULL,
    email VARCHAR(100),
    telephone VARCHAR(20)
);
CREATE TABLE IF NOT EXISTS Itineraire (
    id_itineraire INT PRIMARY KEY,
    distance_km DECIMAL(10,2) NOT NULL,
    duree_min INT NOT NULL,
    chemin TEXT
);

CREATE TABLE IF NOT EXISTS Plat (
    id_plat INT PRIMARY KEY,
    nom_plat VARCHAR(100) NOT NULL,
    type VARCHAR(50) NOT NULL,
    stock INT NOT NULL,
    origine VARCHAR(100),
    regime_alimentaire VARCHAR(50),
    ingredient TEXT,
    lien_photo VARCHAR(255),
    date_fabrication DATE,
    prix_par_personne DECIMAL(10,2) NOT NULL,
    date_peremption DATE
);
CREATE TABLE IF NOT EXISTS Commande (
    id_commande INT PRIMARY KEY,
    statu_commande VARCHAR(50) NOT NULL,
    date_commande DATETIME NOT NULL,
    montant DECIMAL(10,2) NOT NULL,
    paiement VARCHAR(50),
    id_client INT,
    FOREIGN KEY (id_client) REFERENCES Client(id_client)
);
CREATE TABLE IF NOT EXISTS Transaction (
    id_transaction INT PRIMARY KEY,
    mode_paiement VARCHAR(50) NOT NULL,
    statu_paiement VARCHAR(50) NOT NULL,
    date_paiement DATETIME,
    id_commande INT,
    FOREIGN KEY (id_commande) REFERENCES Commande(id_commande)
);
CREATE TABLE IF NOT EXISTS Livraison (
    id_livraison INT PRIMARY KEY,
    date_livraison DATETIME NOT NULL,
    addresse_livraison VARCHAR(255) NOT NULL,
    statu_livraison VARCHAR(50) NOT NULL,
    id_commande INT,
    FOREIGN KEY (id_commande) REFERENCES Commande(id_commande)
);
CREATE TABLE IF NOT EXISTS Ligne (
    id_ligne INT PRIMARY KEY,
    quantite INT NOT NULL,
    prix_total DECIMAL(10,2) NOT NULL,
    date_livraison DATETIME,
    lieu VARCHAR(255),
    id_commande INT,
    FOREIGN KEY (id_commande) REFERENCES Commande(id_commande)
);
CREATE TABLE IF NOT EXISTS Avis (
    id_avis INT PRIMARY KEY,
    note INT NOT NULL,
    commentaire TEXT,
    date_avis DATE NOT NULL,
    id_client INT,
    id_cuisinier INT,
    FOREIGN KEY (id_client) REFERENCES Client(id_client),
    FOREIGN KEY (id_cuisinier) REFERENCES Cuisinier(id_cuisinier)
);
CREATE TABLE IF NOT EXISTS Commande_Itineraire (
    id_commande INT,
    id_itineraire INT,
    PRIMARY KEY (id_commande, id_itineraire),
    FOREIGN KEY (id_commande) REFERENCES Commande(id_commande),
    FOREIGN KEY (id_itineraire) REFERENCES Itineraire(id_itineraire)
);
CREATE TABLE IF NOT EXISTS Ligne_Plat (
    id_ligne INT,
    id_plat INT,
    PRIMARY KEY (id_ligne, id_plat),
    FOREIGN KEY (id_ligne) REFERENCES Ligne(id_ligne),
    FOREIGN KEY (id_plat) REFERENCES Plat(id_plat)
);
CREATE TABLE IF NOT EXISTS Cuisinier_Plat (
    id_cuisinier INT,
    id_plat INT,
    PRIMARY KEY (id_cuisinier, id_plat),
    FOREIGN KEY (id_cuisinier) REFERENCES Cuisinier(id_cuisinier),
    FOREIGN KEY (id_plat) REFERENCES Plat(id_plat)
);

-- INSERT INTO Client (id_client, nom, prenom, addresse, email, telephone) 
-- VALUES (1, 'Durand', 'Medhy', 'Rue Cardinet, 15, 75017 Paris', 'Mdurand@gmail.com', '1234567890');
-- INSERT INTO Cuisinier (id_cuisinier, nom, prenom, addresse, email, telephone) 
-- VALUES (1, 'Dupond', 'Marie', 'Rue de la République, 30, 75011 Paris', 'Mdupond@gmail.com', '1234567890');
-- INSERT INTO Plat (id_plat, nom_plat, type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption) 
-- VALUES (1, 'Raclette', 'Plat', 100, 'Française', NULL, 'raclette fromage: 250g, pommes_de_terre: 200g, jambon: 200g, cornichon: 3p', NULL, '2025-01-10', 10, '2025-01-15');
-- INSERT INTO Plat (id_plat, nom_plat, type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption) 
-- VALUES (2, 'Salade de fruit', 'Dessert', 100, 'Indifférent', 'Végétarien', 'fraise: 100g, kiwi: 100g, sucre: 10g', NULL, '2025-01-10', 5, '2025-01-15');
-- INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client) 
-- VALUES (1, 'En cours', '2025-03-01 15:25:46', 60.00, 'Carte de crédit', 1);
-- INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client) 
-- VALUES (2, 'En cours', '2025-03-01 15:25:46', 30.00, 'Carte de crédit', 1);
-- INSERT INTO Ligne (id_ligne, quantite, prix_total, date_livraison, lieu, id_commande) 
-- VALUES (101, 6, 60.00, '2025-03-01 15:25:46', 'Adresse de livraison par défaut', 1);
-- INSERT INTO Ligne (id_ligne, quantite, prix_total, date_livraison, lieu, id_commande) 
-- VALUES (201, 6, 30.00, '2025-03-01 15:25:46', 'Adresse de livraison par défaut', 2);
-- INSERT INTO Ligne_Plat (id_ligne, id_plat) VALUES (101, 1);
-- INSERT INTO Ligne_Plat (id_ligne, id_plat) VALUES (201, 2);
-- INSERT INTO Cuisinier_Plat (id_cuisinier, id_plat) VALUES (1, 1);
-- INSERT INTO Cuisinier_Plat (id_cuisinier, id_plat) VALUES (1, 2);


SELECT * FROM Client;

SELECT * FROM Cuisinier;

SELECT id_plat, nom_plat, type, prix_par_personne, origine, regime_alimentaire 
FROM Plat;

SELECT id_plat, nom_plat, type, stock, prix_par_personne, origine, regime_alimentaire, date_fabrication, date_peremption 
FROM Plat
ORDER BY type, nom_plat;
