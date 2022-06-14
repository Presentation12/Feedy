import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GetApiService } from '../../get-api.service'

declare function submit():any;
declare function continueSubmit():any;
declare function continueSubmit2():any;

@Component({
  selector: 'app-ajuda',
  templateUrl: './ajuda.component.html',
  styleUrls: ['./ajuda.component.css']
})
export class AjudaComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  NotificacoesList:any=[];
  Cliente:any={};
  conta:any= {};
  notDetail:any="";
  showDetails:number=0;

  constructor(private service: GetApiService){  }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.changeTip();
    this.getClienteByToken();
    this.refreshNotificacoes();
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

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
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

  ContinueSubmit(){
    continueSubmit();
  }

  ContinueSubmit2(){
    continueSubmit2();
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.service.changeTip(this.tipText);
      this.service.changeTipLi(this.selected);
    }, 5000);
  }
}
