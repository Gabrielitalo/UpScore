import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { ComprarConsultaDialogComponent } from '../ComprarConsultaDialog/ComprarConsultaDialog.component';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'app-MovConsultasCompradas',
  templateUrl: './MovConsultasCompradas.component.html',
  styleUrls: ['./MovConsultasCompradas.component.css'],
})
export class MovConsultasCompradasComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    public snack: SnackBarService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) {
    this.modo = this.route.snapshot.paramMap.get('modo');
  }

  public grid: any = [];
  public modo: any;
  public saldoValue = 0;
  isMobile = false;
  public pagtoExt: any;
  ngOnInit() {
    this.deviceService.isMobile$.subscribe((value) => {
      this.isMobile = value;
    });
    if (this.modo == '1')
      this.newQuery();

    this.getGrid();
    this.getValue();
  }

  private async getGrid(): Promise<void> {
    this.grid = await this.request.getData(`/MovContaCorrente/GetLastBuy10`);
  }

  private async getValue(): Promise<void> {
    this.saldoValue = await this.request.getData(`/MovContaCorrente/GetAvailableValue`);
    // console.log(this.saldoValue);
  }


  public async getAsaas(row: any): Promise<void> {
    this.pagtoExt = await this.request.getData(
      `/MovConsultasCompradas/GetAsaasId?payId=${row.idExtPagto}`
    );
    window.open(this.pagtoExt.invoiceUrl, '_blank');
  }
  newQuery(): void {
    const dialogRef = this.dialog.open(ComprarConsultaDialogComponent, {
      width: '550px',
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.getGrid();
    });
  }
}
