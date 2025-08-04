import { Component, OnInit } from '@angular/core';
import { IPagination } from '../../../interfaces/IPagination';
import { MatDialog } from '@angular/material/dialog';
import { SnackBarService } from '../../../services/SnackBar.service';
import { AuthService } from '../../../services/Auth.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { RequestsService } from '../../../services/Requests.service';
import { Router } from '@angular/router';
import { ContractManagerComponent } from './ContractManager/ContractManager.component';
import { ReprovalProposalComponent } from '../MovPropostas/ReprovalProposal/ReprovalProposal.component';
import { ValidationsBackService } from '../../../services/validations-back.service';
import { WarnDialogComponent } from '../../../shared/WarnDialog/WarnDialog.component';

@Component({
  selector: 'app-MovContratos',
  templateUrl: './MovContratos.component.html',
  styleUrls: ['./MovContratos.component.css'],
})
export class MovContratosComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    public auth: AuthService,
    private validationBack: ValidationsBackService,
    public snack: SnackBarService,
    private dialog: MatDialog
  ) { }

  public isLoading = false;
  public allDataLoaded = false;
  private lastScrollTop = 0;
  public Itens: any = [];
  public equipe: any = [];
  public pagination: IPagination = {
    pageCount: 0,
    totalItens: 0,
    currentPage: 1,
    itensPerPage: 30,
    itens: [],
    totals: [],
  };

  public filtros: any = {
    nome: '',
    inscricao: '',
    dataInicial: '',
    dataFinal: '',
    situacao: 0,
    vendedor: 0,
  };

  public situacoes: any = {
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

  ngOnInit(): void {
    // const datas = this.cc.getDataInicialEFinalDoMes();
    // this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    // this.filtros.dataFinal = this.cc.DataFormatoBr(datas.dataFinal);
    this.getAll();
    this.getEquipe();
  }

  private async getEquipe(): Promise<void> {
    this.equipe = await this.request.getData(
      `/CadEquipe/GetAllCurrentCompanyAsync`
    );
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/MovContratos/GetAll?page=${this.pagination.currentPage}&nome=${this.filtros.nome
      }&insc=${this.filtros.inscricao}&dataInicial=${this.cc.DataFormatoUs(
        this.filtros.dataInicial
      )}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}&situacao=${this.filtros.situacao}&vendedor=${this.filtros.vendedor}`
    );

    const novosItens = response?.itens || [];
    if (novosItens.length > 0) {
      this.pagination.itens.push(...novosItens);
    } else {
      // Se não retornou mais itens, considera que já carregou tudo
      this.allDataLoaded = true;
    }

    this.isLoading = false;
  }

  public ViewProposal(proposal: any): void {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Comercial/${proposal.fk_MovPropostas}`;
    window.open(url, '_blank');
  }
  onScroll(event: any): void {
    const element = event.target;
    const currentScroll = element.scrollTop;

    if (currentScroll > this.lastScrollTop) {
      const nearBottom =
        element.scrollHeight - currentScroll <= element.clientHeight + 1;

      if (nearBottom && !this.isLoading && !this.allDataLoaded) {
        this.pagination.currentPage += 1;
        this.getAll();
      }
    }

    this.lastScrollTop = currentScroll;
  }

  openDialog(id: number): void { }

  getDetails(id: any): void {
    this.getValidationClient(id, 1);
    // const baseUrl = window.location.origin;
    // const url = `${baseUrl}/Clientes/${id}`;
    // window.open(url, '_blank');
  }
  getValidationClient(id: any, isValid: any): void {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Clientes/${id}/${isValid}`;
    window.open(url, '_blank');
  }
  newQuery(): void { }

  public SituacaoDesc(sit: any): string {
    const s = this.situacoes[sit];
    return s.Texto;
  }

  public filter(): void {
    this.pagination.itens = [];
    this.pagination.currentPage = 1;
    this.allDataLoaded = false;
    this.getAll();
  }

  public async UpdateStatus(contractId: number, status: number): Promise<any> {
    const response = await this.request.getData(
      `/MovContratos/UpdateStatus?contractId=${contractId}&status=${status}`
    );
    if (response.type != 1) {
      if (response?.length == null) {
        this.snack.CreateNotification(response.content);
        return;
      }
      if (response?.length > 0) {
        const hasValidation = response?.find(
          (d: any) => d.type == 2 || d.type == 3
        );
        if (hasValidation != null) {
          this.snack.CreateNotification(
            'Os dados do titular estão incompletos, clique em alterar os dados do titular para corrigir'
          );
          this.validationBack.openDialog(response);
          return response;
        }
      }
    } else {
      window.location.reload();
    }
    this.snack.CreateNotification('Status do contrato alterado com sucesso');
    this.filter();
  }
  public manager(proposalId: any): void {
    this.dialog.open(ContractManagerComponent, {
      width: '500px',
      data: proposalId,
    });
  }
  public openReproval(proposalId: any): void {
    this.dialog.open(ReprovalProposalComponent, {
      width: '350px',
      data: { tipo: 'contrato', proposalId },
    });
  }
  public async openAproval(row: any): Promise<void> {
    const response = await this.UpdateStatus(row.id, 3);
    const hasValidation = response?.find((d: any) => d.type == 2 || d.type == 3);
    if (hasValidation) {
      this.getValidationClient(row.clienteId, true);
    }
  }
  public openDeleteDialog(id: any, info: any): void {
    const dialogRef = this.dialog.open(WarnDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirmar ação',
        content: `Deseja realizar a exclusão do contrato do cliente ${info}?`,
        canRevert: false
      },
    });
    dialogRef.afterClosed().subscribe(result => {
      this.delete(id);
    });
  }
  private async delete(id: any): Promise<void> {
    await this.request.deleteData(`/MovContratos/${id}`)
    this.snack.CreateNotification('Contrato excluído com sucesso');
    this.filter();
  }
}
