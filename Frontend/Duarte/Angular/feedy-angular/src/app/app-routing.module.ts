import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GerAnimaisComponent } from './ger/ger-animais/ger-animais.component';
import { GerHistoricoComponent } from './ger/ger-historico/ger-historico.component';
import { VetAnimaisComponent } from './vet/vet-animais/vet-animais.component';
import { VetPortalComponent } from './vet/vet-portal/vet-portal.component';
import { VetPrescricoesComponent } from './vet/vet-prescricoes/vet-prescricoes.component';

const routes: Routes = [
  {
    path: "vet-animais",
    component: VetAnimaisComponent
  },
  {
    path: "vet-prescricoes",
    component: VetPrescricoesComponent
  },
  {
    path: "vet-portal",
    component: VetPortalComponent
  },
  {
    path: "ger-historico",
    component: GerHistoricoComponent
  },
  {
    path: "ger-animais",
    component: GerAnimaisComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
