import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { AuthService } from '../../../services/Auth.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { Router } from '@angular/router';
import { CadEmpresasDetalhesComponent } from './CadEmpresasDetalhes/CadEmpresasDetalhes.component';
import { MatDialog } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { IPagination } from '../../../interfaces/IPagination';

@Component({
  selector: 'app-CadEmpresas',
  templateUrl: './CadEmpresas.component.html',
  styleUrls: ['./CadEmpresas.component.css']
})
export class CadEmpresasComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
    public router: Router) { }

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
      `/CadEmpresas/GetAll?page=${this.pagination.currentPage}&itensPerPage=${this.pagination.itensPerPage}`
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

  public VerificaTipo(tipo: any): string {
    return tipo == 1 ? 'Escritório' : 'Individual';
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

  filter(): void {
    this.allDataLoaded = false;
    this.pagination.itens = [];
    this.getAll();
  }
  
  openDialog(id: number): void {
    const dialogRef = this.dialog.open(CadEmpresasDetalhesComponent, {
      width: '1100px',
      data: { id: id },
    });
    dialogRef.afterClosed().subscribe((result) => {
      this.filter();
    });
  }


}
