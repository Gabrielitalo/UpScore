/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { AnotacoesNegativasComponent } from './AnotacoesNegativas.component';

describe('AnotacoesNegativasComponent', () => {
  let component: AnotacoesNegativasComponent;
  let fixture: ComponentFixture<AnotacoesNegativasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AnotacoesNegativasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AnotacoesNegativasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
