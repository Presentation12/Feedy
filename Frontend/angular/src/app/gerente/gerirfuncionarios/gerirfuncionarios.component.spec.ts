import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GerirfuncionariosComponent } from './gerirfuncionarios.component';

describe('GerirfuncionariosComponent', () => {
  let component: GerirfuncionariosComponent;
  let fixture: ComponentFixture<GerirfuncionariosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GerirfuncionariosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GerirfuncionariosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
