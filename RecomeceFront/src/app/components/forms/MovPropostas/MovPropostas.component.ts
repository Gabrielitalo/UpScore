import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { AuthService } from '../../../services/Auth.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { IPagination } from '../../../interfaces/IPagination';
import { Router } from '@angular/router';
import { ReprovalProposalComponent } from './ReprovalProposal/ReprovalProposal.component';
import { AprovalProposalComponent } from './AprovalProposal/AprovalProposal.component';
import { LoaderService } from '../../../services/loader.service';
import { DeviceService } from '../../../services/device.service';

@Component({
  selector: 'app-MovPropostas',
  templateUrl: './MovPropostas.component.html',
  styleUrls: ['./MovPropostas.component.css'],
})
export class MovPropostasComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    public auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
    private loader: LoaderService,
    private deviceService: DeviceService
  ) { }

  public isLoading = false;
  public allDataLoaded = false;
  private lastScrollTop = 0;
  public equipe: any = [];
  public Itens: any = [];
  isMobile = false;
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

  ngOnInit(): void {
    // const datas = this.cc.getDataInicialEFinalDoMes();
    // this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    // this.filtros.dataFinal = this.cc.DataFormatoBr(datas.dataFinal);
    this.deviceService.isMobile$.subscribe(value => {
      this.isMobile = value;
    });
    this.getAll();
    this.getEquipe();
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/MovPropostas/GetAll?page=${this.pagination.currentPage}&nome=${this.filtros.nome
      }&insc=${this.filtros.inscricao}&dataInicial=${this.cc.DataFormatoUs(
        this.filtros.dataInicial
      )}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}&situacao=${this.filtros.situacao}&vendedor=${this.filtros.vendedor}`
    );

    // console.log(response);
    const novosItens = response?.itens || [];
    if (novosItens.length > 0) {
      this.pagination.itens.push(...novosItens);
    } else {
      // Se não retornou mais itens, considera que já carregou tudo
      this.allDataLoaded = true;
    }

    const total = this.pagination.itens.reduce((total: any, proposta: any) => {
      const valor =
        proposta.valorAprovado && proposta.valorAprovado !== 0
          ? proposta.valorAprovado
          : proposta.valorContrato || 0;

      return total + valor;
    }, 0);

    this.pagination.totals[0] = total;

    this.isLoading = false;
  }

  private async getEquipe(): Promise<void> {
    this.equipe = await this.request.getData(
      `/CadEquipe/GetAllCurrentCompanyAsync`
    );
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
    this.router.navigate([`Comercial/${id}`]);
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

  public async UpdateStatus(proposalId: number, status: number): Promise<void> {
    const respose = await this.request.getData(
      `/MovPropostas/UpdateStatus?proposalId=${proposalId}&status=${status}`
    );
    this.filter();
  }

  public openReproval(proposalId: any): void {
    this.dialog.open(ReprovalProposalComponent, {
      width: '350px',
      data: { tipo: 'proposta', proposalId },
    });
  }
  public async openAproval(row: any): Promise<void> {
    if (row.limpaNome == 1) {
      const isValid = await this.CheckPayments(row.id);
      if (isValid == false) {
        this.snack.CreateNotification(
          'Preencha os dados de pagamento antes de aprovar a proposta!'
        );
        return;
      }
      this.dialog.open(AprovalProposalComponent, {
        width: '900px',
        data: row,
      });
    } else if (row.limpaNome == 2) {
      // Venda avulsa
      await this.UpdateStatus(row.id, 3);
    }
  }

  public async CheckPayments(row: any): Promise<boolean> {
    const respose = await this.request.getData(
      `/MovPropostasDuplicatas/CheckPayments?proposalId=${row}`
    );
    return respose;
  }

  public async getProposalPdf(proposalId: any, name: string): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.downloadData(
      `/MovPropostas/GetProposalPdf?proposalId=${proposalId}`
    );
    this.loader.toogleLoader();
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    const agora = new Date();
    const pad = (n: any) => n.toString().padStart(2, '0');
    const nomeArquivo = `Proposta_${name}_${agora.getFullYear()}-${pad(
      agora.getMonth() + 1
    )}-${pad(agora.getDate())}_${pad(agora.getHours())}-${pad(
      agora.getMinutes()
    )}-${pad(agora.getSeconds())}.pdf`;
    link.download = nomeArquivo;
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
  }
  public async GetPDF(): Promise<void> {
    const response = await this.request.downloadData(`/MovPropostas/GeneratePdf`);
    const link = document.createElement('a');
    link.href = URL.createObjectURL(response);
    link.download = 'relatorio.pdf';
    link.click();
    URL.revokeObjectURL(link.href); // limpa a memória
  }
}
