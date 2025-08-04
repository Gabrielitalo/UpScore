import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';

@Component({
  selector: 'app-Dashboard',
  templateUrl: './Dashboard.component.html',
  styleUrls: ['./Dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public auth: AuthService,
    public snack: SnackBarService
  ) {}

  public dash: any;
  public equipe: any = [];
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
    this.getEquipe();
  }

  private async getEquipe(): Promise<void> {
    this.equipe = await this.request.getData(
      `/CadEquipe/GetAllCurrentCompanyAsync`
    );
    this.getDash();
  }

  private async getDash(): Promise<void> {
    this.dash = await this.request.getData(
      `/MovPropostas/GetDashboard?dataInicial=${this.cc.DataFormatoUs(this.filtros.dataInicial)}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}&vendedor=${this.filtros.vendedor}`
    );
    // console.log(this.dash);
  }

  public situacoesPropostas: any = {
    1: {
      Texto: 'Gerado',
      Valor: 1,
      Classe: 'gerado',
    },
    2: {
      Texto: 'Negociação',
      Valor: 2,
      Classe: 'negociacao',
    },
    3: {
      Texto: 'Aprovado',
      Valor: 3,
      Classe: 'aprovado',
    },
    4: {
      Texto: 'Recusado',
      Valor: 4,
      Classe: 'recusado',
    },
  };

  public situacoesContratos: any = {
    1: {
      Texto: 'Gerado',
      Valor: 1,
      Classe: 'gerado',
    },
    2: {
      Texto: 'Análise',
      Valor: 2,
      Classe: 'negociacao',
    },
    3: {
      Texto: 'Aprovado',
      Valor: 3,
      Classe: 'aprovado',
    },
    4: {
      Texto: 'Recusado',
      Valor: 4,
      Classe: 'recusado',
    },
  };
  public filter(): void {
    this.getDash();
  }
}
