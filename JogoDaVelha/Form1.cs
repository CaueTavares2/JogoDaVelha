using System;
using System.Drawing;
using System.Windows.Forms;
using JogoDaVelha.Properties; // Importa os recursos (imagens X e O)

namespace JogoDaVelha
{
    // A classe principal do seu formulário
    public partial class Form1 : Form
    {
        // 1. Variáveis de Estado (Matriz de Dados e Controle)
        private string[,] MatrizDeEstado = new string[3, 3];
        // X agora é DarkViolet, O é LimeGreen
        private bool VezDoX = true;
        private bool JogoFinalizado = false;
        private int ContadorDeJogadas = 0;

        public Form1()
        {
            InitializeComponent();
            // Chamadas de inicialização
            ConfigurarEstiloEEventos();
            InicializarTabuleiro();
        }

        // --- Configuração e Inicialização ---

        private void ConfigurarEstiloEEventos()
        {
            // Aplica o esquema de cores Moderno/Tech no formulário principal
            this.BackColor = Color.Gainsboro;

            // Itera sobre todos os controles no formulário para configurar as PictureBoxes
            foreach (Control controle in this.Controls)
            {
                // Verifica se é uma PictureBox e se o nome segue o novo padrão 'pb' + número
                if (controle is PictureBox && controle.Name.StartsWith("pb") && controle.Name.Length <= 4)
                {
                    PictureBox pb = (PictureBox)controle;
                    // NOTA SOBRE CS1061: A linha abaixo garante que todas as PictureBoxes usem o mesmo método.
                    // Se você está vendo CS1061, vá para o Form1.Designer.cs (ou a janela de Eventos) 
                    // e certifique-se de que o evento 'Click' de cada PictureBox (pb1, pb2, etc.)
                    // esteja definido como 'PictureBox_Click' e NÃO como 'pb1_Click' ou similar.
                    pb.Click += PictureBox_Click;
                    pb.BackColor = Color.White; // Fundo da célula
                    pb.BorderStyle = BorderStyle.FixedSingle; // Borda sutil (DarkGray)
                }
            }

            // Associa o evento de clique do botão de reinício (nomeado como btnReset)
            btnReset.Click += btnReset_Click;
            lblStatus.ForeColor = Color.DimGray;
        }

        private void InicializarTabuleiro()
        {
            // Zera a matriz, o contador e o estado do jogo
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MatrizDeEstado[i, j] = ""; // Limpa a matriz
                }
            }

            // Limpa as imagens visuais em todas as PictureBoxes
            foreach (Control controle in this.Controls)
            {
                // Verifica se é uma PictureBox e se o nome segue o novo padrão 'pb' + número
                if (controle is PictureBox && controle.Name.StartsWith("pb") && controle.Name.Length <= 4)
                {
                    ((PictureBox)controle).Image = null;
                }
            }

