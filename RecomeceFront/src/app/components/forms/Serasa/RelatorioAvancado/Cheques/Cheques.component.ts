import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-Cheques',
  templateUrl: './Cheques.component.html',
  styleUrls: ['./Cheques.component.css', '../RelatorioAvancado.component.css']
})
export class ChequesComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
  }

}
