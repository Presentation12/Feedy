const searchInput = document.querySelector(".filtertext");
const rows = document.querySelectorAll(".lineTable");
const filter = document.querySelector(".filterbutton");

filter.addEventListener("click", () => {
    const text = searchInput.value.toLowerCase();
    counter = 0;

    rows.forEach(row => {
        if(row.querySelector("td").textContent.toLowerCase().startsWith(text))
        {
            row.style.display = "";
        }
        else{
            row.style.display = "none";
        }
    })
})