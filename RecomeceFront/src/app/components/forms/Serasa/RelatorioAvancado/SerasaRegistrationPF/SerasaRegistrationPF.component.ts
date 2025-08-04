import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-SerasaRegistrationPF',
  templateUrl: './SerasaRegistrationPF.component.html',
  styleUrls: [
    './SerasaRegistrationPF.component.css',
    '../RelatorioAvancado.component.css',
  ],
})
export class SerasaRegistrationPFComponent implements OnInit {
  constructor(public cc: CustomConvertsService) {}
  @Input() data: any;
  @Input() totalDivida = 'back_end';
  @Input() qtdDivida = 'back_end';
  @Input() modelo = 1;

  ngOnInit() {
    // console.log(this.data);
  }

  public calcularIdade(birthDateStr: any) {
    const birthDate = new Date(birthDateStr);
    const today = new Date();

    let idade = today.getFullYear() - birthDate.getFullYear();
    const mesAtual = today.getMonth();
    const mesNascimento = birthDate.getMonth();

    if (
      mesAtual < mesNascimento ||
      (mesAtual === mesNascimento && today.getDate() < birthDate.getDate())
    ) {
      idade--;
    }

    return idade;
  }

  public getGenero(genero: string): any {
    if (this.modelo == 2) return 'SEM DADOS';
    if(genero == '') return 'SEM DADOS';
    return genero == 'M' ? 'Masculino' : 'Feminino';
  }

  public getPhone(row: any): any {
    const phone = `${row.areaCode}${row.phoneNumber}`;
    return this.cc.MascaraTelefone(phone);
  }
  public getPhoneType(row: any): any {
    return row == 'Commercial Phone' ? 'Comercial' : 'Residencial';
  }

  public getFullAddress(row: any): any {
    return `${row.addressLine}-${row.district}-${row.city}-${row.state},${row.zipCode}`;
  }

  public BuscaChancePagamento(score: any, rate: any): any {
    const ret = this.scoreTable.find((entry) => {
      const [min, max] = entry.faixa.match(/\d+/g)?.map(Number) || [];
      return score >= min && score <= max;
    });
    return `${ret?.descricao}${this.calcRisk(rate)}%`;
  }

  public calcRisk(risk: any): any {
    const r = this.cc.DecimalInvariant(risk);
    const riskNew = 100 - r;
    return this.cc.MascaraDecimal(riskNew);
  }
  public scoreTable = [
    {
      faixa: 'Score 0-100',
      risco: '100%',
      descricao: `A chance de um consumidor, com score entre 0 e 100, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 101-200',
      risco: '99,93%',
      descricao: `A chance de um consumidor, com score entre 101 e 200, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 201-300',
      risco: '99,35%',
      descricao: `A chance de um consumidor, com score entre 201 e 300, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 301-400',
      risco: '81,24%',
      descricao: `A chance de um consumidor, com score entre 301 e 400, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 401-500',
      risco: '58,58%',
      descricao: `A chance de um consumidor, com score entre 401 e 500, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 501-600',
      risco: '40,75%',
      descricao: `A chance de um consumidor, com score entre 501 e 600, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 601-700',
      risco: '28,47%',
      descricao: `A chance de um consumidor, com score entre 601 e 700, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 701-800',
      risco: '19,69%',
      descricao: `A chance de um consumidor, com score entre 701 e 800, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 801-900',
      risco: '12,79%',
      descricao: `A chance de um consumidor, com score entre 801 e 900, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
    {
      faixa: 'Score 901-1000',
      risco: '5,95%',
      descricao: `A chance de um consumidor, com score entre 901 e 1000, pagar seus compromissos financeiros nos próximos 12 meses é de `,
    },
  ];
}
