using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeradorInstaladores.Infra;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace GeradorInstaladores.Testes
{
    [TestClass]
    public class TesteSQLite
    {
        [TestMethod]
        public void testa_seed_sqlite()
        {
            SeedsNoBD.FazSeedSQLite();

            using (var db = new GeradorInstaladoresContext())
            {
                int definicoes = (from c in db.DefinicoesGerais
                                  select c).Count();

                Assert.IsTrue(definicoes > 0);
            }
        }
    }
}
