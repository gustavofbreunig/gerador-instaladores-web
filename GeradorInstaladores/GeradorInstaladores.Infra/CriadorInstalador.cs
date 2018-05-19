using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private string _pastaDrivers { get; set; }
        private string _pastaINNO { get; set; }
        private Instalador _instalador { get; set; }

        private string sufixoArquivos = Guid.NewGuid().ToString().Substring(0, 4);

        /// <summary>
        /// Arquivo (.bat) com os instaladores impressoras versão x86
        /// </summary>
        private string _instaladorBATx86
        {
            get
            {
                return "instalador_x86_" + sufixoArquivos + ".bat";
            }
        }

        /// <summary>
        /// Arquivo (.bat) com os instaladores impressoras versão x64
        /// </summary>
        private string _instaladorBATx64
        {
            get
            {
                return "instalador_x64_" + sufixoArquivos + ".bat";
            }
        }

        /// <summary>
        /// Lista de impressoras (.txt)
        /// </summary>
        private string _resumoTXT
        {
            get
            {
                return "resumo_" + sufixoArquivos + ".txt";
            }
        }

        private void CriaResumoTXT()
        {
            try
            {
                string caminho = Path.Combine(_pastaDrivers, _resumoTXT);
                using (FileStream fs = File.Create(caminho))
                {
                    string texto = _instalador.Nome + "\r\n" + "\r\n" + "Lista de equipamentos:" + "\r\n" + "\r\n";

                    foreach (var equipamento in _instalador.Equipamentos)
                    {
                        texto += equipamento.IP + " - " + equipamento.ModeloEquipamento.NomeModelo + " - " + equipamento.Nome + "\r\n";
                    }
                    byte[] textoBytes = Encoding.Default.GetBytes(texto);
                    fs.Write(textoBytes, 0, textoBytes.Length);
                }
            }
            catch (Exception e)
            {
                if (OnErro != null)
                {
                    OnErro(                        
                        this,
                        new ProgressoEventArgs(_instalador.Id, e.Message + "\r\n" + e.StackTrace)
                        );
                }
            }
        }

        /// <summary>
        /// Arquivo (.iss) do INNO Setup
        /// </summary>
        private string _instaladorINNO
        {
            get
            {
                return "script_instalador_" + sufixoArquivos + ".iss";
            }
        }

        /// <summary>
        /// Instalador (.exe)
        /// </summary>
        private string _arquivoSaida
        {
            get
            {
                //retorna um nome de instalado normalizado (sem acentos e espaços) 
                return RemoveDiacritics(_instalador.Nome).Replace(" ", "_") + "_" + sufixoArquivos + ".exe";
            }
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

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

        public CriadorInstalador(Instalador instalador, string pastaDrivers, string pastaINNO)
        {
            _instalador = instalador;
            _pastaINNO = pastaINNO;
            _pastaDrivers = pastaDrivers;
        }

        public void CriaInstaladorINNO()
        {
            CriaResumoTXT();

        }
    }

    /// <summary>
    /// Argumentos para mudança de status da criação de um instalador, como erros, progresso, conclusão, etc..
    /// </summary>
    public class ProgressoEventArgs : EventArgs
    {
        public int IdInstalador { get; private set; }
        public string Mensagem { get; private set; }

        public ProgressoEventArgs(int IdInstalador, string Mensagem)
        {
            this.IdInstalador = IdInstalador;
            this.Mensagem = Mensagem;
        }
    }
}
