import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { Ng2SearchPipeModule } from 'ng2-search-filter';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
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

import { SharedService } from 'src/app/shared-service.service'
import { ScriptsService } from 'src/app/scripts.service';
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

@NgModule({
  declarations: [
    AppComponent,
    PaginaInicialComponent,
    ClienteComponent,
    FuncionarioComponent,
    GeriranimaisComponent,
    ServicosComponent,
    PerfilComponent,
    PagamentoComponent,
    EncomendasComponent,
    LembretesComponent,
    HistoricoServicosComponent,
    LoginComponent,
    GerenteComponent,
    GerirfuncionariosComponent,
    PrescricoesComponent,
    AjudaClienteComponent,
    LoginclienteComponent,
    AjudafuncionarioComponent,
    AnimaisfuncionarioComponent,
    PerfilfuncionarioComponent,
    AnimaisgerenteComponent,
    ServicosgerenteComponent,
    AjudagerenteComponent,
    PerfilgerenteComponent,
    AddanimalComponent,
    AddlembreteComponent,
    LojaComponent,
    EstabelecimentosComponent,
    AdesaoComponent,
    FuncionamentoComponent,
    ParceriasComponent,
    SaberMaisComponent,
    VantagensComponent,
    FaltarComponent,
    FaltarpedidoComponent,
    AvaliacoesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    Ng2SearchPipeModule,
    RouterModule.forRoot([{path : '', component: PaginaInicialComponent}])
  ],
  providers: [SharedService, ScriptsService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
