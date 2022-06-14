import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';

declare function strengthChecker():any; 
declare function toggle():any;
declare function toggle2():any;

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  conta:any;


  constructor(private service: SharedService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  } 

  ngOnInit(): void {
    /* this.loadScript("src/assets/loginCliente.js"); */
    this.login();
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
  this.conta = {
    Email: "ps@gmail.com",
    Pass: "ps"
  }
 this.service.loginCliente(this.conta).subscribe(data=>{
   console.log(data);
 });
}


}
