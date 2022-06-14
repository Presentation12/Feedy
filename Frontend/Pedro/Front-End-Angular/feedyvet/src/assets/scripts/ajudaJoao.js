// Busca o butao submeter e caso seja clicado e os campos de texto estejam preenchidos este mostra uma mensagem
function submit() 
{
    if (document.getElementById("assunto").value != "" && document.getElementById("descricao").value != "") 
    {
        document.getElementById("show").style.display = "block";
    }
    else
    {
        document.getElementById("show2").style.display = "block";
    }
}

// Busca o butao ok e caso seja clicado torna invisivel a mensagem e da refresh na pagina
function ok() 
{
    if(document.getElementById("show").style.display == "block")
    {
        document.getElementById("show").style.display = "none";
        document.querySelector(".help").submit();
    }
    else
    {
        document.getElementById("show2").style.display = "none";
    }

}
