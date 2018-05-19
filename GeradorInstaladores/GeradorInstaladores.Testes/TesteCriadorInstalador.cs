﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeradorInstaladores.Infra;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeradorInstaladores.Testes
{
    [TestClass]
    public class TesteCriadorInstalador
    {
        private ModeloEquipamento _modelo;
        private Instalador _instalador;

        [TestInitialize]
        public void initializeClasses()
        {
            _modelo = new ModeloEquipamento()
            {
                NomeModelo = "RICOH Aficio MP C2550",
                PastaDriverX86 = "mpc2050_x86",
                PastaDriverX64 = "mpc2050_x64",
                ArquivoINF = "OEMSETUP.INF",
                NomeDriver = "RICOH Aficio MP C2550 PCL 5c"
            };

            _instalador = new Instalador()
            {
                Id = 1,
                Nome = "nóme dó cliénte",
                Status = (int)StatusCompilacao.NaoIniciado,
                Equipamentos = new List<Equipamento>()
            };

            _instalador.Equipamentos.Add(new Equipamento()
            {
                Id = 1,
                IP = "192.168.10.1",
                Nome = "Impressora 01",
                ModeloEquipamento = _modelo
            });

            _instalador.Equipamentos.Add(new Equipamento()
            {
                Id = 2,
                IP = "192.168.10.2",
                Nome = "Impressora 02",
                ModeloEquipamento = _modelo
            });
        }

        [TestMethod]
        public void teste_criacao_instalador_inno()
        {
            CriadorInstalador criador =
                new CriadorInstalador(_instalador,
                @"D:\temp\instaladorImpressoras",
                @"C:\Program Files (x86)\Inno Setup 5"
                );

            criador.OnErro += Criador_OnErro;

            criador.CriaInstaladorINNO();

        }

        private void Criador_OnErro(object sender, ProgressoEventArgs e)
        {
            //grava erro no log do BD

            throw new Exception(e.Mensagem);
        }
    }
}