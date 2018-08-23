using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeradorInstaladores.Infra;
using Quartz;
using Quartz.Impl;
using System.IO;

namespace GeradorInstaladores.Controllers
{
    public class ModelModeloComId
    {
        public int ID { get; set; }
        public string Modelo { get; set; }
    }

    public class ModelRequisicaoInstalador_Equipamento
    {
        public string Nome { get; set; }
        public string IP { get; set; }
        public string Modelo { get; set; }
        public int idModelo { get; set; }
    }

    public class ModelRequisicaoInstalador
    {
        public string Nome { get; set; }
        public ModelRequisicaoInstalador_Equipamento[] Equipamentos { get; set; }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Download(int IdInstalador)
        {
            ViewBag.IdInstalador = IdInstalador;
            return View();
        }

        public ActionResult DownloadBin(string id)
        {
            ObterExeInstalador obterExe = new ObterExeInstalador(Int32.Parse(id));
            FileInfo f = obterExe.RetornaArquivo();

            //response sem colocar todo arquivo em memória
            Response.Clear();
            Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            Response.AppendHeader("Content-Disposition", "filename=" + f.Name);
            Response.TransmitFile(f.FullName);
            Response.End();

            return RedirectToAction("Download", new { IdInstalador = Int32.Parse(id) });
        }

        [HttpGet]
        public JsonResult ObterStatusInstalador(int IdInstalador)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                Instalador i = db.Instaladores.Find(IdInstalador);

                string status = Enum.GetName(typeof(StatusCompilacao), i.Status);

                return Json(new { Status = status, Mensagens = i.MensagensProgresso }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObterModelos()
        {
            var modelosview = new List<ModelModeloComId>();

            using (var db = new GeradorInstaladoresContext())
            {
                var modelos = from m in db.ModelosEquipamentos
                              orderby m.NomeModelo
                              select new
                              {
                                  ID = m.Id,
                                  Modelo = m.NomeModelo
                              };

                foreach (var modelo in modelos)
                {
                    modelosview.Add(
                        new ModelModeloComId()
                        {
                            ID = modelo.ID,
                            Modelo = modelo.Modelo
                        });
                }
            }

            return Json(modelosview, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequisitaInstalador(ModelRequisicaoInstalador dadosInstalador)
        {
            int IdInstaladorGerado = 0;

            //registra o instalador no banco de dados
            using (var db = new GeradorInstaladoresContext())
            {
                var instalador = new Instalador()
                {
                    Nome = dadosInstalador.Nome,
                    Status = (int)StatusCompilacao.NaoIniciado
                };

                instalador.Equipamentos = new List<Equipamento>();

                foreach (var equipamento in dadosInstalador.Equipamentos)
                {
                    //pega o modelo
                    var modelo = db.ModelosEquipamentos.Find(equipamento.idModelo);

                    instalador.Equipamentos.Add(new Equipamento()
                    {
                        IP = equipamento.IP,
                        Nome = equipamento.Nome,
                        ModeloEquipamento = modelo
                    });
                }

                var novoInstaladorGerado = db.Instaladores.Add(instalador);

                db.SaveChanges();

                //aqui a entidade já tem o identificador
                IdInstaladorGerado = novoInstaladorGerado.Id;
            }
           
            //responde com o id unico gerado
            return Json(IdInstaladorGerado);
        }

    }
}