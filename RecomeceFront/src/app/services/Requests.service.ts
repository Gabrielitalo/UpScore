import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './Auth.service';
import { take, lastValueFrom, firstValueFrom } from 'rxjs';
import { ConfigsService } from './configs.service';

@Injectable()
export class RequestsService {

  // private baseUrl = 'https://localhost:7240/api';
   private baseUrl = 'https://api.sistemalimpanome.com.br/api';
  // private baseUrl = 'http://191.252.92.62/api';

  //IP Site: 	170.81.43.111
  constructor(private http: HttpClient,
    private auth: AuthService,
    private configs: ConfigsService
  ) { }

  get(endPoint: string): any {
    return this.http.get(`${this.baseUrl}${endPoint}`,
      {
        headers: {
          Authorization: `Basic ${this.auth.getToken()}`,
        }
      });
  }

  post(endPoint: string, bd: any) {
    return this.http.post(`${this.baseUrl}${endPoint}`, bd,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Basic ${this.auth.getToken()}`,
        }
      }
    );
  }

  upload(endPoint: string, fd: FormData): any {
    return this.http.post(`${this.baseUrl}${endPoint}`, fd,
      {
        headers: {
          Authorization: `Basic ${this.auth.getToken()}`,
        }
      }
    );
  }

  download(endPoint: string): any {
    return this.http.get(`${this.baseUrl}${endPoint}`,
      {
        headers: {
          Authorization: `Basic ${this.auth.getToken()}`,
        },
        responseType: 'blob'
      });
  }

  put(endPoint: string, bd: any): any {
    return this.http.put(`${this.baseUrl}${endPoint}`, bd,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Basic ${this.auth.getToken()}`,
        }
      }
    );
  }

  delete(endPoint: string): any {
    return this.http.delete(`${this.baseUrl}${endPoint}`,
      {
        headers: {
          Authorization: `Basic ${this.auth.getToken()}`,
        }
      });
  }

  async getData(endPoint: string): Promise<any> {
    const token = this.auth.getToken();
    if (!token) // Caso não tenha token não pode avançar
      window.location.href = 'Login'

    return await firstValueFrom(
      this.http.get(`${this.baseUrl}${endPoint}`, {
        headers: {
          Authorization: `Basic ${token}`,
        }
      })
    );
  }

  async postData(endPoint: string, bd: any): Promise<any> {
    return await firstValueFrom(
      this.http.post(`${this.baseUrl}${endPoint}`, bd,
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Basic ${this.auth.getToken()}`,
          },
        })
    );
  }

  async postDataBlob(endPoint: string, bd: any): Promise<any> {
    return await firstValueFrom(
      this.http.post(`${this.baseUrl}${endPoint}`, bd,
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Basic ${this.auth.getToken()}`,
          },
          responseType: 'blob'
        })
    );
  }

  async putData(endPoint: string, bd: any): Promise<any> {
    return await firstValueFrom(
      this.http.put(`${this.baseUrl}${endPoint}`, bd,
        {
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Basic ${this.auth.getToken()}`,
          }
        })
    );
  }

  async deleteData(endPoint: string): Promise<any> {
    return await firstValueFrom(
      this.http.delete(`${this.baseUrl}${endPoint}`,
        {
          headers: {
            Authorization: `Basic ${this.auth.getToken()}`,
          }
        })
    );
  }
  async downloadData(endPoint: string): Promise<any> {
    return await firstValueFrom(
      this.http.get(`${this.baseUrl}${endPoint}`, {
        headers: {
          Authorization: `Basic ${this.auth.getToken()}`,
        },
        responseType: 'blob'
      })
    );
  }

  async PostPut(endPoint: string, bd: any): Promise<any> {
    if (bd.id == 0)
      return await this.postData(endPoint, bd);
    else
      return await this.putData(endPoint, bd);
  }

  getWithToken(endPoint: string, auth: string): any {
    return this.http.get(`${this.baseUrl}${endPoint}`,
      {
        headers: {
          Authorization: `${auth}`,
        }
      });
  }
  postWithToken(endPoint: string, auth: string, bd: any) {
    return this.http.post(`${this.baseUrl}${endPoint}`, bd,
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `${auth}`,
        }
      }
    );
  }
  getExt(endPoint: string): any {
    return this.http.get(`${endPoint}`);
  }

  public async getPromisse(endPoint: string): Promise<any> {
    return new Promise((resolve, reject) => {
      this.get(endPoint).pipe(
        take(1)
      ).subscribe(
        (data: any) => {
          resolve(data); // Resolve the promise with the received data
        },
        (error: any) => {
          reject(error); // Reject the promise if there is an error
        }
      );
    });
  }
  async getWithMode(endPoint: string, modo: string): Promise<any> {
    return await firstValueFrom(
      this.http.get(`${this.baseUrl}${endPoint}`, {
        headers: {
          Modo: `${modo}`,
        }
      })
    );
  }
  public async postPromise(endPoint: string, body: any): Promise<any> {
    try {
      return await lastValueFrom(this.post(endPoint, body).pipe(take(1)));
    } catch (error) {
      throw error;
    }
  }
}
