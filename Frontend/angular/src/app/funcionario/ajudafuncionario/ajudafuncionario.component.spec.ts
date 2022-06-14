import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AjudafuncionarioComponent } from './ajudafuncionario.component';

describe('AjudafuncionarioComponent', () => {
  let component: AjudafuncionarioComponent;
  let fixture: ComponentFixture<AjudafuncionarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AjudafuncionarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AjudafuncionarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
