import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})


export class SharedService {
  readonly APIUrl="https://localhost:5001/api"
  readonly APIUrlPhotos="https://localhost:5001/images/"
  headersAuth = new HttpHeaders().append("Authorization", 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJwc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDbGllbnRlIiwiZXhwIjoxNjUzMzk5Mjg1fQ.TnFMGBphIjlwQhd2EXlqCPLQLYHvpLY0Gi3SjDCcMhOhpZpxKNSObVJ3gV_DNxFEq-pJvxjyLKFKWTBkvK2u5g');

  constructor(private http:HttpClient) { }

  loginCliente(val:any)
  {
    return this.http.post(this.APIUrl+'/cliente/login', val);
  }
  
  marcarServico(val:any)
  {
    return this.http.get(this.APIUrl+'/cliente/marcar', val);
  }

  //#region HARDCODEDAnimal

   //todos animais ainda
   getAnimalList():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/animal', {headers: this.headersAuth});
  }

  //hardcode
  getAnimalCliente():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/animal/getanimalsbyclient/3', {headers: this.headersAuth});
  }

  getServicoEstabelecimento():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/estabelecimento/1', {headers: this.headersAuth})
  }

  addAnimal(val:any){
    return this.http.post(this.APIUrl+'/animal/3/addanimal', val, {headers: this.headersAuth})
  }

  patchAnimal(val:any){
    return this.http.patch(this.APIUrl+'/animal/3/editanimal', val, {headers: this.headersAuth})
  }

  removeAnimal(val:any){
    return this.http.patch(this.APIUrl+'/animal/'+val+'/del', null, {headers: this.headersAuth})
  }

  UploadPhoto(val:any){
    return this.http.post(this.APIUrl+'/animal/SaveFile', val, {headers: this.headersAuth})
  }

  //#endregion

  //#region HARDCODEDEstabelecimento

  getAvaliacoesEstabelecimento():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/avaliacaoestabelecimentoutilizador/1/checkavaliacoes', {headers: this.headersAuth});
  }

  //#endregion

  //#region HARDCODEDCliente

  getClientebyID():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/1', {headers: this.headersAuth});
  }

  getClienteByToken():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/getclientebytoken', {headers: this.headersAuth});
  }

  getPagamentosCliente():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/1/getpaymentlist', {headers: this.headersAuth});
  }

  //#endregion

  //#region HARDCODEDNotificacoes

  //hardcode
  getNotificacoes():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/notificacao/getnotifications/1', {headers: this.headersAuth});
  }

  

 

  //#region HARDCODEDLembrete

  //hardcode
  getLembretesList():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/lembrete/showall/1', {headers: this.headersAuth});
  }

  removeLembrete(val:any):Observable<any[]>{
    return this.http.delete<any>(this.APIUrl+'/lembrete/'+val+'/remreminder', {headers: this.headersAuth});
  }

  //#endregion

  //#region HARDCODEDServico
  getNextServico():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/last/1', {headers: this.headersAuth});
  }
  //#endregion

  //#region HARDCODEDEncomendas

  getEncomendasCliente():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/encomenda/1/hclient', {headers: this.headersAuth});
  }

  //#endregion

  //#region HARDCODEDEstabelecimento

  getEstabelecimentoById():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/1', {headers: this.headersAuth});
  }

  getAllEstabelecimentos():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/getallestabelecimentos', {headers: this.headersAuth});
  }

  getServicosByIdAnimal(Id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/animal/' + Id , {headers: this.headersAuth});
  }

  //#endregion

  //#region Marcar Servico Cliente

  getCatalogoByIdEstabelecimento(Id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servicocatalogo/'+ Id, {headers: this.headersAuth});
  }


  //#endregion


  //#region Metodos Auxiliares
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


