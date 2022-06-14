let sidebar = document.querySelector(".sidebar");
let closeBtn = document.querySelector("#btn");
let searchBtn = document.querySelector(".bx-search");
let iconBtn = document.querySelector(".li.a");

  closeBtn.addEventListener("click", ()=>{ // Fechar a sidebar quande se carregar no icon
    sidebar.classList.toggle("open");
    menuBtnChange();
  });

  searchBtn.addEventListener("click", ()=>{ // Abrir a sidebar quande se carregar em pesquisar
    sidebar.classList.toggle("open");
    menuBtnChange(); 
  });


  // mudar o butao da sidebar
  function menuBtnChange() {
   if(sidebar.classList.contains("open")){
     closeBtn.classList.replace("bx-menu", "bx-menu-alt-right");
   }else {
     closeBtn.classList.replace("bx-menu-alt-right","bx-menu");
   }
  };


  const selected = document.querySelector(".selected");
  const optionsContainer = document.querySelector(".options-container")
  const optionsList = document.querySelectorAll(".option");
  
  selected.addEventListener("click", () => {
    optionsContainer.classList.toggle("active");
  });
  
  optionsList.forEach(o => {
    o.addEventListener("click", () => {
      selected.innerHTML = o.querySelector("label").innerHTML;
      optionsContainer.classList.remove("active");
    });
  });


  // Funcao com bugs Deveria adicionar um li e span na  ul

  const inputText = document.querySelector('#txt');
  const myButton = document.querySelector('btn-list');
  const list = document.querySelector('.animalContainer ul');
  myButton.addEventListener('click', (e)=>{
    if(inputText.value != ""){
      e.preventDefault();

      //criar li

      const myLi = document.createElement('li');
      myLi.innerHTML = inputText.value;
      list.append(myLi);

      //criar span

      const mySpan = document.createElement('span');
      mySpan.innerHTML = 'x';
      myLi.appendChild(mySpan);
    }
    inputText.value="";
  });