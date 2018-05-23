using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.Infra
{
    public class SeedsNoBD
    {
        public static void FazSeedSQLite()
        {
            //faz o seed dos modelos
            using (var db = new GeradorInstaladoresContext())
            {
                int modelosCadastrados = db.ModelosEquipamentos.Count();

                if (modelosCadastrados == 0)
                {

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "RICOH Aficio MP C2550",
                        PastaDriverX86 = "mpc2050_x86",
                        PastaDriverX64 = "mpc2050_x64",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH Aficio MP C2550 PCL 5c"
                    });

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "Ricoh MP 305",
                        PastaDriverX64 = "mp305_x64",
                        PastaDriverX86 = "mp305_x86",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH MP 305+ PCL 5e"
                    });

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "RICOH Aficio MP 201",
                        PastaDriverX64 = "mp201_x64",
                        PastaDriverX86 = "mp201_x86",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH Aficio MP 201 PCL 5e"
                    });
                }

                int definicoesGravadas = db.DefinicoesGerais.Count();

                if (definicoesGravadas == 0)
                {
                    db.DefinicoesGerais.Add(new DefinicoesGerais()
                    {
                        PastaDrivers = @"D:\temp\instaladorImpressoras",
                        PastaINNO = @"C:\Program Files (x86)\Inno Setup 5",
                        AppName = @"AppName",
                        AppPublisher = @"AppPublisher",
                        AppPublisherURL = @"http://google.com",
                        AppSupportURL = @"http://google.com",
                        AppUpdatesURL = @"http://google.com"
                    });
                }

                db.SaveChanges();
            }
        }
    }
}
