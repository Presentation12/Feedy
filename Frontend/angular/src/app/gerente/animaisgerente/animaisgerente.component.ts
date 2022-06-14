import { Component, OnInit, Input } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-animaisgerente',
  templateUrl: './animaisgerente.component.html',
  styleUrls: ['./animaisgerente.component.css']
})
export class AnimaisgerenteComponent implements OnInit {

  logoSrc = "../../assets/images/logo.svg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  funcionario: any={
    Nome:"",
  };

  animais:any[] = [];

  selected = ["selected", "", ""];

  animalEdit: any;

  editAtivo:number = 0;

  @Input() filterText: any;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  ngOnInit(): void {
    this.changeTip();

    this.service.readVeterinario().subscribe(funcionario => {
      this.funcionario = funcionario;
    })
    this.service.readanimalsbyfuncionario().subscribe(animais => {
      animais.forEach(animal => {
        this.service.readClienteByIdAnimal(animal.IdAnimal).subscribe(cliente => {
          animal.NomeCliente = cliente.Nome + " " + cliente.Apelido;
          animal.EmailCliente = cliente.Email;
        })
      });
      this.animais = animais;
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }

  editar(animal:any){
    this.animalEdit = animal;
    console.log(animal);
    this.changeEditarStatus(1);
  }


  changeEditarStatus(active:number){
    this.editAtivo = active;
  }

  updateAnimal(evento:any){
    this.animalEdit.Nome = evento.target.Nome.value;
    this.animalEdit.Altura = evento.target.Altura.value;
    this.animalEdit.Peso = evento.target.Peso.value;
    this.animalEdit.Classe = evento.target.Classe.value;
    this.animalEdit.DataNascimento = evento.target.DataNascimento.value;
    this.animalEdit.Especie = evento.target.Especie.value;
    this.animalEdit.Estado = evento.target.Estado.value;
    this.animalEdit.Genero = evento.target.Genero.value;
    this.service.updateAnimal(this.animalEdit).subscribe();
    this.changeEditarStatus(0);
  }

}
