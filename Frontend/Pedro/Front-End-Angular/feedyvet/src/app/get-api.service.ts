import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class GetApiService {
  readonly APIUrl="https://localhost:5001/api"
  readonly APIUrlPhotos="https://localhost:5001/images/"

  //private tokenHardcode:any="eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJwc0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJDbGllbnRlIiwiZXhwIjoxNjUzNzYxMDQ3fQ.DCg6mRIAl8mLW4MUgEeDt0Awz9RhXjHESiHm-TIR607sP5YEsIiR90Pfn02mUTzAmJZ35C8e-t7OxzT2sidBmA"
  headersAuth = new HttpHeaders().append("Authorization", `Bearer ${localStorage.getItem('token')}`);
  //headersAuth = new HttpHeaders().append("Authorization", `Bearer ${this.tokenHardcode}`);

  constructor(private http:HttpClient) { }

  //#region Animal

  getAnimalList():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/animal', {headers: this.headersAuth});
  }

  getAnimalClienteID(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/animal/getanimalsbyclient/'+val, {headers: this.headersAuth});
  }

  getServicoEstabelecimento(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/estabelecimento/'+val, {headers: this.headersAuth})
  }

  addAnimal(val:any, id:any){
    return this.http.post(this.APIUrl+'/animal/'+id+'/addanimal', val, {headers: this.headersAuth})
  }

  patchAnimal(val:any, id:any){
    return this.http.patch(this.APIUrl+'/animal/'+id+'/editanimal', val, {headers: this.headersAuth})
  }

  removeAnimal(val:any){
    return this.http.patch(this.APIUrl+'/animal/'+val+'/del', null, {headers: this.headersAuth})
  }

  UploadPhoto(val:any){
    return this.http.post(this.APIUrl+'/animal/SaveFile', val, {headers: this.headersAuth})
  }

  //#endregion

  //#region Avaliacoes

  getAvaliacoesEstabelecimento(id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/avaliacaoestabelecimentoutilizador/'+id+'/checkavaliacoes', {headers: this.headersAuth});
  }

  //#endregion

  //#region Cliente

  loginCliente(account:any){
    return this.http.post(this.APIUrl+'/cliente/login', account)
  }

  registoCliente(account:any){
    return this.http.post(this.APIUrl+'/cliente', account)
  }

  getClientebyID(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/'+val, {headers: this.headersAuth});
  }

  getClienteByToken():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/getclientebytoken', {headers: this.headersAuth});
  }

  getPagamentosCliente(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/cliente/'+val+'/getpaymentlist', {headers: this.headersAuth});
  }

  patchDadosCliente(cliente:any):Observable<any[]>{
    return this.http.patch<any>(this.APIUrl+'/cliente', cliente,{headers:this.headersAuth});
  }

  //UpdateFoto
  patchPasswordCliente(cliente:any):Observable<any[]>{
    return this.http.patch<any>(this.APIUrl+'/Cliente/recoverpass',cliente,{headers:this.headersAuth});
  }

  //#endregion

  //#region Funcionario

  loginFuncionario(account:any){
    return this.http.post(this.APIUrl+'/funcionario/login', account)
  }

  getFuncionarioByToken():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/funcionario/getfuncionariobytoken', {headers: this.headersAuth});
  }

  getFuncionariosByEstabelecimento(id:any){
    return this.http.get(this.APIUrl+'/funcionario/showfuncionarios/'+id, {headers: this.headersAuth})
  }

  getEstabelecimentosNameAndPhoto(): Observable<any[]>
  {
    return this.http.get<any>(this.APIUrl + '/estabelecimento/getallestabelecimentosnames');
  }

  // mostra todas as clinicas de um veterinario
  getClincasVeterinario(id:any): Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/funcionario/'+id+'/clinicas', { headers: this.headersAuth });
  }

  // update dos dados do veterinario (patch)
  editDadosFuncionario(funcionario: any): Observable<any[]> {
    return this.http.patch<any>(this.APIUrl + '/funcionario/editaccount', funcionario, { headers: this.headersAuth });
  }

  //Upddate Dados Funcionario
  patchDadosFuncionario(funcionario:any):Observable<any[]>{
    return this.http.patch<any>(this.APIUrl+'/funcionario',funcionario,{headers:this.headersAuth});
  }

  // update password (patch)
  editPasswordsFuncionario(funcionario: any): Observable<any[]> {
    return this.http.patch<any>(this.APIUrl + '/funcionario/changepassword', funcionario, { headers: this.headersAuth });
  }

  UploadPhotoFuncionario(funcionario: any) {
    return this.http.post(this.APIUrl + '/funcionario/SaveFile', funcionario, { headers: this.headersAuth })
  }

  addFuncionario(funcionario:any, idEstabelecimento:any){
    return this.http.post(this.APIUrl+'/Funcionario/'+idEstabelecimento+'/addFunc',funcionario,{headers:this.headersAuth});
  }

  AlterarEstadoFuncionario(funcionario:any):Observable<any[]>{
    return this.http.patch<any>(this.APIUrl+'/Funcionario/changestatus',funcionario,{headers:this.headersAuth});
  }

  getdadosfuncionario(idFunc:any, idEst:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/funcionario/'+idFunc+'/'+idEst,{headers:this.headersAuth});
  }

  //#endregion

  //#region Notificacoes

  getNotificacoes(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/notificacao/getnotifications/'+val, {headers: this.headersAuth});
  }

  getNotifacoesFuncionario(id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/notificacaoFuncionario/getnotifications/'+id, {headers: this.headersAuth})
  }

  DeleteNotificationFuncionario(notificacao:any){
    return this.http.patch(this.APIUrl+'/notificacaoFuncionario/delete',notificacao, {headers: this.headersAuth})
  }

  DeleteNotification(notificacao:any){
    return this.http.patch(this.APIUrl+'/notificacao/delete',notificacao, {headers: this.headersAuth})
  }



  //#endregion

  //#region Gerente

  getGerente(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/funcionario/'+val, {headers: this.headersAuth});
  }

  GetServicosRestantesGer(idGerente:any, IdEstabelecimento:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/funcionario/'+idGerente+'/'+IdEstabelecimento+'/dailyservices', {headers: this.headersAuth});
  }

  GetServicosEfetuadosGer(idGerente:any, IdEstabelecimento:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/funcionario/'+idGerente+'/'+IdEstabelecimento+'/servicesdone', {headers: this.headersAuth});
  }

  GetLucroDiario(idEstabelecimento:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/'+idEstabelecimento+'/getdaily', {headers: this.headersAuth});
  }

  //#endregion

  //#region Lembrete

  addlembrete(lembrete:any){
    return this.http.post(this.APIUrl+'/lembrete/addlembrete/', lembrete,{headers: this.headersAuth});
  }

  getLembretesList(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/lembrete/showall/'+val, {headers: this.headersAuth});
  }

  getLembretesListByAnimal(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/lembrete/showallanimal/'+val, {headers: this.headersAuth});
  }

  removeLembrete(val:any):Observable<any[]>{
    return this.http.delete<any>(this.APIUrl+'/lembrete/'+val+'/remreminder', {headers: this.headersAuth});
  }

  //#endregion

  //#region Servico

  getNextServico(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/last/'+val, {headers: this.headersAuth});
  }

  GetHistoricoAnimal(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/animal/'+val, {headers: this.headersAuth});
  }

   //Repete a função acima
  getServicosByIdAnimal(Id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/servico/animal/' + Id , {headers: this.headersAuth});
  }

  marcarServico(id:any, serv:any){
    return this.http.post(this.APIUrl+'/servico/'+id+'/MarcarServico', serv,{headers: this.headersAuth})
  }

  //#endregion

  //#region Encomendas

  getEncomendasCliente(val:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/encomenda/'+val+'/hclient', {headers: this.headersAuth});
  }

  //post da encomenda
  postEncomenda(encomenda : any): Observable<any[]>
  {
    return this.http.post<any>(this.APIUrl + '/encomenda/1',encomenda, {headers: this.headersAuth});
  }


  //#endregion

  //#region Estabelecimento

  getEstabelecimentoById(id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/'+id, {headers: this.headersAuth});
  }

  getEstabelecimentoByGerente(idGerente:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/estabelecimentoger/'+idGerente, {headers: this.headersAuth})
  }

  getAllEstabelecimentos():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/estabelecimento/getallestabelecimentos', {headers: this.headersAuth});
  }

  // buscar os estabelecimentos para mostrar
  getListaEstabelecimentos(): Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/estabelecimento', { headers: this.headersAuth });
  }

  // bucar todos os distritos existentes de estabelecimentos
  getDistritos(): Observable<any[]> {
    return this.http.get<any>(this.APIUrl + '/estabelecimento/distritos', { headers: this.headersAuth });
  }

  //#endregion

  //#region StockEstabelecimento

  getArtigos(): Observable<any[]>{
    return this.http.get<any>(this.APIUrl + '/stockestabelecimento', {headers:this.headersAuth});
  }

  //#endregion

  //#region EncomendaStock

  //post da encomenda e suas quantidades
  postEncomendaStock(encomendaStock: any): Observable<any[]>
  {
    return this.http.post<any>(this.APIUrl + '/encomendastock', encomendaStock, {headers: this.headersAuth});
  }

  //#endregion

  //#region Marcar Servico Cliente
    getCatalogoByIdEstabelecimento(Id:any):Observable<any[]>{
      return this.http.get<any>(this.APIUrl+'/servicocatalogo/'+ Id, {headers: this.headersAuth});
    }

  //#endregion

  // Duarte

  readanimalsbyfuncionario(): Observable<any[]>{
    return this.http.get<any[]>(`${this.APIUrl}/animal/getanimalsbyfuncionario`, {headers: this.headersAuth});
  }

  readClienteByIdAnimal(idAnimal: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/cliente/getclientebyanimal/${idAnimal}`, {headers: this.headersAuth});
  }


  readVeterinario(): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/funcionario/getfuncionariobytoken`, {headers: this.headersAuth});
  }


  readPrescricoes(): Observable<any[]>{
    return this.http.get<any[]>(`${this.APIUrl}/prescricao/getprescricoesbytoken`, {headers: this.headersAuth});
  }

  readTratamentoByPrescricao(idPrescricao: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/tratamento/gettratamentobyprescricao/${idPrescricao}`, {headers: this.headersAuth});
  }

  readStockById(idStock: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/stockestabelecimento/getstockbyid/${idStock}`, {headers: this.headersAuth});
  }

  readServicoByIdPrescricao(idPrescricao: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/servico/getservicobyidprescricao/${idPrescricao}`, {headers: this.headersAuth});
  }

  readEstabelecimentoByIdServico(IdServico: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/estabelecimento/getestabelecimentobyidservico/${IdServico}`, {headers: this.headersAuth});
  }

  readClienteByIdServico(IdServico: number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/cliente/getclientebyservico/${IdServico}`, {headers: this.headersAuth});
  }

  readPersonalServicesMissing(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.APIUrl}/funcionario/${IdFuncionario}/personaldailyservices`, {headers: this.headersAuth});
  }

  readPersonalServicesDone(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.APIUrl}/funcionario/${IdFuncionario}/servicesdone`, {headers: this.headersAuth});
  }

  changeStatusServico(servico:any): Observable<any>{
    return this.http.patch(`${this.APIUrl}/servico/changestatusservico`, servico, {headers: this.headersAuth});
  }

  rescheduleServico(servico:any): Observable<any>{
    return this.http.patch(`${this.APIUrl}/servico/reschedule`, servico, {headers: this.headersAuth});
  }

  getClienteById(IdCliente:number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/cliente/${IdCliente}`, {headers: this.headersAuth});
  }

  getAnimalById(IdAnimal:number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/animal/${IdAnimal}`, {headers: this.headersAuth});
  }

  readHistoricoFuncionario(IdFuncionario:number): Observable<any[]>{
    return this.http.get<any[]>(`${this.APIUrl}/servico/funcionario/${IdFuncionario}`, {headers: this.headersAuth});
  }

  readDetailsServico(IdServico:number): Observable<any>{
    return this.http.get<any>(`${this.APIUrl}/servico/${IdServico}/details`, {headers: this.headersAuth});
  }

  updateAnimal(animal:any): Observable<any>{
    return this.http.patch<any>(`${this.APIUrl}/animal`, animal, {headers: this.headersAuth});
  }

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
//#endregion
}
