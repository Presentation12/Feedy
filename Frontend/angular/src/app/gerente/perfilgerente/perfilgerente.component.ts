import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';

declare function showPass(pass: any): any;
declare function editable(): any;
declare function nonEditable(): any;
declare function insertsPass(buton: any, passes: any): any;
declare function cancel(buton: any, passes: any): any;

@Component({
  selector: 'app-perfilgerente',
  templateUrl: './perfilgerente.component.html',
  styleUrls: ['./perfilgerente.component.css']
})
export class PerfilgerenteComponent implements OnInit {

  Gerente: any = {};
  ListaClinicas: any = [];
  fotoFileName:string="";
  fotoFilePath:string="";
  PassNew: any;
  PassConfirm: any;
  Account: any;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url: any) {
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ShowPass(pass: any) {
    showPass(pass);
  }

  Editable() {
    editable();
  }

  NonEditable() {
    nonEditable();
  }

  InsertsPass(buton: any, passes: any) {
    insertsPass(buton, passes);
  }

  Cancel(buton: any, passes: any) {
    cancel(buton, passes);
  }

  edit(): void {
    this.Account = {
      FuncionarioFoto: `${this.Gerente.FuncionarioFoto}`,
      Nome: `${this.Gerente.Nome}`,
      Apelido: `${this.Gerente.Apelido}`,
      Email: `${this.Gerente.Email}`,
      Telemovel: `${this.Gerente.Telemovel}`
    }
    this.service.editDadosFuncionario(this.Account).subscribe();
    nonEditable();
  }

  editPassword(): void {
    this.Account = {
      Pass: `${this.PassNew}`
    }
    if((this.PassNew == this.PassConfirm))
    {
      this.service.editPasswordsFuncionario(this.Account).subscribe();
      nonEditable();
    }
    else
    {
      alert("Passwords não equivalentes");
    }
  }

  uploadPhoto(event:any){
    var file = event.target.files[0];
    const formData:FormData=new FormData();
    formData.append("uploadedFile", file,file.name)

    this.service.UploadPhotoFuncionario(formData).subscribe(data => {
      this.fotoFileName = data.toString();
      this.Gerente.FuncionarioFoto = this.service.APIUrlPhotos+this.fotoFileName;
    })
  }

  refreshDadosGerente()
  {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
    })
  }

  refreshListaClinicasGerente() {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
      this.service.getClincasVeterinario(this.Gerente.IdFuncionario).subscribe(data => {
        this.ListaClinicas = data;
      })
    })
  }

  logout(){
    localStorage.setItem('token', '')
    }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/perfilJoao.js");
    this.refreshDadosGerente();
    this.refreshListaClinicasGerente();
  }

}
