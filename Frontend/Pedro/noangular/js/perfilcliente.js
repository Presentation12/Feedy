let newAnimalImg = document.createElement("img");
let newAnimalName = document.createElement("pa");
let newAnimalDelete = document.createElement("button");
let plusAnimal = document.querySelector(".listanimaisplus");
let apagarAnimalList = document.querySelectorAll(".apagaranimal");
let lembreteApagar = document.querySelectorAll(".lembretebtnlist");

plusAnimal.addEventListener("click", () => {
    let newAnimal = document.createElement("div");

    newAnimal.classList.add("listanimais");

    newAnimalImg.src = `../src/guaxinim.jpg`
    newAnimalImg.classList.add("logolistanimal");
    
    newAnimalName.textContent = "Nome Animal";

    newAnimalDelete.innerHTML = "X";
    newAnimalDelete.classList.add("apagaranimal");

    newAnimal.appendChild(newAnimalImg);
    newAnimal.appendChild(newAnimalName);
    newAnimal.appendChild(newAnimalDelete);
    
    document.getElementById("animais").appendChild(newAnimal);
})

for(let i = 0; i < apagarAnimalList.length; i++){
    apagarAnimalList[i].addEventListener("click", () => {
        document.getElementById("animais").removeChild(document.getElementById("animais").lastElementChild);
    })
}

lembreteApagar.forEach(btn => {
    btn.addEventListener("click", () => {
        btn.parentElement.style = "display: none";
    })
});