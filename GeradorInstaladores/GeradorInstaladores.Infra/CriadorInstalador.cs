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
        private string _AppName { get; set; }
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

        private enum ArquiteturaDoBatFile { x86, x64 };

        private void CriaBAT(ArquiteturaDoBatFile arq)
        {
            try
            {
                string nome_arquivo_bat;
                if (arq == ArquiteturaDoBatFile.x86)
                {
                    nome_arquivo_bat = Path.Combine(_pastaDrivers, _instaladorBATx86);
                }
                else if (arq == ArquiteturaDoBatFile.x64)
                {
                    nome_arquivo_bat = Path.Combine(_pastaDrivers, _instaladorBATx64);
                }
                else
                {
                    throw new Exception("Arquitetura incorreta/não implementada");
                }

                using (FileStream fs = File.Create(nome_arquivo_bat))
                {
                    StringBuilder sb = new StringBuilder();

                    //instalação dos drivers, agrupado por modelo
                    var modelos =
                        _instalador.Equipamentos
                        .GroupBy(p => p.ModeloEquipamento)
                        .Select(p => p.Key)
                        .ToArray();

                    sb.AppendLine("@echo off"); //tira a exibição do caminho durante a execução do batch

                    sb.AppendLine();
                    sb.AppendLine("REM DRIVERS");

                    foreach (var modelo in modelos)
                    {
                        sb.AppendLine("echo Instalando o driver " + modelo.NomeDriver);

                        string local_do_inf;
                        if (arq == ArquiteturaDoBatFile.x86)
                        {
                            local_do_inf = Path.Combine(modelo.PastaDriverX86, modelo.ArquivoINF);
                        }
                        else if (arq == ArquiteturaDoBatFile.x64)
                        {
                            local_do_inf = Path.Combine(modelo.PastaDriverX64, modelo.ArquivoINF);
                        }
                        else
                        {
                            throw new Exception("Arquitetura incorreta/não implementada");
                        }

                        //https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/rundll32-printui
                        sb.Append("rundll32 printui.dll,PrintUIEntry");//Automates many printer configuration tasks. printui.dll is the executable file that contains the functions used by the printer configuration dialog boxes
                        sb.Append(" /ia"); //Installs a printer driver by using an .inf file.
                        sb.Append(" /m " + "\"" + modelo.NomeDriver + "\""); //Specifies the driver model name. (This value can be specified in the .inf file.)
                        sb.Append(" /f " + "\"" + local_do_inf + "\""); //Species the Universal Naming Convention (UNC) path and name of the .inf file name or the output file name, depending on the task that you are performing.
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("REM PORTAS DE IMPRESSAO");

                    //instalação das portas
                    foreach (var equipamento in _instalador.Equipamentos)
                    {
                        sb.AppendLine("echo Criando porta de impressao para " + equipamento.Nome + "...");

                        //https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/prnport
                        sb.Append(@"cscript %WINDIR%\System32\Printing_Admin_Scripts\pt-BR\prnport.vbs");
                        sb.Append(" -a"); //creates a standard TCP/IP printer port.
                        sb.Append(" -r IP_" + equipamento.IP); //Specifies the port to which the printer is connected.
                        sb.Append(" -h"); //Specifies (by IP address) the printer for which you want to configure the port.
                        sb.Append(" " + equipamento.IP);
                        sb.Append(" -o raw"); //Specifies which protocol the port uses: TCP raw or TCP lpr.
                        sb.Append(" -n 9100"); //If you use TCP raw, you can optionally specify the port number by using the -n parameter. The default port number is 9100.
                        sb.AppendLine();
                    }

                    //instalação dos equipamentos
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("REM IMPRESSORAS");

                    foreach (var equipamento in _instalador.Equipamentos)
                    {
                        string local_do_inf;
                        if (arq == ArquiteturaDoBatFile.x86)
                        {
                            local_do_inf = Path.Combine(equipamento.ModeloEquipamento.PastaDriverX86, equipamento.ModeloEquipamento.ArquivoINF);
                        }
                        else if (arq == ArquiteturaDoBatFile.x64)
                        {
                            local_do_inf = Path.Combine(equipamento.ModeloEquipamento.PastaDriverX64, equipamento.ModeloEquipamento.ArquivoINF);
                        }
                        else
                        {
                            throw new Exception("Arquitetura incorreta/não implementada");
                        }

                        sb.AppendLine("echo Adicionando impressora " + equipamento.Nome + "...");

                        sb.Append("rundll32 printui.dll,PrintUIEntry");
                        sb.Append(" /if"); //Installs a printer by using an .inf file.
                        sb.Append(" /b " + "\"" + equipamento.Nome + "\""); //Specifies the base printer name.
                        sb.Append(" /f " + "\"" + local_do_inf + "\""); //Species the Universal Naming Convention (UNC) path and name of the .inf file name or the output file name, depending on the task that you are performing.
                        sb.Append(" /r " + "\"" + "IP_" + equipamento.IP + "\""); //Specifies the port to which the printer is connected.
                        sb.Append(" /m " + "\"" + equipamento.ModeloEquipamento.NomeDriver + "\""); //Specifies the driver model name. (This value can be specified in the .inf file.)
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                    sb.AppendLine("pause");

                    //default encoding para ser suportado pelo INNO, unicode não suportas
                    byte[] textoBytes = Encoding.Default.GetBytes(sb.ToString());
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
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(_instalador.Nome);
                    sb.AppendLine();
                    sb.AppendLine("Lista de equipamentos:");
                    sb.AppendLine();

                    foreach (var equipamento in _instalador.Equipamentos)
                    {
                        sb.AppendLine(equipamento.IP + " - " + equipamento.ModeloEquipamento.NomeModelo + " - " + equipamento.Nome);
                    }

                    //default encoding para ser suportado pelo INNO, unicode não suportas
                    byte[] textoBytes = Encoding.Default.GetBytes(sb.ToString());
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
        /// Cria o arquivo .iss com os comandos para gerar o instalador
        /// </summary>
        private void CriaArquivoISS()
        {
            try
            {
                string caminho = Path.Combine(_pastaDrivers, _instaladorINNO);
                using (FileStream fs = File.Create(caminho))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("[Setup]");
                    sb.AppendFormat("AppName={0}\r\n", _AppName);
                    sb.AppendLine("AppVersion=1.0");
                    sb.AppendLine("CreateAppDir=no");
                    //sb.AppendFormat("OutputBaseFilename={0}\r\n", _arquivoSaida); esse parâmetro é passado ao INNO Compiler
                    sb.AppendFormat("InfoBeforeFile={0}\r\n", _resumoTXT);
                    sb.AppendLine("Compression=lzma2");
                    sb.AppendLine("SolidCompression=yes");
                    sb.AppendLine("Uninstallable=no");

                    sb.AppendLine();

                    sb.AppendLine("[Languages]");
                    sb.AppendLine("Name: \"brazilianportuguese\"; MessagesFile: \"compiler:Languages\\BrazilianPortuguese.isl\"");

                    sb.AppendLine();
                    sb.AppendLine("[Messages]");
                    sb.AppendLine("BeveledLabel=AAB3BF192316348537C09A2CEC92E0F94C8FF6F313D33B961AA2DF345EB43296");

                    sb.AppendLine();
                    sb.AppendLine("[Files]");

                    sb.AppendFormat("Source: \"{0}\"; DestDir: {{tmp}}; Flags: deleteafterinstall\r\n", _instaladorBATx64);
                    sb.AppendFormat("Source: \"{0}\"; DestDir: {{tmp}}; Flags: deleteafterinstall\r\n", _instaladorBATx86);

                    //drivers 
                    var modelos =
                        _instalador.Equipamentos
                        .GroupBy(p => p.ModeloEquipamento)
                        .Select(p => p.Key)
                        .ToArray();

                    foreach (var modelo in modelos)
                    {
                        sb.AppendFormat("Source: \"{0}\\*\"; DestDir: {{tmp}}\\{0}; Flags: recursesubdirs createallsubdirs deleteafterinstall\r\n", modelo.PastaDriverX64);
                        sb.AppendFormat("Source: \"{0}\\*\"; DestDir: {{tmp}}\\{0}; Flags: recursesubdirs createallsubdirs deleteafterinstall\r\n", modelo.PastaDriverX86);
                    }

                    sb.AppendLine();
                    sb.AppendLine("[Run]");
                    sb.AppendFormat("Filename: {{tmp}}\\{0}; Check: IsWin64 \r\n", _instaladorBATx64);
                    sb.AppendFormat("Filename: {{tmp}}\\{0}; Check: not IsWin64 \r\n", _instaladorBATx86);

                    //default encoding para ser suportado pelo INNO, unicode não suportas
                    byte[] textoBytes = Encoding.Default.GetBytes(sb.ToString());
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

        public CriadorInstalador(
            Instalador instalador,
            string pastaDrivers,
            string pastaINNO,
            string AppName)
        {
            _instalador = instalador;
            _pastaINNO = pastaINNO;
            _pastaDrivers = pastaDrivers;
            _AppName = AppName;
        }

        public void CriaInstaladorINNO()
        {
            if (OnMensagemProgresso != null)
            {
                OnMensagemProgresso(
                    this,
                    new ProgressoEventArgs(_instalador.Id, "Criando resumo (.txt)...")
                    );
            }

            CriaResumoTXT();

            if (OnMensagemProgresso != null)
            {
                OnMensagemProgresso(
                    this,
                    new ProgressoEventArgs(_instalador.Id, "Criando batchs (.bat)...")
                    );
            }

            CriaBAT(ArquiteturaDoBatFile.x86);
            CriaBAT(ArquiteturaDoBatFile.x64);

            if (OnMensagemProgresso != null)
            {
                OnMensagemProgresso(
                    this,
                    new ProgressoEventArgs(_instalador.Id, "Criando inno installer (.iss)...")
                    );
            }

            CriaArquivoISS();

            if (OnMensagemProgresso != null)
            {
                OnMensagemProgresso(
                    this,
                    new ProgressoEventArgs(_instalador.Id, "Compilando...")
                    );
            }

            CompilaInstaladorINNO();

            if (OnConclusao != null)
            {
                OnConclusao(this,
                    new ProgressoEventArgs(_instalador.Id, _arquivoSaida)
                    );
            }
        }

        private void CompilaInstaladorINNO()
        {
            try
            {
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.FileName = Path.Combine(_pastaINNO, "ISCC.exe");
                pProcess.StartInfo.Arguments = _instaladorINNO + " /F" + "\"" + _arquivoSaida.Replace(".exe", "") /*o .exe é colocado pelo inno*/ + "\"";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.RedirectStandardError = true;
                pProcess.StartInfo.WorkingDirectory = _pastaDrivers;
                pProcess.Start();

                string strOutput = pProcess.StandardOutput.ReadToEnd();
                string strErrorOutput = pProcess.StandardError.ReadToEnd();

                pProcess.WaitForExit();

                //informa mensagens de saída do compilador INNO
                if (OnMensagemProgresso != null)
                {
                    OnMensagemProgresso(this, new ProgressoEventArgs(_instalador.Id, strOutput));
                }

                //se tiver erro, manda para a saída de erro
                if (!System.String.IsNullOrWhiteSpace(strErrorOutput) && OnErro != null)
                {
                    OnErro(this, new ProgressoEventArgs(_instalador.Id, "Erro compilando no INNO Setup: \r\n" + strErrorOutput));
                }
            }
            catch (Exception e)
            {
                if (OnErro != null)
                {
                    OnErro(
                        this,
                        new ProgressoEventArgs(_instalador.Id, e.GetType().ToString() + ":" + e.Message + "\r\n" + e.StackTrace)
                        );
                }
            }
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
