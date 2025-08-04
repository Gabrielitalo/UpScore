import { Component, Inject, OnInit } from '@angular/core';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { RequestsService } from '../../../../services/Requests.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { CepService } from '../../../../services/cep.service';
import { CnpjService } from '../../../../services/cnpj.service';

@Component({
  selector: 'app-CadEmpresasDetalhes',
  templateUrl: './CadEmpresasDetalhes.component.html',
  styleUrls: ['./CadEmpresasDetalhes.component.css']
})
export class CadEmpresasDetalhesComponent implements OnInit {

  constructor(
    public auth: AuthService,
    private cepService: CepService,
    private cnpjService: CnpjService,
    public snack: SnackBarService,
    private request: RequestsService,
    private dialogRef: MatDialogRef<CadEmpresasDetalhesComponent>,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) { }

  cidadeSelecionada: any;
  cidades: any[] = [];
  timeout: any;
  public isLoading = true;
  public formData: any = {
    id: 0,
    cidades: { id: 0 },
    idEmpresaResponsavel: null,
    ativo: true,
    tipo: null,
    inscricao: '',
    nome: '',
    nomeResponsavel: '',
    cpfResponsavel: '',
    emailResponsavel: '',
    telefoneResponsavel: '',
    logradouro: '',
    numero: '',
    bairro: '',
    cep: '',
    complemento: '',
    servidor: null,
    bancoDados: null,
    idAsaas: '',
    chaveApiAsaas: '',
    whiteLabelConfig: {}
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
    const response = await this.request.getData(`/CadEmpresas/${id}`);
    if (response.length > 0) {
      this.formData = response[0];
      this.formData.inscricao = this.cc.MascaraCnpjCpf(this.formData.inscricao)
      this.formData.cep = this.cc.MascaraCEP(this.formData.cep)
      this.formData.telefoneResponsavel = this.cc.MascaraTelefone(this.formData.telefoneResponsavel)
      this.isLoading = false;
      this.cidadeSelecionada = { id: this.formData.fk_Cidades, texto: this.formData.cidade }
      this.formData.cidades = { id: this.formData.fk_Cidades };
      this.formData.whiteLabelConfig = JSON.parse(response[0].whiteLabelConfig)
      this.formData.whiteLabelConfig.valorNomeLote = this.cc.MascaraDecimal(this.formData.whiteLabelConfig?.valorNomeLote ?? 0);
    }
  }

  public async createUser(userType: number): Promise<void> {
    const response = await this.request.getData(`/CadEquipe/CreateFromCompany?companyId=${this.formData.id}&userType=${userType}`);
    this.snack.CreateNotification('Sucesso ao realizar operação');
  }

  public async createUserConsulta(): Promise<void> {
    await this.createUser(4);
  }

  public async Salvar(): Promise<void> {
    if (this.cidadeSelecionada?.id == null) {
      this.snack.CreateNotification('Escolha a cidade do franqueado');
      return;
    }
    const payload = {
      ...this.formData,
      ativo: this.formData.ativo ? 1 : 0,
      cidades: { id: this.cidadeSelecionada.id },
    };

    payload.whiteLabelConfig.valorNomeLote = this.cc.getInvariantValueFromMasked(payload.whiteLabelConfig.valorNomeLote);

    if (payload.whiteLabelConfig.valorNomeLote == 0) {
      this.snack.CreateNotification('Necessário informar o custo por nome do lote');
      return;
    }

    const response = await this.request.PostPut(`/CadEmpresas`, payload);
    if (response > 0 || response?.id > 0) {
      this.snack.CreateNotification('Sucesso ao realizar operação');
      this.dialogRef.close();
    } else {
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

  buscarCidades(query: string): void {
    clearTimeout(this.timeout); // cancela debounce anterior

    if (!query || typeof query !== 'string') {
      this.cidades = [];
      return;
    }

    if (query.length < 3)
      return;
    // debounce manual (opcional)
    this.timeout = setTimeout(async () => {
      const response = await this.request.getData(`/Cidades/AutoComplete?texto=${query}`);
      this.cidades = response;
    }, 300);
  }

  displayCidade(cidade: any): string {
    return cidade?.texto || '';
  }
}
