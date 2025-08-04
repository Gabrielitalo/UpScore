import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-RendaEstimada',
  templateUrl: './RendaEstimada.component.html',
  styleUrls: ['./RendaEstimada.component.css', '../RelatorioAvancado.component.css']
})
export class RendaEstimadaComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
    
  }

}
