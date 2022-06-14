import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-servicosgerente',
  templateUrl: './servicosgerente.component.html',
  styleUrls: ['./servicosgerente.component.css']
})
export class ServicosgerenteComponent implements OnInit {

  logoSrc = "../../assets/images/logo.svg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  funcionario: any={
    Nome:""
  };

  servicos:any[] = [];

  servicoEdit: any;

  editAtivo: number = 0;

  selected = ["selected", "", ""];

  @Input() filterText: any;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  ngOnInit(): void {
    this.changeTip();
    this.service.readVeterinario().subscribe(funcionario => {
      console.log(funcionario);
      this.funcionario = funcionario;
      this.service.readHistoricoFuncionario(funcionario.IdFuncionario).subscribe(servicos => {
        servicos.forEach(servico => {
          this.service.getAnimalById(servico.IdAnimal).subscribe(animal => {
            servico.NomeAnimal = animal.Nome;
            servico.AnimalFoto = animal.AnimalFoto;
          })
          this.service.readDetailsServico(servico.IdServico).subscribe(details => {
            servico.TipoServico = details[0].TipoServico;
          })
        });
        this.servicos = servicos;
      })
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
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
      this.service.rescheduleServico(this.servicoEdit).subscribe();
    }
    if (evento.target.Estado.value != this.servicoEdit.Estado){
      this.servicoEdit.Estado = evento.target.Estado.value;
      this.service.changeStatusServico(this.servicoEdit).subscribe();
    }
    this.changeEditarStatus(0);
  }

}
