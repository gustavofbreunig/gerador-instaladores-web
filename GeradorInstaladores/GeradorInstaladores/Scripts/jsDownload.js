$("#lnkDownload").hide();
$("#msgErro").hide();


function AtualizaLinkDownloadEMensagens()
{
    var idInstalador = $("#IdInstalador").val();

    $.ajax({
        method: 'GET',
        data: { IdInstalador: idInstalador },
        url: '/Home/ObterStatusInstalador',
        success: function (result) {
            if (result.Status == "NaoIniciado" || result.Status == "Compilando")
            {
                $("#imgDownload").show();
                $("#lnkDownload").hide();
                $("#msgErro").hide();
            }
            else if (result.Status == "Terminado")
            {
                $("#imgDownload").hide();
                $("#lnkDownload").show();
                $("#msgErro").hide();
            }
            else if (result.Status == "Erro")
            {
                $("#imgDownload").hide();
                $("#lnkDownload").hide();
                $("#msgErro").show();
            }

            //converte newline para elementos <br />
            var msgs = result.Mensagens.replace(/(?:\r\n|\r|\n)/g, '<br />');

            $("#txtMensagensProgresso").html(msgs);
        },
        error: function (xhr) {
            alert("Erro ao carregar: " + xhr.status + " " + xhr.statusText);
        }
    });
}

//atualiza pagina, escondendo links e mostrando o texto do progresso
meuTimeout = setInterval(AtualizaLinkDownloadEMensagens, 5000);