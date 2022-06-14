import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SharedService } from 'src/app/shared.service';

@Component({
  selector: 'app-marcar-servico',
  templateUrl: './marcar-servico.component.html',
  styleUrls: ['./marcar-servico.component.css']
})
export class MarcarServicoComponent implements OnInit {


  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  AnimaisList:any=[];
  NotificacoesList:any=[];
  ClinicaList:any=[];
  CatalogoList:any=[];
  Estabelecimento:any=[];


  constructor(private service: SharedService ) { }

  public loadScript(url : any){
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  servico=["Consulta","Vacinacão","Análises","Farmácia","Grooming"];
  distrito=["Lisboa","Evora","Porto","Beja","Braga"];
  clinica=["Feedy1","Feedy2","Feedy3","Feedy4"];
  animal=["Nikita", "Max", "Brito", "Pantufa"];
  data=[];

  selectedS: any = {};
  selectedD: string = '';
  selectedC: any = [];
  selectedA: string = '';



  ngOnInit(): void {
    this.listaServicosEstabelecimento();
    
  }
  

  
    listaServicosEstabelecimento(){
    this.service.getAllEstabelecimentos().subscribe(data =>{
      this.Estabelecimento = data;
      
      
    });
      
  }
  
  listaCatalogoEstabelecimento(){
    console.log(this.selectedC);
      this.service.getCatalogoByIdEstabelecimento(this.selectedC.IdEstabelecimento).subscribe(data=> {
        this.CatalogoList = data;
        console.log(this.selectedC);
      })
    
  }

  selecionarClinica(e:any){
    this.selectedC= e;
    console.log(this.selectedC);
  }


/*  this.service.getServicosByIdEstabelecimento().subscribe(data =>{
      this.ServicoList = data;
    })
   */

}