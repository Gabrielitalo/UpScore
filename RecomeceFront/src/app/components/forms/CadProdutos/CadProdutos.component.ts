import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { AuthService } from '../../../services/Auth.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { Router } from '@angular/router';
import { IPagination } from '../../../interfaces/IPagination';
import { MatDialog } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { CadProdutosDetalhesComponent } from './CadProdutosDetalhes/CadProdutosDetalhes.component';

@Component({
  selector: 'app-CadProdutos',
  templateUrl: './CadProdutos.component.html',
  styleUrls: ['./CadProdutos.component.css']
})
export class CadProdutosComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    private auth: AuthService,
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

  ngOnInit(): void {
    this.getAll();
  }

  public async getAll(): Promise<void> {
    const response = await this.request.getData(
      `/CadProdutos/GetAll?page=${this.pagination.currentPage}&itensPerPage=${this.pagination.itensPerPage}`
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
      const nearBottom = element.scrollHeight - currentScroll <= element.clientHeight + 1;

      if (nearBottom && !this.isLoading && !this.allDataLoaded) {
        this.pagination.currentPage += 1;
        this.getAll();
      }
    }

    this.lastScrollTop = currentScroll;
  }

  public GetTipoProdutoDesc(tipo: any): any {
    if (tipo == 1)
      return 'Limpa Nome'
    else if (tipo == 2)
      return 'Genérico'
  }

  private filter(): void {
    this.pagination.itens = [];
    this.allDataLoaded = false;
    this.getAll();
  }
  openDialog(id: number): void {
    const dialogRef = this.dialog.open(CadProdutosDetalhesComponent, {
      width: '1100px',
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.filter();
    });
  }
}
