import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';


@Component({
  selector: 'app-addanimal',
  templateUrl: './addanimal.component.html',
  styleUrls: ['./addanimal.component.css']
})
export class AddanimalComponent implements OnInit {
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  NotificacoesList:any=[];
  LembretesList:any=[];
  Cliente:any={};
  animal:any={
      Nome: "",
      Peso: "",
      Altura: "",
      Classe: "",
      Especie: "",
      Genero: "",
      AnimalFoto: "https://localhost:5001/images/avatar_cao.png",
      DataNascimento: "",
  };
  fotoFileName:any;
  addProcess:number = 1;
  canAdd:number = 0;

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/programlimiterPedro.js");
    this.loadScript("https://unpkg.com/sweetalert/dist/sweetalert.min.js");
    this.getClienteByToken();
    this.changeTip();
    this.refreshNotificacoes();
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  getClienteByToken(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente=data;
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  uploadPhoto(event:any){
    var file = event.target.files[0];
    const formData:FormData=new FormData();
    formData.append("uploadedFile", file,file.name)

    this.service.UploadPhoto(formData).subscribe(data => {
      this.fotoFileName = data.toString();
      this.animal.AnimalFoto = this.service.APIUrlPhotos+this.fotoFileName;
    })
  }

  refreshNotificacoes(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.service.getNotificacoes(this.Cliente.IdCliente).subscribe(data => {
        this.NotificacoesList=data;
      })
    })
  }

  adicionarAnimal(){
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
      this.addProcess = 0;
      if(!this.animal.Peso) this.animal.Peso = 0;
      if(!this.animal.Altura) this.animal.Altura = 0;
      if(!this.animal.Especie) this.animal.Especie = "Não indicado";
      if(!this.animal.Classe) this.animal.Classe = "Não indicado";

      this.service.addAnimal(this.animal, this.Cliente.IdCliente).subscribe();
    })
  }

  checkInfo(info:any){
    if(info.Nome && info.Genero && info.DataNascimento) this.canAdd = 1;
    else this.canAdd = 0;
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }
}
