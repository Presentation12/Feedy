import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PaginaInicialComponent } from './pagina-inicial/pagina-inicial.component';
import { ClienteComponent } from './cliente/cliente.component';
import { FuncionarioComponent } from './funcionario/funcionario.component';
import { GeriranimaisComponent } from './cliente/geriranimais/geriranimais.component';
import { ServicosComponent } from './cliente/servicos/servicos.component';
import { PerfilComponent } from './cliente/perfil/perfil.component';
import { PagamentoComponent } from './cliente/pagamento/pagamento.component';
import { EncomendasComponent } from './cliente/encomendas/encomendas.component';
import { LembretesComponent } from './cliente/geriranimais/lembretes/lembretes.component';
import { HistoricoServicosComponent } from './cliente/geriranimais/historico-servicos/historico-servicos.component';
import { LoginComponent } from './funcionario/login/login.component';
import { GerenteComponent } from './gerente/gerente.component';
import { GerirfuncionariosComponent } from './gerente/gerirfuncionarios/gerirfuncionarios.component';
import { PrescricoesComponent } from './funcionario/prescricoes/prescricoes.component';
import { AjudaClienteComponent } from './cliente/ajuda-cliente/ajuda-cliente.component';
import { LoginclienteComponent } from './cliente/logincliente/logincliente.component';
import { AjudafuncionarioComponent } from './funcionario/ajudafuncionario/ajudafuncionario.component';
import { AnimaisfuncionarioComponent } from './funcionario/animaisfuncionario/animaisfuncionario.component';
import { PerfilfuncionarioComponent } from './funcionario/perfilfuncionario/perfilfuncionario.component';
import { AnimaisgerenteComponent } from './gerente/animaisgerente/animaisgerente.component';
import { ServicosgerenteComponent } from './gerente/servicosgerente/servicosgerente.component';
import { AjudagerenteComponent } from './gerente/ajudagerente/ajudagerente.component';
import { PerfilgerenteComponent } from './gerente/perfilgerente/perfilgerente.component';
import { AddanimalComponent } from './cliente/geriranimais/addanimal/addanimal.component';
import { AddlembreteComponent } from './cliente/geriranimais/lembretes/addlembrete/addlembrete.component';
import { LojaComponent } from './cliente/loja/loja.component';
import { EstabelecimentosComponent } from './cliente/estabelecimentos/estabelecimentos.component';
import { AdesaoComponent } from './pagina-inicial/adesao/adesao.component';
import { FuncionamentoComponent } from './pagina-inicial/funcionamento/funcionamento.component';
import { ParceriasComponent } from './pagina-inicial/parcerias/parcerias.component';
import { SaberMaisComponent } from './pagina-inicial/saber-mais/saber-mais.component';
import { VantagensComponent } from './pagina-inicial/vantagens/vantagens.component';
import { FaltarComponent } from './gerente/faltar/faltar.component';
import { FaltarpedidoComponent } from './funcionario/faltarpedido/faltarpedido.component';
import { AvaliacoesComponent } from './gerente/avaliacoes/avaliacoes.component';

const routes: Routes = [
  {path: 'paginalinicial', component:PaginaInicialComponent},
  {path: 'paginalinicial/adesao', component:AdesaoComponent},
  {path: 'paginalinicial/funcionamento', component:FuncionamentoComponent},
  {path: 'paginalinicial/parcerias', component:ParceriasComponent},
  {path: 'paginalinicial/sabermais', component:SaberMaisComponent},
  {path: 'paginalinicial/vantagens', component:VantagensComponent},
  {path : 'cliente/login', component:LoginclienteComponent},
  {path : 'cliente', component:ClienteComponent},
  {path : 'cliente/geriranimais', component:GeriranimaisComponent},
  {path : 'cliente/pagamentos', component:PagamentoComponent},
  {path : 'cliente/encomendas', component:EncomendasComponent},
  {path : 'cliente/ajudacliente', component:AjudaClienteComponent},
  {path : 'cliente/perfil', component:PerfilComponent},
  {path : 'cliente/loja', component:LojaComponent},
  {path : 'cliente/servicos', component:ServicosComponent},
  {path : 'cliente/estabelecimentos', component:EstabelecimentosComponent},
  {path : 'cliente/lembretes', component:LembretesComponent},
  {path : 'cliente/addlembrete', component:AddlembreteComponent},
  {path : 'cliente/historicoservicos', component:HistoricoServicosComponent},
  {path : 'cliente/addanimal', component:AddanimalComponent},
  {path : 'funcionario',component:FuncionarioComponent},
  {path : 'funcionario/login',component:LoginComponent},
  {path : 'gerente', component:GerenteComponent},
  {path : 'gerente/gerirfuncionarios', component:GerirfuncionariosComponent},
  {path : 'gerente/veranimais', component:AnimaisgerenteComponent},
  {path : 'funcionario/prescricoes', component:PrescricoesComponent},
  {path : 'funcionario/ajuda', component:AjudafuncionarioComponent},
  {path : 'funcionario/veranimais', component:AnimaisfuncionarioComponent},
  {path : 'funcionario/perfil', component:PerfilfuncionarioComponent},
  {path : 'gerente/servicos', component:ServicosgerenteComponent},
  {path : 'gerente/ajuda', component:AjudagerenteComponent},
  {path : 'gerente/perfil', component:PerfilgerenteComponent},
  {path : 'gerente/faltar', component:FaltarComponent},
  {path : 'funcionario/faltar', component:FaltarpedidoComponent},
  {path : 'gerente/avaliacoes', component:AvaliacoesComponent}
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
