import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-faltar',
  templateUrl: './faltar.component.html',
  styleUrls: ['./faltar.component.css']
})
export class FaltarComponent implements OnInit {

  AnimaisServicoList:any=[];
  Gerente:any={};
  Data: Date = new Date();
  Estabelecimento:any={};
  NotificacaoList:any=[];
  Pedido:any={
    dataInicial:"",
    dataFinal:"",
    motivo:""
  }
  canSubmit:number = 0;

  constructor(private service : SharedService, private scripts : ScriptsService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.refreshNotifications();
    this.refreshGerente();
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

  Faltar(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;

      this.service.funcionarioFaltar(this.Gerente.IdFuncionario, this.Pedido).subscribe(
        data => alert("Pedido de faltar executado com sucesso!"),
        error => alert("Erro: Se este erro premanecer, por favor contacte-nos"));
    })
  }

  refreshGerente(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Gerente=data;
    })
  }

  CheckInfo(pedido:any){
    if(pedido.dataInicial && pedido.dataFinal) this.canSubmit = 1;
    else this.canSubmit = 0
  }

  logout(){
    localStorage.setItem('token', '');
  }
}
