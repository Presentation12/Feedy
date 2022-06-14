import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'

declare function submit():any;
declare function ok():any;

@Component({
  selector: 'app-ajudagerente',
  templateUrl: './ajudagerente.component.html',
  styleUrls: ['./ajudagerente.component.css']
})
export class AjudagerenteComponent implements OnInit {

  Gerente: any = {};

  public loadScript(url : any)
  {
    let node = document.createElement("script");
    node.src=url;
    node.type ='text/javascript';
    document.body.append(node);
  }

  constructor(private service: SharedService) { }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/ajudaJoao.js");
    this.refreshDadosGerente();
  }

  Submit()
  {
    submit();
  }

  Ok()
  {
    ok();
  }

  logout(){
    localStorage.setItem('token', '');
  }

  refreshDadosGerente() {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
    })
  }

}
