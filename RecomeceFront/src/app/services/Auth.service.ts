import { Injectable } from '@angular/core';
import { ISession } from '../interfaces/ISession';
import { APP_VERSION } from '../configs/version';

@Injectable()
export class AuthService {

  private version = APP_VERSION;

  constructor() {
    console.log('system version', this.version);
  }

  private isValid(obj: ISession): boolean {
    const dt = Date.now();
    const a = Date.parse('' + obj.expiresSession)
    const mili = dt - a;
    const hours = Math.floor(mili / 1000 / 60 / 60)
    if (hours < 4)
      return true;
    return false;
  }

  public logout(): void {
    this.cleanSession();
  }

  public deleteAllCookies() {
    const cookies = document.cookie.split(";");
    for (let i = 0; i < cookies.length; i++) {
      let cookie = cookies[i];
      let eqPos = cookie.indexOf("=");
      let name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
      document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
  }
  private getStorage(): any {
    let x = localStorage.getItem('session');
    return JSON.parse(x!);
  }
  public isAuthenticated(): boolean {
    let obj: ISession = this.getSession();
    if (!obj)
      return false;

    // console.log('version', obj?.version, this.version);
    // if (obj?.version != this.version) {
    //   // this.cleanSession();
    //   window.location.reload();
    //   return false;
    // }
    if (obj.token) {
      return this.isValid(obj);
    }
    return false;
  }
  public async setSession(r: any): Promise<string> {
    // r.version = this.version;
    await localStorage.setItem('session', JSON.stringify(r));
    return 'OK';
  }
  public cleanSession(): void {
    localStorage.removeItem('session');
  }
  public getSession(): ISession {
    let obj = this.getStorage();
    let s: ISession = obj;
    return s;
  }
  public getIdType(): number {
    return this.getSession()?.idType;
  }
  public getIdRole(): number {
    return this.getSession()?.idRole;
  }
  public getUsername(): string {
    return this.getSession()?.username;
  }
  public getCompany(): number {
    return this.getSession()?.idCompany;
  }
  public getToken(): string {
    return this.getSession()?.token;
  }
  public getExpiresSession(): Date {
    return this.getSession()?.expiresSession;
  }
  public isGlobalAdm(): boolean {
    return this.getIdRole() == 0;
  }
  public isQa(): boolean {
    return this.getSession().isQa;
  }
  public isAdm(): boolean {
    return this.getIdRole() == 1;
  }
  public isConsulta(): boolean {
    return this.getIdRole() == 4;
  }
  public isAssociacao(): boolean {
    return this.getSession()?.isAssociacao;
  }
  public getWhiteLabelConfig(): any {
    return this.getSession()?.whiteLabelConfig;
  }
}
