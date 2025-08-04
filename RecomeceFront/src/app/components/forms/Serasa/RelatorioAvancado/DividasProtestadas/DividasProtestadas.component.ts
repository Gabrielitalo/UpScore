import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-DividasProtestadas',
  templateUrl: './DividasProtestadas.component.html',
  styleUrls: ['./DividasProtestadas.component.css', '../RelatorioAvancado.component.css']
})
export class DividasProtestadasComponent implements OnInit {
  
  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
  }

}
