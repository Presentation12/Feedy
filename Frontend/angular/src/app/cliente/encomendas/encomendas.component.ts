import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

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

  constructor(private scripts: ScriptsService, private service: SharedService) { }

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
    localStorage.setItem('token', '')
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
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }
}
