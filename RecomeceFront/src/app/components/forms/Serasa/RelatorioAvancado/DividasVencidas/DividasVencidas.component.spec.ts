/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DividasVencidasComponent } from './DividasVencidas.component';

describe('DividasVencidasComponent', () => {
  let component: DividasVencidasComponent;
  let fixture: ComponentFixture<DividasVencidasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DividasVencidasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DividasVencidasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
