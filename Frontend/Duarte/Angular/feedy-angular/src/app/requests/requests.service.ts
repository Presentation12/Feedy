import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AnimaisService {

  baseURL = 'https://localhost:5001/api/';

  // token! depois meter em cookies!
  headersAuth = new HttpHeaders().append("Authorization", 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJqYkBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiR2VyZW50ZSIsIkdlcmVudGVfNCJdLCJleHAiOjE2NTQxMzA0NTN9.Bfm12MoyNcg9NCax7C2l-E1Ea1Dykxk0nbB14pptK7UzO8ID671eh2nvbJUyVXqe_Zhe_FEhkrLbemfajTHTMA');

  constructor(private http: HttpClient) { }

  


  readanimalsbyfuncionario(): Observable<any[]>{
    return this.http.get<any[]>(`${this.baseURL}animal/getanimalsbyfuncionario`, {headers: this.headersAuth});
  }

  readClienteByIdAnimal(idAnimal: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}cliente/getclientebyanimal/${idAnimal}`, {headers: this.headersAuth});
  }


  readVeterinario(): Observable<any>{
    return this.http.get<any>(`${this.baseURL}funcionario/getfuncionariobytoken`, {headers: this.headersAuth});
  }


  readPrescricoes(): Observable<any[]>{
    return this.http.get<any[]>(`${this.baseURL}prescricao/getprescricoesbytoken`, {headers: this.headersAuth});
  }

  readTratamentoByPrescricao(idPrescricao: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}tratamento/gettratamentobyprescricao/${idPrescricao}`, {headers: this.headersAuth});
  }

  readStockById(idStock: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}stockestabelecimento/getstockbyid/${idStock}`, {headers: this.headersAuth});
  }

  readServicoByIdPrescricao(idPrescricao: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}servico/getservicobyidprescricao/${idPrescricao}`, {headers: this.headersAuth});
  }

  readEstabelecimentoByIdServico(IdServico: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}estabelecimento/getestabelecimentobyidservico/${IdServico}`, {headers: this.headersAuth});
  }

  readClienteByIdServico(IdServico: number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}cliente/getclientebyservico/${IdServico}`, {headers: this.headersAuth});
  }

  readPersonalServicesMissing(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.baseURL}funcionario/${IdFuncionario}/personaldailyservices`, {headers: this.headersAuth});
  }
    
  readPersonalServicesDone(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.baseURL}funcionario/${IdFuncionario}/servicesdone`, {headers: this.headersAuth});
  }

  changeStatusServico(servico:any): Observable<any>{
    return this.http.patch(`${this.baseURL}servico/changestatusservico`, servico, {headers: this.headersAuth});
  }

  rescheduleServico(servico:any): Observable<any>{
    return this.http.patch(`${this.baseURL}servico/reschedule`, servico, {headers: this.headersAuth});
  }

  getClienteById(IdCliente:number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}cliente/${IdCliente}`, {headers: this.headersAuth});
  }

  getAnimalById(IdAnimal:number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}animal/${IdAnimal}`, {headers: this.headersAuth});
  }

  readHistoricoFuncionario(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.baseURL}servico/funcionario/${IdFuncionario}`, {headers: this.headersAuth});
  }

  readDetailsServico(IdServico:number): Observable<any>{
    return this.http.get<any>(`${this.baseURL}servico/${IdServico}/details`, {headers: this.headersAuth});
  }

  updateAnimal(animal:any): Observable<any>{
    return this.http.patch<any>(`${this.baseURL}animal`, animal, {headers: this.headersAuth});
  }

}
