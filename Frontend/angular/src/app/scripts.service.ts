import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ScriptsService {

  changeTip(tip:string){
    const text1 = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
    const text2 = "Deve dar de comer ao seu cão duas vezes por dia.";
    const text3 = "Deve levar o seu cão à rua, pelo menos, uma vez por dia.";
    if (tip == ""){
      return text1;
    }
    if (tip == text1){
      return text2;
    }
    else if (tip == text2){
      return text3;
    }
    else if (tip == text3){
      return text1;
    }
    else{
      return "error";
    }
  }

  changeTipLi(lis:string[]){
    if (lis[0] == "selected"){
      lis[0] = "";
      lis[1] = "selected";
    }
    else if (lis[1] == "selected"){
      lis[1] = "";
      lis[2] = "selected";
    }
    else if (lis[2] == "selected"){
      lis[2] = "";
      lis[0] = "selected";
    }
  }
}
