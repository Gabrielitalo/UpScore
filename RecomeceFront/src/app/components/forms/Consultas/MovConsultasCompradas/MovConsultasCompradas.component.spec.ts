/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MovConsultasCompradasComponent } from './MovConsultasCompradas.component';

describe('MovConsultasCompradasComponent', () => {
  let component: MovConsultasCompradasComponent;
  let fixture: ComponentFixture<MovConsultasCompradasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovConsultasCompradasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovConsultasCompradasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
