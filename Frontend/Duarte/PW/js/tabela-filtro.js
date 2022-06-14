const input = document.querySelector("input");
const rows = document.querySelectorAll(".table-row");
const button = document.querySelector(".btn-call-to-action");

function pesquisa(){
    const search = input.value.toLowerCase();
    rows.forEach(row => {
        if(row.querySelector(".name").textContent.toLowerCase().startsWith(search))
        {
            row.style.display = "";
        }
        else{
            row.style.display = "none";
        }
    })
}


button.addEventListener("click", () => {
    pesquisa();
})



input.addEventListener("keypress", function(event) {
  if (event.key === "Enter") {
    event.preventDefault();
    pesquisa();
  }
});


