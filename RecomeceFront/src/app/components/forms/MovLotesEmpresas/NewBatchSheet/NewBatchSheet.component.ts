import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { LoaderService } from '../../../../services/loader.service';
import { AuthService } from '../../../../services/Auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-NewBatchSheet',
  templateUrl: './NewBatchSheet.component.html',
  styleUrls: ['./NewBatchSheet.component.css']
})
export class NewBatchSheetComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public auth: AuthService,
    private loader: LoaderService,
    public snack: SnackBarService,
    public router: Router
  ) { }

  public companys: any = [];
  public formData: any = {
    tipo: 1,
    companyId: 0
  };
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  ngOnInit() {
    this.getCompanys();
    this.formData.companyId = +this.auth.getCompany();
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    const reader = new FileReader();

    reader.onload = () => {
      const base64 = (reader.result as string).split(',')[1];
      const payload = {
        base64: base64,
        fileName: file.name,
        tipo: this.formData.tipo
      };
      this.upload(payload);
    };

    reader.readAsDataURL(file);
  }

  public async getCompanys(): Promise<void> {
    const request = await this.request.getData(`/CadEmpresas/GetAllCompany`)
    this.companys = request;
  }
  public async upload(payload: any): Promise<void> {
    // console.log(payload);
    this.loader.toogleLoader();

    const request = await this.request.postData(`/MovLotesEmpresas/NewBatchXlsx?companyId=${this.formData.companyId}`, payload)
    console.log(request);
    if (request?.id) {
      this.router.navigate([`/Lotes/${request?.id}`])
      this.snack.CreateNotification('Sucesso ao criar lote');
    }
    else {
      this.snack.CreateNotification('Houve um problema ao processar o lote, tente novamente em alguns minutos');
    }
    // console.log(request);
    this.loader.toogleLoader();
  }

  selectFile(id: any) {
    this.fileInput.nativeElement.value = ''; // limpa o valor anterior
    this.fileInput.nativeElement.click(); // abre seletor de arquivos
  }
}
