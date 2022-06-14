import { Component, OnInit, Input } from '@angular/core';
import { AnimaisService } from 'src/app/requests/requests.service';
import { MainService } from 'src/app/scripts/main.service';

@Component({
  selector: 'app-ger-historico',
  templateUrl: './ger-historico.component.html',
  styleUrls: ['./ger-historico.component.css']
})
export class GerHistoricoComponent implements OnInit {

  logoSrc = "../assets/logo/roxo.svg";
  arrowSrc = "../assets/img/left-svgrepo-com.svg";
  doctorSrc = "../assets/img/usman-yousaf-pTrhfmj2jDA-unsplash.jpg";
  animalSrc = "../assets/img/pexels-pixabay-45201.jpg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  funcionario: any;

  servicos:any[] = [];

  servicoEdit: any;

  editAtivo: number = 0;

  selected = ["selected", "", ""];

  @Input() filterText: any;

  constructor(private mainService: MainService,
    private requestsService: AnimaisService) { }

  ngOnInit(): void {
    this.changeTip();
    this.requestsService.readVeterinario().subscribe(funcionario => {
      console.log(funcionario);
      this.funcionario = funcionario;
      this.requestsService.readHistoricoFuncionario(funcionario.IdFuncionario).subscribe(servicos => {
        servicos.forEach(servico => {
          this.requestsService.getAnimalById(servico.IdAnimal).subscribe(animal => {
            servico.NomeAnimal = animal.Nome;
          })
          this.requestsService.readDetailsServico(servico.IdServico).subscribe(details => {
            servico.TipoServico = details[0].TipoServico;
          })
        });
        this.servicos = servicos;
        console.log(this.servicos);
      })
    })

    
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.mainService.changeTip(this.tipText);
      this.mainService.changeTipLi(this.selected);
    }, 10000);

  }

  editar(servico:any){
    this.servicoEdit = servico;
    this.changeEditarStatus(1);
  }

  changeEditarStatus(active:number){
    this.editAtivo = active;
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
