import { Component, OnInit, Input} from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';
import { ScriptsService } from 'src/app/scripts.service';

@Component({
  selector: 'app-prescricoes',
  templateUrl: './prescricoes.component.html',
  styleUrls: ['./prescricoes.component.css']
})
export class PrescricoesComponent implements OnInit {

  logoSrc = "../../assets/images/logo.svg";

  veterinario:any={Nome:""};

  prescricoes:any[]=[];

  @Input() filterText: any;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  ngOnInit(): void {
    // GET PRESCRICOES
    this.service.readPrescricoes().subscribe(prescricoes =>{
      this.prescricoes = prescricoes;
      // GET TRATAMENTO PRESCRICAO
      this.prescricoes.forEach(prescricao => {
        this.service.readTratamentoByPrescricao(prescricao.IdPrescricao).subscribe(tratamento => {
          // GET STOCK
          if (tratamento.IdStock != null){
            this.service.readStockById(tratamento.IdStock).subscribe(stock => {
              console.log(stock);
              prescricao.Tipo = stock.TipoStock;
              prescricao.NomeMedicamento = stock.Nome;
              prescricao.QuantidadeStock = tratamento.Quantidade;
            })
          }
        })
        this.service.readServicoByIdPrescricao(prescricao.IdPrescricao).subscribe(servico => {
          prescricao.IdServico = servico.IdServico;
          this.service.readEstabelecimentoByIdServico(servico.IdServico).subscribe(estabelecimento => {
            prescricao.NomeEstabelecimento = estabelecimento.Nome;
          })
          this.service.readClienteByIdServico(servico.IdServico).subscribe(cliente => {
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


    this.service.readVeterinario().subscribe(veterinario => {
      this.veterinario = veterinario;
      console.log(veterinario);
    })
  }

  logout(){
    localStorage.setItem('token', '')
  }

}
