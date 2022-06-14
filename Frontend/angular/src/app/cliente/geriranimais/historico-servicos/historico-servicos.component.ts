import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-historico-servicos',
  templateUrl: './historico-servicos.component.html',
  styleUrls: ['./historico-servicos.component.css']
})
export class HistoricoServicosComponent implements OnInit {

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
  ServicoCancel:any={
    IdServico:""
  };
  ServicoRemarcar:any={
    IdServico:"",
    DataServico:"",
    IdFuncionario: "",
    IdAnimal: "",
    IdEstabelecimento: "",
    IdCliente: "",
    IdServicoCatalogo:""
  };
  canRemarcar:number = 0;
  remarcarbtn:number = 0;

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
    this.changeTip();
    this.getClienteByToken();
    this.refreshAnimalCliente();
    this.selectAnimalRefresh();
    this.refreshNotificacoes();
  }

  deleteNotificacao(not:any){
    if(confirm("Deseja apagar esta notificação?")) this.service.DeleteNotification(not).subscribe()
  }

  listaServicosAnimal(){
    console.log(this.SelectedAnimal);
      this.service.GetHistoricoAnimal(this.SelectedAnimal.IdAnimal).subscribe(data =>{
        this.ListServicos = data;
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

  logout(){
    localStorage.setItem('token', '');
  }

  CancelaServico(servico:any){
    console.log(servico)
    this.ServicoCancel.IdServico = servico.IdServico
    if(confirm("Tem a certeza que pretende cancelar o serviço?")) this.service.CancelarService(this.ServicoCancel).subscribe(
      data => alert("Serviço cancelado com sucesso!"),
      error => alert("Erro ao cancelar o serviço, por favor tente mais tarde")
    )
  }

  EnableReschedule(servico:any){
    this.ServicoRemarcar.IdServico = servico.IdServico;
    this.ServicoRemarcar.IdFuncionario = servico.IdFuncionario;
    this.ServicoRemarcar.IdAnimal = servico.IdAnimal;
    this.ServicoRemarcar.IdEstabelecimento = servico.IdEstabelecimento;
    this.ServicoRemarcar.IdServico = servico.IdServico;
    this.ServicoRemarcar.IdCliente = servico.IdCliente;
    this.ServicoRemarcar.IdServicoCatalogo = servico.IdServicoCatalogo;
    this.canRemarcar=1;
  }

  CheckData(){
    if(this.ServicoRemarcar.DataServico) this.remarcarbtn=1;
    else this.remarcarbtn=0;
  }

  RescheduleService(){
    console.log(this.ServicoRemarcar)
    this.service.rescheduleServico(this.ServicoRemarcar).subscribe(data => alert("Data enviada com sucesso! Caso o funcionário alterar a data, você será notificado"),
     error=>alert("Erro ao remarcar serviço (Data incompatível)"))
    this.canRemarcar=0;
  }

  Cancel(){
    this.canRemarcar=0;
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
      AnimalFoto: "../../../assets/images/avatar.png",
      DataNascimento: "Teste"
    }
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }
}
