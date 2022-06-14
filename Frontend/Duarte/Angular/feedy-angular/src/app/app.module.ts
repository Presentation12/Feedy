import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { BrowserModule } from '@angular/platform-browser';
import { Ng2SearchPipeModule } from 'ng2-search-filter';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { VetAnimaisComponent } from './vet/vet-animais/vet-animais.component';
import { VetPrescricoesComponent } from './vet/vet-prescricoes/vet-prescricoes.component';
import { VetPortalComponent } from './vet/vet-portal/vet-portal.component';
import { GerHistoricoComponent } from './ger/ger-historico/ger-historico.component';
import { GerAnimaisComponent } from './ger/ger-animais/ger-animais.component';

@NgModule({
  declarations: [
    AppComponent,
    VetAnimaisComponent,
    VetPrescricoesComponent,
    VetPortalComponent,
    GerHistoricoComponent,
    GerAnimaisComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    Ng2SearchPipeModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
