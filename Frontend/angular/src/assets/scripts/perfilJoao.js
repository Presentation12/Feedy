// visualizar password
function showPass(pass) {

  var x = document.getElementById(pass);

  if (x.type === "password") {
    x.type = "text";
  }

  else {
    x.type = "password";
  }
}

// torna editavel determinados campos do perfil veterinario para edição alem de ficar visivel os botoes de guardar e cancelar e invisivel o de alterar
function editable() {
  document.getElementById("nome").disabled = false;
  document.getElementById("apelido").disabled = false;
  document.getElementById("email").disabled = false;
  document.getElementById("contacto").disabled = false;

  document.getElementById("alterar").style.display = "none";
  document.getElementById("editar").style.display = "block";
}

// torna não editavel determinados campos do perfil veterinario alem de ficar invisivel os botoes de guardar e cancelar e visivel o de alterar
function nonEditable() {
  document.getElementById("nome").disabled = true;
  document.getElementById("apelido").disabled = true;
  document.getElementById("email").disabled = true;
  document.getElementById("contacto").disabled = true;

  document.getElementById("alterar").style.display = "block";
  document.getElementById("editar").style.display = "none";
  document.querySelector(".submit").submit();
}

// torna visivel as caixas para inserção de password e butao de cancelar e confirmar
// torna invisivel butao de alterar
function insertsPass(buton, passes) {
  document.getElementById(buton).style.display = "none";
  document.getElementById(passes).style.display = "block";
}

// torna invisivel as caixas para inserção de password e butao de cancelar e confirmar
// torna visivel butao de alterar
function cancel(buton, passes) {
  document.getElementById(passes).style.display = "none";
  document.getElementById(buton).style.display = "block";
  document.querySelector(".submit").submit();
}

// confirmaçao de suspenção de conta
function confirmDialog() {

  pass1 = document.getElementById("pass").value;
  pass2 = document.getElementById("pass2").value;

  if ((pass1 == pass2) && (pass1 != "") && (pass2 != "")) {
      document.getElementById("show5").style.display = "block";
  }
  else if (pass1 != pass2) {
      document.getElementById("show4").style.display = "block";
  }
  else {
      document.getElementById("show3").style.display = "block";
  }
}