import { Component, OnInit } from '@angular/core';
import { IPagination } from '../../../interfaces/IPagination';
import { MatDialog } from '@angular/material/dialog';
import { SnackBarService } from '../../../services/SnackBar.service';
import { AuthService } from '../../../services/Auth.service';
import { LoaderService } from '../../../services/loader.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { RequestsService } from '../../../services/Requests.service';
import { Router } from '@angular/router';
import { NewBatchAssociacaoComponent } from './NewBatchAssociacao/NewBatchAssociacao.component';
import { LoteAssociacaoDetailsComponent } from './LoteAssociacaoDetails/LoteAssociacaoDetails.component';

@Component({
  selector: 'app-MovLotesAssociacao',
  templateUrl: './MovLotesAssociacao.component.html',
  styleUrls: ['./MovLotesAssociacao.component.css']
})
export class MovLotesAssociacaoComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private loader: LoaderService,
    public auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
  ) { }

  public isLoading = false;
  public allDataLoaded = false;
  private lastScrollTop = 0;
  public Itens: any = [];
  public pagination: IPagination = {
    pageCount: 0,
    totalItens: 0,
    currentPage: 1,
    itensPerPage: 30,
    itens: [],
    totals: []
  };

  public filtros: any = {
    membro: '',
    inscricao: '',
    dataInicial: '',
    dataFinal: '',
    situacao: 0
  }

  public situacoes: any = {
    0: {
      Texto: 'Aguardando pagamento',
      Valor: 0,
      Classe: 'recusado'
    },
    1: {
      Texto: 'Aprovado',
      Valor: 2,
      Classe: 'aprovado'
    },
    2: {
      Texto: 'Ativo',
      Valor: 3,
      Classe: 'aprovado'
    },
    4: {
      Texto: 'Recusado',
      Valor: 4,
      Classe: 'recusado'
    },
  }

  ngOnInit(): void {
    const datas = this.cc.getDataInicialEFinalDoMes();
    this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    this.filtros.dataFinal = this.cc.DataFormatoBr(datas.dataFinal);
    this.getAll();
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/MovLotesAssociacao/GetAll?page=${this.pagination.currentPage}&membro=${this.filtros.membro}&dataInicial=${this.cc.DataFormatoUs(this.filtros.dataInicial)}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}`
    );

    console.log(response.itens);
    const novosItens = response?.itens || [];
    if (novosItens.length > 0) {
      this.pagination.itens.push(...novosItens);
    } else {
      // Se não retornou mais itens, considera que já carregou tudo
      this.allDataLoaded = true;
    }

    this.isLoading = false;
  }

  onScroll(event: any): void {
    const element = event.target;
    const currentScroll = element.scrollTop;

    if (currentScroll > this.lastScrollTop) {
      const nearBottom = element.scrollHeight - currentScroll <= element.clientHeight + 1;

      if (nearBottom && !this.isLoading && !this.allDataLoaded) {
        this.pagination.currentPage += 1;
        this.getAll();
      }
    }

    this.lastScrollTop = currentScroll;
  }


  openDialog(id: number): void {

  }

  getDetails(id: any): void {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Clientes/${id}`;
    window.open(url, '_blank');
  }

  async newQuery(): Promise<void> {
    this.dialog.open(NewBatchAssociacaoComponent, {
      width: '768px',
    });
  }

  public viewDetails(batch: any): void {
    // this.router.navigate([`Lotes/${batch.id}`])
    this.dialog.open(LoteAssociacaoDetailsComponent, {
      width: '950px',
      data: { id: batch.id },
    });
  }

  public SituacaoDesc(sit: any): string {
    const s = this.situacoes[sit];
    return s.Texto;
  }

  public filter(): void {
    this.pagination.itens = [];
    this.allDataLoaded = false;
    this.getAll();
  }

  public async DownloadExcel(id: any): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(`/MovLotesEmpresasBeneficiarios/DownloadExcel?batchId=${id}`);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    link.download = 'Lote.xlsx';
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
    this.loader.toogleLoader();
  }

}
