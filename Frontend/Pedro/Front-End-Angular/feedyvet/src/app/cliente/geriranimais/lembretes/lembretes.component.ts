import { Component, OnInit } from '@angular/core';
import { GetApiService } from 'src/app/get-api.service';

@Component({
  selector: 'app-lembretes',
  templateUrl: './lembretes.component.html',
  styleUrls: ['./lembretes.component.css']
})
export class LembretesComponent implements OnInit {

  conta:any= {};
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  Cliente:any={};
  SelectedAnimal:any;
  SelectedAnimalPrev:any;
  check:number=0;
  edicaoAtive:number=0;
  ListLembretesSelected:any=[];

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  constructor(private service: GetApiService) { }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.changeTip();
    this.getClienteByToken();
    this.refreshAnimalCliente();
    this.selectAnimalRefresh();
    this.refreshNotificacoes();
  }

  refreshAnimalCliente(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getAnimalClienteID(this.Cliente.IdCliente).subscribe(data => {
        this.AnimaisList=data;
      })
    })
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  login(): void {
    this.conta={
      Email: "ps@gmail.com",
      Pass: "ps"
    }
    this.service.loginCliente(this.conta).subscribe(data=>{
     if (data == "Cliente não existe! (Parameter 'account')") alert("Cliente não existe!")
     else if(data == "Password Errada. (Parameter 'account')") alert("Password Errada.")
     else{
       alert("Login com sucesso! (Conta hardcode, pois a página login não era minha função)")
       alert("Carregue no F5!")
       localStorage.setItem('token', data.toString());
     }
    });
   }

  logout(){
    alert("Saiu da conta com sucesso!");
    alert("Carregue no F5!");
    localStorage.setItem('token', '');
  }

  deleteAnimal(animal:any){
    if(confirm('Tem a certeza que pretende apagar o animal?'))
    this.service.removeAnimal(animal.IdAnimal).subscribe()
  }

  EditarAnimal(animal:any){
    this.edicaoAtive = 1;
  }

  CancelarEdicao(){
    this.SelectedAnimal = this.SelectedAnimalPrev;
    if(this.SelectedAnimal.Nome == "Teste") this.check = 0;
    this.edicaoAtive = 0;
  }

  patchAnimal(animal:any){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.patchAnimal(animal, this.Cliente.IdCliente).subscribe()
    })
    this.edicaoAtive = 0;
  }

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  GetLembreteSelected(animal:any){
    this.service.getLembretesListByAnimal(animal.IdAnimal).subscribe(data => {
      this.ListLembretesSelected = data;
    })
  }

  selectAnimal(animal:any={}){
    this.SelectedAnimalPrev={
      IdAnimal: `${this.SelectedAnimal.IdAnimal}`,
      Nome: `${this.SelectedAnimal.Nome}`,
      Peso: `${this.SelectedAnimal.Peso}`,
      Altura: `${this.SelectedAnimal.Altura}`,
      Classe: `${this.SelectedAnimal.Classe}`,
      Especie: `${this.SelectedAnimal.Especie}`,
      Estado: `${this.SelectedAnimal.Estado}`,
      Genero: `${this.SelectedAnimal.Genero}`,
      AnimalFoto: `${this.SelectedAnimal.AnimalFoto}`,
      DataNascimento: `${this.SelectedAnimal.DataNascimento}`

    }

    this.SelectedAnimal={
      IdAnimal: `${animal.IdAnimal}`,
      Nome: `${animal.Nome}`,
      Peso: `${animal.Peso}`,
      Altura: `${animal.Altura}`,
      Classe: `${animal.Classe}`,
      Especie: `${animal.Especie}`,
      Estado: `${animal.Estado}`,
      Genero: `${animal.Genero}`,
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
      AnimalFoto: "../../../assets/images/avatar.png",
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
