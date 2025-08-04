/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { CadEmpresasContasBancariasDetalhesComponent } from './CadEmpresasContasBancariasDetalhes.component';

describe('CadEmpresasContasBancariasDetalhesComponent', () => {
  let component: CadEmpresasContasBancariasDetalhesComponent;
  let fixture: ComponentFixture<CadEmpresasContasBancariasDetalhesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CadEmpresasContasBancariasDetalhesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CadEmpresasContasBancariasDetalhesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
