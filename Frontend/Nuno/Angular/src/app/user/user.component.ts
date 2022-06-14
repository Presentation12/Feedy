import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';

@Component({
  selector: 'app-cliente',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})

export class UserComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  LembretesList:any=[];
  Cliente:any={};
  NextServico:any={};
  animal:any;

  constructor(private service: SharedService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(){
    this.loadScript("../assets/programlimiter.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.changeTip();
    this.refreshNotificacoes();
    this.getClienteById();
    this.refreshLembreteCliente();
    this.refreshNextService();
    this.refreshAnimalCliente();
  }

  getClienteById(){
    this.service.getClientebyID().subscribe(data => {
      this.Cliente=data;
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

  adicionarAnimalTeste(){
    let date: Date = new Date();

    this.animal={
      Nome: "TesteAngular",
      Peso: "0",
      Altura: "0",
      Classe: "Teste",
      Especie: "Teste",
      Genero: "Teste",
      AnimalFoto: "assets/guaxinim.jpg",
      DataNascimento: date.toJSON()
    }

    this.service.addAnimal(this.animal).subscribe();
  }

  deleteAnimal(animal:any){
    if(confirm('Tem a certeza que pretende apagar o animal?'))
      this.service.removeAnimal(animal.IdAnimal).subscribe()
  }

  deleteLembrete(lembrete:any){
    if(confirm('Tem a certeza que pretende apagar o lembrete?'))
      this.service.removeLembrete(lembrete.IdLembrete).subscribe(data => {
    })
  }

  refreshLembreteCliente(){
    this.service.getLembretesList().subscribe(data => {
      this.LembretesList=data;
    })
  }

  refreshNextService(){
    this.service.getNextServico().subscribe(data => {
      this.NextServico=data;
    })
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.service.changeTip(this.tipText);
      this.service.changeTipLi(this.selected);
    }, 5000);
  }
}