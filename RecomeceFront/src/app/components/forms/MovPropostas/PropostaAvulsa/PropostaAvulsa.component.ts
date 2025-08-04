import { Component, Input, OnInit } from '@angular/core';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { RequestsService } from '../../../../services/Requests.service';
import { LoaderService } from '../../../../services/loader.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ValidationsBackService } from '../../../../services/validations-back.service';

@Component({
  selector: 'app-PropostaAvulsa',
  templateUrl: './PropostaAvulsa.component.html',
  styleUrls: ['./PropostaAvulsa.component.css'],
})
export class PropostaAvulsaComponent implements OnInit {
  constructor(
    private auth: AuthService,
    public snack: SnackBarService,
    private request: RequestsService,
    private validationBack: ValidationsBackService,
    private loader: LoaderService,
    public cc: CustomConvertsService
  ) {}

  @Input() produto: any;
  @Input() proposalId: any;
  clienteSelecionado: any;
  clients: any[] = [];
  public querys: any;
  timeout: any;
  @Input() formData: any = {
    id: 0,
    cadEquipe: { id: 0 },
    cadProdutos: { id: 0 },
    cadClientes: { id: 0 },
    vendedorId: 0,
    situacao: 0,
    termometro: 0,
    numeroContrato: '',
    valorDivida: 0.0,
    valorContrato: 0.0,
    valorAprovado: 0.0,
    valorAvulso: 0.0,
    valorEntrada: 0.0,
    percDesconto: 0.0,
    dataHoraCadastro: null,
    dataHoraFechamento: null,
    observacao: '',
    vendedorNome: null,
    beneficiarios: [],
    duplicatas: [],
  };
  ngOnInit() {
    if (this.formData.id == 0) {
      const dt = this.cc.ConverteDateBr(new Date());
      this.formData.dataHoraCadastro = dt;
    }
    if (this.formData.id > 0) {
      this.clienteSelecionado = {
        id: this.formData.cadClientes.id,
        texto: this.formData.cadClientes.nome,
      };
    }
  }

  newClient(): void {
    this.cc.OpenNewTab('Clientes/0');
  }

  private validations(payload: any): boolean {
    if (payload.cadClientes?.id == null || payload.cadClientes?.id == 0) {
      this.snack.CreateNotification(
        'Necessário escolher o cliente desta venda'
      );
      return false;
    }
    if (payload.cadProdutos?.id == 0) {
      this.snack.CreateNotification(
        'Necessário escolher o produto desta venda'
      );
      return false;
    }
    return true;
  }
  public async save(): Promise<void> {
    const payload = {
      ...this.formData,
    };

    payload.cadClientes.id = this.clienteSelecionado?.id;
    payload.cadProdutos.id = this.produto.id;
    payload.dataHoraCadastro = this.cc.DataFormatoUs(payload.dataHoraCadastro);
    payload.valorContrato = this.cc.getInvariantValueFromMasked(
      payload.valorContrato
    );
    if (this.validations(payload) == false) return;
    this.loader.toogleLoader();
    const response = await this.request.postData(
      `/MovPropostas/NewCustomSale`,
      payload
    );
    if (Number(response) > 0 && this.proposalId == 0)
      window.location.href = `/Comercial/${response}`;
    this.snack.CreateNotification('Venda criada com sucesso');
    this.loader.toogleLoader();
  }
  buscarCliente(query: string): void {
    clearTimeout(this.timeout); // cancela debounce anterior

    if (!query || typeof query !== 'string') {
      this.clients = [];
      return;
    }

    if (query.length < 3) return;
    // debounce manual (opcional)
    this.timeout = setTimeout(async () => {
      const response = await this.request.getData(
        `/CadClientes/AutoComplete?texto=${query}`
      );
      this.clients = response;
    }, 300);
  }

  displayCliente(cidade: any): string {
    return cidade?.texto || '';
  }
}
