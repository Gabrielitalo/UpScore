import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../services/CustomConverts.service';

@Component({
  selector: 'app-RelatorioAcerta',
  templateUrl: './RelatorioAcerta.component.html',
  styleUrls: ['./RelatorioAcerta.component.css', '../RelatorioBoaVista/RelatorioBoaVista.component.css']
})
export class RelatorioAcertaComponent implements OnInit {

  @Input() reportData: any;

  constructor(
    public cc: CustomConvertsService,

  ) { }

  ngOnInit() {
    // console.log(this.reportData.arquivoRetorno);
  }


  public getGenero(): any {
    const g = this.reportData.arquivoRetorno.identificacao.sexoConsultado;
    return g == '1' ? 'MASCULINO' : g == 2 ? 'FEMININO' : 'NÃO INFORMADO'
  }

  public getQuerysCount(): any {
    return (
      +this.reportData.arquivoRetorno?.resumoConsultas_anteriores_90_dias?.total_1 +
      +this.reportData.arquivoRetorno?.resumoConsultas_anteriores_90_dias?.total_2 +
      +this.reportData.arquivoRetorno?.resumoConsultas_anteriores_90_dias?.total_3 +
      +this.reportData.arquivoRetorno?.resumoConsultas_anteriores_90_dias?.total_4
    )
  }

  getTipoDescricao(tipo: string): string {
    const tipos: { [key: string]: string } = {
      'CC': 'Cartão de Crédito',
      'CD': 'Crédito Direto',
      'CH': 'Cheque',
      'CP': 'Crédito Pessoal',
      'CV': 'Crédito Veículos',
      'OU': 'Outros'
    };
  
    return tipos[tipo] || 'Tipo desconhecido';
  }
}
