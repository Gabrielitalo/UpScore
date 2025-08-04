import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CnpjService {

  constructor(private http: HttpClient,
  ) { }

  public async GetCNPJ(valor: string): Promise<any> {
    const cnpj = valor.replace(/\D/g, '');
  
    if (cnpj.length < 14 || !cnpj) return;
  
    try {
      return await firstValueFrom(
        this.http.get(`http://www.receitaws.com.br/v1/cnpj/${cnpj}`, {
          headers: { 'Accept': 'application/json' }
        })
      );
    } catch (err) {
      console.error('Erro ao buscar CNPJ:', err);
      return null;
    }
  }
  
}
