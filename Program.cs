using System;
using System.Windows.Forms; // Agregar esta l�nea

namespace Buscaminas
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicaci�n.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Crear una instancia de Form1
        }
    }
}