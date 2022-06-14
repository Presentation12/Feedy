let notifications = document.querySelectorAll(".notrow")

for(let i = 0; i < notifications.length; i++){
    notifications[i].addEventListener("click", () => {
        swal({
            title: "Notificação",
            text: notifications[i].textContent,
            button: "Continuar"
        });
    })  
}