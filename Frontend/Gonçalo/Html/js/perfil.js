document.getElementById("edit1").addEventListener("click", () => {

    document.getElementById("nome").disabled = false;
    document.getElementById("apelido").disabled = false;
    document.getElementById("email").disabled = false;
    document.getElementById("tel").disabled = false;

    document.getElementById("buttons").style.display = "block";
    document.getElementById("edit1").style.display = "none";

})

cancelares1 = document.querySelectorAll(".cancelar1");
cancelares1.forEach(element => {
    element.addEventListener("click", () => {

        document.getElementById("nome").disabled = true;
        document.getElementById("apelido").disabled = true;
        document.getElementById("email").disabled = true;
        document.getElementById("tel").disabled = true;

        document.getElementById("buttons").style.display = "none";
        document.getElementById("edit1").style.display = "block";

    })
});

document.getElementById("edit2").addEventListener("click", () => {
    document.getElementById("password").style.display = "block";
    document.getElementById("edit2").style.display = "none";
})

cancelares2 = document.querySelectorAll(".cancelar2");
cancelares2.forEach(element => {
    element.addEventListener("click", () => {
        document.getElementById("password").style.display = "none";
        document.getElementById("edit2").style.display = "block";
    })
});

function mostraPass(pass) {

    var x = document.getElementById(pass);
  
    if (x.type === "password") {
      x.type = "text";
    }
  
    else {
      x.type = "password";
    }
  }