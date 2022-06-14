function showPass(pass) {

    var x = document.getElementById(pass);

    if (x.type === "password") {
      x.type = "text";
    }

    else {
      x.type = "password";
    }
  }

  function editable() {
    document.getElementById("nome").disabled = false;
    document.getElementById("apelido").disabled = false;
    document.getElementById("email").disabled = false;
    document.getElementById("tel").disabled = false;


    document.getElementById("edit1").style.display = "none";
    document.getElementById("foto").style.display = "block";
    document.getElementById("buttons").style.display = "block";
  }


  function nonEditable() {
    document.getElementById("nome").disabled = true;
    document.getElementById("apelido").disabled = true;
    document.getElementById("email").disabled = true;
    document.getElementById("tel").disabled = true;

    document.getElementById("edit1").style.display = "block";
    document.getElementById("foto").style.display = "none";
    document.getElementById("buttons").style.display = "none";
   // document.querySelector(".submit").submit();
  }


  function insertsPass() {
    document.getElementById("edit2").style.display = "none";
    document.getElementById("password").style.display = "block";
  }

  function cancel() {
    document.getElementById("password").style.display = "none";
    document.getElementById("edit2").style.display = "block";
    //document.querySelector(".submit").submit();
  }


  function changePlan() {
    document.getElementById("planoOp").disabled = false;
    document.getElementById("confirmar1").style.display = "block";
    document.getElementById("alterar1").style.display = "none";
}

// ao clicar para guardar plano tornam se não editaveis as select boxes
function disablePlan() {
    document.getElementById("planoOp").disabled = true;
    document.getElementById("confirmar1").style.display = "none";
    document.getElementById("alterar1").style.display = "block";
}

// alteraçoes na div e campo do metodo e campo preco, dependendo dos campos selecionados no metodo e plano
function verifyPlan() {
    tipoPlano = document.getElementById("planoOp").value;
    preco = document.getElementById("preco");

    if (tipoPlano == "---") {
        preco.innerHTML = "---";
    }
    if (tipoPlano == "Free") {
        preco.innerHTML = "Gratuito";
    }
    if (tipoPlano == "Mensal") {
        preco.innerHTML = "1.99 €/Mês";
    }
    if (tipoPlano == "Anual") {
        preco.innerHTML = "17.99 €/Ano";
    }
}
