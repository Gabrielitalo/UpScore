import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-ParticipacoesSocietarias',
  templateUrl: './ParticipacoesSocietarias.component.html',
  styleUrls: ['./ParticipacoesSocietarias.component.css', '../RelatorioAvancado.component.css']
})
export class ParticipacoesSocietariasComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;

  ngOnInit() {
  }

}
