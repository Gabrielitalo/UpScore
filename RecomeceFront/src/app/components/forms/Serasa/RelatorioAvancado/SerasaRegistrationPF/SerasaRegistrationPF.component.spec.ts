/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SerasaRegistrationPFComponent } from './SerasaRegistrationPF.component';

describe('SerasaRegistrationPFComponent', () => {
  let component: SerasaRegistrationPFComponent;
  let fixture: ComponentFixture<SerasaRegistrationPFComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SerasaRegistrationPFComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SerasaRegistrationPFComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
