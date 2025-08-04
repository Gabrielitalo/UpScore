import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { AuthService } from '../../../services/Auth.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { IPagination } from '../../../interfaces/IPagination';
import { DeviceService } from '../../../services/device.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-MovContaCorrente',
  templateUrl: './MovContaCorrente.component.html',
  styleUrls: ['./MovContaCorrente.component.css']
})
export class MovContaCorrenteComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) { }

  public isLoading = false;
  public allDataLoaded = false;
  private lastScrollTop = 0;
  public Itens: any = [];
  isMobile = false;
  public saldoValue = 0;
  public pagination: IPagination = {
    pageCount: 0,
    totalItens: 0,
    currentPage: 1,
    itensPerPage: 30,
    itens: [],
    totals: []
  };
  public filtros: any = {
    tipo: '0',
    dataInicial: '',
    dataFinal: ''
  }

  ngOnInit(): void {
    this.getAll();
    this.getValue()
  }

  private async getValue(): Promise<void> {
    this.saldoValue = await this.request.getData(`/MovContaCorrente/GetAvailableValue`);
    // console.log(this.saldoValue);
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/MovContaCorrente/GetAll?page=${this.pagination.currentPage}&itensPerPage=${this.pagination.itensPerPage}&tipo=${+this.filtros.tipo}&dataInicial=${this.cc.DataFormatoUs(this.filtros.dataInicial)}&dataFinal=${this.cc.DataFormatoUs(this.filtros.dataFinal)}`
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

  public filter(): void {
    this.pagination.itens = [];
    this.allDataLoaded = false;
    this.getAll();
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

  openQuery(row: any): void {
    if (row.tipoConsulta == 2)
      this.cc.OpenNewTab(`RelatorioBoaVista/${row.fk_Origem}`)
    else if (row.tipoConsulta == 1)
      this.cc.OpenNewTab(`RelatorioAvancado/${row.fk_Origem}`)
    else
      this.snack.CreateNotification('Não é possível efetuar o redirect, faça a busca na tela de Consultas');
  }

  getHistorico(row: any): any {
    let historico = row.historico;
    if (row.inscricao)
      historico += ` - ${this.cc.MascaraCnpjCpf(row.inscricao)}`
    return historico
  }
}
