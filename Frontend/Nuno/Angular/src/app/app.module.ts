import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserComponent } from './user/user.component';
import { LoginComponent } from './user/login/login.component';
import { MarcarServicoComponent } from './user/marcar-servico/marcar-servico.component';

import { SharedService } from './shared.service';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { GerirAnimaisHistoricoComponent } from './user/gerir-animais-historico/gerir-animais-historico.component';



@NgModule({
  declarations: [
    AppComponent,
    UserComponent,
    LoginComponent,
    MarcarServicoComponent,
    GerirAnimaisHistoricoComponent,
   
  
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([{path:'', component:MarcarServicoComponent }])
  ],
  providers: [SharedService],
  bootstrap: [AppComponent]
})
export class AppModule { }
