import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

declare function Filter():any;

@Component({
  selector: 'app-gerirfuncionarios',
  templateUrl: './gerirfuncionarios.component.html',
  styleUrls: ['./gerirfuncionarios.component.css']
})

export class GerirfuncionariosComponent implements OnInit {

  ListaFuncionarios:any=[];
  Gerente:any={};
  ativo:any=0;
  FuncionarioSelecionado:any={};
  Estabelecimento:any;
  @Input() filterText:any;
  newFunc: any={
    Nome:"",
    Apelido:"",
    Especialidade:"",
    Email:"",
    Telemovel:"",
    Pass:"",
    IdEstabelecimento:"",
    Codigo:"",
    FuncionarioFoto:"https://localhost:5001/images/avatar.png"
  };
  estabelecimento:any={};
  addProcess:number = 0;
  funcionario:any={};
  fotoFileName:any;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url : any)
  {
    let node = document.createElement("script");
    node.src=url;
    node.type ='text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/lista_funcionarios.js");
    this.RefreshListaFuncionarios();
    this.RefreshGerente();
  }

  Filter(){
    Filter();
  }

  RefreshListaFuncionarios(){
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {
        this.Estabelecimento = data
        this.service.getdadosfuncionario(this.Gerente.IdFuncionario, this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.ListaFuncionarios=data
        });
      })
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  RefreshGerente(){
    this.service.getFuncionarioByToken().subscribe(data=>{this.Gerente=data});
  }

  Editar(funcionario:any){
    this.ativo=1;
    this.FuncionarioSelecionado=funcionario;
  }

  Ok(){
    this.ativo=0;
  }

  UpdateFuncionariosDados(){
    this.service.patchDadosFuncionario(this.FuncionarioSelecionado).subscribe();
    this.Ok();
  }

  add()
  {
    this.addProcess = 1;
  }

  addFuncionario(){
    this.service.getFuncionarioByToken().subscribe(data => {
    this.funcionario=data;
    this.service.getEstabelecimentoByGerente(this.funcionario.IdFuncionario).subscribe(data=>{
      this.estabelecimento = data;
      this.addProcess = 0;
      this.newFunc.IdEstabelecimento = this.estabelecimento.IdEstabelecimento;
      this.service.addFuncionario(this.newFunc,this.estabelecimento.IdEstabelecimento).subscribe();
    })
    })
  }

  uploadPhoto(event:any){
    var file = event.target.files[0];
    const formData:FormData=new FormData();
    formData.append("uploadedFile", file,file.name)

    this.service.UploadPhoto(formData).subscribe(data => {
      this.fotoFileName = data.toString();
      this.newFunc.FuncionarioFoto = this.service.APIUrl+this.fotoFileName;
    })
  }

  cancel()
  {
    this.addProcess = 0;
  }

  RemoveFuncionario(){
    this.FuncionarioSelecionado.Estado="Inativo";
    this.service.AlterarEstadoFuncionario(this.FuncionarioSelecionado).subscribe();
    this.Ok();
  }
}
