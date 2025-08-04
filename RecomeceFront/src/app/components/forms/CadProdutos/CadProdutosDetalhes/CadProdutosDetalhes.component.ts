import { Component, Inject, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { ValidationsBackService } from '../../../../services/validations-back.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { LoaderService } from '../../../../services/loader.service';

@Component({
  selector: 'app-CadProdutosDetalhes',
  templateUrl: './CadProdutosDetalhes.component.html',
  styleUrls: ['./CadProdutosDetalhes.component.css']
})
export class CadProdutosDetalhesComponent implements OnInit {

  constructor(
    private auth: AuthService,
    public snack: SnackBarService,
    private request: RequestsService,
    private validationBack: ValidationsBackService,
    private dialogRef: MatDialogRef<CadProdutosDetalhesComponent>,
    private loader: LoaderService,
    public cc: CustomConvertsService,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) { }


  public isLoading = true;
  public cadProdutos: any = {
    id: 0,
    cadEmpresas: {
      id: 0,
    },
    situacao: 1,
    descricao: '',
    custo: 0,
    limpaNome: 1,
    dataInicial: '',
    tipoConsulta: 1
  };

  public cadProdutosFaixas: any = {
    id: 0,
    cadProdutos: {
      id: 0
    },
    valorDivida: 0,
    valorServico: 0,
    valorEntradaMinima: 0
  };

  public cadProdutosFaixasPagamento: any = {
    id: 0,
    cadProdutosFaixas: {
      id: 0
    },
    tipo: 1,
    modo: 2,
    valorMaxParcelas: 2,
    descricao: ''
  };

  public tempFaixa: any;

  public newProduct(): any {

  }


  ngOnInit() {
    const id = this.data.id;
    if (id > 0) {
      this.cadProdutos.Id = id;
      this.getData(id);
    }
    else {
      this.isLoading = false;
    }
  }

  public async getData(id: number): Promise<void> {
    this.isLoading = true;
    const response = await this.request.getData(`/CadProdutos/GelAllRanges?id=${id}`);
    this.isLoading = false;
    const obj = JSON.parse(response);
    // console.log(obj);
    if (obj.id > 0) {
      this.cadProdutos = obj;
      this.cadProdutos.custo = this.cc.MascaraDecimal(this.cadProdutos.custo);
      this.cadProdutosFaixas.cadProdutos.id = this.data.id;
      for (let index = 0; index < this.cadProdutos.faixas?.length; index++) {
        this.cadProdutos.faixas[index].valorDivida = this.cc.MascaraDecimal(this.cadProdutos.faixas[index].valorDivida);
        this.cadProdutos.faixas[index].valorServico = this.cc.MascaraDecimal(this.cadProdutos.faixas[index].valorServico);
        this.cadProdutos.faixas[index].valorEntradaMinima = this.cc.MascaraDecimal(this.cadProdutos.faixas[index].valorEntradaMinima);
      }

    }
  }

  public async Salvar(): Promise<void> {
    const payload = {
      ...this.cadProdutos,
      situacao: this.cadProdutos.situacao ? 1 : 0,
      custo: this.cc.getInvariantValueFromMasked(this.cadProdutos.custo),
    };

    // console.log(payload);return;
    const response = await this.request.PostPut(`/CadProdutos`, payload);
    if (response > 0 || response?.id > 0) {
      this.snack.CreateNotification('Sucesso ao realizar operação');
      this.dialogRef.close();
      await this.getData(this.data.id);
    } else {
      this.validationBack.openDialog(response);
      this.snack.CreateNotification('Não foi possível realizar a operação, tente novamente mais tarde');
    }
  }

  public NewFaixa(): void {
    this.cadProdutosFaixas.cadProdutosFaixasPagamento = [];
    this.cadProdutosFaixas.cadProdutosFaixasPagamento.push(this.cadProdutosFaixasPagamento)
    this.tempFaixa = this.cadProdutosFaixas
    // console.log(this.tempFaixa);
  }

  public NewRule(): void {
    const temp = { ...this.cadProdutosFaixasPagamento }; // Clonagem por spread
    temp.tipo = 2;
    this.tempFaixa.cadProdutosFaixasPagamento.push(temp);
    // console.log(this.tempFaixa);
  }


  public async saveRow(): Promise<void> {
    const temp = { ...this.tempFaixa }; // Clonagem por spread
    temp.valorDivida = this.cc.getInvariantValueFromMasked(temp.valorDivida);
    temp.valorServico = this.cc.getInvariantValueFromMasked(temp.valorServico);
    temp.valorEntradaMinima = this.cc.getInvariantValueFromMasked(temp.valorEntradaMinima);
    const ranges = [...this.tempFaixa.cadProdutosFaixasPagamento]

    const obj = {
      cadProdutosFaixas: temp,
      cadProdutosFaixasPagamento: ranges
    }
    const response = await this.request.postData(`/CadProdutosFaixas/NewRange`, obj);
    // console.log(response);
    this.tempFaixa = null;
    this.getData(this.data.id)
  }

  public async AlteraValorFaixa(row: any): Promise<void> {
    const temp = { ...row }; // Clonagem por spread
    temp.valorDivida = this.cc.getInvariantValueFromMasked(temp.valorDivida);
    temp.valorServico = this.cc.getInvariantValueFromMasked(temp.valorServico);
    temp.valorEntradaMinima = this.cc.getInvariantValueFromMasked(temp.valorEntradaMinima);
    const response = await this.request.putData(`/CadProdutosFaixas`, temp);
    this.snack.CreateNotification('Sucesso ao alterar informação');
  }
  public async AlteraValorFaixaCond(row: any): Promise<void> {
    const temp = { ...row }; // Clonagem por spread
    temp.modo = +temp.modo;
    temp.tipo = +temp.tipo;
    temp.valorMaxParcelas = +temp.valorMaxParcelas;
    const response = await this.request.putData(`/CadProdutosFaixasPagamento`, temp);
    this.snack.CreateNotification('Sucesso ao alterar condição de pagamento');
  }
  public async NovaCondicao(row: any): Promise<void> {
    const temp = { ...this.cadProdutosFaixasPagamento }; // Clonagem por spread
    temp.tipo = 2;
    temp.cadProdutosFaixas.id = row.id;
    const response = await this.request.postData(`/CadProdutosFaixasPagamento`, temp);
    this.getData(this.data.id);
  }

  public async deleteRow(row: any, sub: any): Promise<void> {
    await this.request.deleteData(`/CadProdutosFaixasPagamento/${sub.id}`);
    const index = row.pagamentos.indexOf(sub);
    if (index > -1)
      row.pagamentos.splice(index, 1);
  }

  public deleteTempRow(row: any) {
    const index = this.tempFaixa.cadProdutosFaixasPagamento.indexOf(row);
    if (index > -1)
      this.tempFaixa.cadProdutosFaixasPagamento.splice(index, 1);
  }
}
