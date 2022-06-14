import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GetApiService } from '../get-api.service'

@Component({
  selector: 'app-cliente',
  templateUrl: './cliente.component.html',
  styleUrls: ['./cliente.component.css']
})

export class ClienteComponent implements OnInit {

  conta:any= {};
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  LembretesList:any=[];
  Cliente:any={};
  NextServico:any={};
  animal:any;
  checkServico:number = 0;
  stringAux:any="";
  notDetail:any="";
  showDetails:number=0;

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
    this.getClienteByToken();
    this.changeTip();
    this.refreshNotificacoes();
    this.refreshLembreteCliente();
    this.refreshNextService();
    this.refreshAnimalCliente();
  }

  login(): void {
    this.conta={
      Email: "ps@gmail.com",
      Pass: "ps"
    }
    this.service.loginCliente(this.conta).subscribe(data=>{
     if (data == "Cliente não existe! (Parameter 'account')") alert("Cliente não existe!")
     else if(data == "Password Errada. (Parameter 'account')") alert("Password Errada.")
     else{
       alert("Login com sucesso! (Conta hardcode, pois a página login não era minha função)")
       alert("Carregue no F5!")
       localStorage.setItem('token', data.toString());
     }
    });
   }

  selectNot(notificacao:any){
    this.notDetail={
      Descricao:`${notificacao.Descricao}`,
      DataNotificacao:`${notificacao.DataNotificacao}`
    }
  }

  showDetailsNot(){
    this.showDetails = 1;
  }

  tiraDetails(){
    this.showDetails = 0;
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  refreshAnimalCliente(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
      this.service.getAnimalClienteID(this.Cliente.IdCliente).subscribe(data => {
        this.AnimaisList=data;
      })
    })
  }

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  deleteLembrete(lembrete:any){
    if(confirm('Tem a certeza que pretende apagar o lembrete?')){
      this.service.removeLembrete(lembrete.IdLembrete).subscribe(data => {
        this.stringAux = data;
        if(this.stringAux == 'Lembrete já foi eliminado, dê refresh para atualizar a página')
          alert ('Lembrete já foi eliminado, dê refresh para atualizar a página')
      })
    }
  }

  refreshLembreteCliente(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getLembretesList(this.Cliente.IdCliente).subscribe(data => {
        this.LembretesList=data;
      })
    })
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  refreshNextService(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNextServico(this.Cliente.IdCliente).subscribe(data => {
        this.NextServico=data;
    })
    })
  }
  changeTip(){
    setInterval(() => {
      this.tipText = this.service.changeTip(this.tipText);
      this.service.changeTipLi(this.selected);
    }, 5000);
  }

  logout(){
    alert("Saiu da conta com sucesso!");
    alert("Carregue no F5!");
    localStorage.setItem('token', '');
  }
}
