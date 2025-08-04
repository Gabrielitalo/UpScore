import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-AnotacoesNegativas',
  templateUrl: './AnotacoesNegativas.component.html',
  styleUrls: ['./AnotacoesNegativas.component.css', '../RelatorioAvancado.component.css']
})
export class AnotacoesNegativasComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }
  @Input() data: any;
  @Input() totalDivida: any;
  @Input() qtdDivida: any;
  @Input() modelo = 1;

  ngOnInit() {
  }

  public checkValue(item: any): any {
    if (item == 0)
        return 'Sem registros';
    else
        return `R$ ${this.cc.MascaraDecimal(item)}`;
}


}
