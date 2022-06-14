// Ao clicar no butao com id filtro este faz com que so aparecam as linhas que contenham o distrito, tipo e o que é escrito na barra de pesquisa
function filter() {
    const rows = document.querySelectorAll(".lineTable");
    const searchInput = document.querySelector(".barra");

    const tipo = document.getElementById("tipoOp").value;
    const distrito = document.getElementById("distritoOp").value;
    const text = searchInput.value.toLowerCase();

    rows.forEach(row => {

        //quando se escolhe os dois filtros ou ambos estao limpos = "---"
        if ((row.querySelector(".tipo").textContent == tipo
            && row.querySelector(".distrito").textContent == distrito)
            || (tipo == "---" && distrito == "---")) {
            //barra de pesquisa
            if (row.querySelector(".nome").textContent.toLowerCase().startsWith(text)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        }

        //quando nao se escolhe distrito, apenas tipo
        else if (row.querySelector(".tipo").textContent == tipo && distrito == "---") {
            //barra de pesquisa
            if (row.querySelector(".nome").textContent.toLowerCase().startsWith(text)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        }

        //quando nao se escolhe tipo, apenas distrito
        else if (row.querySelector(".distrito").textContent == distrito && tipo == "---") {
            //barra de pesquisa
            if (row.querySelector(".nome").textContent.toLowerCase().startsWith(text)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        }

        //quando nao existe o filtro
        else {
            row.style.display = "none";
        }
    })
}