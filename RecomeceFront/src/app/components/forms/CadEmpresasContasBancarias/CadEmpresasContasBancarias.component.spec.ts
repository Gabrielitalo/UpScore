/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { CadEmpresasContasBancariasComponent } from './CadEmpresasContasBancarias.component';

describe('CadEmpresasContasBancariasComponent', () => {
  let component: CadEmpresasContasBancariasComponent;
  let fixture: ComponentFixture<CadEmpresasContasBancariasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CadEmpresasContasBancariasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CadEmpresasContasBancariasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
