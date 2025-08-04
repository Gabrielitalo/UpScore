import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IPagination } from '../../../interfaces/IPagination';
import { SnackBarService } from '../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../../services/Auth.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { RequestsService } from '../../../services/Requests.service';
import { Router } from '@angular/router';
import { NewBatchComponent } from './NewBatch/NewBatch.component';
import { ViewBeneficiariosLoteDialogComponent } from './ViewBeneficiariosLoteDialog/ViewBeneficiariosLoteDialog.component';
import { NewBatchSheetComponent } from './NewBatchSheet/NewBatchSheet.component';
import { LoaderService } from '../../../services/loader.service';
import { WarnDialogComponent } from '../../../shared/WarnDialog/WarnDialog.component';

@Component({
  selector: 'app-MovLotesEmpresas',
  templateUrl: './MovLotesEmpresas.component.html',
  styleUrls: ['./MovLotesEmpresas.component.css']
})
export class MovLotesEmpresasComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private loader: LoaderService,
    public auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
  ) { }

  @Output() data = new EventEmitter<any>;
  @Input() associacaoId: any = '';
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
      Classe: 'negociacao'
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
    // const datas = this.cc.getDataInicialEFinalDoMes();
    // this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    // this.filtros.dataFinal = this.cc.DataFormatoBr(datas.dataFinal);
    this.getAll();
  }

  public async getAll(): Promise<void> {
    // console.log('associacaoId', this.associacaoId);
    const response = await this.request.getData(
      `/MovLotesEmpresas/GetAll?page=${this.pagination.currentPage}&membro=${this.filtros.membro}&dataInicial=${this.cc.DataFormatoUs(this.filtros.dataInicial)}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}&associacaoId=${this.associacaoId}`
    );

    // console.log(response.itens);
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
    if (this.auth.getWhiteLabelConfig().tipo == 1)
      this.dialog.open(NewBatchComponent, {
        width: '768px',
      });
    else
      this.dialog.open(NewBatchSheetComponent, {
        width: '450px',
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

  public async UpdateStatus(contractId: string, status: number): Promise<void> {
    this.loader.toogleLoader();
    const respose = await this.request.getData(`/MovLotesEmpresas/UpdateStatus?id=${contractId}&situacao=${status}`)
    this.loader.toogleLoader();
    this.snack.CreateNotification('Status do lote alterado com sucesso');
    this.filter();
  }
  public viewDetails(batch: any): void {
    if (!this.associacaoId)
      this.router.navigate([`Lotes/${batch.id}`])
    else {
      this.data.emit(batch.id)
      // const baseUrl = window.location.origin;
      // const url = `${baseUrl}/Lotes/${batch.id}`;
      // window.open(url, '_blank');
    }
  }
  public callAprovaBatch(batch: any): void {
    const dialogRef = this.dialog.open(WarnDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirmar ação',
        content: `Confirma aprovação do pagamento deste lote?`,
      },
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.confirmed == true)
        this.updateBatch(batch);
    });
  }

  public callReproveBatch(batch: any): void {
    const dialogRef = this.dialog.open(WarnDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirmar ação',
        content: `Confirma a reprovação deste lote?`,
        canRevert: false
      },
    });

    dialogRef.afterClosed().subscribe(async result => {
      if (result?.confirmed == true)
        await this.UpdateStatus(batch, 4);
    });
  }

  public callDeleteBatch(batch: any): void {
    const dialogRef = this.dialog.open(WarnDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirmar ação',
        content: `Confirma a exclusão deste lote?`,
        canRevert: false
      },
    });

    dialogRef.afterClosed().subscribe(async result => {
      if (result?.confirmed == true)
        await this.deleteBatch(batch);
    });
  }
  private async deleteBatch(id: any): Promise<void> {
    this.loader.toogleLoader();
    console.log('chamou');
    await this.request.deleteData(`/MovLotesEmpresas/${id}`);
    this.loader.toogleLoader();
    this.snack.CreateNotification('Lote excluído com sucesso');
    this.filter();
  }
  private async updateBatch(id: any): Promise<void> {
    await this.UpdateStatus(id, 1);
  }
  public openReproval(proposalId: any): void {
    // this.dialog.open(ReprovalProposalComponent, {
    //   width: '350px',
    //   data: { tipo: 'contrato', proposalId },
    // });
  }
  public async openAproval(row: any): Promise<void> {
    await this.UpdateStatus(row.id, 3);
  }

  public async DownloadWord(id: any): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(`/MovLotesEmpresas/GenerateWord?batchId=${id}`);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    link.download = 'TabelaPessoasLote.docx';
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
    this.loader.toogleLoader();
  }

  public async DownloadZip(id: any): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(`/MovLotesEmpresas/GenerateZipFiles?batchId=${id}`);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    link.download = 'Lote.zip';
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
    this.loader.toogleLoader();
  }
  public async DownloadXlx(): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(`/MovLotesEmpresas/DownloadXlsModel`);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    link.download = 'ModeloLote.xlsx';
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
    this.loader.toogleLoader();
  }
}
