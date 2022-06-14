import { formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AnimaisService } from 'src/app/requests/requests.service';
import { MainService } from 'src/app/scripts/main.service';

@Component({
  selector: 'app-vet-portal',
  templateUrl: './vet-portal.component.html',
  styleUrls: ['./vet-portal.component.css']
})
export class VetPortalComponent implements OnInit {

  logoSrc = "../assets/logo/roxo.svg";
  arrowSrc = "../assets/img/left-svgrepo-com.svg";
  doctorSrc = "../assets/img/usman-yousaf-pTrhfmj2jDA-unsplash.jpg";
  animalSrc = "../assets/img/pexels-pixabay-45201.jpg";

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  dateNow = new Date();
  dataFinal = this.formatarData(this.dateNow);

  selected = ["selected", "", ""];

  servicos:any[] = [];

  servicosHistorico: any[] = [];

  servicosPastNumber:number = 0;

  servicoEdit: any;

  editAtivo: number = 0;

  servicoDetail: any;

  detailAtivo: number = 0;

  veterinario:any;

  constructor(private mainService: MainService,
              private requestsService: AnimaisService) { }

  ngOnInit(): void {
    this.changeTip();

    this.requestsService.readVeterinario().subscribe(veterinario => {
      this.veterinario = veterinario;
      console.log(veterinario);
      this.requestsService.readPersonalServicesMissing(veterinario.IdFuncionario).subscribe(servicos => {        
        servicos.forEach(servico => {
          this.requestsService.getClienteById(servico.IdCliente).subscribe(cliente => {
            servico.TelemovelCliente = cliente.Telemovel;
            servico.EmailCliente = cliente.Email;
            servico.NomeCliente = cliente.Nome + " " + cliente.Apelido;
          })
          this.requestsService.getAnimalById(servico.IdAnimal).subscribe(animal => {
            servico.NomeAnimal = animal.Nome;
          })
        });
        this.servicos = servicos;
        console.log(this.servicos);
      })
      this.requestsService.readPersonalServicesDone(veterinario.IdFuncionario).subscribe(servicos => {
        this.servicosPastNumber = servicos.length;
      })
      this.requestsService.readHistoricoFuncionario(veterinario.IdFuncionario).subscribe(servicos => {
        let contador = 0;  
        servicos.forEach(servico => {
          if (contador < 4){
            this.requestsService.getAnimalById(servico.IdAnimal).subscribe(animal => {
              servico.NomeAnimal = animal.Nome;
              // foto animal
            })
            this.requestsService.readDetailsServico(servico.IdServico).subscribe(details => {
              servico.TipoServico = details[0].TipoServico;
              
            })
          }
          contador++;
        });
        this.servicosHistorico = servicos;
        console.log(this.servicosHistorico);
      })
    })

    
    
  }


  changeTip(){
    setInterval(() => {
      this.tipText = this.mainService.changeTip(this.tipText);
      this.mainService.changeTipLi(this.selected);
    }, 10000);
  }
  formatarData(data:Date){
    const meses = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
    let mes;
    mes = meses[data.getMonth()];
    return (`Hoje, ${data.getDate()} de ${mes} de ${data.getFullYear()}`)
  }
  editar(servico:any){
    this.servicoEdit = servico;
    this.changeEditarStatus(1);
  }
  detail(servico:any){
    this.servicoDetail = servico;
    console.log(this.servicoDetail);
    this.changeDetailStatus(1);
  }
  changeEditarStatus(active:number){
    this.editAtivo = active;
  }
  changeDetailStatus(active:number){
    this.detailAtivo = active;
  }
  updateServico(evento:any){
    if (evento.target.DataServico.value != this.servicoEdit.DataServico){
      this.servicoEdit.DataServico = evento.target.DataServico.value;
      this.requestsService.rescheduleServico(this.servicoEdit).subscribe();
    }
    if (evento.target.Estado.value != this.servicoEdit.Estado){
      this.servicoEdit.Estado = evento.target.Estado.value;
      this.requestsService.changeStatusServico(this.servicoEdit).subscribe();
    }
    this.changeEditarStatus(0);
  }
}


