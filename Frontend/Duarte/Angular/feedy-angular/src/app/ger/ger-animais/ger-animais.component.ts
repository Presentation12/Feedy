import { Component, OnInit, Input } from '@angular/core';
import { AnimaisService } from 'src/app/requests/requests.service';
import { MainService } from 'src/app/scripts/main.service';

@Component({
  selector: 'app-ger-animais',
  templateUrl: './ger-animais.component.html',
  styleUrls: ['./ger-animais.component.css']
})
export class GerAnimaisComponent implements OnInit {

  logoSrc = "../assets/logo/roxo.svg";
  arrowSrc = "../assets/img/left-svgrepo-com.svg";
  doctorSrc = "../assets/img/usman-yousaf-pTrhfmj2jDA-unsplash.jpg";
  animalSrc = "../assets/img/pexels-pixabay-45201.jpg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  funcionario: any;

  animais:any[] = [];

  selected = ["selected", "", ""];

  animalEdit: any;
  
  editAtivo:number = 0;

  @Input() filterText: any;


  constructor(private mainService: MainService,
    private requestsService: AnimaisService) { }

  ngOnInit(): void {
    this.changeTip();

    this.requestsService.readVeterinario().subscribe(funcionario => {
      console.log(funcionario);
      this.funcionario = funcionario;
    })
    this.requestsService.readanimalsbyfuncionario().subscribe(animais => {
      animais.forEach(animal => {
        this.requestsService.readClienteByIdAnimal(animal.IdAnimal).subscribe(cliente => {
          animal.NomeCliente = cliente.Nome + " " + cliente.Apelido;
          animal.EmailCliente = cliente.Email;
        })
      });
      this.animais = animais;
      console.log(this.animais);
    })
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.mainService.changeTip(this.tipText);
      this.mainService.changeTipLi(this.selected);
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
    this.requestsService.updateAnimal(this.animalEdit).subscribe();
    this.changeEditarStatus(0);
  }
}
