import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

declare function submit():any;
declare function ok():any;

@Component({
  selector: 'app-ajudafuncionario',
  templateUrl: './ajudafuncionario.component.html',
  styleUrls: ['./ajudafuncionario.component.css']
})
export class AjudafuncionarioComponent implements OnInit {

  Veterinario: any = {};

  constructor(private service: SharedService) { }

  public loadScript(url : any)
  {
    let node = document.createElement("script");
    node.src=url;
    node.type ='text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/ajudaJoao.js");
    this.refreshDadosVeterinario();
  }

  logout(){
    localStorage.setItem('token', '');
  }

  Submit()
  {
    submit();
  }

  Ok()
  {
    ok();
  }

  refreshDadosVeterinario() {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Veterinario = data;
    })
  }

}
