import { Component, OnInit, Input} from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-animaisfuncionario',
  templateUrl: './animaisfuncionario.component.html',
  styleUrls: ['./animaisfuncionario.component.css']
})
export class AnimaisfuncionarioComponent implements OnInit {

  logoSrc = "../../assets/images/logo.svg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  selected = ["selected", "", ""];

  animais:any[]=[];

  veterinario:any={Nome:""};

  @Input() filterText: any;

  constructor(private requestsService: SharedService,
              private mainService: ScriptsService) { }

  ngOnInit(): void {
    this.changeTip();

    // ERROR HANDLING ?
    // EMPTY FIELDS HANDLING ?

    // GET ALL
    this.requestsService.readanimalsbyfuncionario().subscribe( animais => {
      this.animais = animais;
      //console.log("=>", animais);
      this.animais.forEach(animal => {
        this.requestsService.readClienteByIdAnimal(animal.IdAnimal).subscribe( dono => {
          if (dono.Nome == null){
            animal.NomeDono = "Sem dono";
            animal.EmailDono = "Sem dono";
          }
          else {
            animal.NomeDono = dono.Nome + " " + dono.Apelido;
            animal.EmailDono = dono.Email;
          }

        })
      });
    })

    this.requestsService.readVeterinario().subscribe(veterinario => {
      this.veterinario = veterinario;
    })
  }

  logout(){
    localStorage.setItem('token', '');
  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.mainService.changeTip(this.tipText);
      this.mainService.changeTipLi(this.selected);
    }, 10000);
  }

}
