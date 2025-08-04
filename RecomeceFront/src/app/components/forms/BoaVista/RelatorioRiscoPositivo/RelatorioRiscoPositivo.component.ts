import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';

@Component({
  selector: 'app-RelatorioRiscoPositivo',
  templateUrl: './RelatorioRiscoPositivo.component.html',
  styleUrls: ['./RelatorioRiscoPositivo.component.css', '../RelatorioBoaVista/RelatorioBoaVista.component.css']
})
export class RelatorioRiscoPositivoComponent implements OnInit {

  @Input() reportData: any;

  constructor(
    public cc: CustomConvertsService,

  ) { }

  ngOnInit() {
    // console.log(this.reportData);
  }

}
