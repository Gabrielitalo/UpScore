import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CepService {

  constructor(private http: HttpClient,
  ) { }


  public async GetCEP(valor: string): Promise<any> {
    //Nova variável "cep" somente com dígitos.
    var cep = valor.replace(/\D/g, '');
    if (valor.length < 8)
      return;
    //Verifica se campo cep possui valor informado.
    if (cep != "") {

      //Expressão regular para validar o CEP.
      var validacep = /^[0-9]{8}$/;

      //Valida o formato do CEP.
      if (validacep.test(cep)) {

        return await firstValueFrom(
          this.http.get(`https://viacep.com.br/ws/${cep}/json/`)
        );
      } //end if.
      else {
        return { code: 500, msg: 'Formato de CEP inválido' }
      }
    }
  }
}
