import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { SnackBarService } from '../../../services/SnackBar.service';
import { Router } from '@angular/router';
import { CustomConvertsService } from '../../../services/CustomConverts.service';
import { MatDialog } from '@angular/material/dialog';
import { CadEmpresasContasBancariasDetalhesComponent } from './CadEmpresasContasBancariasDetalhes/CadEmpresasContasBancariasDetalhes.component';
import { AuthService } from '../../../services/Auth.service';

@Component({
  selector: 'app-CadEmpresasContasBancarias',
  templateUrl: './CadEmpresasContasBancarias.component.html',
  styleUrls: ['./CadEmpresasContasBancarias.component.css']
})
export class CadEmpresasContasBancariasComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    private auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
    public router: Router) { }


  public itens: any = [];

  ngOnInit() {
    this.getAll();
  }

  public async getAll(): Promise<void> {
    this.itens = await this.request.getData(`/CadEmpresasContasBancarias/GetAllCompany?empresaId=${this.auth.getCompany()}`)
  }

  openDialog(id: number): void {
    this.dialog.open(CadEmpresasContasBancariasDetalhesComponent, {
      width: '400px',
      data: { id: id }, 
    });
  }
}
