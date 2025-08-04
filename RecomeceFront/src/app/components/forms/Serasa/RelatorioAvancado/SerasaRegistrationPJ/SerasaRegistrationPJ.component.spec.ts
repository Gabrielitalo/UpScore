/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SerasaRegistrationPJComponent } from './SerasaRegistrationPJ.component';

describe('SerasaRegistrationPJComponent', () => {
  let component: SerasaRegistrationPJComponent;
  let fixture: ComponentFixture<SerasaRegistrationPJComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SerasaRegistrationPJComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SerasaRegistrationPJComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
