import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { Router } from '@angular/router';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { LoaderService } from '../../../../services/loader.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'app-ComprarConsultaDialog',
  templateUrl: './ComprarConsultaDialog.component.html',
  styleUrls: ['./ComprarConsultaDialog.component.css'],
})
export class ComprarConsultaDialogComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    public snack: SnackBarService,
    private dialog: MatDialog,
    private loader: LoaderService,
    private deviceService: DeviceService
  ) { }

  public querys: any = [];
  isMobile = false;
  public valorComprar: any = '100,00';

  ngOnInit() {
    this.deviceService.isMobile$.subscribe((value) => {
      this.isMobile = value;
    });
    this.getQuerys();
  }


  private async getQuerys(): Promise<void> {
    const response = await this.request.getData(`/CadConsultas/GetAll?page=0&itensPerPage=60`);
    this.querys = response?.itens;
  }


  public async buy(): Promise<void> {
    const value = this.cc.getInvariantValueFromMasked(this.valorComprar);
    if (value < 100) {
      this.snack.CreateNotification('O valor mínimo deverá ser maior que R$ 100,00');
      return;
    }
    this.loader.toogleLoader();
    await this.request.getData(`/MovContaCorrente/AddAccountValue?value=${value}`);
    window.location.href = 'FinanceiroConsultas';
  }
}
