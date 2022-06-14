import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-gerir-animais-historico',
  templateUrl: './gerir-animais-historico.component.html',
  styleUrls: ['./gerir-animais-historico.component.css']
})
export class GerirAnimaisHistoricoComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  Cliente:any={};
  SelectedAnimal:any;
  check:number=0;
  edicaoAtive:number=0;
  fotoFileName:string="";
  fotoFilePath:string="";
  ServicoAnimal:any="";
  ListServicos:any=[];


  constructor(private service: SharedService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../assets/programlimiter.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js"); 
    this.changeTip();
    this.getClienteById();
    this.refreshAnimalCliente();
    this.selectAnimalRefresh();
    this.refreshNotificacoes();
   

    
  }

  listaServicosAnimal(){
    console.log(this.SelectedAnimal);
      this.service.getServicosByIdAnimal(this.SelectedAnimal.IdAnimal).subscribe(data =>{
        this.ListServicos = data;
      })
  

  }

  refreshAnimalCliente(){
    this.service.getAnimalCliente().subscribe(data => {
      this.AnimaisList=data;
    })
  }


  refreshNotificacoes(){
    this.service.getNotificacoes().subscribe(data => {
      this.NotificacoesList=data;
    })
  }

  getClienteById(){
    this.service.getClientebyID().subscribe(data => {
      this.Cliente=data;
    })
  }

  selectAnimal(animal:any={}){
    this.SelectedAnimal={
      IdAnimal: `${animal.IdAnimal}`,
      Nome: `${animal.Nome}`,
      Peso: `${animal.Peso}`,
      Altura: `${animal.Altura}`,
      Classe: `${animal.Classe}`,
      Especie: `${animal.Especie}`,
      Estado: `${animal.Estado}`,
      Genero: `${animal.Classe}`,
      AnimalFoto: `${animal.AnimalFoto}`,
      DataNascimento: `${animal.DataNascimento}`
    }
    this.check = 1;
  }

  selectAnimalRefresh(){
    this.SelectedAnimal={
      Nome: "Teste",
      Peso: "Teste",
      Altura: "Teste",
      Classe: "Teste",
      Especie: "Teste",
      Genero: "Teste",
      AnimalFoto: "assets/avatar.png",
      DataNascimento: "Teste"
    }
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.service.changeTip(this.tipText);
      this.service.changeTipLi(this.selected);
    }, 5000);
  }


}
