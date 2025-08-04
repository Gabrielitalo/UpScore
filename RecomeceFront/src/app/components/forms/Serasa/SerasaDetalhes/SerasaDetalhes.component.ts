import { Component, OnInit } from '@angular/core';
import { RequestsService } from '../../../../services/Requests.service';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/Auth.service';
import printJS from 'print-js';

@Component({
  selector: 'app-SerasaDetalhes',
  templateUrl: './SerasaDetalhes.component.html',
  styleUrls: ['./SerasaDetalhes.component.css']
})
export class SerasaDetalhesComponent implements OnInit {

  constructor(private request: RequestsService,
    public cc: CustomConvertsService,
    public router: Router,
    private route: ActivatedRoute,
    private auth: AuthService,
  ) {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.markID = idParam ? idParam : '';
  }

  private markID = '';
  public concentre: any;
  public isNadaConsta = false;

  ngOnInit() {
    this.GetConcentre()
  }

  private async GetConcentre(): Promise<void> {
    const response = await this.request.getData(`/MovCadClientesSerasa/GetConcentreMarkID?markID=${this.markID}`)
    this.concentre = response;
    this.isNadaConsta = this.concentre.regA900?.tipoReg == 'A900' ? true : false;
  }

  print() {
    // printJS({
    //   printable: 'printArea',
    //   type: 'html',
    //   targetStyles: ['*'],
    //   style: '@page { size: A4; margin: 20mm; }'
    // });
    const conteudo = document.getElementById('printArea')?.innerHTML;
    const estilo = `
      <style>
      .concentre {
        font-family: 'Inter Tight Regular', sans-serif;
        width: 205mm;
        height: 297mm;
        background: white;
        box-sizing: border-box;
        page-break-after: always;
        font-size: 8px !important;
      }
      
      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        font-size: 14px;
        font-weight: bold;
        margin-bottom: 10px;
        border-bottom: 2px solid #000;
        padding-bottom: 5px;
      }
      
      .title {
        color: #0066b3;
        font-weight: bold;
        font-size: 16px;
        margin: 10px 0;
      }
      
      table {
        width: 100%;
        border-collapse: collapse;
        margin-top: -1px;
        font-size: 11px;
      }
      
      th,
      td {
        border: 1px solid #999;
        padding: 6px;
        vertical-align: top;
        text-align: left;
      }
      
      th {
        font-weight: bold;
      }
      
      .section {
        font-size: 11px;
      }
      
      .blue-label {
        font-weight: bold;
        padding: 6px;
        border: 1px solid #999;
        color: #0066b3;
        margin-top: -1px;
      }
      
      .black-label {
        font-weight: bold;
        padding: 6px;
        border: 1px solid #999;
        color: #020202;
        margin-top: -1px;
      }
      
      .score-section {
        margin-top: 30px;
        text-align: center;
        border: 1px solid #999;
        font-size: 11px;
      }
      
      .score-number {
        font-size: 28px;
        color: #70c9f5;
        font-weight: 500;
        margin-top: -120px;
      }
      
      .chance-text {
        font-size: 14px;
        color: #999;
        margin-top: 30px;
        font-weight: 600;
      }
      
      .image-score {
        margin-top: -45px;
        max-width: 275px;
      }
      
      .serasa-title {
        border: none !important;
        text-align: left;
        border-bottom: 1px solid #999 !important;
      }
      .nada-consta{
        font-size: 16px;
        margin-left: 5px;
        font-weight: 600;
      }
      </style>
    `;
    printJS({
      printable: estilo + conteudo,
      type: 'raw-html'
    });
  }

  public GeraDataBr(data: any): string {
    if (!data || data.length !== 8) return '';

    const dia = data.substring(0, 2);
    const mes = data.substring(2, 4);
    const ano = data.substring(4, 8);

    return `${dia}/${mes}/${ano}`;
  }
  public GeraDataUsBr(data: any): string {
    if (!data || data.length !== 8) return '';

    const ano = data.substring(0, 4);
    const mes = data.substring(4, 6);
    const dia = data.substring(6, 8);

    return `${dia}/${mes}/${ano}`;
  }

  public Back(): void {
    this.router.navigate(['Consultas']);
  }
  public BuscaChancePagamento(score: any): any {
    const ret = this.scoreTable.find(entry => {
      const [min, max] = entry.faixa.match(/\d+/g)?.map(Number) || [];
      return score >= min && score <= max;
    });
    return ret?.descricao;
  }
  public scoreTable = [
    {
      faixa: "Score 0-100",
      risco: "100%",
      descricao: "A chance de um consumidor, com score entre 0 e 100, pagar seus compromissos financeiros nos próximos 12 meses é de 0,00%"
    },
    {
      faixa: "Score 101-200",
      risco: "99,93%",
      descricao: "A chance de um consumidor, com score entre 101 e 200, pagar seus compromissos financeiros nos próximos 12 meses é de 0,07%"
    },
    {
      faixa: "Score 201-300",
      risco: "99,35%",
      descricao: "A chance de um consumidor, com score entre 201 e 300, pagar seus compromissos financeiros nos próximos 12 meses é de 0,65%"
    },
    {
      faixa: "Score 301-400",
      risco: "81,24%",
      descricao: "A chance de um consumidor, com score entre 301 e 400, pagar seus compromissos financeiros nos próximos 12 meses é de 18,76%"
    },
    {
      faixa: "Score 401-500",
      risco: "58,58%",
      descricao: "A chance de um consumidor, com score entre 401 e 500, pagar seus compromissos financeiros nos próximos 12 meses é de 41,42%"
    },
    {
      faixa: "Score 501-600",
      risco: "40,75%",
      descricao: "A chance de um consumidor, com score entre 501 e 600, pagar seus compromissos financeiros nos próximos 12 meses é de 59,25%"
    },
    {
      faixa: "Score 601-700",
      risco: "28,47%",
      descricao: "A chance de um consumidor, com score entre 601 e 700, pagar seus compromissos financeiros nos próximos 12 meses é de 71,53%"
    },
    {
      faixa: "Score 701-800",
      risco: "19,69%",
      descricao: "A chance de um consumidor, com score entre 701 e 800, pagar seus compromissos financeiros nos próximos 12 meses é de 80,31%"
    },
    {
      faixa: "Score 801-900",
      risco: "12,79%",
      descricao: "A chance de um consumidor, com score entre 801 e 900, pagar seus compromissos financeiros nos próximos 12 meses é de 87,21%"
    },
    {
      faixa: "Score 901-1000",
      risco: "5,95%",
      descricao: "A chance de um consumidor, com score entre 901 e 1000, pagar seus compromissos financeiros nos próximos 12 meses é de 94,05%"
    }
  ];

}
