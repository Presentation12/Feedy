import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-addlembrete',
  templateUrl: './addlembrete.component.html',
  styleUrls: ['./addlembrete.component.css']
})
export class AddlembreteComponent implements OnInit {

  SelectedAnimal:any;
  SelectedAnimalPrev:any;
  AnimaisList:any=[];
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  NotificacoesList:any=[];
  LembretesList:any=[];
  Cliente:any={};
  lembrete:any={
      IdAnimal: "",
      LembreteDescricao: "",
      DataLembrete: "",
      HoraLembrete: "",
      Frequencia: "",
  };
  addProcess:number = 1;
  canAdd:number = 0;
  appear:number = 0;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.getClienteByToken();
    this.changeTip();
    this.refreshNotificacoes();
    this.refreshAnimalCliente();
    this.selectAnimalRefresh();
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  selectAnimalRefresh(){
    this.SelectedAnimal={
      IdAnimal: -1,
      Nome: "Teste",
      Peso: "Teste",
      Altura: "Teste",
      Classe: "Teste",
      Especie: "Teste",
      Genero: "Teste",
      AnimalFoto: "../../assets/images/avatar.png",
      DataNascimento: "Teste"
    }
  }

  logout(){
    localStorage.setItem('token', "");
  }

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  selectAnimal(animal:any={}){
    this.SelectedAnimalPrev={
      IdAnimal: `${this.SelectedAnimal.IdAnimal}`,
      Nome: `${this.SelectedAnimal.Nome}`
    }

    this.SelectedAnimal={
      IdAnimal: `${animal.IdAnimal}`,
      Nome: `${animal.Nome}`
    }

    this.appear = 1;
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  refreshAnimalCliente(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getAnimalClienteID(this.Cliente.IdCliente).subscribe(data => {
        this.AnimaisList=data;
      })
    })
  }

  AddLembrete(){
    this.lembrete.IdAnimal = this.SelectedAnimal.IdAnimal;
    console.log(this.lembrete);
    this.service.addlembrete(this.lembrete).subscribe();
    this.addProcess = 0;
  }

  checkInfo(info:any){
    if(info.LembreteDescricao && info.HoraLembrete && info.Frequencia) this.canAdd = 1;
    else this.canAdd = 0;
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }

}
