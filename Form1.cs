using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Buscaminas
{
    public partial class Form1 : Form
    {
        private int[,] tablero;
        private bool[,] reveladas;
        private bool[,] marcadas;
        private Button[,] botones;
        private int filas, columnas, minas, minasRestantes;
        private Label lblMinasRestantes;
        private Button btnReiniciar;
        private bool juegoTerminado;

        public Form1()
        {
            InitializeComponent();
            MostrarConfiguracion();
        }

        private void MostrarConfiguracion()
        {
            int filas = 10; // Valor por defecto
            int columnas = 10; // Valor por defecto
            int minas = 20; // Valor por defecto

            // Pedir al usuario que ingrese el número de filas, columnas y minas
            string inputFilas = Interaction.InputBox("Bienvenido al Buscaminas\nPor favor, ingrese el número de filas (mínimo 2, máximo 20):", "Configuración", "10");
            if (string.IsNullOrEmpty(inputFilas))
            {
                // Si el usuario cancela, cerrar la aplicación
                Application.Exit();
                return;
            }

            if (!int.TryParse(inputFilas, out filas) || filas < 2 || filas > 20)
            {
                MessageBox.Show("Número de filas inválido o fuera de rango. Se usará el valor por defecto (10).");
                filas = 10;
            }

            string inputColumnas = Interaction.InputBox("Por favor, ingrese el número de columnas (mínimo 2, máximo 20):", "Configuración", "10");
            if (string.IsNullOrEmpty(inputColumnas))
            {
                // Si el usuario cancela, cerrar la aplicación
                Application.Exit();
                return;
            }

            if (!int.TryParse(inputColumnas, out columnas) || columnas < 2 || columnas > 20)
            {
                MessageBox.Show("Número de columnas inválido o fuera de rango. Se usará el valor por defecto (10).");
                columnas = 10;
            }

            string inputMinas = Interaction.InputBox("Por favor, ingrese el número de minas (mínimo 1, máximo 20):", "Configuración", "20");
            if (string.IsNullOrEmpty(inputMinas))
            {
                // Si el usuario cancela, cerrar la aplicación
                Application.Exit();
                return;
            }

            if (!int.TryParse(inputMinas, out minas) || minas < 2 || minas > filas * columnas)
            {
                MessageBox.Show("Número de minas inválido o fuera de rango. Se usará el valor por defecto (20).");
                minas = 20;
            }

            IniciarJuego(filas, columnas, minas); // Pasar los valores de filas, columnas y minas al método IniciarJuego
        }




        private void IniciarJuego(int filas, int columnas, int minas)
        {
            this.filas = filas;
            this.columnas = columnas;
            this.minas = minas; // Corregir la asignación de minas
            minasRestantes = minas;
            juegoTerminado = false;

            tablero = new int[filas, columnas];
            reveladas = new bool[filas, columnas];
            marcadas = new bool[filas, columnas];

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            tableLayoutPanel1.RowCount = filas;
            tableLayoutPanel1.ColumnCount = columnas;

            for (int i = 0; i < filas; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / filas));
            }

            for (int i = 0; i < columnas; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / columnas));
            }

            botones = new Button[filas, columnas];

            GenerarTablero();

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Button btn = new Button();
                    btn.Dock = DockStyle.Fill;
                    btn.BackColor = Color.LightGray;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.Black;
                    btn.MouseUp += Btn_MouseUp;
                    botones[i, j] = btn;
                    tableLayoutPanel1.Controls.Add(btn, j, i);
                }
            }

            if (lblMinasRestantes == null)
            {
                lblMinasRestantes = new Label();
                lblMinasRestantes.AutoSize = true;
                lblMinasRestantes.Dock = DockStyle.Top;
                this.Controls.Add(lblMinasRestantes);
            }

            if (btnReiniciar == null)
            {
                btnReiniciar = new Button();
                btnReiniciar.Text = "Reiniciar";
                btnReiniciar.AutoSize = true;
                btnReiniciar.Dock = DockStyle.Bottom;
                btnReiniciar.Click += BtnReiniciar_Click;
                this.Controls.Add(btnReiniciar);
            }

            lblMinasRestantes.Text = $"Minas restantes: {minasRestantes}";
        }


        private void BtnReiniciar_Click(object sender, EventArgs e)
        {
            MostrarConfiguracion();
        }


        private void GenerarTablero()
        {
            // Inicializar todas las celdas como celdas vacías
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    tablero[i, j] = 0;
                }
            }

            // Colocar las minas aleatoriamente en el tablero
            Random random = new Random();
            while (minas > 0)
            {
                int fila = random.Next(filas);
                int columna = random.Next(columnas);

                if (tablero[fila, columna] != -1)
                {
                    tablero[fila, columna] = -1;
                    minas--;
                }
            }

            // Calcular los números de celdas adyacentes
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (tablero[i, j] != -1)
                    {
                        int minasAdyacentes = ContarMinasAdyacentes(i, j);
                        tablero[i, j] = minasAdyacentes;
                    }
                }
            }
        }

        private int ContarMinasAdyacentes(int fila, int columna)
        {
            int minasAdyacentes = 0;

            for (int i = Math.Max(0, fila - 1); i <= Math.Min(filas - 1, fila + 1); i++)
            {
                for (int j = Math.Max(0, columna - 1); j <= Math.Min(columnas - 1, columna + 1); j++)
                {
                    if (i != fila || j != columna)
                    {
                        if (tablero[i, j] == -1)
                        {
                            minasAdyacentes++;
                        }
                    }
                }
            }

            return minasAdyacentes;
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (!juegoTerminado)
            {
                Button btnClickeado = (Button)sender;
                int fila = tableLayoutPanel1.GetRow(btnClickeado);
                int columna = tableLayoutPanel1.GetColumn(btnClickeado);

                if (e.Button == MouseButtons.Left)
                {
                    SeleccionarCelda(fila, columna);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    MarcarDesmarcarCelda(fila, columna);
                }

                ActualizarTablero();
            }
        }

        private void SeleccionarCelda(int fila, int columna)
        {
            if (!reveladas[fila, columna] && !marcadas[fila, columna])
            {
                if (tablero[fila, columna] == -1)
                {
                    // Perdió el juego
                    RevelarTodasLasMinas();
                    juegoTerminado = true;
                    DialogResult result = MessageBox.Show("Has perdido el juego. ¿Deseas continuar?", "Juego terminado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Continuar
                        minasRestantes = Math.Max(0, minasRestantes - 1);
                        reveladas[fila, columna] = true; // Revelar la celda con la mina detonada
                        lblMinasRestantes.Text = $"Minas restantes: {minasRestantes}";
                    }
                    else
                    {
                        // Cerrar la aplicación
                        Application.Exit();
                    }
                }
                else
                {
                    RevelarCelda(fila, columna);
                }
            }
        }

        private void MarcarDesmarcarCelda(int fila, int columna)
        {
            if (!reveladas[fila, columna] && !juegoTerminado)
            {
                marcadas[fila, columna] = !marcadas[fila, columna];
                minasRestantes += marcadas[fila, columna] ? -1 : 1;
                minasRestantes = Math.Max(0, minasRestantes);
                lblMinasRestantes.Text = $"Minas restantes: {minasRestantes}";
            }
        }

        private void RevelarCelda(int fila, int columna)
        {
            if (!reveladas[fila, columna] && tablero[fila, columna] != -1)
            {
                reveladas[fila, columna] = true;

                if (tablero[fila, columna] == 0)
                {
                    // Revelar celdas adyacentes vacías
                    for (int i = Math.Max(0, fila - 1); i <= Math.Min(filas - 1, fila + 1); i++)
                    {
                        for (int j = Math.Max(0, columna - 1); j <= Math.Min(columnas - 1, columna + 1); j++)
                        {
                            RevelarCelda(i, j);
                        }
                    }
                }

                // Verificar si se ha ganado el juego
                bool juegoGanado = true;
                for (int i = 0; i < filas; i++)
                {
                    for (int j = 0; j < columnas; j++)
                    {
                        if (!reveladas[i, j] && tablero[i, j] != -1)
                        {
                            juegoGanado = false;
                            break;
                        }
                    }
                    if (!juegoGanado)
                        break;
                }

                if (juegoGanado)
                {
                    juegoTerminado = true;
                    MessageBox.Show("¡Felicidades, has ganado el juego!");
                    
                }
            }
        }

        private void RevelarTodasLasMinas()
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    if (tablero[i, j] == -1)
                    {
                        reveladas[i, j] = true;
                    }
                }
            }
        }

        private void ActualizarTablero()
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    Button btn = botones[i, j];
                    btn.Text = ObtenerValorCelda(i, j).ToString();
                    btn.BackColor = ObtenerColorCelda(i, j);
                }
            }
        }

        private char ObtenerValorCelda(int fila, int columna)
        {
            if (marcadas[fila, columna])
                return '?';
            else if (reveladas[fila, columna])
            {
                if (tablero[fila, columna] == -1)
                    return '*';
                else
                    return tablero[fila, columna].ToString()[0];
            }
            else
                return ' ';
        }

        private Color ObtenerColorCelda(int fila, int columna)
        {
            if (!reveladas[fila, columna])
                return Color.LightGray;
            else if (tablero[fila, columna] == -1)
                return Color.Red;
            else if (tablero[fila, columna] == 0)
                return Color.LightGreen;
            else
                return Color.White;
        }
    }
}