            VezDoX = true;
            JogoFinalizado = false;
            ContadorDeJogadas = 0;
            lblStatus.Text = "Vez do: X";
            lblStatus.ForeColor = Color.DimGray;
        }


        // --- Lógica Principal do Jogo (Chamada em cada clique da PictureBox) ---

        private void PictureBox_Click(object sender, EventArgs e)
        {
            // 0. Condição de Parada
            if (JogoFinalizado) return;

            PictureBox pbClicada = (PictureBox)sender;

            // 1. Mapeamento: Obtém o número (1 a 9) e converte para (Linha, Coluna)
            // Novo Mapeamento: pb1 -> 1, pb9 -> 9
            int numeroControle = int.Parse(pbClicada.Name.Substring(2));
            (int linha, int coluna) = MapearNumeroParaMatriz(numeroControle);

            // 2. Checar Jogada Válida (célula vazia)
            if (MatrizDeEstado[linha, coluna] != "")
            {
                return;
            }

            // 3. Determinar Símbolo e Imagem (Cores Moderno/Tech INVERTIDAS)
            string simbolo;
            Image imagemSimbolo;

            if (VezDoX)
            {
                simbolo = "X";
                // X (DarkViolet)
                // CORREÇÃO CS0117: Usando o novo nome de recurso X_DarkViolet
                imagemSimbolo = Resources.X_DarkViolet;
            }
            else
            {
                simbolo = "O";
                // O (LimeGreen)
                // CORREÇÃO CS0117: Usando o novo nome de recurso O_LimeGreen
                imagemSimbolo = Resources.O_LimeGreen;
            }

            // 4. Atualizar Estado (Matriz e UI)
            MatrizDeEstado[linha, coluna] = simbolo;
            pbClicada.Image = imagemSimbolo;
            ContadorDeJogadas++;

            // 5. Verificar Resultado
            if (VerificarVitoria(simbolo))
            {
                lblStatus.Text = $"VENCEDOR: {simbolo}!";
                // Destaca o vencedor com a cor CORRETA (X=DarkViolet, O=LimeGreen)
                lblStatus.ForeColor = (simbolo == "X") ? Color.DarkViolet : Color.LimeGreen;
                JogoFinalizado = true;
                return;
            }

            // Verifica empate se todas as 9 jogadas foram feitas sem vencedor
            if (ContadorDeJogadas == 9)
            {
                lblStatus.Text = "EMPATE! (Jogo Finalizado)";
                JogoFinalizado = true;
                return;
            }

            // 6. Trocar a Vez e atualizar o Label
            VezDoX = !VezDoX;
            lblStatus.Text = "Vez do: " + (VezDoX ? "X" : "O");
        }

        /// <summary>
        /// Converte o número sequencial do controle (1 a 9) para coordenadas (Linha, Coluna) da matriz (0,0 a 2,2).
        /// </summary>
        /// <param name="numero">Número do PictureBox (1 a 9).</param>
        /// <returns>Tupla (Linha, Coluna).</returns>
        private (int linha, int coluna) MapearNumeroParaMatriz(int numero)
        {
            // O mapeamento é: (Número - 1) / 3 para a linha, e (Número - 1) % 3 para a coluna.
            int index = numero - 1;
            int linha = index / 3;
            int coluna = index % 3;
            return (linha, coluna);
        }

        private bool VerificarVitoria(string simbolo)
        {
            // A. Verifica Linhas e Colunas (3 em linha, 3 em coluna)
            for (int i = 0; i < 3; i++)
            {
                // Linhas
                if (MatrizDeEstado[i, 0] == simbolo && MatrizDeEstado[i, 1] == simbolo && MatrizDeEstado[i, 2] == simbolo)
                {
                    return true;
                }
                // Colunas
                if (MatrizDeEstado[0, i] == simbolo && MatrizDeEstado[1, i] == simbolo && MatrizDeEstado[2, i] == simbolo)
                {
                    return true;
                }
            }

            // B. Verifica Diagonais

            // Diagonal Principal (Superior Esquerda para Inferior Direita)
            if (MatrizDeEstado[0, 0] == simbolo && MatrizDeEstado[1, 1] == simbolo && MatrizDeEstado[2, 2] == simbolo)
            {
                return true;
            }

            // Diagonal Secundária (Superior Direita para Inferior Esquerda)
            if (MatrizDeEstado[0, 2] == simbolo && MatrizDeEstado[1, 1] == simbolo && MatrizDeEstado[2, 0] == simbolo)
            {
                return true;
            }

            return false;
        }

        // --- Evento do Botão Reset ---

        private void btnReset_Click(object sender, EventArgs e)
        {
            InicializarTabuleiro();
        }

        private void pb4_Click(object sender, EventArgs e)
        {

        }

        private void pb5_Click(object sender, EventArgs e)
        {

        }

        private void pb6_Click(object sender, EventArgs e)
        {

        }

        private void pb7_Click(object sender, EventArgs e)
        {

        }

        private void pb8_Click(object sender, EventArgs e)
        {

        }

        private void pb9_Click(object sender, EventArgs e)
        {

        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {

        }
    }
}
