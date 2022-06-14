import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';
import { FormArray, FormControl, FormGroup } from '@angular/forms';

declare function submit():any;
declare function continueSubmit():any;
declare function continueSubmit2():any;

@Component({
  selector: 'app-ajuda-cliente',
  templateUrl: './ajuda-cliente.component.html',
  styleUrls: ['./ajuda-cliente.component.css']
})

export class AjudaClienteComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  NotificacoesList:any=[];
  Cliente:any={};

  constructor(private scripts: ScriptsService, private service: SharedService) { }

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

  logout(){
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
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }
}
