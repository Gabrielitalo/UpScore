import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { AuthService } from '../../../../services/Auth.service';
import { Router } from '@angular/router';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { RequestsService } from '../../../../services/Requests.service';
import { IPagination } from '../../../../interfaces/IPagination';
import { SerasaQueryDialogComponent } from '../SerasaQueryDialog/SerasaQueryDialog.component';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'app-Serasa',
  templateUrl: './Serasa.component.html',
  styleUrls: ['./Serasa.component.css'],
})
export class SerasaComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    public snack: SnackBarService,
    public auth: AuthService,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) {}

  public isLoading = false;
  public allDataLoaded = false;
  private lastScrollTop = 0;
  isMobile = false;
  public Itens: any = [];
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
  };

  ngOnInit(): void {
    this.deviceService.isMobile$.subscribe(value => {
      this.isMobile = value;
    });
    const datas = this.cc.getDataInicialEFinalDoMes();
    this.filtros.dataInicial = this.cc.DataFormatoBr(datas.dataInicial);
    this.filtros.dataFinal = `${this.cc.DataFormatoBr(datas.dataFinal)}`;
    this.getAll();
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/LogConsultas/QuerySerasa?page=${this.pagination.currentPage}&nome=${
        this.filtros.nome
      }&insc=${this.filtros.inscricao}&dataInicial=${this.cc.DataFormatoUs(
        this.filtros.dataInicial
      )}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)} 23:59`
    );

    // console.log(response);

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
      const nearBottom =
        element.scrollHeight - currentScroll <= element.clientHeight + 1;
      if (nearBottom && !this.isLoading && !this.allDataLoaded) {
        this.pagination.currentPage += 1;
        this.getAll();
      }
    }

    this.lastScrollTop = currentScroll;
  }

  openDialog(id: number): void {}

  getAdv(row: any): void {
    if (row.tipo == 2) this.router.navigate([`RelatorioBoaVista/${row.id}`]);
    else if (row.legado == 0) {
      if (row.consultaId == 7 || row.consultaId == 8)
        this.router.navigate([`RelatorioBasico/${row.id}`]);
      else this.router.navigate([`RelatorioAvancado/${row.id}`]);
    } else this.router.navigate([`Consultas/${row.markID}`]);
  }
  getDetails(markID: any): void {
    this.router.navigate([`Consultas/${markID}`]);
  }
  newQuery(): void {
    const dialogRef = this.dialog.open(SerasaQueryDialogComponent, {
      width: '450px',
    });
  }

  public filter(): void {
    this.pagination.itens = [];
    this.pagination.currentPage = 1;
    this.allDataLoaded = false;
    this.getAll();
  }
}
