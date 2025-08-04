import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { LoaderService } from '../../../../services/loader.service';
import { SnackBarService } from '../../../../services/SnackBar.service';

@Component({
  selector: 'app-NewBatchAssociacao',
  templateUrl: './NewBatchAssociacao.component.html',
  styleUrls: ['./NewBatchAssociacao.component.css']
})
export class NewBatchAssociacaoComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    private loader: LoaderService,
    public snack: SnackBarService,
  ) { }

  public batchs: any;
  public situacoes: any = {
    0: {
      Texto: 'Aguardando pagamento',
      Valor: 0,
      Classe: 'recusado'
    },
    1: {
      Texto: 'Aprovado',
      Valor: 2,
      Classe: 'aprovado'
    },
    2: {
      Texto: 'Ativo',
      Valor: 3,
      Classe: 'aprovado'
    },
    4: {
      Texto: 'Recusado',
      Valor: 4,
      Classe: 'recusado'
    },
  }

  ngOnInit() {
    this.getAvailables()
  }

  private async getAvailables(): Promise<void> {
    const response = await this.request.getData(`/MovLotesAssociacao/GetAllAvailable`)
    this.batchs = response;
    console.log(response);
  }

  public async newBatch(): Promise<void> {
    this.loader.toogleLoader();
    // const filtrados = this.contracts.filter((item: any) => item.checked == true);
    const idsFiltrados = this.batchs
    .filter((item: any) => item.checked == true)
    .map((item: any) => item.id);
  
    console.log(idsFiltrados);
    const respose = await this.request.postData(`/MovLotesAssociacao/NewBatch`, idsFiltrados)
    window.location.reload();
    // console.log(idsFiltrados);
  }

}
