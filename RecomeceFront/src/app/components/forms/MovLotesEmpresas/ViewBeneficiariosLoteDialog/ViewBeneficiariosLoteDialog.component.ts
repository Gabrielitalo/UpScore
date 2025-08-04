import { Component, Inject, Input, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-ViewBeneficiariosLoteDialog',
  templateUrl: './ViewBeneficiariosLoteDialog.component.html',
  styleUrls: ['./ViewBeneficiariosLoteDialog.component.css']
})
export class ViewBeneficiariosLoteDialogComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    public snack: SnackBarService,
    public router: Router,
    public cc: CustomConvertsService,
    private request: RequestsService) {
    this.id = this.route.snapshot.paramMap.get('id');
  }

  public baseClients: any;
  public batch: any;
  @Input() id: any = 0;
  @Input() isDialog = false;
  public clients: any;
  public filtros: any = {
    membro: '',
    inscricao: '',
    dataInicial: '',
    dataFinal: '',
    situacao: 2
  }

  ngOnInit() {
    this.getBatch()
    this.getClients();
  }

  public Back(): void {
    this.router.navigate(['Lotes']);
  }

  private async getBatch(): Promise<void> {
    const response = await this.request.getData(`/MovLotesEmpresas/${this.id}`)
    this.batch = response;
    // console.log(response);
  }

  public getSituacao(row: any): any {
    if (row == 0)
      return 'Aguardando Pagamento'
    else if (row == 1)
      return 'Processado'
  }

  private async getClients(): Promise<void> {
    if (this.id == null)
      return;

    const response = await this.request.getData(`/MovLotesEmpresasBeneficiarios/GetAll?batchId=${this.id}`)
    this.baseClients = response;
    this.clients = response;
    // this.changeFilter()
  }

  public situacoes: any = {
    0: {
      Texto: 'Inválido',
      Valor: 0,
      Classe: 'recusado'
    },
    1: {
      Texto: 'Válido',
      Valor: 1,
      Classe: 'aprovado'
    },
  }

  public changeFilter(): void {
    const situacoes = this.filtros.situacao == 2
    ? [0, 1]
    : [+this.filtros.situacao];

    let filter = this.baseClients.filter((f: any) => situacoes.includes(f.situacao));
    if(this.filtros.membro.length > 0){
      filter = filter.filter((f: any) => 
      (f.nome.toLowerCase().includes(this.filtros.membro.toLowerCase())) ||
      (f.inscricaoFormatada.toLowerCase().includes(this.filtros.membro.toLowerCase())) 
      )
    }
    this.clients = filter;
  }
  public deleteRow(row: any): void {
    console.log(row);
  }

  public async saveRow(row: any): Promise<void> {
    if (this.cc.validarCpfCnpj(row.inscricaoFormatada) == false) {
      this.snack.CreateNotification('CPF ou CNPJ informado é inválido');
      return
    }
    row.isEditing = null;
    const response = await this.request.getData(`/MovLotesEmpresasBeneficiarios/Update?id=${row.id}&inscricao=${row.inscricaoFormatada}&nome=${row.nome}`)
    this.snack.CreateNotification('Pessoa atualizada com sucesso');
    await this.getClients();
    this.changeFilter();
  }

  public editRow(row: any): void {
    row.isEditing = true;
  }
}
