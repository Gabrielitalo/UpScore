import { Component, OnInit } from '@angular/core';
import { IPagination } from '../../../interfaces/IPagination';
import { RequestsService } from '../../../services/Requests.service';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { AuthService } from '../../../services/Auth.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { CadEquipeDetalhesComponent } from './CadEquipeDetalhes/CadEquipeDetalhes.component';

@Component({
  selector: 'app-CadEquipe',
  templateUrl: './CadEquipe.component.html',
  styleUrls: ['./CadEquipe.component.css']
})
export class CadEquipeComponent implements OnInit {

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
      `/CadEquipe/GetAll?page=${this.pagination.currentPage}&itensPerPage=${this.pagination.itensPerPage}`
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
    if (tipo == 0)
      return 'Global';
    else if (tipo == 1)
      return 'Admin';
    else if (tipo == 2)
      return 'Vendedor';
    else if (tipo == 3)
      return 'Financeiro';
    else
      return 'Vendedor';
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
    this.dialog.open(CadEquipeDetalhesComponent, {
      width: '1100px',
      data: { id: id },
    });
  }

}
