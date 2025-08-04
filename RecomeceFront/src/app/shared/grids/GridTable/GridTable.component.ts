import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-GridTable',
  templateUrl: './GridTable.component.html',
  styleUrls: ['./GridTable.component.css']
})
export class GridTableComponent implements OnInit {

  @Input() grid: any = {};
  constructor() { }

  displayedColumns: string[] = [];
  dataSource: any[] = [];

  ngOnInit() {
    if (this.grid && this.grid.display && this.grid.data) {
      this.displayedColumns = this.grid.display;
      this.dataSource = this.grid.data;
    } else {
      // console.warn("Nenhum dado foi recebido para a tabela!");
    }
  }
}
