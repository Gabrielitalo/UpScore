import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponentComponent } from './components/auth/LoginComponent/LoginComponent.component';
import { CadProdutosComponent } from './components/forms/CadProdutos/CadProdutos.component';
import { CadProdutosDetalhesComponent } from './components/forms/CadProdutos/CadProdutosDetalhes/CadProdutosDetalhes.component';
import { CadEmpresasContasBancariasComponent } from './components/forms/CadEmpresasContasBancarias/CadEmpresasContasBancarias.component';
import { CadEmpresasComponent } from './components/forms/CadEmpresas/CadEmpresas.component';
import { CadEquipeComponent } from './components/forms/CadEquipe/CadEquipe.component';
import { MovPropostasComponent } from './components/forms/MovPropostas/MovPropostas.component';
import { MovPropostasDetalhesComponent } from './components/forms/MovPropostas/MovPropostasDetalhes/MovPropostasDetalhes.component';
import { SerasaDetalhesComponent } from './components/forms/Serasa/SerasaDetalhes/SerasaDetalhes.component';
import { SerasaComponent } from './components/forms/Serasa/Serasa/Serasa.component';
import { CadClientesDetalhesComponent } from './components/forms/CadClientes/CadClientesDetalhes/CadClientesDetalhes.component';
import { MovContratosComponent } from './components/forms/MovContratos/MovContratos.component';
import { MovLotesEmpresasComponent } from './components/forms/MovLotesEmpresas/MovLotesEmpresas.component';
import { MovConsultasCompradasComponent } from './components/forms/Consultas/MovConsultasCompradas/MovConsultasCompradas.component';
import { CadClientesComponent } from './components/forms/CadClientes/CadClientes.component';
import { DashboardComponent } from './components/forms/dashboard/Dashboard/Dashboard.component';
import { RelatorioAvancadoComponent } from './components/forms/Serasa/RelatorioAvancado/RelatorioAvancado.component';
import { BatchManagerComponent } from './components/forms/MovLotesEmpresas/BatchManager/BatchManager.component';
import { ViewBeneficiariosLoteDialogComponent } from './components/forms/MovLotesEmpresas/ViewBeneficiariosLoteDialog/ViewBeneficiariosLoteDialog.component';
import { MovLotesAssociacaoComponent } from './components/forms/MovLotesAssociacao/MovLotesAssociacao.component';
import { DashboardAssociacaoComponent } from './components/forms/dashboard/DashboardAssociacao/DashboardAssociacao.component';
import { RelatorioBoaVistaComponent } from './components/forms/BoaVista/RelatorioBoaVista/RelatorioBoaVista.component';
import { MovContaCorrenteComponent } from './components/forms/MovContaCorrente/MovContaCorrente.component';
import { RelatorioBasicoComponent } from './components/forms/Serasa/RelatorioBasico/RelatorioBasico.component';

const routes: Routes = [
  { path: '', redirectTo: 'Login', pathMatch: 'full' },
  { path: 'Login', component: LoginComponentComponent },
  { path: 'Login/:modo', component: LoginComponentComponent },
  { path: 'Produtos', component: CadProdutosComponent },
  { path: 'Produtos/:id', component: CadProdutosDetalhesComponent },
  { path: 'ContasBancarias', component: CadEmpresasContasBancariasComponent },
  { path: 'Empresas', component: CadEmpresasComponent },
  { path: 'Equipe', component: CadEquipeComponent },
  { path: 'Clientes', component: CadClientesComponent },
  { path: 'Comercial', component: MovPropostasComponent },
  { path: 'Comercial/:id', component: MovPropostasDetalhesComponent },
  { path: 'Consultas', component: SerasaComponent },
  { path: 'Consultas/:id', component: SerasaDetalhesComponent },
  { path: 'Clientes/:id', component: CadClientesDetalhesComponent },
  { path: 'Clientes/:id/:valid', component: CadClientesDetalhesComponent },
  { path: 'Contratos', component: MovContratosComponent },
  { path: 'Lotes', component: MovLotesEmpresasComponent },
  { path: 'Lotes/:id', component: ViewBeneficiariosLoteDialogComponent },
  { path: 'FinanceiroConsultas', component: MovConsultasCompradasComponent },
  { path: 'FinanceiroConsultas/:modo', component: MovConsultasCompradasComponent },
  { path: 'Dashboard', component: DashboardComponent },
  { path: 'DashboardAssociacao', component: DashboardAssociacaoComponent },
  { path: 'RelatorioAvancado/:id', component: RelatorioAvancadoComponent },
  { path: 'RelatorioBasico/:id', component: RelatorioBasicoComponent },
  { path: 'LotesAssociacao', component: MovLotesAssociacaoComponent },
  { path: 'RelatorioBoaVista/:id', component: RelatorioBoaVistaComponent },
  { path: 'ContaCorrente', component: MovContaCorrenteComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
