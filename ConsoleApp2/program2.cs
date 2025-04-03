using System;

namespace LivinParis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Liv'in Paris - Application de partage de repas";
            
            try
            {
                UserInterface ui = new UserInterface();
                ui.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erreur fatale: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
                Console.WriteLine("\nAppuyez sur une touche pour quitter...");
                Console.ReadKey();
            }
        }
    }
}
