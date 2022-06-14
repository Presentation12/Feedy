import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-faltarpedido',
  templateUrl: './faltarpedido.component.html',
  styleUrls: ['./faltarpedido.component.css']
})
export class FaltarpedidoComponent implements OnInit {

  Funcionario:any={};
  NotificacaoList:any=[]
  notDetail:any="";
  showDetails:number=0;
  Pedido:any={
    dataInicial:"",
    dataFinal:"",
    motivo:""
  }
  canSubmit:number = 0;

  constructor(private service : SharedService, private script : ScriptsService) { }

  ngOnInit(): void {
    this.refreshNotifications();
    this.refreshFuncinario();
  }

  DeleteNotification(notificacao:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotificationFuncionario(notificacao).subscribe()
  }

  refreshNotifications(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Funcionario = data;
      this.service.getNotifacoesFuncionario(this.Funcionario.IdFuncionario).subscribe(data => {
        this.NotificacaoList = data;
      })
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

  tiraDetails(){
    this.showDetails = 0;
  }

  Faltar(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Funcionario = data;

      this.service.funcionarioFaltar(this.Funcionario.IdFuncionario, this.Pedido).subscribe(
        data => alert("Pedido de faltar executado com sucesso!"),
        error => alert("Erro: Se este erro premanecer, por favor contacte-nos"));
    })
  }

  refreshFuncinario(){
    this.service.getFuncionarioByToken().subscribe(data =>{
      this.Funcionario=data;
    })
  }

  CheckInfo(pedido:any){
    console.log(this.Pedido)
    if(pedido.dataInicial && pedido.dataFinal && pedido.motivo) this.canSubmit = 1;
    else this.canSubmit = 0
  }

  logout(){
    localStorage.setItem('token', '');
  }

}
