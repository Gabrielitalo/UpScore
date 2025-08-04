import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-DividasVencidas',
  templateUrl: './DividasVencidas.component.html',
  styleUrls: [
    './DividasVencidas.component.css',
    '../RelatorioAvancado.component.css',
  ],
})
export class DividasVencidasComponent implements OnInit {
  constructor(public cc: CustomConvertsService) {}

  @Input() data: any;

  ngOnInit() {
  }
}
