function Filter(){
    const searchInput = document.querySelector(".filtertext");
    const rows = document.querySelectorAll(".lineTable");

    const text = searchInput.value.toLowerCase();

    rows.forEach(row => {
        if(row.querySelector("td").textContent.toLowerCase().startsWith(text))
        {
            row.style.display = "";
        }
        else{
            row.style.display = "none";
        }
    })
}