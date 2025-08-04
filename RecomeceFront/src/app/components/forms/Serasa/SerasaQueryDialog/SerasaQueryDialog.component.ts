import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/Auth.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { MatDialog } from '@angular/material/dialog';
import { LoaderService } from '../../../../services/loader.service';

@Component({
  selector: 'app-SerasaQueryDialog',
  templateUrl: './SerasaQueryDialog.component.html',
  styleUrls: ['./SerasaQueryDialog.component.css']
})
export class SerasaQueryDialogComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private route: ActivatedRoute,
    private auth: AuthService,
    public snack: SnackBarService,
    private dialog: MatDialog,
    private loader: LoaderService,
  ) { }

  public formData: any = {
    inscricao: '',
    productId: 0,
    querys: {},
    products: {},
    productChoosed: {},
    personType: 1
  }
  ngOnInit() {
    // this.getQtd();
    this.getQuerys();
  }

  public async getQtd(): Promise<void> {
    const response = await this.request.getData(
      `/MovConsultasCompradas/GetAvailable`
    );
    this.formData.querys = response;
    // console.log(response);
  }

  public async getQuerys(): Promise<void> {
    const response = await this.request.getData(`/CadConsultas/GetAll?page=0&itensPerPage=50`);
    this.formData.products = response.itens;
    // console.log(this.formData.products);

  }

  public setProduct(row: any): void {
    this.formData.productChoosed = row;
    if (row.id == 1 || row.id == 3 || row.id == 5 || row.id == 7)
      this.formData.personType = 1;
    else
      this.formData.personType = 2;

  }
  public async newQuery(): Promise<void> {
    this.loader.toogleLoader();
    if (this.cc.validarCpfCnpj(this.formData.inscricao) == false) {
      this.snack.CreateNotification('Necessário informar um CPF ou CNPJ válido');
      return;
    }

    const response = await this.request.getData(
      `/LogConsultas/NewQuery?inscricao=${this.formData.inscricao}&productId=${this.formData.productChoosed.id}`
    );

    console.log(response);

    if (response == '401') {
      this.snack.CreateNotification('Você não possui saldo de consultas, realize a compra de novas consultas e tente novamente');
      this.loader.toogleLoader();
      return;
    }
    // console.log(response);
    // window.location.href = `RelatorioAvancado/${response.id}`
    if (response.tipo == 2)
      window.location.href = `RelatorioBoaVista/${response.id}`
    else if (response.tipo == 1) {
      if (response.consultaId == 7 || response.consultaId == 8)
        window.location.href = `RelatorioBasico/${response.id}`
      else
        window.location.href = `RelatorioAvancado/${response.id}`
    }
    else
      window.location.href = `Consultas/${response.markID}`
  }
}
