import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { ValidationsBackService } from '../../../../services/validations-back.service';
import { LoaderService } from '../../../../services/loader.service';
import { DeviceService } from '../../../../services/device.service';

@Component({
  selector: 'app-MovPropostasDetalhes',
  templateUrl: './MovPropostasDetalhes.component.html',
  styleUrls: ['./MovPropostasDetalhes.component.css'],
})
export class MovPropostasDetalhesComponent implements OnInit {
  constructor(
    private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private route: ActivatedRoute,
    private validationBack: ValidationsBackService,
    public auth: AuthService,
    public snack: SnackBarService,
    private loader: LoaderService,
    private dialog: MatDialog,
    private deviceService: DeviceService
  ) {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.proposalId = idParam ? Number(idParam) : 0;
  }

  public origens: any = [];
  public proposalId = 0;
  public isOwnerBenefit = true;
  public products: any = [];
  public tempInsc = '';
  isMobile = false;
  public equipe: any = [];
  cidadeSelecionada: any;
  productChoosed: any;
  cidades: any[] = [];
  timeout: any;
  desconto: any = 0;
  tipoDesconto: string = '2';
  public diagnosticoCobrado = false;
  public totals = {
    totalServico: 0,
    totalDivida: 0,
    valorDesconto: 0,
    valorServicoLiquido: 0,
    valorParcelas: 0,
  };
  public formData: any = {
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
    cadOrigensId: 0,
    beneficiarios: [],
    duplicatas: [],
  };
  public pagto: any = {
    condPagtoEntrada: '3',
    dataEntrada: '',
    valorEntrada: 0,
    condPagtoParcela: '2',
    dataPrimeiraParcela: '',
    numeroParcela: 2,
  };

  ngOnInit() {
    this.deviceService.isMobile$.subscribe(value => {
      this.isMobile = value;
    });
    this.pagto.dataEntrada = this.cc.RetornaDataAtualBr();
    this.bindProducts();
    this.getOrigens();
  }

  public async getOrigens(): Promise<void> {
    const response = await this.request.getData(`/MovPropostas/GetOrigens`);
    this.origens = response;
  }

  public Back(): void {
    this.router.navigate(['Comercial']);
  }

  private async bindProducts(): Promise<void> {
    const response = await this.request.getData(`/CadProdutos/GelAllActive`);
    this.products = response;
    // console.log(this.products);
    await this.getEquipe();
    await this.getProposal();
  }

  public canEdit(): boolean {
    if (this.formData.situacao == 3 || this.formData.situacao == 4)
      return false;
    else return true;
  }

  private async getProposal(): Promise<void> {
    if (this.proposalId == 0) return;
    const response = await this.request.getData(
      `/MovPropostas/${this.proposalId}`
    );
    const json = JSON.parse(response[0]?.proposta);
    if (json) {
      this.changeProduct(json.ProdutoId);
      this.formData.valorAvulso = this.cc.MascaraDecimal(json.ValorAvulso);
      if (json.ValorAvulso > 0) this.diagnosticoCobrado = true;

      // console.log(json);
      this.formData.id = json.Id;
      this.formData.situacao = json.Situacao;
      this.formData.cadProdutos.id = json.ProdutoId;
      this.formData.cadEquipe.id = json.VendedorId;
      this.formData.cadClientes.id = json.TitularId;
      this.formData.beneficiarios = json.Beneficiarios;
      this.formData.vendedorNome = json.VendedorNome;
      this.formData.cadClientes.nome = json.TitularNome;
      this.formData.cadOrigensId = json.CadOrigensId;
      this.formData.valorContrato = this.cc.MascaraDecimal(json.ValorContrato);
      this.formData.observacao = json.Observacao;
      this.formData.dataHoraCadastro = this.cc.DataTempoFormatoBr(json.DataCadastro);
      if (json.ValorAprovado > 0) this.desconto = json.Desconto;
      this.totals.totalServico = json.ValorContrato;
      this.totals.valorServicoLiquido = json.ValorAprovado;
      this.aplicarDesconto(false);
      await this.getPayments();
      this.TotalizarDivida();
      this.TotalizarParcelas();
    } else {
      console.log('Erro ao obter proposta...');
    }
  }

