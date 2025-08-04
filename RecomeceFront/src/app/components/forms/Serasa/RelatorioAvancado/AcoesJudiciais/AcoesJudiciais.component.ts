import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-AcoesJudiciais',
  templateUrl: './AcoesJudiciais.component.html',
  styleUrls: ['./AcoesJudiciais.component.css', '../RelatorioAvancado.component.css']
})
export class AcoesJudiciaisComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
  }

}
