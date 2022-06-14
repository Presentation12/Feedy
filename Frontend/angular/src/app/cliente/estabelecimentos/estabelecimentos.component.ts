import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

declare function filter(): any;

@Component({
  selector: 'app-estabelecimentos',
  templateUrl: './estabelecimentos.component.html',
  styleUrls: ['./estabelecimentos.component.css']
})
export class EstabelecimentosComponent implements OnInit {

  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  EstabelecimentoSelecionado: any = {};
  DetalheCheck: number = 0;
  ListaEstabelecimentos: any = [];
  ListaDistritos: any = [];
  Cliente: any = {};
  Comentario:any={
    IdCliente:"",
    IdEstabelecimento:"",
    Avaliacao:"",
    Texto:""
  };

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url: any) {
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/estabelecimentosJoao.js");
    this.refreshDadosCliente();
    this.refreshListaEstabelecimentos();
    this.refreshListaDistritos();
    this.changeTip();
  }

  logout(){
    localStorage.setItem('token', '');
  }

  changeTip() {
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }

  Filter() {
    filter();
  }

  Details(Est: any) {
    this.EstabelecimentoSelecionado = Est;
    this.DetalheCheck = 1;
  }

  Avaliar(Est: any) {
    this.EstabelecimentoSelecionado = Est;
    this.DetalheCheck = 2;
  }

  Ok() {
    this.DetalheCheck = 0;
  }

  SubmitComment() {
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data
      this.Comentario.IdCliente = this.Cliente.IdCliente
      this.Comentario.IdEstabelecimento = this.EstabelecimentoSelecionado.IdEstabelecimento

      this.service.avaliarEstabelecimento(this.Comentario).subscribe()


      this.DetalheCheck = 0;
    })
  }

  refreshListaEstabelecimentos() {
    this.service.getListaEstabelecimentos().subscribe(data => {
      this.ListaEstabelecimentos = data;
    });
  }

  refreshListaDistritos() {
    this.service.getDistritos().subscribe(data => {
      this.ListaDistritos = data;
    });
  }

  refreshDadosCliente() {
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
    })
  }

}
