import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared-service.service';

@Component({
  selector: 'app-parcerias',
  templateUrl: './parcerias.component.html',
  styleUrls: ['./parcerias.component.css']
})
export class ParceriasComponent implements OnInit {

  ListaEstabelecimentos: any = [];

  constructor(private service : SharedService) { }

  ngOnInit(): void {
    this.refreshListaEstabelecimentos();
  }
  refreshListaEstabelecimentos() {
    this.service.getEstabelecimentosNameAndPhoto().subscribe(data=>{
      this.ListaEstabelecimentos = data;
      console.log(this.ListaEstabelecimentos)
      });
  }

}
