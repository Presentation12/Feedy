list = document.querySelectorAll(".lineTable");

list.forEach(member => {
    member.addEventListener("click", () => {
        swal({
            title: "Notificação",
            text: member.textContent,
            button: "Continuar"
        });
    })
});
