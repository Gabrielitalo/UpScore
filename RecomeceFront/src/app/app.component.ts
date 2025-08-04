import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/Auth.service';
import { Router } from '@angular/router';
import { CustomConvertsService } from './services/CustomConverts.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { DeviceService } from './services/device.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'Sistema';

  menuOpen = true;
  public menuContainer: any = [];
  public menuTab = -1;
  public hideMenu = false;
  public isLoadingSomething = false;
  public logo = '../assets/storm-logo.png';
  public whiteLabelConfig: any;
  isMobile: boolean = false;

  constructor(
    private http: HttpClient,
    public auth: AuthService,
    public converts: CustomConvertsService,
    public router: Router,
    private breakpointObserver: BreakpointObserver,
    private deviceService: DeviceService
  ) {
    this.MontaMenu();
  }
  ngOnInit(): void {
    // this.http.get('/version.json')
    //   .subscribe(response => {
    //     // ok
    //     console.log(response);
        
    //   });

    this.checkMobile()
    if (this.auth.isAuthenticated() == false)
      this.hideMenu = !this.auth.isAuthenticated();

    this.whiteLabelConfig = this.auth.getWhiteLabelConfig();

    if (this.whiteLabelConfig == null) return;
    this.logo = `../assets/${this.whiteLabelConfig.logo.split('.')[0]}128.png`;
  }

  private checkMobile(): void {
    this.breakpointObserver
      .observe(['(max-width: 767px)'])
      .subscribe((result) => {
        this.isMobile = result.matches;
        this.deviceService.setMobileState(result.matches);
      });
  }
  ngAfterContentInit() {
    this.isAuth();
  }

  public toogleLoader(): void {
    this.isLoadingSomething = !this.isLoadingSomething;
  }

  public logout(): void {
    this.auth.logout();
    location.href = 'Login';
  }

  private async isAuth(): Promise<void> {
    let s = this.auth.isAuthenticated();
    if (
      s == false &&
      (!window.location.href.includes('Login') ||
        !window.location.href.includes('Token') ||
        !window.location.href.includes('Registrar') ||
        !window.location.href.includes('RecuperarSenha'))
    ) {
      this.auth.logout();
    }
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  public CloseAllOpens(): void {
    let isFind = this.menuContainer.filter((d: any) => d.isOpen == true);
    if (isFind?.length > 0) isFind[0].isOpen = false;
  }

  public SubMenu(current: any, row: number): void {
    this.CloseAllOpens();
    this.menuTab = row;

    current.isOpen = !current.isOpen;
  }

  private MontaMenu(): void {
    // this.MenuDemo();
    const idRole = this.auth.getIdRole();
    if (idRole == 0)
      // Adm Global
      // this.MenuAdmGlobal();
      this.MenuDemo();
    else if (idRole == 1)
      // Adm
      // this.MenuAdmLocal();
      this.MenuDemo();
    else if (idRole == 2)
      // Vendedor
      this.MenuVendedor();
    else if (idRole == 4) this.MenuConsulta();
    else if (idRole == 5) this.MenuAssociacao();
  }

  public ShowMenu(): void {
    this.hideMenu = false;
  }

  public Navegar(rota: any): void {
    this.router.navigate([rota.Rota]);
  }

  private MenuDemo(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Dashboard',
        Icone: 'dashboard',
        Descricao: '',
        Rota: 'Dashboard',
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Cadastros',
        Icone: 'settings',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Produtos',
            Icone: 'production_quantity_limits',
            Descricao: '',
            Rota: '/Produtos',
          },
          // {
          //   Tipo: 0,
          //   Titulo: 'Franqueados',
          //   Icone: 'apartment',
          //   Descricao: '',
          //   Rota: '/Empresas',
          // },
          {
            Tipo: 0,
            Titulo: 'Equipe',
            Icone: 'person',
            Descricao: '',
            Rota: '/Equipe',
          },
          {
            Tipo: 0,
            Titulo: 'Clientes',
            Icone: 'groups',
            Descricao: '',
            Rota: '/Clientes',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Comercial',
        Icone: 'support_agent',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Propostas',
            Icone: 'work',
            Descricao: '',
            Rota: '/Comercial',
          },
          {
            Tipo: 0,
            Titulo: 'Contratos',
            Icone: 'receipt_long',
            Descricao: '',
            Rota: 'Contratos',
          },
          {
            Tipo: 0,
            Titulo: 'Lotes',
            Icone: 'gavel',
            Descricao: '',
            Rota: 'Lotes',
          },
        ],
      },
      {
        Tipo: 0,
        Titulo: 'Conta Corrente',
        Icone: 'currency_exchange',
        Descricao: '',
        Rota: 'ContaCorrente',
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'Consultas',
      },
    ];
  }
  private MenuAdmGlobal(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Dashboard',
        Icone: 'dashboard',
        Descricao: '',
        Rota: 'Dashboard',
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Cadastros',
        Icone: 'settings',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Contas Bancárias',
            Icone: 'account_balance',
            Descricao: '',
            Rota: 'ContasBancarias',
          },
          {
            Tipo: 0,
            Titulo: 'Produtos',
            Icone: 'production_quantity_limits',
            Descricao: '',
            Rota: '/Produtos',
          },
          {
            Tipo: 0,
            Titulo: 'Franqueados',
            Icone: 'apartment',
            Descricao: '',
            Rota: '/Empresas',
          },
          {
            Tipo: 0,
            Titulo: 'Equipe',
            Icone: 'person',
            Descricao: '',
            Rota: '/Equipe',
          },
          {
            Tipo: 0,
            Titulo: 'Clientes',
            Icone: 'groups',
            Descricao: '',
            Rota: '/Clientes',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Comercial',
        Icone: 'support_agent',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Propostas',
            Icone: 'work',
            Descricao: '',
            Rota: '/Comercial',
          },
          {
            Tipo: 0,
            Titulo: 'Contratos',
            Icone: 'receipt_long',
            Descricao: '',
            Rota: 'Contratos',
          },
          {
            Tipo: 0,
            Titulo: 'Lotes',
            Icone: 'gavel',
            Descricao: '',
            Rota: 'Lotes',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Financeiro',
        Icone: 'attach_money',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Contas a Pagar',
            Icone: 'request_quote',
            Descricao: '',
            Rota: '',
          },
          {
            Tipo: 0,
            Titulo: 'Contas a Receber',
            Icone: 'savings',
            Descricao: '',
            Rota: '',
          },
        ],
      },
      {
        Tipo: 0,
        Titulo: 'Relatórios',
        Icone: 'print_error',
        Descricao: '',
        Rota: 'CadReguas',
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'Consultas',
      },
      {
        Tipo: 0,
        Titulo: 'Ajuda',
        Icone: 'contact_support',
        Descricao: '',
        Rota: 'Ajuda',
      },
    ];
  }

  private MenuAdmLocal(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Dashboard',
        Icone: 'dashboard',
        Descricao: '',
        Rota: 'Dashboard',
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Cadastros',
        Icone: 'settings',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Contas Bancárias',
            Icone: 'account_balance',
            Descricao: '',
            Rota: 'ContasBancarias',
          },
          {
            Tipo: 0,
            Titulo: 'Franqueados',
            Icone: 'apartment',
            Descricao: '',
            Rota: '/Empresas',
          },
          {
            Tipo: 0,
            Titulo: 'Equipe',
            Icone: 'person',
            Descricao: '',
            Rota: '/Equipe',
          },
          {
            Tipo: 0,
            Titulo: 'Clientes',
            Icone: 'groups',
            Descricao: '',
            Rota: '/Clientes',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Comercial',
        Icone: 'support_agent',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Propostas',
            Icone: 'work',
            Descricao: '',
            Rota: '',
          },
          {
            Tipo: 0,
            Titulo: 'Contratos',
            Icone: 'receipt_long',
            Descricao: '',
            Rota: '',
          },
          {
            Tipo: 0,
            Titulo: 'Lotes',
            Icone: 'gavel',
            Descricao: '',
            Rota: '',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Financeiro',
        Icone: 'attach_money',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Contas a Pagar',
            Icone: 'request_quote',
            Descricao: '',
            Rota: '',
          },
          {
            Tipo: 0,
            Titulo: 'Contas a Receber',
            Icone: 'savings',
            Descricao: '',
            Rota: '',
          },
        ],
      },
      {
        Tipo: 0,
        Titulo: 'Relatórios',
        Icone: 'print_error',
        Descricao: '',
        Rota: 'CadReguas',
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'MovCobrancas',
      },
      {
        Tipo: 0,
        Titulo: 'Ajuda',
        Icone: 'contact_support',
        Descricao: '',
        Rota: 'Ajuda',
      },
    ];
  }

  private MenuVendedor(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Dashboard',
        Icone: 'dashboard',
        Descricao: '',
        Rota: 'Dashboard',
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Cadastros',
        Icone: 'settings',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Clientes',
            Icone: 'groups',
            Descricao: '',
            Rota: '/Clientes',
          },
        ],
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Comercial',
        Icone: 'support_agent',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Propostas',
            Icone: 'work',
            Descricao: '',
            Rota: '/Comercial',
          },
        ],
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'Consultas',
      },
      // {
      //   Tipo: 0,
      //   Titulo: 'Relatórios',
      //   Icone: 'print_error',
      //   Descricao: '',
      //   Rota: 'CadReguas'
      // },
    ];
  }

  private MenuConsulta(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Equipe',
        Icone: 'person',
        Descricao: '',
        Rota: '/Equipe',
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'Consultas',
      },
      {
        Tipo: 0,
        Titulo: 'Conta Corrente',
        Icone: 'currency_exchange',
        Descricao: '',
        Rota: 'ContaCorrente',
      },
      {
        Tipo: 0,
        Titulo: 'Lotes Parceiros',
        Icone: 'gavel',
        Descricao: '',
        Rota: 'Lotes',
      },
    ];
  }

  private MenuAssociacao(): void {
    this.menuContainer = [
      {
        Tipo: 0,
        Titulo: 'Dashboard',
        Icone: 'dashboard',
        Descricao: '',
        Rota: 'DashboardAssociacao',
      },
      {
        isOpen: false,
        Tipo: 1,
        Titulo: 'Cadastros',
        Icone: 'settings',
        Descricao: '',
        Sub: [
          {
            Tipo: 0,
            Titulo: 'Empresas',
            Icone: 'apartment',
            Descricao: '',
            Rota: '/Empresas',
          },
          {
            Tipo: 0,
            Titulo: 'Equipe',
            Icone: 'person',
            Descricao: '',
            Rota: '/Equipe',
          },
        ],
      },
      {
        Tipo: 0,
        Titulo: 'Consultas',
        Icone: 'query_stats',
        Descricao: '',
        Rota: 'Consultas',
      },
      {
        Tipo: 0,
        Titulo: 'Lotes Parceiros',
        Icone: 'gavel',
        Descricao: '',
        Rota: 'Lotes',
      },
      {
        Tipo: 0,
        Titulo: 'Associação',
        Icone: 'balance',
        Descricao: '',
        Rota: 'LotesAssociacao',
      },
    ];
  }
}
