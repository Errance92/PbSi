using System;
namespace Karaté
{
    using System;
    using System.Collections.Generic;

    namespace Karaté
    {
        public class GrapheCC
        {
            private List<NoeudCC> noeudsCC;
            private List<LienCC> liensCC;

            public List<NoeudCC> NoeudsCC
            {
                get { return noeudsCC; }
            }

            public List<LienCC> LiensCC
            {
                get { return liensCC; }
            }

            public GrapheCC()
            {
                noeudsCC = new List<NoeudCC>();
                liensCC = new List<LienCC>();

                Dictionary<string, NoeudCC> dicoNoeuds = new Dictionary<string, NoeudCC>();

                DbAccess db = new DbAccess();
                List<Commande> commandes = db.RecupererCommandes();

                foreach (Commande commande in commandes)
                {
                    string cleClient = "Client_" + commande.IdClient;
                    string cleCuisinier = "Cuisinier_" + commande.IdCuisinier;

                    if (!dicoNoeuds.ContainsKey(cleClient))
                    {
                        NoeudCC client = new NoeudCC(commande.IdClient, "Client " + commande.IdClient);
                        dicoNoeuds[cleClient] = client;
                        noeudsCC.Add(client);
                    }

                    if (!dicoNoeuds.ContainsKey(cleCuisinier))
                    {
                        NoeudCC cuisinier = new NoeudCC(commande.IdCuisinier, "Cuisinier " + commande.IdCuisinier);
                        dicoNoeuds[cleCuisinier] = cuisinier;
                        noeudsCC.Add(cuisinier);
                    }

                    NoeudCC n1 = dicoNoeuds[cleClient];
                    NoeudCC n2 = dicoNoeuds[cleCuisinier];
                    LienCC lien = new LienCC(n1, n2);
                    liensCC.Add(lien);
                }

            }
        }
    }
}

