/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DividasProtestadasComponent } from './DividasProtestadas.component';

describe('DividasProtestadasComponent', () => {
  let component: DividasProtestadasComponent;
  let fixture: ComponentFixture<DividasProtestadasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DividasProtestadasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DividasProtestadasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
