import { Component, OnInit, inject } from '@angular/core';
import { RequestsService } from '../../../services/Requests.service';
import { AuthService } from '../../../services/Auth.service';

import {
  MatSnackBar,
  MatSnackBarHorizontalPosition,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';
import { SnackBarService } from '../../../services/SnackBar.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-LoginComponent',
  templateUrl: './LoginComponent.component.html',
  styleUrls: ['./LoginComponent.component.css']
})
export class LoginComponentComponent implements OnInit {

  hide = true; // Para alternar a visibilidade da senha
  tab = 0;
  public userName = '';
  public passWord = '';
  public mode: any = '';

  constructor(private request: RequestsService,
    private auth: AuthService,
    private route: ActivatedRoute,
    public snack: SnackBarService,
    public router: Router) {
    this.mode = this.route.snapshot.paramMap.get('modo');
  }

  ngOnInit() {
    const isLogged = this.auth.isAuthenticated();
    const idRole = this.auth.getIdRole();
    if (isLogged == true)
      if (idRole == 4 || idRole == 5)
        window.location.href = 'Consultas';
      else
        window.location.href = 'Comercial';
  }

  togglePasswordVisibility() {
    this.hide = !this.hide;
  }

  tabChanged(index: number) {
    this.tab = index;
  }

  public async Login(): Promise<void> {
    const data = await this.request.getWithMode(`/Login?userType=${this.tab}&email=${this.userName}&password=${this.passWord}`, this.mode);
    if (data.token) {
      data.isQa = this.mode == 'qa' ? true : false;
      await this.auth.setSession(data);
      const idRole = this.auth.getIdRole();
      if (idRole == 4 || idRole == 5)
        window.location.href = 'Consultas';
      else
        window.location.href = 'Comercial';
    }
    else if (data.type == 2) {
      this.snack.CreateNotification('Usuário ou senha não encontrado!')
    }
    console.log(data)

  }
}
