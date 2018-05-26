using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeradorInstaladores.Infra;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeradorInstaladores.Testes
{
    [TestClass]
    public class TesteCriadorInstalador
    {
        private ModeloEquipamento _modelo_mpc2550;
        private ModeloEquipamento _modelo_mp201;
        private Instalador _instalador;

        [TestInitialize]
        public void initializeClasses()
        {
            _modelo_mpc2550 = new ModeloEquipamento()
            {
                Id = 1,
                NomeModelo = "RICOH Aficio MP C2550",
                PastaDriverX86 = "mpc2550_x86",
                PastaDriverX64 = "mpc2550_x64",
                ArquivoINF = "OEMSETUP.INF",
                NomeDriver = "RICOH Aficio MP C2550 PCL 5c"
            };

            _modelo_mp201 = new ModeloEquipamento()
            {
                Id = 2,
                NomeModelo = "RICOH Aficio MP 201",
                PastaDriverX86 = "mp201_x86",
                PastaDriverX64 = "mp201_x64",
                ArquivoINF = "OEMSETUP.INF",
                NomeDriver = "RICOH Aficio MP 201 PCL 5e"
            };

            _instalador = new Instalador()
            {
                Id = 1,
                Nome = "ACME do Brasil - acentuação áéíóú",
                Status = (int)StatusCompilacao.NaoIniciado,
                Equipamentos = new List<Equipamento>()
            };

            _instalador.Equipamentos.Add(new Equipamento()
            {
                Id = 1,
                IP = "192.168.10.232",
                Nome = "teste",
                ModeloEquipamento = _modelo_mpc2550
            });

            _instalador.Equipamentos.Add(new Equipamento()
            {
                Id = 2,
                IP = "192.168.10.2",
                Nome = "Impressora 02",
                ModeloEquipamento = _modelo_mp201
            });

            _instalador.Equipamentos.Add(new Equipamento()
            {
                Id = 3,
                IP = "192.168.10.3",
                Nome = "Impressora 03",
                ModeloEquipamento = _modelo_mp201
            });
        }

        [TestMethod]
        public void teste_criacao_instalador_inno()
        {
            CriadorInstalador criador =
                new CriadorInstalador(_instalador,
                @"D:\temp\instaladorImpressoras",
                @"C:\Program Files (x86)\Inno Setup 5",
                @"Teste"
                );

            criador.OnErro += Criador_OnErro;
            criador.OnMensagemProgresso += Criador_OnMensagemProgresso;

            criador.CriaInstaladorINNO();

        }

        private void Criador_OnMensagemProgresso(object sender, ProgressoEventArgs e)
        {
            Debug.WriteLine(e.Mensagem);
        }

        private void Criador_OnErro(object sender, ProgressoEventArgs e)
        {
            Debug.WriteLine("ERRO:");
            Debug.WriteLine(e.Mensagem);
        }
    }
}
