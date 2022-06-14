import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-servicos',
  templateUrl: './servicos.component.html',
  styleUrls: ['./servicos.component.css']
})
export class ServicosComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  Cliente:any={};
  FuncionariosList:any=[];
  CatalogoList:any=[];
  Estabelecimento:any=[];
  EstabelecimentoSelected:any={};
  EstabelecimentoSelectedPrevious:any={};
  check:number = 0;
  NewServico:any={
    IdCliente:"",
    IdAnimal:"",
    IdFuncionario:"",
    IdServicoCatalogo:"",
    DataServico:"",
    Descricao:"",
    Estado:"",
    MetodoPagamento:""
  }
  data:any="";
  hora:any="";
  idAnimal:any="";
  idFuncionario:any="";
  idServicoCat:any="";
  success:number=0;

  constructor(private service: SharedService ) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.listaServicosEstabelecimento();
    this.getClienteByToken();
    this.refreshAnimalCliente();
    this.refreshNotificacoes();
  }

  GetFuncionariosByEstabelecimento(idEstabelecimento:any){
    this.service.getFuncionariosByEstabelecimento(idEstabelecimento).subscribe(data=>{
      this.FuncionariosList = data;
    })
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  MarcarServico(){
    this.NewServico.IdEstabelecimento = this.EstabelecimentoSelected.IdEstabelecimento
    this.NewServico.IdCliente = this.Cliente.IdCliente

    var splits = this.idAnimal.split(" - #")
    this.NewServico.IdAnimal = splits[splits.length-1]

    this.NewServico.DataServico = `${this.data}T${this.hora}`
    this.NewServico.Estado = "Marcado"

    var splits2 = this.idFuncionario.split(" - #")
    this.NewServico.IdFuncionario = splits2[splits2.length-1]

    var splits3 = this.idServicoCat.split(" - #")
    this.NewServico.IdServicoCatalogo = splits3[splits3.length-1]

    this.NewServico.Descricao = splits3[0]

    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.marcarServico(this.Cliente.IdCliente, this.NewServico).subscribe(data => {
        if(data == null) alert("Data indisponível para este funcionário, tente noutra altura!")
        else this.success = 1;
      });
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  refreshAnimalCliente(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getAnimalClienteID(this.Cliente.IdCliente).subscribe(data => {
        this.AnimaisList=data;
      })
    })
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  listaServicosEstabelecimento(){
    this.service.getAllEstabelecimentos().subscribe(data =>{
      this.Estabelecimento = data;
    });
  }

  listaCatalogoEstabelecimento(){
      this.service.getCatalogoByIdEstabelecimento(this.EstabelecimentoSelected.IdEstabelecimento).subscribe(data=> {
        this.CatalogoList = data;
    })
  }

  selectAnimal(estabelecimento:any={}){
    this.EstabelecimentoSelectedPrevious={
      IdEstabelecimento: `${this.EstabelecimentoSelected.IdEstabelecimento}`,
      Nome: `${this.EstabelecimentoSelected.Nome}`,
      TipoEstabelecimento: `${this.EstabelecimentoSelected.TipoEstabelecimento}`,
      Estado: `${this.EstabelecimentoSelected.Estado}`,
      Contacto: `${this.EstabelecimentoSelected.Contacto}`,
      AvaliacaoMedia: `${this.EstabelecimentoSelected.AvaliacaoMedia}`,
      EstabelecimentoFoto: `${this.EstabelecimentoSelected.EstabelecimentoFoto}`
    }

    this.EstabelecimentoSelected={
      IdEstabelecimento: `${estabelecimento.IdEstabelecimento}`,
      Nome: `${estabelecimento.Nome}`,
      TipoEstabelecimento: `${estabelecimento.TipoEstabelecimento}`,
      Estado: `${estabelecimento.Estado}`,
      Contacto: `${estabelecimento.Contacto}`,
      AvaliacaoMedia: `${estabelecimento.AvaliacaoMedia}`,
      EstabelecimentoFoto: `${estabelecimento.EstabelecimentoFoto}`
    }

    this.check = 1;
  }
}
