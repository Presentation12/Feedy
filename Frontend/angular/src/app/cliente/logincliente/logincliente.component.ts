import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { Router } from '@angular/router';

declare function strengthChecker():any;
declare function toggle():any;
declare function toggle2():any;

@Component({
  selector: 'app-logincliente',
  templateUrl: './logincliente.component.html',
  styleUrls: ['./logincliente.component.css']
})
export class LoginclienteComponent implements OnInit {
  canAdd:number=0;
  conta:any= {
    Email: "",
    Pass: ""
  };
  novaConta={
    Nome:"",
    Email:"",
    Apelido:"",
    Pass:"",
    Telemovel:"",
    IdMorada:""
  };
  checkAccount:number=0;

  constructor(private service: SharedService, private router : Router) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../../assets/scripts/functionNuno.js");
  }

  StrengthChecker(){
    strengthChecker();
  }

  Toggle(){
    toggle();
  }

  Toggle2(){
    toggle2();
  }

  login(): void {
   this.service.loginCliente(this.conta).subscribe(data=>{
    if (data == "Cliente não existe! (Parameter 'account')") alert("Cliente não existe!")
    else if(data == "Password Errada. (Parameter 'account')") alert("Password Errada.")
    else{
      localStorage.setItem('token', data.toString());
      this.router.navigateByUrl('/cliente').then(() =>{
        this.router.navigate([decodeURI('/cliente')]);
      });
    }
   });
  }

  registaConta() {
    this.service.registoCliente(this.novaConta)
    .subscribe(data => alert("Conta registada com sucesso!"), error => alert("Erro no registo, tenta inserir outros dados"))
  }

  checkInfo(info:any){
    let number = 0;

    if(info.Nome && info.Email && info.Pass){
      for (let i = 0; i < info.Email.length; i++) {
        const element = info.Email[i];
        if (element=='@') {
          number = 1
        }

      }
    }
    else this.canAdd = 0;
    if (number==1) {
      this.canAdd=1;
    }else this.canAdd=0;
  }
}
