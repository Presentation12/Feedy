import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ClienteComponent } from './cliente/cliente.component';
import { GerenteComponent } from './gerente/gerente.component';
import { GeriranimaisComponent } from './cliente/geriranimais/geriranimais.component';
import { PagamentosComponent } from './cliente/pagamentos/pagamentos.component';
import { EncomendasComponent } from './cliente/encomendas/encomendas.component';
import { GetApiService } from './get-api.service'
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AjudaComponent } from './cliente/ajuda/ajuda.component';
import { RouterModule } from '@angular/router';
import { LembretesComponent } from './cliente/geriranimais/lembretes/lembretes.component';
import { AddlembreteComponent } from './cliente/geriranimais/lembretes/addlembrete/addlembrete.component';

@NgModule({
  declarations: [
    AppComponent,
    ClienteComponent,
    GerenteComponent,
    GeriranimaisComponent,
    PagamentosComponent,
    EncomendasComponent,
    AjudaComponent,
    LembretesComponent,
    AddlembreteComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([{path : '', component: ClienteComponent}])
  ],
  providers: [GetApiService],
  bootstrap: [AppComponent]
})
export class AppModule { }
