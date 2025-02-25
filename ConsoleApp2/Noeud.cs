using System;
namespace Karaté
{
    public class Noeud
    {
        private int id;

        public Noeud(int id)
        {
            this.id = id;
        }

        public int Id
        {
            get { return id; }
        }
    }
}