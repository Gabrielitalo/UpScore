import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  constructor() { }

  public toogleLoader(): void {
    const btn = document.getElementById('BtnBackDrop');
    btn?.click();
  }

}
