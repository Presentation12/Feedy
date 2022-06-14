import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PerfilfuncionarioComponent } from './perfilfuncionario.component';

describe('PerfilfuncionarioComponent', () => {
  let component: PerfilfuncionarioComponent;
  let fixture: ComponentFixture<PerfilfuncionarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PerfilfuncionarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PerfilfuncionarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
