/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { RelatorioRiscoPositivoComponent } from './RelatorioRiscoPositivo.component';

describe('RelatorioRiscoPositivoComponent', () => {
  let component: RelatorioRiscoPositivoComponent;
  let fixture: ComponentFixture<RelatorioRiscoPositivoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RelatorioRiscoPositivoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RelatorioRiscoPositivoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
