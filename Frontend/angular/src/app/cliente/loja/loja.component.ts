import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';

declare function filter(): any;
declare function submit(): any;

@Component({
  selector: 'app-loja',
  templateUrl: './loja.component.html',
  styleUrls: ['./loja.component.css']
})
export class LojaComponent implements OnInit {

  ArtigoSelecionado: any = {};
  DetalheCheck: number = 0;
  PayCheck: number = 0;
  tipText = "Deve lavar o seu cão, pelo menos, uma vez por mês.";
  selected = ["selected", "", ""];
  Cliente: any = {};
  ListaEstabelecimentos: any = [];
  ListaArtigos: any = [];
  Carrinho: any = [];
  ArtigoCarrinho: any = {};
  Qtd: any;
  PrecoFinal: any = 0;
  Encomenda: any;
  MetodoPagamento: any;
  EncomendaStock: any = {};
  Mensagem:any = 0;

  constructor(private scripts: ScriptsService, private service: SharedService) { }

  public loadScript(url: any) {
    let node = document.createElement("script");
    node.src = url;
    node.type = 'text/javascript';
    document.body.append(node);
  }

  ngOnInit(): void {
    this.loadScript("../../assets/scripts/loja.js");
    this.changeTip();
    this.refreshDadosCliente();
    this.refreshListaEstabelecimentos();
    this.refreshArtigos();
  }

  logout(){
    localStorage.setItem('token', '');
  }

  buyCarrinho()
  {
    this.Encomenda={
      MetodoPagamento: `${this.MetodoPagamento}`,
      IdMorada: `${this.Cliente.IdMorada}`,
      IdCliente: `${this.Cliente.IdCliente}`
    }
    this.service.postEncomenda(this.Encomenda).subscribe(data=>{
      this.EncomendaStock = data;

      this.Carrinho.forEach((artigo: any) => {
        artigo.IdEncomenda = this.EncomendaStock.IdEncomenda;
        this.service.postEncomendaStock(artigo).subscribe();
      });

    });
    this.aceite();
  }

  aceite()
  {
    this.PayCheck = 0;
    this.Mensagem = 1;
  }

  OkAceite()
  {
    this.Mensagem = 0;
    submit();
  }

  add(artigo : any)
  {
    artigo.Qtd++;
    this.PrecoFinal += +artigo.Preco;
  }

  cancel()
  {
    this.PrecoFinal = 0;
    this.Carrinho = [];
    this.Ok();
  }

  refreshCarrinho() {
    this.ArtigoCarrinho =
    {
      Nome: `${this.ArtigoSelecionado.Nome}`,
      Qtd: `${this.Qtd}`,
      Preco: `${this.ArtigoSelecionado.Preco}`,
      IdStock: `${this.ArtigoSelecionado.IdStock}`,
      IdEncomenda: ""
    }
    this.ArtigoSelecionado.Stock-=this.Qtd;
    this.Carrinho.push(this.ArtigoCarrinho);
    this.Ok();
    this.PrecoFinal += this.Qtd * this.ArtigoSelecionado.Preco;
    this.Qtd = 0;
  }

  refreshArtigos() {
    this.service.getArtigos().subscribe(data => {
      this.ListaArtigos = data;
    })
  }

  refreshListaEstabelecimentos() {
    this.service.getListaEstabelecimentos().subscribe(data => {
      this.ListaEstabelecimentos = data;
    });
  }

  refreshDadosCliente() {
    this.service.getClienteByToken().subscribe(data => {
      this.Cliente = data;
    })
  }

  changeTip() {
    setInterval(() => {
      this.tipText = this.scripts.changeTip(this.tipText);
      this.scripts.changeTipLi(this.selected);
    }, 5000);
  }

  Details(Art: any) {
    this.ArtigoSelecionado = Art;
    this.DetalheCheck = 1;
  }

  Pay()
  {
    this.PayCheck = 1;
  }

  Ok() {
    this.DetalheCheck = 0;
    this.PayCheck = 0
  }

  Filter() {
    filter();
  }

}
