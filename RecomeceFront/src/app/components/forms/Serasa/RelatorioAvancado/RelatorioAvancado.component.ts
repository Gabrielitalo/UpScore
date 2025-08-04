import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/Auth.service';
import printJS from 'print-js';

@Component({
  selector: 'app-RelatorioAvancado',
  templateUrl: './RelatorioAvancado.component.html',
  styleUrls: ['./RelatorioAvancado.component.css']
})
export class RelatorioAvancadoComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private route: ActivatedRoute,
    private auth: AuthService,
  ) {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.id = idParam ? idParam : '';
  }

  private id = '';
  public reportAdv: any;
  public reportData: any;
  public totalDivida = 'back_end';
  public qtdDivida = 'back_end';

  ngOnInit() {
    this.getReport();
  }

  public Back(): void {
    this.router.navigate(['Consultas']);
  }

  private async getReport(): Promise<void> {
    const response = await this.request.getData(`/LogConsultas/${this.id}`);
    if (response) {
      // console.log(response);

      this.reportAdv = response;
      this.reportAdv.personType = this.reportAdv.inscricao.length == 11 ? 1 : 2;

      this.reportData = JSON.parse(response.arquivoRetorno)
      // console.log(this.reportData);

      this.calcDebit();
    }
  }

  public calcDebit(): void {
    const negativeData = this.reportData.reports[0].negativeData;
    const facts = this.reportData.reports[0].facts;
    const totalDivida = negativeData.pefin.summary.balance + negativeData.refin.summary.balance + negativeData.notary.summary.balance + negativeData.check.summary.balance + negativeData.collectionRecords.summary.balance + facts.judgementFilings.summary.balance;
    const qtdDivida = negativeData.pefin.summary.count + negativeData.refin.summary.count + negativeData.notary.summary.count + negativeData.check.summary.count + negativeData.collectionRecords.summary.count + facts.judgementFilings.summary.count;
    this.totalDivida = this.cc.MascaraDecimal(totalDivida);
    this.qtdDivida = qtdDivida;
  }

  print() {
    const conteudo = document.getElementById('printArea')?.innerHTML;
    const estilo = `
      <style>
      .row {
        display: flex;
        flex-wrap: wrap;
        margin-right: -0.5rem;
        margin-left: -0.5rem;
      }

      .serasa-header {
        display: flex;
        align-items: center;
        margin-bottom: 20px;
        gap: 15px;
        font-size: 11px;
      }
      
      .serasa-header p {
        margin: 0;
      }
      
      .serasa-header img {
        width: 110px;
        height: 70px;
      }
      
      .header-content {
        display: flex;
        flex-direction: column;
      }
      
      .container {
        width: 295mm;
        min-height: 295mm;
        height: 100%;
        background: white;
        box-sizing: border-box;
        page-break-after: always;
        font-size: 11px !important;
      }
      
      .section {
        border-bottom: 1px dashed #0066b3;
        padding-bottom: 15px;
      }
      
      .bold {
        font-weight: bold;
      }
      
      .title {
        border-bottom: 1px solid #d5d4d4;
        padding-bottom: 10px;
        margin-bottom: 10px;
      }
      
      .title p {
        margin: 5px;
      }
      
      .title p:first-child {
        color: #0066b3;
        font-weight: bold;
        font-size: 14px;
      }
      
      .blue-color {
        color: #0066b3;
        font-weight: bold;
      }
      
      .serasa-card {
        border: 1px solid #d5d4d4;
        flex: 1;
        border-radius: 5px;
        padding: 10px;
        font-size: 12px;
        min-height: 115px;
      }
      
      .serasa-card p {
        margin: 5px;
        display: flex;
        align-items: center;
      }
      
      .serasa-card mat-icon {
        font-size: 12px;
        height: 12px;
        width: 12px;
        color: #817b7b;
      }
      
      .serasa-card-title {
        font-weight: 600;
      }
      
      .cards-container {
        display: flex;
        align-items: center;
        gap: 5px;
        margin-bottom: 10px;
      }
      
      .chancePagamento {
        border: 1px solid #FBBABA;
        border-radius: 15px;
        color: #F01717;
        padding: 5px;
      }
      
      .space-betweeen {
        display: flex;
        align-items: center;
        justify-content: space-between;
      }
      
      .serasa-table-container {
        border: 1px solid #d5d4d4;
        border-radius: 5px;
        display: flex;
        width: 97.8%;
        flex-direction: column;
        padding-top: 10px;
        padding-bottom: 10px;
        margin-bottom: 10px;
      }
      
      .serasa-table-header {
        border-bottom: 1px solid #d5d4d4;
        display: flex;
        width: 100%;
        align-items: center;
        font-weight: 600;
        font-size: 13px;
      }
      
      .serasa-table-header div {
        flex: 1;
        /* Faz todas as colunas crescerem igualmente */
        border-right: 1px solid #d5d4d4;
        padding: 8px 10px;
        box-sizing: border-box;
        word-break: break-word;
        text-align: left;
      }
      
      /* Remove borda da última coluna */
      .serasa-table-header div:last-child {
        border-right: none !important;
      }
      
      .serasa-table-row {
        border-bottom: 1px solid #d5d4d4;
        display: flex;
        width: 100%;
        align-items: center;
        font-size: 13px;
      }
      
      .serasa-table-row div {
        flex: 1;
        /* Faz todas as colunas crescerem igualmente */
        border-right: 1px solid #d5d4d4;
        padding: 5px 10px;
        box-sizing: border-box;
        word-break: break-word;
        text-align: left;
      }
      
      /* Remove borda da última coluna */
      .serasa-table-row div:last-child {
        border-right: none !important;
      }
      
      
      .card-with-title {
        border: 1px solid #d5d4d4;
        border-radius: 5px;
        display: flex;
        flex-direction: column;
        margin-bottom: 10px;
      }
      
      .card-with-title-title {
        color: #0066b3;
        font-weight: bold;
        font-size: 12px;
        margin: 10px 0;
        padding-left: 5px;
        border-bottom: 1px solid #d5d4d4;
        padding-bottom: 10px;
      }
      
      .card-with-title p {
        margin: 5px;
      }
      
      .card-with-title-body {
        display: flex;
        flex-direction: column;
        padding-left: 15px;
        padding-right: 15px;
      }
      
      .card-with-title-body>.row>.card-info {
        flex: 1;
      }
      
      .row .card-info p:first-child {
        font-weight: bold;
      }
            </style>`;
    printJS({
      printable: estilo + conteudo,
      type: 'raw-html'
    });
  }

}
