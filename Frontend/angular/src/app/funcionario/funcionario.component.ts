import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-funcionario',
  templateUrl: './funcionario.component.html',
  styleUrls: ['./funcionario.component.css']
})
export class FuncionarioComponent implements OnInit {

  logoSrc = "../../assets/images/logo.svg";
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

  veterinario:any={Nome:""};


  constructor(private scripts: ScriptsService, private service: SharedService) { }

  ngOnInit(): void {
    this.changeTip();

    this.service.readVeterinario().subscribe(veterinario => {
      this.veterinario = veterinario;
      this.service.readPersonalServicesMissing(veterinario.IdFuncionario).subscribe(servicos => {
        servicos.forEach(servico => {
          this.service.getClienteById(servico.IdCliente).subscribe(cliente => {
            servico.TelemovelCliente = cliente.Telemovel;
            servico.EmailCliente = cliente.Email;
            servico.NomeCliente = cliente.Nome + " " + cliente.Apelido;
          })
          this.service.getAnimalById(servico.IdAnimal).subscribe(animal => {
            servico.NomeAnimal = animal.Nome;
            servico.AnimalFoto = animal.AnimalFoto;
          })
        });
        this.servicos = servicos;
      })
      this.service.readPersonalServicesDone(veterinario.IdFuncionario).subscribe(servicos => {
        this.servicosPastNumber = servicos.length;
      })
      this.service.readHistoricoFuncionario(veterinario.IdFuncionario).subscribe(servicos => {
        let contador = 0;
        servicos.forEach(servico => {
          if (contador < 4){
            this.service.getAnimalById(servico.IdAnimal).subscribe(animal => {
              servico.NomeAnimal = animal.Nome;
              servico.AnimalFoto = animal.AnimalFoto;
            })
            this.service.readDetailsServico(servico.IdServico).subscribe(details => {
              servico.TipoServico = details[0].TipoServico;

            })
            this.servicosHistorico.push(servico);
          }
          contador++;
        });
      })
    })
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 10000);
  }

  logout(){
    localStorage.setItem('token', '')
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
      this.service.rescheduleServico(this.servicoEdit).subscribe();
    }
    if (evento.target.Estado.value != this.servicoEdit.Estado){
      this.servicoEdit.Estado = evento.target.Estado.value;
      this.service.changeStatusServico(this.servicoEdit).subscribe();
    }
    this.changeEditarStatus(0);
  }

}
