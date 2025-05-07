CREATE DATABASE IF NOT EXISTS `Liv'in Paris`;
USE `Liv'in Paris`;

CREATE TABLE IF NOT EXISTS Client (
    id_client INT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    numero_rue INT(100) NOT NULL,
    ville VARCHAR(100) NOT NULL,
    rue VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    telephone VARCHAR(20),
    montant_achat DOUBlE NOT NULL,
    metro VARCHAR(100) 
);
CREATE TABLE IF NOT EXISTS Cuisinier (
    id_cuisinier INT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    numero_rue VARCHAR(100) NOT NULL,
    ville VARCHAR(100) NOT NULL,
    rue VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    telephone VARCHAR(20),
    metro VARCHAR(100)
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
    _type VARCHAR(50) NOT NULL,
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
    id_cuisinier INT,
    id_plat INT,
    FOREIGN KEY (id_client) REFERENCES Client(id_client),
    FOREIGN KEY (id_cuisinier) REFERENCES Cuisinier(id_cuisinier),
    FOREIGN KEY (id_plat) REFERENCES Plat(id_plat)
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
CREATE TABLE IF NOT EXISTS Utilisateur (
    id_utilisateur INT PRIMARY KEY,
    nom_utilisateur VARCHAR(100) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(100) NOT NULL,
    role VARCHAR(20) NOT NULL,
    id_reference INT
);


INSERT INTO Client (id_client, nom, prenom, numero_rue, ville, rue, email, telephone, montant_achat, metro) VALUES
(1, 'Dubois', 'Marie', 15, 'Paris', 'Rue de la Paix', 'marie.dubois@email.com', '0612345678', 150.50, 'Opéra'),
(2, 'Martin', 'Jean', 42, 'Paris', 'Boulevard Haussmann', 'jean.martin@email.com', '0687654321', 89.90, 'Champs-Élysées'),
(3, 'Petit', 'Sophie', 7, 'Lyon', 'Rue Garibaldi', 'sophie.petit@email.com', '0623456789', 210.75, 'Part-Dieu'),
(4, 'Bernard', 'Thomas', 23, 'Marseille', 'Rue Paradis', 'thomas.bernard@email.com','0698765432', 45.20, 'Vieux-Port'),
(5, 'Leroy', 'Émilie', 56, 'Paris', 'Rue Saint-Honoré', 'emilie.leroy@email.com', '0634567890', 320.00, 'Louvre');


INSERT INTO Cuisinier (id_cuisinier, nom, prenom, numero_rue, ville, rue, email, telephone, metro) VALUES
(1, 'Lambert', 'Michel', '23', 'Paris', 'Rue des Martyrs', 'michel.lambert@email.com', '0745678901', 'Pigalle'),
(2, 'Moreau', 'Claire', '18', 'Paris', 'Rue Montorgueil', 'claire.moreau@email.com', '0756789012', 'Étienne Marcel'),
(3, 'Roux', 'Julien', '92', 'Lyon', 'Rue de la République', 'julien.roux@email.com', '0767890123', 'Bellecour'),
(4, 'Fournier', 'Camille', '10', 'Marseille', 'Rue de Rome', 'camille.fournier@email.com', '0778901234', 'Castellane'),
(5, 'Girard', 'Antoine', '34', 'Paris', 'Avenue de Clichy', 'antoine.girard@email.com', '0789012345', 'Rome');


INSERT INTO Itineraire (id_itineraire, distance_km, duree_min, chemin) VALUES
(1, 3.5, 12, 'Rue de la Paix -> Boulevard des Capucines -> Place Vendôme'),
(2, 5.2, 18, 'Boulevard Haussmann -> Rue du Faubourg Saint-Honoré -> Place de la Concorde'),
(3, 2.8, 10, 'Rue Garibaldi -> Cours Lafayette -> Boulevard Vivier Merle'),
(4, 4.1, 15, 'Rue Paradis -> La Canebière -> Quai du Port'),
(5, 1.9, 8, 'Rue Saint-Honoré -> Rue de Rivoli -> Place du Palais Royal');


INSERT INTO Plat (id_plat, nom_plat, _type, stock, origine, regime_alimentaire, ingredient, lien_photo, date_fabrication, prix_par_personne, date_peremption) VALUES
(1, 'Bœuf Bourguignon', 'Plat principal', 25, 'France', 'Non-végétarien', 'Bœuf, carottes, oignons, champignons, vin rouge, thym, laurier', 'https://photos/boeuf_bourguignon.jpg', '2025-05-05', 18.50, '2025-05-08'),
(2, 'Ratatouille', 'Accompagnement', 30, 'France', 'Végétarien', 'Aubergines, courgettes, poivrons, tomates, oignons, ail, herbes de Provence', 'https://photos/ratatouille.jpg', '2025-05-05', 12.90, '2025-05-10'),
(3, 'Pad Thai', 'Plat principal', 15, 'Thaïlande', 'Omnivore', 'Nouilles de riz, crevettes, tofu, œuf, germes de soja, cacahuètes, citronnelle', 'https://photos/pad_thai.jpg', '2025-05-04', 16.75, '2025-05-07'),
(4, 'Tiramisu', 'Dessert', 20, 'Italie', 'Végétarien', 'Mascarpone, café, biscuits, cacao, œufs, sucre', 'https://photos/tiramisu.jpg', '2025-05-05', 8.20, '2025-05-09'),
(5, 'Couscous Royal', 'Plat principal', 18, 'Maghreb', 'Non-végétarien', 'Semoule, agneau, poulet, merguez, pois chiches, carottes, courgettes, navets, épices', 'https://photos/couscous.jpg', '2025-05-04', 22.00, '2025-05-08');


INSERT INTO Commande (id_commande, statu_commande, date_commande, montant, paiement, id_client, id_cuisinier, id_plat) VALUES
(1, 'Livrée', '2025-05-01 12:30:00', 18.50, 'Carte bancaire', 1, 1, 1),
(2, 'En préparation', '2025-05-05 19:15:00', 12.90, 'PayPal', 2, 2, 2),
(3, 'Validée', '2025-05-05 18:45:00', 16.75, 'Carte bancaire', 3, 3, 3),
(4, 'Livrée', '2025-05-03 20:00:00', 8.20, 'Espèces', 4, 4, 4),
(5, 'En cours de livraison', '2025-05-05 13:20:00', 22.00, 'Carte bancaire', 5, 5, 5);
n
INSERT INTO Transaction (id_transaction, mode_paiement, statu_paiement, date_paiement, id_commande) VALUES
(1, 'Carte bancaire', 'Validé', '2025-05-01 12:35:00', 1),
(2, 'PayPal', 'En attente', NULL, 2),
(3, 'Carte bancaire', 'Validé', '2025-05-05 18:50:00', 3),
(4, 'Espèces', 'Validé', '2025-05-03 20:15:00', 4),
(5, 'Carte bancaire', 'Validé', '2025-05-05 13:25:00', 5);


INSERT INTO Livraison (id_livraison, date_livraison, addresse_livraison, statu_livraison, id_commande) VALUES
(1, '2025-05-01 13:10:00', '15 Rue de la Paix, Paris', 'Livrée', 1),
(2, '2025-05-05 20:00:00', '42 Boulevard Haussmann, Paris', 'Programmée', 2),
(3, '2025-05-05 19:30:00', '7 Rue Garibaldi, Lyon', 'En attente', 3),
(4, '2025-05-03 20:45:00', '23 Rue Paradis, Marseille', 'Livrée', 4),
(5, '2025-05-05 14:00:00', '56 Rue Saint-Honoré, Paris', 'En cours', 5);


INSERT INTO Avis (id_avis, note, commentaire, date_avis, id_client, id_cuisinier) VALUES
(1, 5, 'Excellent plat, livraison rapide !', '2025-05-02', 1, 1),
(2, 4, 'Très bon, mais légèrement trop salé à mon goût.', '2025-05-04', 2, 2),
(3, 5, 'Délicieux et authentique, je recommande !', '2025-05-04', 3, 3),
(4, 3, 'Correct mais pas exceptionnel.', '2025-05-04', 4, 4),
(5, 5, 'Saveurs incroyables, un vrai régal !', '2025-05-05', 5, 5);


INSERT INTO Commande_Itineraire (id_commande, id_itineraire) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);
INSERT INTO Utilisateur (id_utilisateur, nom_utilisateur, mot_de_passe, role, id_reference) 
VALUES (1, 'admin', 'admin123', 'Admin', NULL);




select * from Utilisateur;

