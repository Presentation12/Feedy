const text1 = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
const text2 = "Deve dar de comer ao seu cão duas vezes por dia.";
const text3 = "Deve levar o seu cão à rua, pelo menos, uma vez por dia.";


// CLICK?

function changeTip(){
    

    if (document.querySelector("#tip-1").classList.contains("selected")){
        document.querySelector("#tip-1").classList.remove("selected");
        document.querySelector("#tip-2").classList.add("selected");
        document.querySelector("#tip-text").textContent = text2;
    }
    else if (document.querySelector("#tip-2").classList.contains("selected")){
        document.querySelector("#tip-2").classList.remove("selected");
        document.querySelector("#tip-3").classList.add("selected");
        document.querySelector("#tip-text").textContent = text3;
    }
    else if (document.querySelector("#tip-3").classList.contains("selected")){
        document.querySelector("#tip-3").classList.remove("selected");
        document.querySelector("#tip-1").classList.add("selected");
        document.querySelector("#tip-text").textContent = text1;
    }
}


// changeTip();

setInterval(function(){ 
     changeTip();
}, 15000);


