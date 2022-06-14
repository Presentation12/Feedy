import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';

@Component({
  selector: 'app-avaliacoes',
  templateUrl: './avaliacoes.component.html',
  styleUrls: ['./avaliacoes.component.css']
})
export class AvaliacoesComponent implements OnInit {

  Gerente: any = {};
  Estabelecimento: any;
  ListAvaliacoes: any = [];

  constructor(private service: SharedService) { }

  refreshDadosGerente() {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;
    })
  }

  refreshAvaliacoes() {
    this.service.getFuncionarioByToken().subscribe(data => {
      this.Gerente = data;

      this.service.getEstabelecimentoByGerente(this.Gerente.IdFuncionario).subscribe(data => {

        this.Estabelecimento = data;

        this.service.getAvaliacoesEstabelecimento(this.Estabelecimento.IdEstabelecimento).subscribe(data => {
          this.ListAvaliacoes = data;
        });
      })
    })
  }

  logout() {
    localStorage.setItem('token', '');
  }

  ngOnInit(): void {
    this.refreshDadosGerente();
    this.refreshAvaliacoes();
  }

}
