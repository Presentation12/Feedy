import { Component, ComponentFactoryResolver, OnInit, Input } from '@angular/core';
import { AnimaisService } from 'src/app/requests/requests.service';
import { MainService } from 'src/app/scripts/main.service';


@Component({
  selector: 'app-vet-animais',
  templateUrl: './vet-animais.component.html',
  styleUrls: ['./vet-animais.component.css']
})
export class VetAnimaisComponent implements OnInit {

  logoSrc = "../assets/logo/roxo.svg";
  arrowSrc = "../assets/img/left-svgrepo-com.svg";
  doctorSrc = "../assets/img/usman-yousaf-pTrhfmj2jDA-unsplash.jpg";
  animalSrc = "../assets/img/pexels-pixabay-45201.jpg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  selected = ["selected", "", ""];

  animais:any[]=[];

  veterinario:any;

  

  @Input() filterText: any;


  constructor(private mainService: MainService,
              private requestsService: AnimaisService) { }

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
          
          console.log(animal);
        })
      });
    })

    this.requestsService.readVeterinario().subscribe(veterinario => {
      this.veterinario = veterinario;
      console.log(veterinario);
    })

    

  }

  changeTip(){
    setInterval(() => {
      this.tipText = this.mainService.changeTip(this.tipText);
      this.mainService.changeTipLi(this.selected);
    }, 10000);
  }

}
