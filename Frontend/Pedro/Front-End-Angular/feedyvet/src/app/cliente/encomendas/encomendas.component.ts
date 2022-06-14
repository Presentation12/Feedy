import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GetApiService } from '../../get-api.service'

declare function Filter():any;

@Component({
  selector: 'app-encomendas',
  templateUrl: './encomendas.component.html',
  styleUrls: ['./encomendas.component.css']
})

export class EncomendasComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  NotificacoesList:any=[];
  EncomendasList:any=[];
  Cliente:any={};
  conta:any= {};
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
    this.changeTip();
    this.refreshNotificacoes();
    this.getClienteByToken();
    this.refreshEncomendas();
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.loadScript("../../assets/scripts/filterPedro.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
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

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  filter(){
    Filter();
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  logout(){
    alert("Saiu da conta com sucesso!");
    alert("Carregue no F5!");
    localStorage.setItem('token', '');
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  refreshEncomendas(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getEncomendasCliente(this.Cliente.IdCliente).subscribe(data => {
        this.EncomendasList=data;
      })
    })
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.service.changeTip(this.tipText);
      this.service.changeTipLi(this.selected);
    }, 5000);
  }
}
