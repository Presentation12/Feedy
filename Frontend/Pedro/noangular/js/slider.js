const text1 = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
const text2 = "Deve dar de comer ao seu cão duas vezes por dia.";
const text3 = "Deve levar o seu cão à rua, pelo menos, uma vez por dia.";
const listTips = document.querySelectorAll(".tip")

function changeTip(){
    if (document.querySelector("#tip-1").classList.contains("selectedtip")){
        document.querySelector("#tip-1").classList.remove("selectedtip");
        document.querySelector("#tip-2").classList.add("selectedtip");
        document.querySelector("#tip-text").textContent = text2;
    }
    else if (document.querySelector("#tip-2").classList.contains("selectedtip")){
        document.querySelector("#tip-2").classList.remove("selectedtip");
        document.querySelector("#tip-3").classList.add("selectedtip");
        document.querySelector("#tip-text").textContent = text3;
    }
    else if (document.querySelector("#tip-3").classList.contains("selectedtip")){
        document.querySelector("#tip-3").classList.remove("selectedtip");
        document.querySelector("#tip-1").classList.add("selectedtip");
        document.querySelector("#tip-text").textContent = text1;
    }
}

listTips.forEach(tip => {
    tip.addEventListener("click", () => {
        listTips.forEach(element => {
            if(element.classList.contains("selectedtip")){
                element.classList.remove("selectedtip");
            }
        });

        tip.classList.add("selectedtip");
        if (document.querySelector("#tip-1").classList.contains("selectedtip")){
            document.querySelector("#tip-text").textContent = text1;
        }
        else if (document.querySelector("#tip-2").classList.contains("selectedtip")){
            document.querySelector("#tip-text").textContent = text2;
        }
        else if (document.querySelector("#tip-3").classList.contains("selectedtip")){
            document.querySelector("#tip-text").textContent = text3;
        }
    })
});

setInterval(function(){ 
     changeTip();
}, 5000);