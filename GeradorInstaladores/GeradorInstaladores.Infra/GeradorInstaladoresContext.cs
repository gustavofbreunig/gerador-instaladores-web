using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace GeradorInstaladores.Infra
{
    public class GeradorInstaladoresContext : DbContext
    {
        public DbSet<Instalador> Instaladores { get; set; }
        public DbSet<Equipamento> Equipamentos { get; set; }
        public DbSet<ModeloEquipamento> ModelosEquipamentos { get; set; }
        public DbSet<DefinicoesGerais> DefinicoesGerais { get; set; }

        public GeradorInstaladoresContext()
        : base("default")
        {
            
        }

        //Seeding não é suportado pelo SQLITE, faz na inicialização do app mesmo
    }

    //use o http://sqlitebrowser.org/ para criar o banco de dados com a estrutura,
    //o EF6 não faz automaticamente

    /// <summary>
    /// Definição gerais do sistema.
    /// </summary>
    [Table("DefinicoesGerais")]
    public class DefinicoesGerais
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Pasta onde estão as pastas com os drivers. 
        /// Esta pasta será usada para criar os instaladores
        /// </summary>
        [Required]
        public string PastaDrivers { get; set; }

        /// <summary>
        /// Pasta onde está o compilador do INNO.
        /// </summary>
        [Required]
        public string PastaINNO { get; set; }

        /// <summary>
        /// Configuração AppName do INNO.
        /// http://www.jrsoftware.org/ishelp/index.php?topic=setup_appname
        /// </summary>
        [Required]
        public string AppName { get; set; }
    }

    [Table("Instaladores")]
    public class Instalador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// Nome do arquivo instalador (.exe), depois de compilado.
        /// </summary>
        public string ArquivoInstalador { get; set; }

        /// <summary>
        /// Status de acordo com o enum StatusCompilacao
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// Mensagens de progresso da criação do instalador.
        /// </summary>
        public string MensagensProgresso { get; set; }

        public virtual ICollection<Equipamento> Equipamentos { get; set; }
    }

    [Table("Equipamentos")]
    public class Equipamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        public int InstaladorId { get; set; }

        [ForeignKey("InstaladorId")]
        public Instalador Instalador { get; set; }

        public int ModeloEquipamentoId { get; set; }

        [ForeignKey("ModeloEquipamentoId")]
        public ModeloEquipamento ModeloEquipamento { get; set; }

        [Required]
        public string IP { get; set; }
    }

    [Table("Modelos")]
    public class ModeloEquipamento
    {
        [Key]
        public int Id { get; set; }
     
        /// <summary>
        /// Nome do modelo. Ex: RICOH MP 201 SPF.
        /// </summary>
        [Required]
        public string NomeModelo { get; set; }
        
        /// <summary>
        /// Pasta onde está o driver X86.
        /// </summary>
        [Required]
        public string PastaDriverX86 { get; set; }

        /// <summary>
        /// Pasta onde está o driver X64.
        /// </summary>
        [Required]
        public string PastaDriverX64 { get; set; }

        /// <summary>
        /// Nome do arquivo .inf de dentro da pasta onde está o driver.
        /// </summary>
        [Required]
        public string ArquivoINF { get; set; }

        /// <summary>
        /// Nome do driver, encontre dentro do arquivo .inf, ex: RICOH Aficio MP 201 PCL 5e
        /// </summary>
        [Required]
        public string NomeDriver { get; set; }
    }
}
