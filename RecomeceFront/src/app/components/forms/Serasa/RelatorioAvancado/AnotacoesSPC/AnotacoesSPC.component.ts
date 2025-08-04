import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-AnotacoesSPC',
  templateUrl: './AnotacoesSPC.component.html',
  styleUrls: ['./AnotacoesSPC.component.css', '../RelatorioAvancado.component.css']
})
export class AnotacoesSPCComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  
  @Input() data: any;
  @Input() tipo: any;

  ngOnInit() {
  }

}
