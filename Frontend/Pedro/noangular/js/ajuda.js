document.querySelector(".submit").addEventListener("click", (e) => {
  e.preventDefault()
  if(document.querySelector(".titlehelp").value.length > 0 && document.querySelector(".texthelp").value.length > 0){
    document.querySelector(".modalhelp").style = "display: flex;";

    document.querySelector(".top_navbar").style="opacity:0.2;transition: opacity 0.8s ease"
    document.querySelector(".helpsection").style="opacity:0.2;transition: opacity 0.8s ease"
    document.querySelector(".sidebar").style="opacity:0.2;transition: opacity 0.8s ease"
  }
  else{
    document.querySelector(".modalhelp2").style = "display: flex;";

    document.querySelector(".top_navbar").style="opacity:0.2;transition: opacity 0.8s ease"
    document.querySelector(".helpsection").style="opacity:0.2;transition: opacity 0.8s ease"
    document.querySelector(".sidebar").style="opacity:0.2;transition: opacity 0.8s ease"
  }
})

document.querySelector(".continuarBtn").addEventListener("click", () => {
  document.querySelector(".modalhelp").style = "display: none;";

  document.querySelector(".top_navbar").style="opacity:1;transition: opacity 0.8s ease"
  document.querySelector(".helpsection").style="opacity:1;transition: opacity 0.8s ease"
  document.querySelector(".sidebar").style="opacity:1;transition: opacity 0.8s ease"

  if(document.querySelector(".titlehelp").value.length > 0 && document.querySelector(".texthelp").value.length > 0){
    document.getElementById("help").submit();
  }
})

document.querySelector(".continuarBtn2").addEventListener("click", () => {
  document.querySelector(".modalhelp2").style = "display: none;";

  document.querySelector(".top_navbar").style="opacity:1;transition: opacity 0.8s ease"
  document.querySelector(".helpsection").style="opacity:1;transition: opacity 0.8s ease"
  document.querySelector(".sidebar").style="opacity:1;transition: opacity 0.8s ease"

  if(document.querySelector(".titlehelp").value.length > 0 && document.querySelector(".texthelp").value.length > 0){
    document.getElementById("help").submit();
  }
})