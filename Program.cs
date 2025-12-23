using System;
using System.Windows.Forms;
using CampusEventManager; // Senin proje ismin

namespace CampusEventManager
{
    static class Program
    {
        /// <summary>
        ///  Uygulamanın ana giriş noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // DİKKAT: Buraya yeni formumuzun adını yazıyoruz.
            // Eski hali: Application.Run(new LoginForm()); 
            // Yeni hali:
            Application.Run(new FormLogin());
        }
    }
}