  public changeProduct(ev: any): void {
    const product = this.products.filter((d: any) => d.id == ev);
    if (product) this.productChoosed = product[0];
  }
  public async changeOrigem(ev: any): Promise<void> {
    // Descomentar
    // if (this.canEdit() == false) {
    //   this.snack.CreateNotification('Não é possível alterar a origem');
    //   return;
    // }
    const response = await this.request.getData(
      `/MovPropostas/UpdateOrigem?proposalId=${this.proposalId}&origemId=${ev}`
    );
  }

  public async newProposal(): Promise<void> {
    this.request.getData(`/Proposal/ProposalNew?productID=${this.formData.cadProdutos.id}&insc=${this.tempInsc}&proposalId=${this.proposalId}`);

  }
  public async newBeneficiario(): Promise<void> {
    // const endPoint =
    //   this.proposalId == 0
    //     ? `/MovPropostas/NewProposal?productID=${this.formData.cadProdutos.id}&insc=${this.tempInsc}`
    //     : `/MovPropostas/AddClient?proposalId=${this.proposalId}&insc=${this.tempInsc}`;

    if (this.formData.cadProdutos.id == 0) {
      this.snack.CreateNotification('Escolha um produto para gerar proposta');
      return;
    }

    if (this.cc.validarCpfCnpj(this.tempInsc) == false) {
      this.snack.CreateNotification(
        'Necessário informar um CPF ou CNPJ válido'
      );
      return;
    }

    this.loader.toogleLoader();
    const response: any = await this.request.getData(`/Proposal/ProposalNew?productID=${this.formData.cadProdutos.id}&insc=${this.tempInsc}&proposalId=${this.proposalId}&forceNew=false`);

    if (response?.type == 3) {
      this.validationBack.openDialog(response);

      if(response.item.redirect == 'saldo'){
        const baseUrl = window.location.origin;
        const url = `${baseUrl}/FinanceiroConsultas/1`;
        window.open(url, '_blank');
      }
    }
    // console.log(response);
    if (Number(response) > 0 && this.proposalId == 0)
      window.location.href = `/Comercial/${response}`;

    this.loader.toogleLoader();
    this.tempInsc = '';
    await this.getProposal();
  }

  public TotalizarDivida(): any {
    let total = 0;
    for (let index = 0; index < this.formData.beneficiarios?.length; index++) {
      const element = this.formData.beneficiarios[index];
      total += element.ValorDivida;
    }
    this.totals.totalDivida = total;
  }
  public TotalizarParcelas(): any {
    let total = 0;
    for (let index = 0; index < this.formData.duplicatas?.length; index++) {
      const element = this.formData.duplicatas[index];
      total += element.valor;
    }
    this.totals.valorParcelas = total;
  }
  public TotalizarContrato(): any {
    let total = 0;
    for (let index = 0; index < this.formData.beneficiarios.length; index++) {
      const element = this.formData.beneficiarios[index];
      total += element.ValorContrato;
    }
    this.totals.totalServico = total;
    this.totals.valorServicoLiquido = total;
  }
  aplicarDesconto(save: boolean) {
    let desc = this.desconto;
    if (save == true) desc = this.cc.getInvariantValueFromMasked(this.desconto);

    if (this.tipoDesconto === '1') {
      this.totals.valorDesconto = this.totals.totalServico * desc;
    } else {
      this.totals.valorDesconto = desc;
    }
    this.totals.valorServicoLiquido =
      this.totals.totalServico - this.totals.valorDesconto;
    if (save == true) this.SalvarDesconto();
  }
  public async SalvarDesconto(): Promise<void> {
    const obj = {
      Id: this.proposalId,
      ValorAprovado: this.totals.valorServicoLiquido,
      ValorContrato: this.totals.totalServico,
      ValorDivida: this.totals.totalDivida,
    };
    const response = await this.request.putData(
      `/MovPropostas/UpdateTotals`,
      obj
    );
    this.snack.CreateNotification('Desconto aplicado com sucesso');
  }
  public async getCondPagt(): Promise<void> {
    const response = await this.request.getData(
      `/CadProdutosFaixas/GetProductRangeDebit?productId=${this.formData.cadProdutos.id}&value=${this.totals.totalDivida}`
    );
    // console.log(response);
  }
  public async generatePayments(): Promise<void> {
    this.loader.toogleLoader();
    const obj = {
      condPagtoEntrada: +this.pagto.condPagtoEntrada,
      dataEntrada: this.cc.DataFormatoUs(this.pagto.dataEntrada),
      valorEntrada: this.cc.getInvariantValueFromMasked(
        this.pagto.valorEntrada
      ),
      condPagtoParcela: this.pagto.condPagtoParcela,
      dataPrimeiraParcela: this.cc.DataFormatoUs(
        this.pagto.dataPrimeiraParcela
      ),
      numeroParcela: +this.pagto.numeroParcela,
    };
    const response = await this.request.postData(
      `/MovPropostasDuplicatas/GeneratePayments?proposalId=${this.proposalId}`,
      obj
    );
    this.loader.toogleLoader();
    this.validationBack.openDialog(response);
    await this.getProposal();
  }
  public async getPayments(): Promise<void> {
    const response = await this.request.getData(
      `/MovPropostasDuplicatas/GetAll?proposalId=${this.proposalId}`
    );
    this.formData.duplicatas = response?.itens;
  }

