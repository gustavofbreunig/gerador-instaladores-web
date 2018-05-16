var equipamentos = [];

$(document).ready(function () {
    $.ajax({
        method: 'GET',
        url: '/Home/ObterModelos',
        success: function (result) {
            $('#dropdownModelo').empty();
            for (var i = 0; i < result.length; i++) {
                $('#dropdownModelo').append($('<option>').attr('value', result[i].ID).text(result[i].Modelo));
            }
        },
        error: function (xhr) {
            alert("Erro ao carregar modelos: " + xhr.status + " " + xhr.statusText);
        }
    });

    $('#frmCriaInstalador').on('submit', function (e) {
        e.preventDefault();

        //cria o objeto com todas infos e faz o post
        var infosInstalador = {
            Nome: $('#txtNomeInstalador').val(),
            Equipamentos: equipamentos
        };

        //faz o post e redireciona para a página que espera o instalador ficar pronto
        $.ajax({
            method: 'POST',
            data: 'json',
            data: infosInstalador,
            url: '/Home/RequisitaInstalador',
            success: function (result) {
                window.location.href = '/Download/' + result;
            },
            error: function (xhr) {
                alert("Erro ao carregar modelos: " + xhr.status + " " + xhr.statusText);
            }
        });
    });

    $('#frmAdd').on('submit', function (e) {
        e.preventDefault();

        var eqp =
            {
                Nome: $('#txtFila').val(),
                IP: $('#txtIP').val(),
                idModelo: $('#dropdownModelo').val(),
                Modelo: $('#dropdownModelo option:selected').text()
            };

        equipamentos.splice(0, 0, eqp);
        limpaTable();
        passaEquipamentosParaTable();
    });
});

function limpaTable() {
    $("#tblEquipamentos > tbody").empty();
}

function removeEquipamento(i) {
    equipamentos.splice(i, 1);
    limpaTable();
    passaEquipamentosParaTable();
}

function passaEquipamentosParaTable() {
    for (var i = 0; i < equipamentos.length; i++) {
        $("#tblEquipamentos").find('tbody')
            .append($('<tr>')
                .append($('<td>')
                    .append(equipamentos[i].Nome)
                )
                .append($('<td>')
                    .append(equipamentos[i].IP)
                )
                .append($('<td>')
                    .append(equipamentos[i].Modelo)
                )
                .append($('<td>')
                    .append($('<button>').attr('type', 'button').attr('onclick', 'removeEquipamento(' + i + ');').attr('class', 'btn btn-danger btn-sm').text('Remover'))
                )
            );
    }
}