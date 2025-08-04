import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-ConsultasNaSerasa',
  templateUrl: './ConsultasNaSerasa.component.html',
  styleUrls: ['./ConsultasNaSerasa.component.css', '../RelatorioAvancado.component.css']
})
export class ConsultasNaSerasaComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
  }

}
