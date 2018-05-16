using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.Infra
{
    /// <summary>
    /// Classe responsável pela crição do instalador de impressoras, desacoplada do banco de dados.
    /// </summary>
    public class CriadorInstalador
    {
        /// <summary>
        /// Arquivo (.bat) com os instaladores impressoras versão x86
        /// </summary>
        private string _instaladorBATx86 { get; set; }

        /// <summary>
        /// Arquivo (.bat) com os instaladores impressoras versão x64
        /// </summary>
        private string _instaladorBATx64 { get; set; }

        /// <summary>
        /// Lista de impressoras (.txt)
        /// </summary>
        private string _resumoTXT { get; set; }

        /// <summary>
        /// Arquivo (.iss) do INNO Setup
        /// </summary>
        private string _instaladorINNO { get; set; }

        /// <summary>
        /// Instalador (.exe)
        /// </summary>
        private string _arquivoSaida { get; set; }

        /// <summary>
        /// Manipulador para eventos de mudança de status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void AtualizaStatusHandler(object sender, ProgressoEventArgs e);

        /// <summary>
        /// Ocorre quado algum progresso acontece na criação do instalador.
        /// </summary>
        public event AtualizaStatusHandler OnMensagemProgresso;

        /// <summary>
        /// Ocorre quando acontece um erro na criação do instalador.
        /// </summary>
        public event AtualizaStatusHandler OnErro;

        /// <summary>
        /// Ocorre quando o instalador é criado com sucesso.
        /// </summary>
        public event AtualizaStatusHandler OnConclusao;

        public CriadorInstalador(Instalador instalador, string pastaInstalador, string pastaINNO)
        {

        }

        public void CriaInstaladorINNO()
        {

        }
    }

    /// <summary>
    /// Argumentos para mudança de status da criação de um instalador, como erros, progresso, conclusão, etc..
    /// </summary>
    public class ProgressoEventArgs : EventArgs
    {
        public int _IdInstalador { get; private set; }
        public string _Mensagem { get; private set; }

        public ProgressoEventArgs(int IdInstalador, string Mensagem)
        {
            this._IdInstalador = IdInstalador;
            this._Mensagem = Mensagem;
        }
    }
}
