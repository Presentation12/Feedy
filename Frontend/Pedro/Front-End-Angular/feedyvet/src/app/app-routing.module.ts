import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AjudaComponent } from './cliente/ajuda/ajuda.component';
import { ClienteComponent } from './cliente/cliente.component';
import { EncomendasComponent } from './cliente/encomendas/encomendas.component';
import { GeriranimaisComponent } from './cliente/geriranimais/geriranimais.component';
import { PagamentosComponent } from './cliente/pagamentos/pagamentos.component';
import { GerenteComponent } from './gerente/gerente.component';
import { LembretesComponent } from './cliente/geriranimais/lembretes/lembretes.component';
import { AddlembreteComponent } from './cliente/geriranimais/lembretes/addlembrete/addlembrete.component';

const routes: Routes = [
  {path : 'cliente', component:ClienteComponent},
  {path : 'cliente/geriranimais', component:GeriranimaisComponent},
  {path : 'cliente/pagamentos', component:PagamentosComponent},
  {path : 'cliente/encomendas', component:EncomendasComponent},
  {path : 'cliente/ajudacliente', component:AjudaComponent},
  {path : 'cliente/lembretes', component:LembretesComponent},
  {path : 'cliente/addlembrete', component:AddlembreteComponent},
  {path : 'gerente', component:GerenteComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
