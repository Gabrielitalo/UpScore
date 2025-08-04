import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../../services/Auth.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { RequestsService } from '../../../../services/Requests.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-CadEmpresasContasBancariasDetalhes',
  templateUrl: './CadEmpresasContasBancariasDetalhes.component.html',
  styleUrls: ['./CadEmpresasContasBancariasDetalhes.component.css']
})
export class CadEmpresasContasBancariasDetalhesComponent implements OnInit {

  constructor(private auth: AuthService,
    public snack: SnackBarService,
    private request: RequestsService,
    private dialogRef: MatDialogRef<CadEmpresasContasBancariasDetalhesComponent>,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) { }


  public bancos = this.cc.ListaBancos();
  public formData: any = {
    id: 0,
    cadEmpresas: { id: 0 },
    ativo: true,
    padrao: false,
    codBanco: '',
    agencia: '',
    conta: '',
    chavePix: ''
  }

  ngOnInit() {
    this.formData.cadEmpresas.id = this.auth.getCompany();
    const id = this.data.id;
    if (id > 0) {
      this.formData.id = id;
      this.getData(id)
    }
  }

  public async getData(id: number): Promise<void> {
    const response = await this.request.getData(`/CadEmpresasContasBancarias/${id}`);
    this.formData = response[0];
    this.formData.cadEmpresas = { Id: this.formData.fk_CadEmpresas };
  }

  public async Salvar(): Promise<void> {
    const payload = {
      ...this.formData,
      ativo: this.formData.ativo ? 1 : 0,
      padrao: this.formData.padrao ? 1 : 0,
    };

    const response = await this.request.PostPut(`/CadEmpresasContasBancarias`, payload)
    if (response > 0 || response?.id > 0) {
      this.snack.CreateNotification('Sucesso ao realizar operação')
      this.dialogRef.close();
    }
    else
      this.snack.CreateNotification('Não foi possível realizar a operação, tente novamente mais tarde')
  }

}
