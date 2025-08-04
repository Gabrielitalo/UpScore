import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {MatTableModule} from '@angular/material/table';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { LoginComponentComponent } from './components/auth/LoginComponent/LoginComponent.component';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RequestsService } from './services/Requests.service';
import { AuthService } from './services/Auth.service';
import { SnackBarService } from './services/SnackBar.service';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { MenuComponent } from './shared/Menu/Menu.component';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialogModule } from '@angular/material/dialog';
import {MatTooltipModule} from '@angular/material/tooltip';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CustomConvertsService } from './services/CustomConverts.service';
import { HeaderComponent } from './shared/Header/Header.component';
import { CadProdutosComponent } from './components/forms/CadProdutos/CadProdutos.component';
import { GridTableComponent } from './shared/grids/GridTable/GridTable.component';
import { CadProdutosDetalhesComponent } from './components/forms/CadProdutos/CadProdutosDetalhes/CadProdutosDetalhes.component';
import { CadEmpresasContasBancariasComponent } from './components/forms/CadEmpresasContasBancarias/CadEmpresasContasBancarias.component';
import { CadEmpresasContasBancariasDetalhesComponent } from './components/forms/CadEmpresasContasBancarias/CadEmpresasContasBancariasDetalhes/CadEmpresasContasBancariasDetalhes.component';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CadEmpresasComponent } from './components/forms/CadEmpresas/CadEmpresas.component';
import { CadEmpresasDetalhesComponent } from './components/forms/CadEmpresas/CadEmpresasDetalhes/CadEmpresasDetalhes.component';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { CadEquipeComponent } from './components/forms/CadEquipe/CadEquipe.component';
import { CadEquipeDetalhesComponent } from './components/forms/CadEquipe/CadEquipeDetalhes/CadEquipeDetalhes.component';
import { ValidationsDialogComponent } from './shared/ValidationsDialog/ValidationsDialog.component';
import { MovPropostasComponent } from './components/forms/MovPropostas/MovPropostas.component';
import { MovPropostasDetalhesComponent } from './components/forms/MovPropostas/MovPropostasDetalhes/MovPropostasDetalhes.component';
import {MatDividerModule} from '@angular/material/divider';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { SerasaDetalhesComponent } from './components/forms/Serasa/SerasaDetalhes/SerasaDetalhes.component';
import { SerasaComponent } from './components/forms/Serasa/Serasa/Serasa.component';
import { SerasaQueryDialogComponent } from './components/forms/Serasa/SerasaQueryDialog/SerasaQueryDialog.component';
import { ReprovalProposalComponent } from './components/forms/MovPropostas/ReprovalProposal/ReprovalProposal.component';
import { CadClientesComponent } from './components/forms/CadClientes/CadClientes.component';
import { CadClientesDetalhesComponent } from './components/forms/CadClientes/CadClientesDetalhes/CadClientesDetalhes.component';
import { AprovalProposalComponent } from './components/forms/MovPropostas/AprovalProposal/AprovalProposal.component';
import { MovContratosComponent } from './components/forms/MovContratos/MovContratos.component';
import { ContractManagerComponent } from './components/forms/MovContratos/ContractManager/ContractManager.component';
import { MovLotesEmpresasComponent } from './components/forms/MovLotesEmpresas/MovLotesEmpresas.component';
import { NewBatchComponent } from './components/forms/MovLotesEmpresas/NewBatch/NewBatch.component';
import { ViewBeneficiariosLoteDialogComponent } from './components/forms/MovLotesEmpresas/ViewBeneficiariosLoteDialog/ViewBeneficiariosLoteDialog.component';
import { MovConsultasCompradasComponent } from './components/forms/Consultas/MovConsultasCompradas/MovConsultasCompradas.component';
import { ComprarConsultaDialogComponent } from './components/forms/Consultas/ComprarConsultaDialog/ComprarConsultaDialog.component';
import {MatRadioModule} from '@angular/material/radio';
import { DashboardComponent } from './components/forms/dashboard/Dashboard/Dashboard.component';
import { PropostaAvulsaComponent } from './components/forms/MovPropostas/PropostaAvulsa/PropostaAvulsa.component';
import { LayoutModule } from '@angular/cdk/layout';
import {MatMenuModule} from '@angular/material/menu';
import { WarnDialogComponent } from './shared/WarnDialog/WarnDialog.component';
import { RelatorioAvancadoComponent } from './components/forms/Serasa/RelatorioAvancado/RelatorioAvancado.component';
import { PefinRefinComponent } from './components/forms/Serasa/RelatorioAvancado/PefinRefin/PefinRefin.component';
import { DividasProtestadasComponent } from './components/forms/Serasa/RelatorioAvancado/DividasProtestadas/DividasProtestadas.component';
import { AcoesJudiciaisComponent } from './components/forms/Serasa/RelatorioAvancado/AcoesJudiciais/AcoesJudiciais.component';
import { AnotacoesNegativasComponent } from './components/forms/Serasa/RelatorioAvancado/AnotacoesNegativas/AnotacoesNegativas.component';
import { ParticipacoesSocietariasComponent } from './components/forms/Serasa/RelatorioAvancado/ParticipacoesSocietarias/ParticipacoesSocietarias.component';
import { ChequesComponent } from './components/forms/Serasa/RelatorioAvancado/Cheques/Cheques.component';
import { ConsultasNaSerasaComponent } from './components/forms/Serasa/RelatorioAvancado/ConsultasNaSerasa/ConsultasNaSerasa.component';
import { RendaEstimadaComponent } from './components/forms/Serasa/RelatorioAvancado/RendaEstimada/RendaEstimada.component';
import { AnotacoesSPCComponent } from './components/forms/Serasa/RelatorioAvancado/AnotacoesSPC/AnotacoesSPC.component';
import { SerasaRegistrationPFComponent } from './components/forms/Serasa/RelatorioAvancado/SerasaRegistrationPF/SerasaRegistrationPF.component';
import { SerasaRegistrationPJComponent } from './components/forms/Serasa/RelatorioAvancado/SerasaRegistrationPJ/SerasaRegistrationPJ.component';
import { QuadroSocietarioComponent } from './components/forms/Serasa/RelatorioAvancado/QuadroSocietario/QuadroSocietario.component';
import { LimiteCreditoComponent } from './components/forms/Serasa/RelatorioAvancado/LimiteCredito/LimiteCredito.component';
import { NewBatchSheetComponent } from './components/forms/MovLotesEmpresas/NewBatchSheet/NewBatchSheet.component';
import { BatchManagerComponent } from './components/forms/MovLotesEmpresas/BatchManager/BatchManager.component';
import { MovLotesAssociacaoComponent } from './components/forms/MovLotesAssociacao/MovLotesAssociacao.component';
import { NewBatchAssociacaoComponent } from './components/forms/MovLotesAssociacao/NewBatchAssociacao/NewBatchAssociacao.component';
import { LoteAssociacaoDetailsComponent } from './components/forms/MovLotesAssociacao/LoteAssociacaoDetails/LoteAssociacaoDetails.component';
import { DashboardAssociacaoComponent } from './components/forms/dashboard/DashboardAssociacao/DashboardAssociacao.component';
import { RelatorioBoaVistaComponent } from './components/forms/BoaVista/RelatorioBoaVista/RelatorioBoaVista.component';
import { RelatorioRiscoPositivoComponent } from './components/forms/BoaVista/RelatorioRiscoPositivo/RelatorioRiscoPositivo.component';
import { RelatorioAcertaComponent } from './components/forms/BoaVista/RelatorioAcerta/RelatorioAcerta.component';
import { MovContaCorrenteComponent } from './components/forms/MovContaCorrente/MovContaCorrente.component';
import { RelatorioBasicoComponent } from './components/forms/Serasa/RelatorioBasico/RelatorioBasico.component';
import { DividasVencidasComponent } from './components/forms/Serasa/RelatorioAvancado/DividasVencidas/DividasVencidas.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    MenuComponent,
    LoginComponentComponent,
    GridTableComponent,
    CadProdutosComponent,
    CadProdutosDetalhesComponent,
    CadEmpresasContasBancariasComponent,
    CadEmpresasContasBancariasDetalhesComponent,
    CadEmpresasComponent,
    CadEmpresasDetalhesComponent,
    CadEquipeComponent,
    CadEquipeDetalhesComponent,
    ValidationsDialogComponent,
    MovPropostasComponent,
    MovPropostasDetalhesComponent,
    SerasaComponent,
    SerasaDetalhesComponent,
    SerasaQueryDialogComponent,
    ReprovalProposalComponent,
    AprovalProposalComponent,
    CadClientesComponent,
    CadClientesDetalhesComponent,
    MovContratosComponent,
    ContractManagerComponent,
    MovLotesEmpresasComponent,
    NewBatchComponent,
    ViewBeneficiariosLoteDialogComponent,
    MovConsultasCompradasComponent,
    ComprarConsultaDialogComponent,
    DashboardComponent,
    PropostaAvulsaComponent,
    WarnDialogComponent,
    RelatorioAvancadoComponent,
    PefinRefinComponent,
    DividasProtestadasComponent,
    AcoesJudiciaisComponent,
    AnotacoesNegativasComponent,
    ParticipacoesSocietariasComponent,
    ChequesComponent,
    ConsultasNaSerasaComponent,
    RendaEstimadaComponent,
    AnotacoesSPCComponent,
    SerasaRegistrationPFComponent,
    SerasaRegistrationPJComponent,
    QuadroSocietarioComponent,
    LimiteCreditoComponent,
    NewBatchSheetComponent,
    BatchManagerComponent,
    MovLotesAssociacaoComponent,
    NewBatchAssociacaoComponent,
    LoteAssociacaoDetailsComponent,
    DashboardAssociacaoComponent,
    RelatorioBoaVistaComponent,
    RelatorioRiscoPositivoComponent,
    RelatorioAcertaComponent,
    MovContaCorrenteComponent,
    RelatorioBasicoComponent,
    DividasVencidasComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    BrowserModule,
    BrowserAnimationsModule, 
    FormsModule,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatTabsModule,
    MatIconModule,
    MatTooltipModule,
    AppRoutingModule,
    MatSidenavModule,
    MatToolbarModule,
    MatTableModule,
    MatDialogModule,
    MatSelectModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatDividerModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    LayoutModule,
    MatMenuModule
  ],
  providers: [
    RequestsService,
    AuthService,
    SnackBarService,
    CustomConvertsService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
