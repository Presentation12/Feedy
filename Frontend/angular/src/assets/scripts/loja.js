function filter() {
    const rows = document.querySelectorAll(".lineTable");
    const searchInput = document.querySelector(".barra");

    const tipo = document.getElementById("tipo").value;
    const estabelecimento = document.getElementById("estab").value;
    const text = searchInput.value.toLowerCase();

    rows.forEach(row => {

        if ((row.querySelector(".tip").textContent == tipo && row.querySelector(".est").textContent == estabelecimento)
            || (tipo == "---" && estabelecimento == "---")) {

            if (row.querySelector(".nome").textContent.toLowerCase().startsWith(text)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        }

        //quando se escolhe apenas tipo
        else if (row.querySelector(".tip").textContent == tipo && estabelecimento == "---") {
            
            if (row.querySelector(".nome").textContent.toLowerCase().startsWith(text)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        }

        //quando se escolhe apenas estabelecimento
        else if (row.querySelector(".est").textContent == estabelecimento && tipo == "---") {
            
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

function submit()
{
    document.querySelector(".submit").submit();
}