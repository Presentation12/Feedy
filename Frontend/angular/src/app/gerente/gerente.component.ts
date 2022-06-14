import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';


@Component({
  selector: 'app-gerente',
  templateUrl: './gerente.component.html',
  styleUrls: ['./gerente.component.css']
})
export class GerenteComponent implements OnInit {

  AnimaisServicoList:any=[];
  Gerente:any={};
  Data: Date = new Date();
  ServicoList:any=[];
  ServicosToDo: any={};
  ServicosDone: any={};
  Lucro:any={};
  Estabelecimento:any={};
  AvaliacoesList:any=[];
  AvaliacoesNumber:any={};
  LastAvaliacao:any={};
  NotificacaoList:any=[]
  notDetail:any="";
  showDetails:number=0;
  detailAtivo:number=0;
  ServiceEditAtt:any={
    IdServico:"",
    DataServico:"",
    Estado:""
  };
  canSubmit:number=0;
  funcio:any={
    NomeCliente:"",
    Telemovel:"",
    Email:"",
    NomeAnimal:"",
    Descricao:"",
    DataServico:"",
    Estado:"",
  }

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.GetAvaliacoes();
    this.GetAvaliacoesCount();
    this.GetServicosDoneCount();
    this.GetEstabelecimento();
    this.GetDailyAmount();
    this.GetServicosRestantes();
    this.GetServicosRestantesCount();
    this.refreshNotifications();
    this.refreshAnimalEstabelecimento();
    this.refreshGerente();
  }

  DeleteNotification(notificacao:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotificationFuncionario(notificacao).subscribe()
  }

  showDetailsNot(){
    this.showDetails = 1;
  }

  tiraDetails(){
    this.showDetails = 0;
  }

  RecusarPedido(not:any){
    if(confirm("Deseja recusar o pedido?")) this.service.rejeitarpedido(not).subscribe()
  }

  AceitarPedido(not:any){
    if(confirm("Deseja aceitar o pedido?")) this.service.aceitarpedido(not).subscribe()
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  showStatus(funcionario:any){
    this.funcio=funcionario;
    this.detailAtivo=1;
  }

  editStatus(funcionario:any){
    this.funcio=funcionario;
    this.detailAtivo=2;
  }

  hideStatus(){
    this.detailAtivo=0;
  }

  checkInfo(func:any){
    if(func.DataServico && func.Estado) this.canSubmit = 1;
    else this.canSubmit = 0;
  }

  updateServico(funcionario:any){
    this.ServiceEditAtt.IdServico = funcionario.IdServico
    this.ServiceEditAtt.DataServico = funcionario.DataServico
    this.ServiceEditAtt.Estado = funcionario.Estado
    console.log(this.ServiceEditAtt)

    this.service.UpdateServico(this.ServiceEditAtt).subscribe(
      data => alert("Servico alterado com sucesso!"),
      error => alert("Problema em alterar o serviço!")
    )


    this.detailAtivo=0;
  }

  refreshNotifications(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
      this.service.getNotifacoesFuncionario(this.Gerente.IdFuncionario).subscribe(data => {
        this.NotificacaoList = data;
      })
    })
  }

  refreshGerente(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
    })
  }

  GetServicosRestantes(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
      this.service.GetServicosRestantesGer(this.Gerente.IdFuncionario, this.Estabelecimento.IdEstabelecimento).subscribe(data => {
        this.ServicoList=data;
      })
    })
  }

  GetServicosRestantesCount(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
      this.service.GetServicosRestantesGer(this.Gerente.IdFuncionario, this.Estabelecimento.IdEstabelecimento).subscribe(data => {
        this.ServicosToDo = data.length;
      })
    })
  }

  GetAvaliacoes(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente = data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data;
        this.service.getAvaliacoesEstabelecimento(this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.AvaliacoesList=data;
          this.LastAvaliacao = this.AvaliacoesList[this.AvaliacoesList.length-1];
        })
      })
    })
  }

  GetAvaliacoesCount(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente = data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data
        this.service.getAvaliacoesEstabelecimento(this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.AvaliacoesNumber=data.length;
        })
      })
    })
  }

  GetServicosDoneCount(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data
        this.service.GetServicosEfetuadosGer(this.Gerente.IdFuncionario, this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.ServicosDone = data.length;
        })
      })
    })
  }

  GetDailyAmount(){
    this.service.getFuncionarioByToken().subscribe(data=>{
      this.Gerente = data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data
        this.service.GetLucroDiario(this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.Lucro = data;
        })
      })
    })
  }

  GetEstabelecimento(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data;
      })
    })
  }

  refreshAnimalEstabelecimento(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
          this.Estabelecimento = data;
          this.service.getServicoEstabelecimento(this.Estabelecimento.IdEstabelecimento).subscribe(data => {
            this.AnimaisServicoList=data;
          })
        })
      })
  }

  selectNot(notificacao:any){
    this.notDetail={
      Descricao:`${notificacao.Descricao}`,
      DataNotificacao:`${notificacao.DataNotificacao}`
    }
  }

  logout(){
    localStorage.setItem('token', '');
  }

}
