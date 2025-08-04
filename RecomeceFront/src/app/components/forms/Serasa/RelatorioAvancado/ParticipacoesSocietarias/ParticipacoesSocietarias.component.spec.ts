/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ParticipacoesSocietariasComponent } from './ParticipacoesSocietarias.component';

describe('ParticipacoesSocietariasComponent', () => {
  let component: ParticipacoesSocietariasComponent;
  let fixture: ComponentFixture<ParticipacoesSocietariasComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParticipacoesSocietariasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParticipacoesSocietariasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
