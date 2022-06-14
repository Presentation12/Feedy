import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-cliente',
  templateUrl: './cliente.component.html',
  styleUrls: ['./cliente.component.css']
})
export class ClienteComponent implements OnInit {
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  LembretesList:any=[];
  Cliente:any={};
  NextServico:any={};
  animal:any;
  checkServico:number = 0;
  checkAccount:number = 0;
  stringAux:any="";
  notDetail:any="";
  showDetails:number=0;

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  constructor(private scripts: ScriptsService, private service: SharedService) { }

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

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
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

  tiraDetails(){
    this.showDetails = 0;
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
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }

  logout(){
    localStorage.setItem('token', '');
  }
}
