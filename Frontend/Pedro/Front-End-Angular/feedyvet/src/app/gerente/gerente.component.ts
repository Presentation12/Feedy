import { Component, OnInit } from '@angular/core';
import { GetApiService } from '../get-api.service'

@Component({
  selector: 'app-gerente',
  templateUrl: './gerente.component.html',
  styleUrls: ['./gerente.component.css']
})
export class GerenteComponent implements OnInit {

  conta:any= {};
  loginData:any={};
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
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

  constructor(private service: GetApiService) { }

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

  login(): void {
    this.conta={
      Codigo: 456498132,
      Pass: "mp"
    }
    this.service.loginFuncionario(this.conta).subscribe(data => {
      this.loginData = data

      if (data == "Funcionário não existe!") alert("Funcionário não existe!")
      else if(data == "Password Errada.") alert("Password Errada.")
      else {
        alert("Login com sucesso! (Conta hardcode, pois a página login não era minha função)")
        alert("Carregue no F5!")
        localStorage.setItem('token', this.loginData.token.toString());
      }
    });
   }



  DeleteNotification(notificacao:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotificationFuncionario(notificacao).subscribe()
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
          this.LastAvaliacao = this.AvaliacoesList[0];
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

  logout(){
    alert("Logout feito com sucesso!")
    alert("Clique F5 para dar refresh!")
    localStorage.setItem('token', '');
  }
}
