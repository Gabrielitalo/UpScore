import { Component, Input, OnInit } from '@angular/core';
import { CustomConvertsService } from '../../../../../services/CustomConverts.service';

@Component({
  selector: 'app-SerasaRegistrationPJ',
  templateUrl: './SerasaRegistrationPJ.component.html',
  styleUrls: ['./SerasaRegistrationPJ.component.css', '../RelatorioAvancado.component.css']
})
export class SerasaRegistrationPJComponent implements OnInit {

  constructor(
    public cc: CustomConvertsService,

  ) { }

  @Input() data: any;
  @Input() totalDivida = 'back_end';
  @Input() qtdDivida = 'back_end';
  @Input() modelo = 1;

  ngOnInit() {
  }


  public calcularIdade(birthDateStr: any): string {
    const birthDate = new Date(birthDateStr);
    const today = new Date();
  
    const diffTime = today.getTime() - birthDate.getTime();
    const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
  
    const diffYears = today.getFullYear() - birthDate.getFullYear();
    const diffMonths =
      diffYears * 12 +
      (today.getMonth() - birthDate.getMonth()) -
      (today.getDate() < birthDate.getDate() ? 1 : 0);
  
    if (diffMonths < 1) {
      return `${diffDays} dia${diffDays !== 1 ? 's' : ''}`;
    } else if (diffYears < 1) {
      return `${diffMonths} mês${diffMonths !== 1 ? 'es' : ''}`;
    } else {
      let idade = diffYears;
      const mesAtual = today.getMonth();
      const mesNascimento = birthDate.getMonth();
  
      if (
        mesAtual < mesNascimento ||
        (mesAtual === mesNascimento && today.getDate() < birthDate.getDate())
      ) {
        idade--;
      }
  
      return `${idade} ano${idade !== 1 ? 's' : ''}`;
    }
  }
  
}
