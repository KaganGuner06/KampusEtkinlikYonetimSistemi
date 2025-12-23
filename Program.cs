using System;
using System.Windows.Forms;
using CampusEventManager; 

namespace CampusEventManager
{
    static class Program
    {
        
        
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            
            
            
            Application.Run(new FormLogin());
        }
    }
}