  public async updateOwner(row: any): Promise<void> {
    const atualTitular = this.formData.beneficiarios.find(
      (b: any) => b.IsTitular === 1
    );
    if (atualTitular) {
      atualTitular.IsTitular = 0;
    }
    row.IsTitular = 1;
    this.loader.toogleLoader();
    const response = await this.request.getData(
      `/MovPropostas/UpdateOwner?proposalId=${this.proposalId}&clientId=${row.IdCliente}`
    );
    this.snack.CreateNotification('Titular da proposta alterado com sucesso');
    this.loader.toogleLoader();
  }

  public async deleteMember(row: any): Promise<void> {
    this.loader.toogleLoader();
    await this.request.deleteData(`/MovPropostasBeneficiarios/${row.Id}`);
    this.loader.toogleLoader();
    this.snack.CreateNotification(
      `Beneficiário ${row.Nome} removido com sucesso`
    );
    this.getProposal();
  }
  public viewSerasa(row: any): void {
    const baseUrl = window.location.origin;
    let url;
    console.log(row);
    if (row.legado == 1)
      url = `${baseUrl}/Consultas/${row.markID}`;
    else
      url = `${baseUrl}/RelatorioAvancado/${row.id}`;

    window.open(url, '_blank');
  }
  private async getEquipe(): Promise<void> {
    this.equipe = await this.request.getData(
      `/CadEquipe/GetAllCurrentCompanyAsync`
    );
  }
  public async updateVendedor(): Promise<void> {
    this.loader.toogleLoader();
    const response = await this.request.getData(
      `/MovPropostas/UpdateVendedor?proposalId=${this.proposalId}&equipeId=${this.formData.cadEquipe.id}`
    );
    this.snack.CreateNotification(
      'Vendedor responsável da proposta alterado com sucesso'
    );
    this.loader.toogleLoader();
  }

  public async updateAvulso(): Promise<void> {
    const valorAvulso = this.cc.getInvariantValueFromMasked(
      this.formData.valorAvulso
    );
    const response = await this.request.getData(
      `/MovPropostas/UpdateValorAvulso?proposalId=${this.proposalId}&valorAvulso=${valorAvulso}`
    );
  }
  public async updateSaleDate(): Promise<void> {
    if (!this.auth.isAdm()) {
      this.snack.CreateNotification('Você não tem permissão para alterar a data, solicita a um administrador');
      return;
    }
    const dataHoraCadastro = this.cc.ConvertDataTempoBrToUs(this.formData.dataHoraCadastro);
    const response = await this.request.getData(
      `/MovPropostas/UpdateDate?proposalId=${this.proposalId}&date=${dataHoraCadastro}`
    );
    console.log(response);

    this.snack.CreateNotification('Data da proposta foi alterada');
  }

  public changeCobrouDiagnostico(ev: any): void {
    if (ev.checked == false) {
      this.formData.valorAvulso = '0,00';
      this.updateAvulso();
    }
  }
}
