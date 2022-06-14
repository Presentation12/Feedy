import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';
import { Router } from '@angular/router'

declare function mostraPass(): any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  Account: any;
  Cod: any;
  Pass: any;
  loginData: any={
    token:"",
    claimReturn:""
  }

  constructor(private service : SharedService, private scripts : ScriptsService, private router : Router ) { }

  public loadScript(url: any) {
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/loginFuncionario.js")
  }

  MostraPass() {
    mostraPass();
  }

  login(): void {
    this.Account = {
      Codigo: this.Cod,
      Pass: this.Pass
    }

    this.service.loginFuncionario(this.Account).subscribe(data => {
      this.loginData = data

      if (data == "Funcionário não existe!") alert("Funcionário não existe!")
      else if(data == "Password Errada.") alert("Password Errada.")
      else{
        if(this.loginData.claimReturn == "Funcionario"){
          localStorage.setItem('token', this.loginData.token.toString());
          this.router.navigateByUrl('/funcionario').then(() =>{
            this.router.navigate([decodeURI('/funcionario')]);
          });
        }
        else if (this.loginData.claimReturn == "Gerente"){
          localStorage.setItem('token', this.loginData.token.toString());
          this.router.navigateByUrl('/gerente').then(() =>{
            this.router.navigate([decodeURI('/gerente')]);
          });
        }
        else alert("Algo inesperado aconteceu! Contacto-nos para mais informaçõs")

      }
    });
  }
}
