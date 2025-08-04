import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { RequestsService } from '../../../../services/Requests.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { CnpjService } from '../../../../services/cnpj.service';
import { CepService } from '../../../../services/cep.service';
import { AuthService } from '../../../../services/Auth.service';
import { ValidationsBackService } from '../../../../services/validations-back.service';

@Component({
  selector: 'app-CadEquipeDetalhes',
  templateUrl: './CadEquipeDetalhes.component.html',
  styleUrls: ['./CadEquipeDetalhes.component.css']
})
export class CadEquipeDetalhesComponent implements OnInit {


  constructor(
    private auth: AuthService,
    private cepService: CepService,
    private cnpjService: CnpjService,
    public snack: SnackBarService,
    private request: RequestsService,
    private validationBack: ValidationsBackService,
    private dialogRef: MatDialogRef<CadEquipeDetalhesComponent>,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) { }

  public isLoading = true;
  public formData: any = {
    id: 0,
    fkCadEmpresas: 0,
    ativo: true,
    tipo: null,
    nome: '',
    inscricao: '',
    email: '',
    telefone: '',
    logradouro: '',
    numero: '',
    bairro: '',
    cep: '',
    complemento: ''
  }

  ngOnInit() {
    const id = this.data.id;
    if (id > 0) {
      this.formData.Id = id;
      this.getData(id);
    }
    else {
      this.isLoading = false;
    }
  }

  public async getData(id: number): Promise<void> {
    this.isLoading = true;
    const response = await this.request.getData(`/CadEquipe/${id}`);
    if (response.length > 0) {
      this.formData = response[0];
      this.formData.cadEmpresas = { id: this.formData.fk_CadEmpresas }
      this.formData.inscricao = this.cc.MascaraCnpjCpf(this.formData.inscricao)
      this.formData.cep = this.cc.MascaraCEP(this.formData.cep)
      this.formData.telefone = this.cc.MascaraTelefone(this.formData.telefone)
      this.isLoading = false;
    }
  }

  public async Salvar(): Promise<void> {
    const payload = {
      ...this.formData,
      ativo: this.formData.ativo ? 1 : 0,
    };

    const response = await this.request.PostPut(`/CadEquipe`, payload);
    if (response > 0 || response?.id > 0) {
      this.snack.CreateNotification('Sucesso ao realizar operação');
      this.dialogRef.close();
    } else {
      this.validationBack.openDialog(response);
      this.snack.CreateNotification('Não foi possível realizar a operação, tente novamente mais tarde');
    }
  }

  public async getCEP(valor: any): Promise<void> {
    if (valor.length < 9)
      return;
    const cep = await this.cepService.GetCEP(valor);
    this.formData.logradouro = cep.logradouro;
    this.formData.bairro = cep.bairro;
  }

  public async getCNPJ(valor: any): Promise<void> {
    if (valor.length < 14)
      return;
    const cnpj = await this.cnpjService.GetCNPJ(valor);
  }

  public getTipos(): any {
    let tipos: any = [];

    if (this.auth.isAssociacao())
    {
      tipos.push({ id: 4, texto: 'Consulta' })
      tipos.push({ id: 5, texto: 'Associação' })
    }
    else {
      tipos.push({ id: 1, texto: 'Administrador' })
      tipos.push({ id: 2, texto: 'Vendedor' })
      tipos.push({ id: 3, texto: 'Financeiro' })
      tipos.push({ id: 4, texto: 'Consulta' })
    }

    return tipos;
  }
}
