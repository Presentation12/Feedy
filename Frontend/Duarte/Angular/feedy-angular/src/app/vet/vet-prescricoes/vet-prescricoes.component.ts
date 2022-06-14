import { Component, OnInit, Input } from '@angular/core';
import { AnimaisService } from 'src/app/requests/requests.service';
import { MainService } from 'src/app/scripts/main.service';

@Component({
  selector: 'app-vet-prescricoes',
  templateUrl: './vet-prescricoes.component.html',
  styleUrls: ['./vet-prescricoes.component.css']
})
export class VetPrescricoesComponent implements OnInit {


  logoSrc = "../assets/logo/roxo.svg";
  arrowSrc = "../assets/img/left-svgrepo-com.svg";
  doctorSrc = "../assets/img/usman-yousaf-pTrhfmj2jDA-unsplash.jpg";
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";

  selected = ["selected", "", ""];


  veterinario:any;

  prescricoes:any[]=[];

  @Input() filterText: any;

  constructor(private mainService: MainService,
              private requestsService: AnimaisService) { }

  ngOnInit(): void {
    this.changeTip();

    // GET PRESCRICOES
    this.requestsService.readPrescricoes().subscribe(prescricoes =>{
      this.prescricoes = prescricoes;
      // GET TRATAMENTO PRESCRICAO
      this.prescricoes.forEach(prescricao => {
        this.requestsService.readTratamentoByPrescricao(prescricao.IdPrescricao).subscribe(tratamento => {
          // GET STOCK
          if (tratamento.IdStock != null){
            this.requestsService.readStockById(tratamento.IdStock).subscribe(stock => {
              console.log(stock);
              prescricao.Tipo = stock.TipoStock;
              prescricao.NomeMedicamento = stock.Nome;
              prescricao.QuantidadeStock = tratamento.Quantidade;
            })
          }
        })
        this.requestsService.readServicoByIdPrescricao(prescricao.IdPrescricao).subscribe(servico => {
          prescricao.IdServico = servico.IdServico;
          this.requestsService.readEstabelecimentoByIdServico(servico.IdServico).subscribe(estabelecimento => {
            prescricao.NomeEstabelecimento = estabelecimento.Nome;
          })
          this.requestsService.readClienteByIdServico(servico.IdServico).subscribe(cliente => {
            prescricao.NomeCliente = cliente.Nome + " " + cliente.Apelido;
          })
        })
      })
      this.prescricoes.forEach(prescricao => {
        if (prescricao.Tipo == null){
          prescricao.Tipo = "Indefinido";
          prescricao.NomeMedicamento = "Indefinido";
          prescricao.QuantidadeStock = "Indefinido";
        }
      })
      console.log(prescricoes);
      
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
