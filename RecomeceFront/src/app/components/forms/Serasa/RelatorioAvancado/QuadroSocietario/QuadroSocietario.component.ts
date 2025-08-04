import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-QuadroSocietario',
  templateUrl: './QuadroSocietario.component.html',
  styleUrls: ['./QuadroSocietario.component.css', '../RelatorioAvancado.component.css']
})
export class QuadroSocietarioComponent implements OnInit {


  constructor(
    public cc: CustomConvertsService,

  ) { }

  @Input() data: any;
  @Input() totalDivida = 'back_end';
  @Input() qtdDivida = 'back_end';
  @Input() modelo = 1;
  
  ngOnInit() {
  }

}
