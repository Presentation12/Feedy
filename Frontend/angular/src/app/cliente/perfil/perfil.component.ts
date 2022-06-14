import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';
import { Router } from '@angular/router'

declare function showPass(pass:any):any;
declare function editable():any;
declare function nonEditable():any;
declare function insertsPass():any;
declare function changePlan():any;
declare function cancel():any;
declare function verifyPlan():any;
declare function disablePlan():any;


@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {

  fotoFileName:any;
  Pass:any;
  ConfirmPass:any;
  ClienteDados:any={};
  cliente:any={};
  oldMail:any;

  constructor(private scripts: ScriptsService, private service: SharedService, private router: Router) { }

  public loadScript(url : any)
  {
    let node = document.createElement("script");
    node.src=url;
    node.type ='text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/perfilGoncalo.js");
    this.RefreshDadosCliente();
  }

  logout(){
    localStorage.setItem('token', '');
  }

  ChangePlan()
  {
    changePlan();
  }

  VerifyPlan()
  {
    verifyPlan();
  }

  DisablePlan()
  {
    disablePlan();
  }

  ShowPass(pass:any){
    showPass(pass);
  }

  Editable(){
    editable();
    this.oldMail = `${this.cliente.Email}`;
  }

  NonEditable(){
    nonEditable();
  }

  InsertsPass(){
    insertsPass();
  }

  Cancel(){
    cancel();
  }

  RefreshDadosCliente(){
    this.service.getClienteByToken().subscribe(data=>{
      this.cliente=data;
    })
  }

  UpdateDadosCliente(){
    this.ClienteDados={
      ClienteFoto:`${this.cliente.ClienteFoto}`,
      Nome:`${this.cliente.Nome}`,
      Apelido:`${this.cliente.Apelido}`,
      Email:`${this.cliente.Email}`,
      Telemovel:`${this.cliente.Telemovel}`,
      IdCliente: `${this.cliente.IdCliente}`,
      TipoConta:`${this.cliente.TipoConta}`
    }

    if(this.oldMail != this.ClienteDados.Email){
      alert(`O seu novo mail agora é: ${this.cliente.Email}.\nTem que fazer login novamente`)
      this.router.navigateByUrl('/cliente/login').then(() =>{
      this.router.navigate([decodeURI('/cliente/login')]);
      });
    }
    this.service.patchDadosCliente(this.ClienteDados).subscribe();

    nonEditable();
  }

  UpdateClientePassword(){
    this.ClienteDados={
      Pass:`${this.Pass}`,
      Email:`${this.cliente.Email}`
    }
    if (this.Pass==this.ConfirmPass)
    {
      this.service.patchPasswordCliente(this.ClienteDados).subscribe();
      this.Cancel();
    }
    else{alert("Passwords Didnt Match")}
  }

  uploadPhoto(event:any){
    var file = event.target.files[0];
    const formData:FormData=new FormData();
    formData.append("uploadedFile", file,file.name)

    this.service.UploadPhoto(formData).subscribe(data => {
      this.fotoFileName = data.toString();
      this.cliente.ClienteFoto = this.service.APIUrlPhotos+this.fotoFileName;
    })
  }

}
