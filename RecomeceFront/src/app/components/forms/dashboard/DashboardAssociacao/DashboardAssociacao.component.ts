import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';

@Component({
  selector: 'app-DashboardAssociacao',
  templateUrl: './DashboardAssociacao.component.html',
  styleUrls: ['./DashboardAssociacao.component.css', '../Dashboard/Dashboard.component.css']
})
export class DashboardAssociacaoComponent implements OnInit {

  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public auth: AuthService,
    public snack: SnackBarService
  ) {}

  public dash: any;
  public filtros: any = {
    nome: '',
    inscricao: '',
    dataInicial: '',
    dataFinal: '',
    situacao: 0,
    vendedor: 0
  };

  ngOnInit() {
    const datas = this.cc.getDataInicialEFinalDoMes();
    this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    this.filtros.dataFinal = this.cc.DataFormatoBr(datas.dataFinal);
    this.getDash();
  }

  private async getDash(): Promise<void> {
    this.dash = await this.request.getData(
      `/MovLotesAssociacao/GetDashboard?dataInicial=${this.cc.DataFormatoUs(this.filtros.dataInicial)}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}`
    );
    // console.log(this.dash);
  }

  public filter(): void {
    this.getDash();
  }
}
