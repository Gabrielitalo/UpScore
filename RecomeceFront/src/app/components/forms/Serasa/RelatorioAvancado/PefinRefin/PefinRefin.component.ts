import { Component, Input, OnInit, input } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-PefinRefin',
  templateUrl: './PefinRefin.component.html',
  styleUrls: ['./PefinRefin.component.css', '../RelatorioAvancado.component.css']
})
export class PefinRefinComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,
  ) { }

  @Input() data: any;
  @Input() type: any;

  ngOnInit() {
  }

  public getObj(): any {
    if (this.type == 1)
      return this.data.pefinResponse;
    else
      return this.data.refinResponse;
  }

}
