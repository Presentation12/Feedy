var button = document.createElement("input");
var selectedAnimals = document.querySelectorAll(".listanimais");
var cancelLogoBtn = document.querySelector(".cancelLogoChange");
var saveLogoBtn = document.querySelector(".saveLogoChange");
var deleteAnimal = document.querySelector(".eliminarbtn");
var backup = document.querySelector(".activeanimallogo").src;

//#region Editar os dados do Animal + Botao de guardar alterações
function EditarAnimal(){
    nomestat = document.getElementById("nome");
    nomestat.disabled = false;

    document.querySelector(".editable").lastElementChild.style = "background-color: #ffffff;border: 1px solid purple"

    button.setAttribute("type", "submit");
    button.value = "Guardar Alterações";
    button.style = "font-family: Poppins-Regular, sans-serif;font-size: 14px;font-weight: 600;padding: 5px;cursor: pointer;border-radius: 15px 15px 15px 15px;background-color:#5C2976;color:white;border:2px solid #5C2976";

    document.querySelector(".infobtn").appendChild(button);
}

button.addEventListener("mouseover", () => {
    button.style = "font-family: Poppins-Regular, sans-serif;font-size: 14px;font-weight: 600;padding: 5px;cursor: pointer;border-radius: 15px 15px 15px 15px;background-color:white;color:#5C2976;border:2px solid #5C2976";
})

button.addEventListener("mouseout", () => {
    button.style = "font-family: Poppins-Regular, sans-serif;font-size: 14px;font-weight: 600;padding: 5px;cursor: pointer;border-radius: 15px 15px 15px 15px;background-color:#5C2976;color:white;border:2px solid #5C2976";
})

button.addEventListener("click", () => {
    nomestat = document.getElementById("nome");
    if(nomestat.value.length > 0){
        nomestat.placeholder = nomestat.value;
        document.querySelector(".nameselected").textContent = nomestat.value
        document.querySelector(".activeanimal").lastElementChild.textContent = nomestat.value;
    }
    nomestat.disabled = true;

    document.querySelector(".editable").lastElementChild.style = "background-color: #F1F1F1;border: 0"

    button.remove();
})
//#endregion

//#region Animal Selecionado
for(let i = 0; i < selectedAnimals.length; i++){
    selectedAnimals[i].addEventListener("click", function(){
        document.querySelector(".activeanimal").classList.remove("activeanimal");
        document.querySelector(".activeanimallogo").classList.remove(".activeanimallogo");

        selectedAnimals[i].classList.add("activeanimal");
        selectedAnimals[i].children[0].classList.add("activeanimallogo");
        backup = selectedAnimals[i].children[0].src;

        document.querySelector(".selected").src = selectedAnimals[i].children[0].src;
        document.querySelector(".nameselected").innerHTML = selectedAnimals[i].children[1].innerHTML;

        document.querySelector(".logoanimalinfo").src = selectedAnimals[i].children[0].src;
        document.getElementById("nome").placeholder = selectedAnimals[i].children[1].textContent;
    })
}
//#endregion

//#region Alterar Foto animal
function mouseoverIconEdit(){
  var icon = document.querySelector(".iconedit");
  icon.style = "opacity: 1;"
  document.querySelector(".logoanimalinfo").style = "opacity: 0.5;"
}

function mouseoutIconEdit(){
  var icon = document.querySelector(".iconedit");
  icon.style = "opacity: 0;"
  document.querySelector(".logoanimalinfo").style = "opacity: 1;"
}

function mouseoverClick(){
  document.querySelector(".logoanimalmodal").style = "display: flex;";

  document.querySelector(".top_navbar").style="opacity:0.2;transition: opacity 0.8s ease"
  document.querySelector(".animalselected").style="opacity:0.2;transition: opacity 0.8s ease"
  document.querySelector(".box").style="opacity:0.2;transition: opacity 0.8s ease"

  document.querySelector(".sidebar").style="opacity:0.2;transition: opacity 0.8s ease"
}

function imageInput(){
  const fileReader = new FileReader();

  const uploaded_image = fileReader.result;
  document.querySelector(".displaylogo").style.backgroundImage = `url(${uploaded_image})`;

  document.querySelector(".logoanimalinfo").src = uploaded_image;
  document.querySelector(".selected").src = uploaded_image;
  document.querySelector(".activeanimal").children[0].src = uploaded_image;

  fileReader.readAsDataURL(this.files[0]);
}

cancelLogoBtn.addEventListener("click", () =>{
    cancelLogoAnimal(backup);
})

function cancelLogoAnimal(aux){
    document.querySelector(".logoanimalmodal").style = "display: none;";

    document.querySelector(".top_navbar").style="opacity:1;transition: opacity 0.8s ease"
    document.querySelector(".animalselected").style="opacity:1;transition: opacity 0.8s ease"
    document.querySelector(".box").style="opacity:1;transition: opacity 0.8s ease"

    document.querySelector(".sidebar").style="opacity:1;transition: opacity 0.8s ease"

    document.querySelector(".logoanimalinfo").src = aux;
    document.querySelector(".selected").src = aux;
    document.querySelector(".activeanimal").children[0].src = aux;
}

saveLogoBtn.addEventListener("click", () => {
    document.querySelector(".logoanimalmodal").style = "display: none;";

    document.querySelector(".top_navbar").style="opacity:1;transition: opacity 0.8s ease"
    document.querySelector(".animalselected").style="opacity:1;transition: opacity 0.8s ease"
    document.querySelector(".box").style="opacity:1;transition: opacity 0.8s ease"

    document.querySelector(".sidebar").style="opacity:1;transition: opacity 0.8s ease"
})

//#endregion

//#region Apagar Animal

deleteAnimal.addEventListener("click", () => {
    var animalEscolhido = document.querySelector(".activeanimal");
    animalEscolhido.style = "display: none";
    animalEscolhido.classList.remove("activeanimal");
    animalEscolhido.classList.remove("listanimais");

    var novoAnimalEscolhido = document.querySelector(".listanimais")
    novoAnimalEscolhido.classList.add("activeanimal");

    document.querySelector(".selected").src = novoAnimalEscolhido.children[0].src;
    document.querySelector(".nameselected").innerHTML = novoAnimalEscolhido.children[1].innerHTML;

    document.querySelector(".logoanimalinfo").src = novoAnimalEscolhido.children[0].src;
    document.getElementById("nome").placeholder = novoAnimalEscolhido.children[1].textContent;
});

//#endregion
