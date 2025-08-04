import {
  Component,
  OnInit,
  Inject,
  Input,
  Output,
  EventEmitter,
  Optional,
} from '@angular/core';
import { AuthService } from '../../../../services/Auth.service';
import { CepService } from '../../../../services/cep.service';
import { CnpjService } from '../../../../services/cnpj.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { RequestsService } from '../../../../services/Requests.service';
import { ValidationsBackService } from '../../../../services/validations-back.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-CadClientesDetalhes',
  templateUrl: './CadClientesDetalhes.component.html',
  styleUrls: ['./CadClientesDetalhes.component.css'],
})
export class CadClientesDetalhesComponent implements OnInit {
  constructor(
    private cepService: CepService,
    private route: ActivatedRoute,
    public router: Router,
    private cnpjService: CnpjService,
    public snack: SnackBarService,
    private request: RequestsService,
    private validationBack: ValidationsBackService,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) @Optional() public data?: { id: number }
  ) {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.id = idParam ? Number(idParam) : 0;

    const id = this.data?.id;

    if (idParam == null) {
      if (id) this.id = id;
    }
    const isValid = this.route.snapshot.paramMap.get('valid');
    if (isValid != null) this.completaCadastro();
    this.isValid = isValid;
  }

  @Input() id: any = 0;
  @Input() isValid: any = 0;
  @Output() isSave = new EventEmitter<any>();
  cidadeSelecionada: any;
  cidades: any[] = [];
  public querys: any;
  timeout: any;
  public isLoading = true;
  public serasaQuerys: any;
  public proposals: any;
  public contracts: any;
  public formData: any = {
    id: 0,
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
    complemento: '',
    profissao: '',
    nacionalidade: '',
    estadoCivil: '',
  };

  ngOnInit() {
    if (this.id > 0) {
      if (this.isValid != null) this.completaCadastro();

      this.formData.Id = this.id;
      this.getData(this.id);
    } else {
      this.isLoading = false;
    }
  }

  public completaCadastro(): void {
    this.snack.CreateNotification('Complete o cadastro do titular!');
  }
  public async getData(id: number): Promise<void> {
    this.isLoading = true;
    const response = await this.request.getData(`/CadClientes/${id}`);
    if (response.length > 0) {
      this.formData = response[0];
      this.formData.inscricao = this.cc.MascaraCnpjCpf(this.formData.inscricao);
      this.formData.cep = this.cc.MascaraCEP(this.formData.cep);
      this.formData.telefone = this.cc.MascaraTelefone(this.formData.telefone);
      this.isLoading = false;
      this.cidadeSelecionada = {
        id: this.formData.fk_Cidades,
        texto: this.formData.cidade,
      };
      this.formData.cidades = { id: this.formData.fk_Cidades };
      this.formData.cadEquipe = { id: this.formData.fk_CadEquipe };
      this.GetQuerys();
    }
  }

  public async Salvar(): Promise<void> {
    if (this.cidadeSelecionada.id != this.formData.fk_Cidades)
      this.formData.cidades = { id: this.cidadeSelecionada.id };

    const payload = {
      ...this.formData,
      ativo: this.formData.ativo ? 1 : 0,
    };

    const response = await this.request.PostPut(`/CadClientes`, payload);
    if (response > 0 || response?.id > 0) {
      this.snack.CreateNotification('Sucesso ao realizar operação');
      this.isSave.emit(true);
    } else {
      this.validationBack.openDialog(response);
      this.snack.CreateNotification(
        'Não foi possível realizar a operação, tente novamente mais tarde'
      );
    }
  }

  public async getCEP(valor: any): Promise<void> {
    if (valor.length < 9) return;
    const cep = await this.cepService.GetCEP(valor);
    console.log(cep.ibge);
    if (cep?.ibge) {
      const response = await this.request.getData(
        `/Cidades/GetByIbge?codigoIBGE=${cep?.ibge}`
      );
      this.cidadeSelecionada = {
        id: response.id,
        texto: response.nomeCidade,
      };
    }
    this.formData.logradouro = cep.logradouro;
    this.formData.bairro = cep.bairro;
  }

  public async getCNPJ(valor: any): Promise<void> {
    if (valor.length < 14) return;
    const cnpj = await this.cnpjService.GetCNPJ(valor);
  }

  buscarCidades(query: string): void {
    clearTimeout(this.timeout); // cancela debounce anterior

    if (!query || typeof query !== 'string') {
      this.cidades = [];
      return;
    }

    if (query.length < 3) return;
    // debounce manual (opcional)
    this.timeout = setTimeout(async () => {
      const response = await this.request.getData(
        `/Cidades/AutoComplete?texto=${query}`
      );
      this.cidades = response;
    }, 300);
  }

  displayCidade(cidade: any): string {
    return cidade?.texto || '';
  }

  private async GetQuerys(): Promise<void> {
    const response = await this.request.getData(
      `/LogConsultas/GetAllClients?clientId=${this.formData.id}`
    );
    this.serasaQuerys = response;
    this.getProposals();
  }

  getDetails(row: any): void {
    const baseUrl = window.location.origin;
    let url = '';
    if (row.legado == 1)
      url = `${baseUrl}/Consultas/${row.markID}`;
    else
      url = `${baseUrl}/RelatorioAvancado/${row.id}`;
    window.open(url, '_blank');
  }

  public ViewProposal(proposal: any): void {
    const baseUrl = window.location.origin;
    const url = `${baseUrl}/Comercial/${proposal}`;
    window.open(url, '_blank');
  }

  private async getProposals(): Promise<void> {
    const response = await this.request.getData(
      `/MovPropostas/GetAllFromClient?clientId=${this.id}`
    );
    this.proposals = response?.itens;
  }
}
