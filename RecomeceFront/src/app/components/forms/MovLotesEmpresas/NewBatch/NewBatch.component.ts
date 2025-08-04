import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { SnackBarService } from '../../../../services/SnackBar.service';
import { LoaderService } from '../../../../services/loader.service';

@Component({
  selector: 'app-NewBatch',
  templateUrl: './NewBatch.component.html',
  styleUrls: ['./NewBatch.component.css']
})
export class NewBatchComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    private loader: LoaderService,
    public snack: SnackBarService,
  ) { }

  public contracts: any;
  ngOnInit() {
    this.getContracts();
  }

  private async getContracts(): Promise<void> {
    const respose = await this.request.getData(`/MovLotesEmpresas/GetAllContractsAvailables`)
    this.contracts = respose;
  }

  public async newBatch(): Promise<void> {
    this.loader.toogleLoader();
    // const filtrados = this.contracts.filter((item: any) => item.checked == true);
    const idsFiltrados = this.contracts
    .filter((item: any) => item.checked == true)
    .map((item: any) => item.id);
  
    const respose = await this.request.postData(`/MovLotesEmpresas/NewCompanyBatch`, idsFiltrados)
    window.location.reload();
    // console.log(idsFiltrados);
  }

}
