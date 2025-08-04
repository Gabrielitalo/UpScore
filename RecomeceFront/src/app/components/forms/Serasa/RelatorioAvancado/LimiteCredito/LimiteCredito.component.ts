import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-LimiteCredito',
  templateUrl: './LimiteCredito.component.html',
  styleUrls: ['./LimiteCredito.component.css', '../RelatorioAvancado.component.css']
})
export class LimiteCreditoComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }

  @Input() data: any;
  @Input() totalDivida = 'back_end';
  @Input() qtdDivida = 'back_end';
  

  ngOnInit() {
  }

}
